using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(CharacterController))]
//[RequireComponent (typeof(CharacterMotor))]
[RequireComponent (typeof(AudioSource))]
public class SimpleEnemy : MonoBehaviour {
	
	#region members
	public float hp = 100.0f;
	public bool wander = true;
	public float wanderInterval = 5.0f;
	public float wanderIntervalRandom = 2.5f;
	public GameObject[] deathPrefabs;
	public float runSpeedMultiplier = 2.5f;
	public float avoidanceRayDistance = 4.0f;
	public float turnAngle = 30.0f;
	public float attackDistance = 1.8f;
	public float minimumAttackDistance = 1.8f;
	public float attackDamage = -10.0f;
	public float attackTime = 2.0f;
	public GameObject alertPrefab;//an optional prefab with the matching Alert Tag assigned, causes swarming behavior
	public string alertTag = "EnemyAlarm";
	private bool soundedAlarm = false;
	public float alarmInterval = 2.0f;
	private float rayCounter;
	public float rayInterval = 4.0f;
	public Vector3 lookRayOffset = Vector3.up;
	public LayerMask lookRayMask;
	private bool canAttack = true;
	public string[] targetTags;
	public GameObject target;
	public GameObject image;
	public GameObject leader;//transform for us to follow
	public float followDistance = 4.0f;
	public AudioClip attack;
	public AudioClip attackHit;
	public AudioClip footstep;
	public AudioClip die;
//	[HideInInspector]
//	public CharacterMotor characterMotor;
	[HideInInspector]
	public CharacterController characterController;
	public string idleAnim = "Idle";
	public string walkAnim = "Walk";
	public string runAnim = "Run";
	public string attackAnim = "Attack";
	
	private Vector3 moveDir = Vector3.forward;
	#endregion
	
	void Start () {
		rayCounter = rayInterval;
//		characterMotor = GetComponent<CharacterMotor>();
		characterController = GetComponent<CharacterController>();
		StartCoroutine(PeriodicDirectionChange(wanderInterval));
	}
	
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == alertTag) {
			AttackNearestTarget();
		}
	}
	
	void Update () {
		rayCounter -= Time.deltaTime;
		if (target == null) {//seeking behavior
			if (wander) {
				image.GetComponent<Animation>().Play(walkAnim);
				if (leader == null) {
//					if (characterMotor == null)
						characterController.SimpleMove(transform.TransformDirection(moveDir.normalized));//characterMotor.inputMoveDirection = transform.TransformDirection( moveDir.normalized);
//					else
//						characterMotor.inputMoveDirection = transform.TransformDirection( moveDir.normalized);
				}
			}
			else {
				image.GetComponent<Animation>().Play(idleAnim);
			}
			
			if (leader != null) {
				FollowTheLeader();
				return;
			}
			if (rayCounter <= 0) {
				rayCounter = rayInterval;
				//Debug.DrawRay(transform.position + lookRayOffset, transform.TransformDirection(moveDir.normalized));
				RaycastHit hinfo;
				if (Physics.Raycast(transform.position + lookRayOffset, transform.TransformDirection(Vector3.forward), out hinfo, avoidanceRayDistance, lookRayMask)) {
					if (CheckIfPossibleTarget( hinfo.collider.gameObject))
						target = hinfo.collider.gameObject;
					transform.RotateAround(transform.position, Vector3.up, turnAngle);
					if (Random.value <= 0.5f)
						turnAngle *= -1;
				}
			}
		}
		else {
			if (!soundedAlarm && alertPrefab != null) {
				Instantiate(alertPrefab, transform.position, transform.rotation);//instantiate a prefab, which should be an invisible trigger used to alert others nearby
				soundedAlarm = true;
				StartCoroutine(ResetAlarm());
				
			}
			float dist = Vector3.Distance(transform.position, target.transform.position);
			if (canAttack && (dist  < (attackDistance) )) {//attacking behavior
				image.GetComponent<Animation>().Play(attackAnim);
				StartCoroutine(Attack(attackTime));
				canAttack = false;
			}
			if (dist > minimumAttackDistance) {//chase!
				image.GetComponent<Animation>().Play(runAnim);
				transform.LookAt(target.transform, Vector3.up);
//				if (characterMotor == null)
					characterController.SimpleMove(transform.TransformDirection(moveDir.normalized) * runSpeedMultiplier);//characterMotor.inputMoveDirection = transform.TransformDirection(moveDir.normalized) * runSpeedMultiplier;
//				else
//					characterMotor.inputMoveDirection = transform.TransformDirection(moveDir.normalized) * runSpeedMultiplier;
			}
		}
		transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
		
	}
	
	void FollowTheLeader () {
		if (Vector3.Distance(transform.position, leader.transform.position) < followDistance)
			return;
		if (target != null)
			return;
		transform.LookAt(leader.transform.position, Vector3.up);
		//characterController.Move(transform.TransformDirection(moveDir.normalized);//characterMotor.inputMoveDirection = transform.TransformDirection( moveDir.normalized);
		
	}
	
	IEnumerator Attack(float delay) {
		yield return new WaitForSeconds(delay);
		StopAllCoroutines();
		if (target != null) {
			if (attack != null)
				GetComponent<AudioSource>().PlayOneShot(attack);
			if (Vector3.Distance(transform.position, target.transform.position) < attackDistance) {
				target.SendMessage("ModifyHealth", attackDamage, SendMessageOptions.DontRequireReceiver);
				if (attackHit != null)
					GetComponent<AudioSource>().PlayOneShot(attackHit);
			}
			canAttack = true;
		}
	}
	
	IEnumerator PeriodicDirectionChange(float delay) {
		yield return new WaitForSeconds(delay);
		transform.Rotate(new Vector3(0.0f,turnAngle,0.0f));
		
		StartCoroutine(PeriodicDirectionChange(wanderInterval + Random.Range(0.0f, wanderIntervalRandom)));
	}
	
	public void SetTarget (GameObject tgt) {
		target = tgt;
	}
	public void ClearTarget () {
		target = null;
	}
	
	public void ModifyHealth (float val) {
		if (target == null) {
			AttackNearestTarget();
			if (!soundedAlarm && alertPrefab != null) {
				Instantiate(alertPrefab, transform.position, transform.rotation);//instantiate a prefab, which should be an invisible trigger used to alert others nearby
				soundedAlarm = true;
				StartCoroutine(ResetAlarm());
			}
		}
		hp += val;
		if (hp <= 0.0f) {
			if (die != null)
				GetComponent<AudioSource>().PlayOneShot(die);
			if (deathPrefabs.Length > 0) {
				foreach (GameObject pfab in deathPrefabs) {
					Instantiate(pfab, transform.position, transform.rotation);
				}
			}
			Destroy(gameObject);
		}
	}
	
	public void AttackNearestTarget () {
		GameObject tgt = FindNearestTarget();
		if (tgt != null) {
			target = tgt;
		}
	}
	
	public void AttackedBy (GameObject attacker) {
		if (attacker != null) {
			if (attacker.GetComponent<Health>() != null)
				target = attacker;
			TargetingComputer tcomp = attacker.GetComponent<TargetingComputer>();
			if (tcomp != null && tcomp.mainBody.GetComponent<Health>() != null)
				target = tcomp.mainBody.gameObject;
		}
	}
	
	public bool CheckIfPossibleTarget (GameObject possibleTarget) {
		bool ret = false;
		
		for (int i = 0; i < targetTags.Length; i++) {
			if (possibleTarget.tag == targetTags[i])
				ret = true;
		}
		
		return ret;
	}
	
	public IEnumerator ResetAlarm () {
		yield return new WaitForSeconds(alarmInterval);
		soundedAlarm = false;
	}
	
	public GameObject FindNearestTarget () {
		//populate a list of all possible targets
		List<GameObject> targets = new List<GameObject>();
		for (int i = 0; i < targetTags.Length; i++) {
			GameObject[] tgts = GameObject.FindGameObjectsWithTag(targetTags[i]);
			foreach (GameObject tgt in tgts) {
				targets.Add(tgt);
			}
		}
		//then find the closest thing we can kill, if any
		GameObject targ = null;
		float lastDistance = Mathf.Infinity;
		for (int i = 0; i < targets.Count; i++) {
			float dist = Vector3.Distance(transform.position, targets[i].transform.position);
			if (dist < lastDistance) {
				lastDistance = dist;
				targ = targets[i];
			}
		}
		return targ;
	}
}