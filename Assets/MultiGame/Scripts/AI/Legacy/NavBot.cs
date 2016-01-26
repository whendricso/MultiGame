using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {


[RequireComponent (typeof(NavMeshAgent))]
public class NavBot : MonoBehaviour {
	
	public GameObject target;
	//public Vector3 navTarget;
	public bool mouseSelection = true;
	public bool selected = false;
	private bool canDeselect = true;
	public float sprintSpeedBonus = 2.0f;
	public bool sprint = false;
	public KeyCode sprintKey = KeyCode.LeftControl;
	
	public GameObject image;
	public string walkAnim = "Walk";
	public string runAnim = "Run";
	public string idleAnim = "Idle";
	public string fireAnim = "Fire";
	
	public GameObject selectionIndicator;
	private bool useIndicator = false;
	
	public GameObject rangedProjectile;
	public GameObject muzzleTransform;
	public float maxRange = 30.0f;
	public float minRange = 5.0f;
	public int shotsInMag = 32;
	public float refireTime = 0.6f;
	private float refireCouter;
	
	
	[HideInInspector]
	NavMeshAgent agent;
	[HideInInspector]
	public float originalSpeed;
	
	void Start () {
		refireCouter = refireTime;
		agent = GetComponent<NavMeshAgent>();
		originalSpeed = agent.speed;
		if (!selected && selectionIndicator != null) {
			useIndicator = true;
			selectionIndicator.SetActive(false);
		}
	}
	
	void Update () {
		refireCouter -= Time.deltaTime;
		if (target != null && (CheckIsInRange(target))) {
			if (refireCouter <= 0 && muzzleTransform != null) {
				GameObject bullet = Instantiate(rangedProjectile, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
				bullet.SendMessage("SetOwner", gameObject, SendMessageOptions.DontRequireReceiver);
				refireCouter = refireTime;
			}
			
			return;
		}
		
		if (sprint)
			agent.speed = originalSpeed + sprintSpeedBonus;
		else
			agent.speed = originalSpeed;
		
		if (image != null)
			UpdateAnimations();
		if (Input.GetMouseButtonUp (1)) {
			canDeselect = true;
		}
		if (Input.GetKeyUp(KeyCode.Escape)) {
			canDeselect = true;
			Select(false);
		}
		if (selected && Input.GetMouseButtonDown(0)) {
			if (Input.GetKey(sprintKey))
				sprint = true;
			else
				sprint = false;
			if (mouseSelection) {
				Select(false);
				RaycastHit hinfo;
				Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				bool didHit = Physics.Raycast(mouseRay, out hinfo);
				if (didHit) {
					agent.SetDestination(hinfo.point);
				}
			}
		}
		
	}
	
	void UpdateAnimations () {
		if ((target != null && rangedProjectile != null) && CheckIsInRange(target)) {
			image.GetComponent<Animation>().CrossFade(fireAnim);
			return;
		}
		if (!agent.hasPath)
			image.GetComponent<Animation>().CrossFade(idleAnim);
		else {
			if (!sprint)
				image.GetComponent<Animation>().CrossFade(walkAnim);
			else
				image.GetComponent<Animation>().CrossFade(runAnim);
		}
	}
	
//	void OnMouseUpAsButton () {
//		Select();
//	}
	
	public void Select () {
		if (!selected)
			Select(true);
		else
			Select(false);
	}
	
	public void Select (bool val) {
		if (!canDeselect && val == false)
			return;
		selected = val;
		canDeselect = false;
		
		if (useIndicator)
			selectionIndicator.SetActive(val);
	}
	
	public bool CheckIsInRange (GameObject potentialTarget) {
		bool _ret = false;
		
		float _distance = Vector3.Distance(transform.position, potentialTarget.transform.position);
		if (minRange < _distance && _distance < maxRange) {
			_ret = true;
		}
		
		return _ret;
	}


}
}
//Copyright 2013-2015 William Hendrickson
