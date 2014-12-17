using UnityEngine;
using System.Collections;

/// <summary>
/// Simple minion will follow the player's command, and attack enemies or flee.
/// </summary>
[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(CharacterMotor))]
[RequireComponent (typeof(AudioSource))]
public class SimpleMinion : MonoBehaviour {
	
	public float hp = 100.0f;
	public GameObject[] deathPrefabs;
	[HideInInspector]
	public bool selected = false;
	public enum OrderTypes {Idle, Follow, MoveTo};
	public OrderTypes orders = OrderTypes.Idle;
	public enum AiStates {Idle, Follow, MoveTo, Attack, AttackRanged};
	public AiStates aiState = AiStates.Follow;
	public float runSpeedMultiplier = 2.5f;
	public float avoidanceRayDistance = 4.0f;
	public float turnAngle = 30.0f;
	public float attackDistance = 1.8f;
	public float attackDamage = -10.0f;
	public float attackTime = 2.0f;
	public Vector3 lookRayOffset = Vector3.up;
	public LayerMask lookRayMask;
	private bool canAttack = true;
	public string targetTag = "Player";
	public GameObject target;
	private GameObject player;
	public GameObject image;
	public GameObject rangedProjectile;
	public GameObject projectileSpawnTransform;
	public Vector3 aimOffset = Vector3.up;//an offset to aim at, so we don't shoot at the target's feet
	public float engagementRange = 45.0f;//max range of ranged attacks
	public float rangedEngagementFraction = 0.66666f;//multiplied by engagement range to determine when we are close enough to get some good shots
	public float minimumRange = 10.0f;
	private Leader leader;//leader for us to follow
	public float followDistance = 4.0f;
	public AudioClip attack;
	public AudioClip attackHit;
	public AudioClip footstep;
	public AudioClip die;
	[HideInInspector]
	public CharacterMotor characterMotor;
	[HideInInspector]
	public CharacterController characterController;
	public string idleAnim = "Idle";
	public string walkAnim = "Walk";
	public string runAnim = "Run";
	public string attackAnim = "Attack";
	public string rangedAttackAnim = "Shoot";
	
	private Vector3 moveDir = Vector3.forward;
	
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		if (image == null) {
			Debug.LogError("Simple Minion " + gameObject.name + " needs an image with an Animation component set in the inspector.");
			enabled = false;
			return;
		}
	}
	
	void Update () {
		transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);//prevent tilting on the X axis
		if (player == null) {
			player = GameObject.FindGameObjectWithTag("Player");
			leader = player.GetComponent<Leader>();
		}
		if (player != null)
			ProcessStates();
	}
	
	void ProcessStates () {
		if (target == null)
			ResumeOrders();
		switch (aiState) {
		case AiStates.Follow:
			if (target != null)
				aiState = AiStates.AttackRanged;
			if (leader == null) {
				aiState = AiStates.Idle;
				return;
			}
			if (Vector3.Distance(transform.position, leader.transform.position) > followDistance) {
				transform.LookAt(leader.transform.position, Vector3.up);
				characterMotor.inputMoveDirection = transform.TransformDirection(moveDir.normalized);
			}
			break;
		case AiStates.AttackRanged:
			if (rangedProjectile == null) {
				aiState = AiStates.Attack;
			}
			else {
				float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
				if (distanceToTarget < minimumRange)
					aiState = AiStates.Attack;
				if ((minimumRange < distanceToTarget && distanceToTarget < engagementRange) && canAttack) {
					canAttack = false;
					StartCoroutine(Fire(attackTime));
				}
			}
			
			break;
		case AiStates.Attack:
			if (( Vector3.Distance(transform.position, target.transform.position) < (attackDistance) )) {//attacking behavior
				if (canAttack) {
					image.animation.Play(attackAnim);
					StartCoroutine(Attack(attackTime));
					canAttack = false;
				}
			}
			break;
		}
	}
	
	IEnumerator Fire (float delay) {
		yield return new WaitForSeconds(delay);
		if (target != null) {
			float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
			if(minimumRange < distanceToTarget && distanceToTarget < engagementRange) {
				image.animation.Blend(rangedAttackAnim);
				GameObject projectile = Instantiate(rangedProjectile, projectileSpawnTransform.transform.position, projectileSpawnTransform.transform.rotation) as GameObject;
				projectile.transform.LookAt(target.transform.position + aimOffset);
			}
		}
		canAttack = true;
	}
	
	IEnumerator Attack(float delay) {
		yield return new WaitForSeconds(delay);
		if (attack != null)
			audio.PlayOneShot(attack);
		if (Vector3.Distance(transform.position, target.transform.position) < attackDistance) {
			target.SendMessage("ModifyHealth", attackDamage, SendMessageOptions.DontRequireReceiver);
			if (attackHit != null)
				audio.PlayOneShot(attackHit);
		}
		canAttack = true;
	}
	
	private void ResumeOrders () {
		switch (orders) {
		case OrderTypes.Follow:
			aiState = AiStates.Follow;
			break;
		case OrderTypes.Idle:
			aiState = AiStates.Idle;
			break;
		case OrderTypes.MoveTo:
			aiState = AiStates.MoveTo;
			break;
		}
	}
	
	public void SetTarget (GameObject tgt) {
		target = tgt;
	}
	public void ClearTarget () {
		target = null;
	}
	
	public void ModifyHealth (float val) {
		hp += val;
		if (hp <= 0.0f) {
			if (die != null)
				audio.PlayOneShot(die);
			if (deathPrefabs.Length > 0) {
				foreach (GameObject pfab in deathPrefabs) {
					Instantiate(pfab, transform.position, transform.rotation);
				}
			}
			Destroy(gameObject);
		}
	}
}