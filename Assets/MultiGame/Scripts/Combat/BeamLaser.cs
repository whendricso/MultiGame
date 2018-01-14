using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Beam Laser")]
	[RequireComponent(typeof(LineRenderer))]
	public class BeamLaser : MultiModule {

		[Header("Important - Must be populated")]
		[Tooltip("A Layer Mask indicating which objects this beam can hit. It will pass through anything not tagged with this mask, and deal damage to anything it hits that has a 'Health' component, or any " +
			"other component that can receive the 'ModifyHealth' message")]
		public LayerMask hitRayMask;

		[Header("Projectile Settings")]
		[RequiredFieldAttribute("How far does the laser go until it stops dead in the middle of space?")]
		public float beamRange;
		[RequiredFieldAttribute("How much damage PER FIXEDUPDATE is dealt by this weapon?")]
		public float damage = 0.2f;
		[RequiredFieldAttribute("What object, if any, should be spawned when we hit something?",RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject splashPrefab;

		private LineRenderer beamLine;
		RaycastHit hinfo;

		public HelpInfo help = new HelpInfo("This component should be attached to a muzzle transform object representing the origin of the laser from the gun. Toggling this object" +
			" on or off will enable/disable the laser beam. The laser does damage every update while active. It extends from the muzzle transform to 'Beam Range' in front of the weapon. It " +
			"does not spawn a separate projectile object, thus saving memory and CPU over a very fast 'Bullet' and also giving an effect that moves with the weapon, whereas a 'Bullet' will " +
			"maintain it's original trajectory after leaving the weapon.");

		public bool debug = false;

		void OnEnable () {
			beamLine = GetComponent<LineRenderer>();
			if (beamLine == null)
				beamLine = gameObject.AddComponent<LineRenderer>();
			beamLine.useWorldSpace = true;
			beamLine.positionCount = 2;//.SetVertexCount(2);

			beamLine.enabled = true;
		}

		void OnDisable () {
			beamLine.enabled = false;
		}

		void FixedUpdate () {
			bool didHit = Physics.Raycast (transform.position, transform.TransformDirection( Vector3.forward), out hinfo, beamRange, hitRayMask, QueryTriggerInteraction.Ignore);
			if (didHit) {
				if(debug) {
					Debug.Log ("Beam Laser hit " + hinfo.collider.name);
					Debug.DrawRay(transform.position, hinfo.point);
				}

				beamLine.SetPosition (0, this.transform.position);
				beamLine.SetPosition (1, hinfo.point);
				hinfo.collider.gameObject.SendMessage("ModifyHealth", -damage, SendMessageOptions.DontRequireReceiver);
				if (splashPrefab != null)
					Instantiate (splashPrefab, hinfo.point, this.transform.rotation);
			}
			else {
				beamLine.SetPosition(0, this.transform.position);
				beamLine.SetPosition(1, this.transform.TransformPoint(Vector3.forward * beamRange));
			}
		}

		[Header("Available Messages")]
		public MessageHelp toggleOnHelp = new MessageHelp("ToggleOn","Turns on the beam");
		public void ToggleOn () {
			enabled = true;
		}

		public MessageHelp toggleOffHelp = new MessageHelp("ToggleOf","Turns off the beam");
		public void ToggleOff () {
			enabled = false;
		}
	}
}