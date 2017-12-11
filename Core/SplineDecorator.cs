using UnityEngine;
using System;
using System.Collections;
using MultiGame;

namespace MultiGame {
	[ExecuteInEditMode]
	public class SplineDecorator : MonoBehaviour {

		public enum InstantiationModes {Awake, Editor};
		public enum ScalingModes {Vector, Radial};

		[Header("Essential Settings")]
		[RequiredFieldAttribute("A reference to the spline component")]
		public BezierSpline spline;
		[Tooltip("A list of decorations we want to distribute along the spline")]
		public GameObject[] decorations;

		[Header("Instantiation Settings")]
		[Tooltip("Should we create these decorations in the editor, or at runtime?")]
		public InstantiationModes instantiationMode = InstantiationModes.Editor;
		[Tooltip("Should we select the decorations randomly? If not, we will cycle through them in sequence instead.")]
		public bool randomize = false;
		[RequiredFieldAttribute("How many times should we place each decoration on the spline?", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public int frequency = 1;
		[Tooltip("How much should we vary the spacing of each object?")]
		[Range(0f,1f)]
		public float linearJitter = 0f;
		[Tooltip("How much should we randomize the object's position after placement on each axis in local coordinates?")]
		public Vector3 jitterVector = Vector3.zero;
		[Tooltip("How much should we vary each object's rotation on each axis?")]
		public Vector3 rotationalJitter = Vector3.zero;

		[Header("Scaling Settings")]
		[Tooltip("If set to Radial, only X and Y are taken into account, Z scale will be set to the X value, eliminating horizontal stretching.")]
		public ScalingModes scalingMode = ScalingModes.Vector;
		[Tooltip("Decorations' minimum size in 3 axes when randomized. Set all of these and Maximum Scale to 1 to disable randomization. Randomization breaks batching for these objects.")]
		public Vector3 minimumScale = Vector3.one;
		[Tooltip("Decorations' maximum size in 3 axes when randomized. Set all of these and Minimum Scale to 1 to disable randomization. Randomization breaks batching for these objects.")]
		public Vector3 maximumScale = Vector3.one;
		[Tooltip("Should each decoration change it's orientation to face the spline direction?")]
		public bool lookForward = false;

		[System.NonSerialized]
		public GameObject[] instantiated;

		[BoolButton]
		public bool refreshDecorations = false;
		private float radius = 0f;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Spline Decorator allows objects to be placed procedurally along a spline that you define in the editor.\n" +
			"\n" +
			"To use, add some 'Decorations' and increase the 'Frequency' until you reach the desired amount. Spacing is influenced by the distance between nodes. Next, you can set some jitter " +
			"options, if you are looking for a mor organic distribution (such as rocks near a road)");

		private void Awake () {
			if (spline == null)
				spline = GetComponentInChildren<BezierSpline>();
			if (spline == null) {
				Debug.LogError("Spline Decorator " + gameObject.name + " requires a Bezier Spline component! Please attatch one to this object or assign one in the Inspector.");
				enabled = false;
				return;
			}
			if (instantiationMode == InstantiationModes.Awake)
				Decorate();
		}

		void OnValidate () {
			if (spline == null)
				spline = GetComponentInChildren<BezierSpline>();
			if(instantiationMode == InstantiationModes.Editor)
				refreshDecorations = true;
		}

		void Update () {
			if (!refreshDecorations)
				return;
			Decorate();
		}

		void Decorate () {
			refreshDecorations = false;
//			for (int j = 0; j < decorations.Length; j++) {
//				if (decorations[j] == null)
//					return;
//			}
			ClearInstances();

			if (frequency <= 0 || decorations == null || decorations.Length == 0) {
				return;
			}
			float stepSize = frequency * decorations.Length;
			if (spline.Loop || stepSize == 1) {
				stepSize = 1f / stepSize;
			}
			else {
				stepSize = 1f / (stepSize - 1);
			}
			if (frequency < 0)
				return;
			for (int p = 0, f = 0; f < frequency; f++) {
				for (int i = 0; i < decorations.Length; i++, p++) {
					if (decorations[i] == null)
						break;
					GameObject decoration;
					#if UNITY_EDITOR
					if (instantiationMode == InstantiationModes.Editor)
						decoration = UnityEditor.PrefabUtility.InstantiatePrefab(decorations[i]) as GameObject;
					else
					#endif
						decoration = Instantiate(decorations[i]) as GameObject;
					if (instantiated == null || instantiated.Length < 1) {
						instantiated = new GameObject[frequency];
						instantiated[i] = decoration;
					}
					else {
						Array.Resize<GameObject>(ref instantiated, i + 1);
						instantiated[i] = decoration;
					}
					Vector3 position = spline.GetPoint(p * stepSize + UnityEngine.Random.Range(-linearJitter, linearJitter));
					decoration.transform.localPosition = position;
					decoration.transform.Translate(new Vector3(UnityEngine.Random.Range(-jitterVector.x, jitterVector.x),UnityEngine.Random.Range(-jitterVector.y, jitterVector.y),UnityEngine.Random.Range(-jitterVector.z, jitterVector.z)));
					if (lookForward) {
						decoration.transform.LookAt(position + spline.GetDirection(p * stepSize));
					}
					decoration.transform.localRotation = Quaternion.Euler(new Vector3(decoration.transform.localRotation.eulerAngles.x + UnityEngine.Random.Range(-rotationalJitter.x, rotationalJitter.x),decoration.transform.localRotation.eulerAngles.y + UnityEngine.Random.Range(-rotationalJitter.y, rotationalJitter.y),decoration.transform.localRotation.eulerAngles.z + UnityEngine.Random.Range(-rotationalJitter.z, rotationalJitter.z)));
					decoration.transform.parent = transform;

					if (minimumScale != Vector3.one || maximumScale != Vector3.one) {
						if (scalingMode == ScalingModes.Vector)
							decoration.transform.localScale = new Vector3 (UnityEngine.Random.Range (minimumScale.x, maximumScale.x), UnityEngine.Random.Range (minimumScale.y, maximumScale.y), UnityEngine.Random.Range (minimumScale.z, maximumScale.z));
						if (scalingMode == ScalingModes.Radial) {
							radius = UnityEngine.Random.Range (minimumScale.x, maximumScale.x);
							decoration.transform.localScale = new Vector3 (radius, UnityEngine.Random.Range (minimumScale.y, maximumScale.y), radius);
					
						}
					}
				}
			}

		}

		void ClearInstances () {
			Transform[] children = transform.GetComponentsInChildren<Transform>();
			for (int i = 0; i < children.Length; i++) {
				if (children[i] != null && children[i].gameObject != this.gameObject)
					DestroyImmediate(children[i].gameObject);
			}
//			if (instantiated == null)
//				return;
//			foreach (GameObject deco in instantiated) {
//				if (deco != this.gameObject)
//					DestroyImmediate(deco);
//			}
//			instantiated = null;
		}
	}
}