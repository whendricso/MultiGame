using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame
{


	[RequireComponent (typeof(UnityEngine.AI.NavMeshAgent))]
	public class NavBot : MultiModule
	{
	
		[RequiredFieldAttribute("An optional default move target",RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject target;
		//public Vector3 navTarget;
		[Tooltip("Should we allow this bot to be selected with the mouse? Requires this object to have an attached collider.")]
		public bool mouseSelection = true;
		[Tooltip("Are we currently selected? Enable to make this bot selected by default.")]
		public bool selected = false;
		private bool canDeselect = true;
		[Tooltip("An optional sprint key, allowing us to make this unit move faster while holding it. Set this to 'None' to disable sprinting.")]
		public KeyCode sprintKey = KeyCode.LeftControl;
		[Tooltip("How much faster can we go while sprinting?")]
		public float sprintSpeedBonus = 2.0f;
		[Tooltip("Do we start sprinting by default?")]
		public bool sprint = false;
	
		[RequiredFieldAttribute("The Skinned Mesh representing this bot.", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject image;
		public string walkAnim = "Walk";
		public string runAnim = "Run";
		public string idleAnim = "Idle";
		public string fireAnim = "Fire";
	
		[RequiredFieldAttribute("An object (such as an arrow over the bot's head) indicating selection. Will be turned on/off automatically.", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject selectionIndicator;
		private bool useIndicator = false;
	
		[RequiredFieldAttribute("If supplied, allows the bot to perform ranged attacks.", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject rangedProjectile;
		[RequiredFieldAttribute("If using a projectile attack, you must provide a muzzle transform to tell the bot where to spawn the projectile.", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public GameObject muzzleTransform;
		[Tooltip("How far away can we begin targeting enemies?")]
		public float maxRange = 30.0f;
		[Tooltip("How close must an enemy be before we stop shooting at them?")]
		public float minRange = 5.0f;
		[Tooltip("How many shots can we fire before we have to reload?")]
		public int shotsInMag = 32;
		[Tooltip("How long between shots?")]
		public float refireTime = 0.6f;
		private float refireCouter;
	
	
		[HideInInspector]
		UnityEngine.AI.NavMeshAgent agent;
		[HideInInspector]
		public float originalSpeed;

		public HelpInfo help = new HelpInfo("Nav Bot is a legacy self-contained navmesh bot. This is excellent for player-commanded tanks or similar applications, " +
			"and is less complex to set-up but lacks the flexibility of the new modular AI system. This AI uses the legacy Animation system and might not be suitable for some projects." +
			"\n\n" +
			"To use with animations, import your model with a 'Legacy' rig and input the animation names above.");

		void Start ()
		{
			refireCouter = refireTime;
			agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
			originalSpeed = agent.speed;
			if (!selected && selectionIndicator != null) {
				useIndicator = true;
				selectionIndicator.SetActive (false);
			}
		}

		void Update ()
		{
			refireCouter -= Time.deltaTime;
			if (rangedProjectile != null) {
				if (target != null && (CheckIsInRange (target))) {
					if (refireCouter <= 0 && muzzleTransform != null) {
						GameObject bullet = Instantiate (rangedProjectile, muzzleTransform.transform.position, muzzleTransform.transform.rotation) as GameObject;
						bullet.SendMessage ("SetOwner", gameObject, SendMessageOptions.DontRequireReceiver);
						refireCouter = refireTime;
					}
				
					return;
				}
			}
		
			if (sprint)
				agent.speed = originalSpeed + sprintSpeedBonus;
			else
				agent.speed = originalSpeed;
		
			if (image != null)
				UpdateAnimations ();
			if (Input.GetMouseButtonUp (1)) {
				canDeselect = true;
			}
			if (Input.GetKeyUp (KeyCode.Escape)) {
				canDeselect = true;
				Select (false);
			}
			if (selected && Input.GetMouseButtonDown (0)) {
				if (Input.GetKey (sprintKey))
					sprint = true;
				else
					sprint = false;
				if (mouseSelection) {
					Select (false);
					RaycastHit hinfo;
					Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
					bool didHit = Physics.Raycast (mouseRay, out hinfo);
					if (didHit) {
						agent.SetDestination (hinfo.point);
					}
				}
			}
		
		}

		void UpdateAnimations ()
		{
			if ((target != null && rangedProjectile != null) && CheckIsInRange (target)) {
				image.GetComponent<Animation> ().CrossFade (fireAnim);
				return;
			}
			if (!agent.hasPath)
				image.GetComponent<Animation> ().CrossFade (idleAnim);
			else {
				if (!sprint)
					image.GetComponent<Animation> ().CrossFade (walkAnim);
				else
					image.GetComponent<Animation> ().CrossFade (runAnim);
			}
		}
	
		//	void OnMouseUpAsButton () {
		//		Select();
		//	}
	
		public MessageHelp selectHelp = new MessageHelp("Select","Toggles the selection state of the Nav Bot.");
		public void Select ()
		{
			if (!selected)
				Select (true);
			else
				Select (false);
		}
			
		public MessageHelp selectBoolHelp = new MessageHelp("Select","Allows you to select or deselect the Nav Bot for player command.",1,"This optional argument allows you to set the selection state directly.");
		public void Select (bool val)
		{
			if (!canDeselect && val == false)
				return;
			selected = val;
			canDeselect = false;
		
			if (useIndicator)
				selectionIndicator.SetActive (val);
		}

		public bool CheckIsInRange (GameObject potentialTarget)
		{
			bool _ret = false;
		
			float _distance = Vector3.Distance (transform.position, potentialTarget.transform.position);
			if (minRange < _distance && _distance < maxRange) {
				_ret = true;
			}
		
			return _ret;
		}


	}
}
//Copyright 2013-2015 William Hendrickson
