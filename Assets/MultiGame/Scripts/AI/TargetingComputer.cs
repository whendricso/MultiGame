using UnityEngine;
using System.Collections;
using MultiGame;


namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Targeting Computer")]
	public class TargetingComputer : MultiModule {
		[RequiredField("The base rigidbody this is parented to. Can be kinematic, but kinematic bodies may produce less accurate results especially at high speeds.", RequiredFieldAttribute.RequirementLevels.Required)]
		public Rigidbody mainBody;
		[RequiredField("How fast does our projectile travel in meters per second (assuming 1 Unity unit == 1 meter)? Used to calculate lead distance.", RequiredFieldAttribute.RequirementLevels.Required)]
		public float shotSpeed;
		[RequiredField("Should we fire at something specific, right now? Good for scripted sequences.", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject target;
		[Tooltip("Should we turn this object so it's Z-axis faces the target, with lead time taken into account? (Recommended)")]
		public bool autoLook = true;//Automatically look at the target position?
		[Tooltip("Should we constrain X to 0 during the turn operation? If true, this object won't look up or down.")]
		public bool constrainX = false;

		public HelpInfo help = new HelpInfo("This component points it's positive Z direction at the target, automatically taking into account lead distance. " +
			"If set up correctly, this can create extremely accurate AI shooters. To make them less difficult, give them a slightly incorrect Shot Speed. Shot Speed assumes" +
			" that the projectile experiences no drag, and travels in a straight line.");

		[Tooltip("WARNING! SLOW OPERATION Should we output useful information to the console?")]
		public bool debug = false;

		void Start() {
			if (mainBody == null) {
				Debug.LogError("Targeting Computer requires a reference to a parent's rigidbody! " + gameObject.name);
				enabled = false;
				return;
			}
		}

		void FixedUpdate() {
			if ((target != null) && (autoLook)) {
				//Debug.Log("Target body: " + target.GetComponent<BodyRegister>().myBody);
				if (target.GetComponent<Rigidbody>() != null)
					transform.LookAt(FirstOrderIntercept(target.transform.GetComponent<Rigidbody>().velocity));
				else
					transform.LookAt(target.transform);
			}
			if (constrainX) {
				transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, transform.eulerAngles.z);
			}
		}

		//first-order intercept using absolute target position
		public Vector3 FirstOrderIntercept(Vector3 targetVelocity) {
			Vector3 targetRelativeVelocity = targetVelocity - mainBody.velocity;
			float t = FirstOrderInterceptTime(shotSpeed,
											target.transform.position - transform.position,
											targetRelativeVelocity);
			return target.transform.position + t * (targetRelativeVelocity);
		}

		//first-order intercept using relative target position
		public float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity) {
			float velocitySquared = targetRelativeVelocity.sqrMagnitude;
			if (velocitySquared < 0.001f)
				return 0f;

			float a = velocitySquared - shotSpeed * shotSpeed;

			//handle similar velocities
			if (Mathf.Abs(a) < 0.001f) {
				float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
				return Mathf.Max(t, 0f); //don't shoot back in time
			}

			float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition),
				c = targetRelativePosition.sqrMagnitude,
				determinant = b * b - 4f * a * c;

			if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
				float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
					t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
				if (t1 > 0f) {
					if (t2 > 0f)
						return Mathf.Min(t1, t2); //both are positive
					else
						return t1; //only t1 is positive
				}
				else
					return Mathf.Max(t2, 0f); //don't shoot back in time
			}
			else if (determinant < 0f) //determinant < 0; no intercept path
				return 0f;
			else //determinant = 0; one intercept path, pretty much never happens
				return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
		}

		public void SetTarget(GameObject tgt) {
			if (!gameObject.activeInHierarchy)
				return;
			if (debug)
				Debug.Log("Targeting Computer " + gameObject.name + " is firing at " + tgt.name);
			target = tgt;
		}

		[Header("Available Messages")]
		public MessageHelp clearTargetHelp = new MessageHelp("ClearTarget", "Causes the Targeting Computer to lose it's current target, allowing it to target something else");
		public void ClearTarget() {
			if (debug)
				Debug.Log("Targeting Computer " + gameObject.name + " is clearing it's target.");
			target = null;
		}

		public MessageHelp toggleAutoLookHelp = new MessageHelp("ToggleAutoLook", "Turns turret auto-rotation on/off which causes the turret to search for targets");
		public void ToggleAutoLook() {
			if (!gameObject.activeInHierarchy)
				return;
			ToggleAutoLook(!autoLook);
		}

		public MessageHelp toggleAutoLookBoolHelp = new MessageHelp("ToggleAutoLook", "Turns turret auto-rotation on/off which causes the turret to search for targets", 1, "Should the turret auto-look?");
		public void ToggleAutoLook(bool val) {
			if (!gameObject.activeInHierarchy)
				return;
			autoLook = val;
		}

		void ReturnFromPool() {
			ClearTarget();
		}
	}
}