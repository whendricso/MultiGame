using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame
{

	[RequireComponent (typeof(CharacterController))]
//[RequireComponent (typeof(CharacterMotor))]
	[RequireComponent (typeof(AudioSource))]
	public class SimpleEnemy : MultiModule
	{
	
		#region members

		[RequiredFieldAttribute("The amount of health this bot has. Does not require a 'Health' component.", RequiredFieldAttribute.RequirementLevels.Required)]
		public float hp = 100.0f;
		[Tooltip("Should this bot wander around like a zombie?")]
		public bool wander = true;
		[Tooltip("If so, how often should we wander?")]
		public float wanderInterval = 5.0f;
		[Tooltip("How much should we vary the wandering interval?")]
		public float wanderIntervalRandom = 2.5f;
		[Tooltip("A list of objects we want to spawn when this bot dies")]
		public GameObject[] deathPrefabs;
		[Tooltip("How many times faster can we run than walk?")]
		public float runSpeedMultiplier = 2.5f;
		[Tooltip("How far ahead should we look for obstacles before turning?")]
		public float avoidanceRayDistance = 4.0f;
		[Tooltip("How far should we turn when turning to wander?")]
		public float turnAngle = 30.0f;
		[Tooltip("How far away can we start attack an enemy? (Will still try to get closer)")]
		public float attackDistance = 1.8f;
		[Tooltip("How far away should we stop while attacking?")]
		public float minimumAttackDistance = 1.8f;
		[RequiredFieldAttribute("How much damage should we do?", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public float attackDamage = -10.0f;
		[RequiredFieldAttribute("How long between attacks?", RequiredFieldAttribute.RequirementLevels.Required)]
		public float attackTime = 2.0f;
		[RequiredFieldAttribute("A prefab to spawn when we detect an enemy. If it's a trigger tagged with 'Alert Tag' then we'll attack the nearest target when entering.", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject alertPrefab;
//an optional prefab with the matching Alert Tag assigned, causes swarming behavior
		[Tooltip("If we enter a trigger with this tag, we will seek out the nearest enemy.")]
		public string alertTag = "EnemyAlarm";
		private bool soundedAlarm = false;
		[Tooltip("How often can we spawn the 'Alert Prefab'?")]
		public float alarmInterval = 2.0f;
		private float rayCounter;
		[Tooltip("How often (in seconds) do we cast a ray to search for obstacles?")]
		public float rayInterval = 4.0f;
		public Vector3 lookRayOffset = Vector3.up;
		[Tooltip("A mask for collision layers representing obstacles we should avoid.")]
		public LayerMask lookRayMask;
		private bool canAttack = true;
		[Tooltip("A list of tags for enemies we can target")]
		public string[] targetTags;
		[RequiredFieldAttribute("An optional target to attack immediately", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject target;
		[RequiredFieldAttribute("Skinned Mesh Renderer character with a 'Legacy' animation rig (not Mecanim compatible, use for older models)",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public GameObject image;
		[HideInInspector]
		public GameObject leader;
//transform for us to follow
		[HideInInspector]
		public float followDistance = 4.0f;
		public AudioClip attack;
		public AudioClip attackHit;
//		public AudioClip footstep;
		public AudioClip die;
		//	[HideInInspector]
		//	public CharacterMotor characterMotor;
		[HideInInspector]
		public CharacterController characterController;
		public string idleAnim = "Idle";
		public string walkAnim = "Walk";
		public string runAnim = "Run";
		public string attackAnim = "Attack";

        private Animator anim;
        private Animation legacy;
		private Vector3 moveDir = Vector3.forward;

		public HelpInfo help = new HelpInfo("A high-efficiency hitscan AI. This system does not use navigation, but instead raycasts into the scene to decide where it can go. Will wander off ledges, it's pretty " +
			"stupid overall but useful for junk enemies. This is a legacy AI system, and uses the legacy Animation component and is not Mecanim compatible.");

        #endregion

        void Start()
        {
            if (image != null) {
                legacy = image.GetComponent<Animation>();
                anim = image.GetComponent<Animator>();
            }
        
			rayCounter = rayInterval;
//		characterMotor = GetComponent<CharacterMotor>();
			characterController = GetComponent<CharacterController> ();
			StartCoroutine (PeriodicDirectionChange (wanderInterval));
		}

		void OnTriggerEnter (Collider other)
		{
			if (other.gameObject.tag == alertTag) {
				AttackNearestTarget ();
			}
		}

		void Update ()
		{
			rayCounter -= Time.deltaTime;
			if (target == null) {//seeking behavior
				if (wander) {
					if (image != null)
					{
						if (legacy != null)
							legacy.Play(walkAnim);
						if (anim != null)
							anim.SetTrigger(walkAnim);
					}
					if (leader == null) {
//					if (characterMotor == null)
						characterController.SimpleMove (transform.TransformDirection (moveDir.normalized));//characterMotor.inputMoveDirection = transform.TransformDirection( moveDir.normalized);
//					else
//						characterMotor.inputMoveDirection = transform.TransformDirection( moveDir.normalized);
					}
				} else {
					if (image != null)
					{
						if (legacy != null)
							legacy.Play(idleAnim);
						if (anim != null)
							anim.SetTrigger(idleAnim);
					}
				}
			
				if (leader != null) {
					FollowTheLeader ();
					return;
				}
				if (rayCounter <= 0) {
					rayCounter = rayInterval;
					//Debug.DrawRay(transform.position + lookRayOffset, transform.TransformDirection(moveDir.normalized));
					RaycastHit hinfo;
					if (Physics.Raycast (transform.position + lookRayOffset, transform.TransformDirection (Vector3.forward), out hinfo, avoidanceRayDistance, lookRayMask)) {
						if (CheckIfPossibleTarget (hinfo.collider.gameObject))
							target = hinfo.collider.gameObject;
						transform.RotateAround (transform.position, Vector3.up, turnAngle);
						if (Random.value <= 0.5f)
							turnAngle *= -1;
					}
				}
			} else {
				if (!soundedAlarm && alertPrefab != null) {
					Instantiate (alertPrefab, transform.position, transform.rotation);//instantiate a prefab, which should be an invisible trigger used to alert others nearby
					soundedAlarm = true;
					StartCoroutine (ResetAlarm ());
				
				}
				float dist = Vector3.Distance (transform.position, target.transform.position);
				if (canAttack && (dist < (attackDistance))) {//attacking behavior
					if (image != null)
					{
						if (legacy != null)
							legacy.Play(attackAnim);
						if (anim != null)
							anim.SetTrigger(attackAnim);
					}
					StartCoroutine (Attack (attackTime));
					canAttack = false;
				}
				if (dist > minimumAttackDistance) {//chase!
					if (image != null) {
						if (legacy != null)
							legacy.Play(runAnim);
						if (anim != null)
							anim.SetTrigger(runAnim);
					}
					transform.LookAt (target.transform, Vector3.up);
//				if (characterMotor == null)
					characterController.SimpleMove (transform.TransformDirection (moveDir.normalized) * runSpeedMultiplier);//characterMotor.inputMoveDirection = transform.TransformDirection(moveDir.normalized) * runSpeedMultiplier;
//				else
//					characterMotor.inputMoveDirection = transform.TransformDirection(moveDir.normalized) * runSpeedMultiplier;
				}
			}
			transform.localEulerAngles = new Vector3 (0.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
		
		}

		void FollowTheLeader ()
		{
			if (Vector3.Distance (transform.position, leader.transform.position) < followDistance)
				return;
			if (target != null)
				return;
			transform.LookAt (leader.transform.position, Vector3.up);
			//characterController.Move(transform.TransformDirection(moveDir.normalized);//characterMotor.inputMoveDirection = transform.TransformDirection( moveDir.normalized);
		
		}

		IEnumerator Attack (float delay)
		{
			yield return new WaitForSeconds (delay);
			StopAllCoroutines ();
			if (target != null) {
				if (attack != null)
					GetComponent<AudioSource> ().PlayOneShot (attack);
				if (Vector3.Distance (transform.position, target.transform.position) < attackDistance) {
					target.SendMessage ("ModifyHealth", attackDamage, SendMessageOptions.DontRequireReceiver);
					if (attackHit != null)
						GetComponent<AudioSource> ().PlayOneShot (attackHit);
				}
				canAttack = true;
			}
		}

		IEnumerator PeriodicDirectionChange (float delay)
		{
			yield return new WaitForSeconds (delay);
			transform.Rotate (new Vector3 (0.0f, turnAngle, 0.0f));
		
			StartCoroutine (PeriodicDirectionChange (wanderInterval + Random.Range (0.0f, wanderIntervalRandom)));
		}

		public void SetTarget (GameObject tgt)
		{
			target = tgt;
		}

		public MessageHelp clearTargetHelp = new MessageHelp("ClearTarget","Removes the target from this bot, causing it to cease persuit.");
		public void ClearTarget ()
		{
			target = null;
		}

		public MessageHelp modifyHealthHelp = new MessageHelp("ModifyHealth","Allows the bot to take damage/healing and die without needing a 'Health' component",3,"The amount we should add to the current value.");
		public void ModifyHealth (float val)
		{
			if (target == null) {
				AttackNearestTarget ();
				if (!soundedAlarm && alertPrefab != null) {
					Instantiate (alertPrefab, transform.position, transform.rotation);//instantiate a prefab, which should be an invisible trigger used to alert others nearby
					soundedAlarm = true;
					StartCoroutine (ResetAlarm ());
				}
			}
			hp += val;
			if (hp <= 0.0f) {
				if (die != null)
					GetComponent<AudioSource> ().PlayOneShot (die);
				if (deathPrefabs.Length > 0) {
					foreach (GameObject pfab in deathPrefabs) {
						Instantiate (pfab, transform.position, transform.rotation);
					}
				}
				Destroy (gameObject);
			}
		}

	
		public MessageHelp attackNearestTargetHelp = new MessageHelp("AttackNearestTarget","Causes the bot to attack the nearest target, but only if it doesn't have a target right now.");
		public void AttackNearestTarget ()
		{
			GameObject tgt = FindNearestTarget ();
			if (tgt != null) {
				target = tgt;
			}
		}

		public void AttackedBy (GameObject attacker)
		{
			if (attacker != null) {
				if (attacker.GetComponent<Health> () != null)
					target = attacker;
				TargetingComputer tcomp = attacker.GetComponent<TargetingComputer> ();
				if (tcomp != null && tcomp.mainBody.GetComponent<Health> () != null)
					target = tcomp.mainBody.gameObject;
			}
		}

		public bool CheckIfPossibleTarget (GameObject possibleTarget)
		{
			bool ret = false;
		
			for (int i = 0; i < targetTags.Length; i++) {
				if (possibleTarget.tag == targetTags [i])
					ret = true;
			}
		
			return ret;
		}

		public IEnumerator ResetAlarm ()
		{
			yield return new WaitForSeconds (alarmInterval);
			soundedAlarm = false;
		}

		public GameObject FindNearestTarget ()
		{
			//populate a list of all possible targets
			List<GameObject> targets = new List<GameObject> ();
			for (int i = 0; i < targetTags.Length; i++) {
				GameObject[] tgts = GameObject.FindGameObjectsWithTag (targetTags [i]);
				foreach (GameObject tgt in tgts) {
					targets.Add (tgt);
				}
			}
			//then find the closest thing we can kill, if any
			GameObject targ = null;
			float lastDistance = Mathf.Infinity;
			for (int i = 0; i < targets.Count; i++) {
				float dist = Vector3.Distance (transform.position, targets [i].transform.position);
				if (dist < lastDistance) {
					lastDistance = dist;
					targ = targets [i];
				}
			}
			return targ;
		}
	}
}