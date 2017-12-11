using UnityEngine;
using System.Collections;
//using UnityStandardAssets.Characters.FirstPerson;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Motion/Mouse Aim")]
	public class MouseAim : MultiModule {

		public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
		[Header("Axis Settings")]
		public RotationAxes axes = RotationAxes.MouseXAndY;
		public float sensitivityX = 2f;
		public float sensitivityY = 2F;
		
		[Header("Constraints")]
		public float minimumX = -360F;
		public float maximumX = 360F;
		
		public float minimumY = -60F;
		public float maximumY = 60F;
		
		[HideInInspector]
		public bool rotateOn = false;
		
		float rotationY = 0F;
		
		private bool lockCursor = false;

		public HelpInfo help = new HelpInfo("This component functions like the old MouseLook component from previous versions of Unity. The object will rotate without constraint " +
			"based on the settings you supply. If it's attached to a 'Smart Cam' component, then that component controls it's activation/deactivation directly. It only works when the cursor is locked.");

		void Start () {
			// Make the rigid body not change rotation
			if (GetComponent<Rigidbody>())
				GetComponent<Rigidbody>().freezeRotation = true;
		}

		void Update () {
			bool lastLockCursor = lockCursor;
			lockCursor = true;
			
	//		Builder builder = GetComponent<Builder>();
	//		if (builder != null)
	//			lockCursor = builder.LockCursor;
			
			if (lockCursor) {
				rotateOn = true;
			}
			else {
				if (Input.GetMouseButtonDown (1)) {
					rotateOn = true;
					Cursor.visible = false;
					Screen.lockCursor = true;
				}
				else if (lastLockCursor || Input.GetMouseButtonUp (1)) {
					rotateOn = false;
					Cursor.visible = true;
					Screen.lockCursor = false;
				}
			}
			
			
			if (rotateOn) {
				if (axes == RotationAxes.MouseXAndY) {
					float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
					
					rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
					rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
					
					transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
				}
				else if (axes == RotationAxes.MouseX) {
					transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
				}
				else {
					rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
					rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
					
					transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
				}
			}
		}
		

	}
}