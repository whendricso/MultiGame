using UnityEngine;
using System.Collections;


/// <summary>
/// Wall section
/// </summary>
public class WallSection : MonoBehaviour {
	
	//wall sections
	public GameObject lone;//prefab for a lone wall section with no neighbors.
	public GameObject inlineConnection;//prefab for a wall that is connected in a line
	public GameObject tIntersection;//the short road should run down the relative -z
	public GameObject intersection;//a 4-way intersection
	public GameObject dogLeg;//L-bend, with the roads going down the relative -Z and +X axes
	
	public Vector3 raycastOffset = Vector3.up;
	public float rayLength = 2.0f;
	public LayerMask rayMask;

	public bool debug = false;

	private bool pN, pS, pE, pW = false;//previous values, to check for changes
	private bool n, s, e, w = false;

	private GameObject myWall;
	
	void Start () {
		Debug.Log("" + pN);
		if (rayLength < 0) {
			rayLength = Mathf.Infinity;
		}
		myWall = Instantiate(lone, transform.position, transform.rotation) as GameObject;
		StartCoroutine(UpdateWallSection());
	}

	public void UpdateWall () {
		StartCoroutine(UpdateWallSection());
	}
	
	public IEnumerator UpdateWallSection () {
		yield return new WaitForFixedUpdate();
		if(CheckForChanges()) {
			if (myWall != null)
				Destroy(myWall);

			//test how many neighbors we have
			int _neighbors = 0;
			if (n)
				_neighbors++;
			if (s)
				_neighbors++;
			if (e)
				_neighbors++;
			if (w)
				_neighbors++;

			switch (_neighbors) {
			case 0:
				if ((!n && !s) && (!e && !w))
					myWall = Instantiate(lone, transform.position, transform.rotation) as GameObject;
				break;
			case 1:
				myWall = Instantiate(lone, transform.position, transform.rotation) as GameObject;
				if (e | w)
					myWall.transform.RotateAround(transform.position, Vector3.up, 90.0f);
				break;
			case 2:
				//if we are in-line with both neighbors...
				if (((n && s) && (!e && !w)) || ((!n && !s) && (e && w))) {
					myWall = Instantiate(inlineConnection, transform.position, transform.rotation) as GameObject;
					if ((!n && !s) && (e && w))
						myWall.transform.RotateAround(transform.position, Vector3.up, 90.0f);
				}
				else {//otherwise, dog leg
					myWall = Instantiate(dogLeg, transform.position, transform.rotation) as GameObject;
					if (s) {
						if (w)
							myWall.transform.RotateAround(transform.position, Vector3.up, 90.0f);
					}
					if (n) {
						if (e)
							myWall.transform.RotateAround(transform.position, Vector3.up, 270.0f);
						else
							myWall.transform.RotateAround(transform.position, Vector3.up, 180.0f);
					}
				}
				break;
			case 3:
				myWall = Instantiate(tIntersection, transform.position, transform.rotation) as GameObject;
				if (!n)
					myWall.transform.RotateAround(transform.position, Vector3.up, 270.0f);
				if (!s)
					myWall.transform.RotateAround(transform.position, Vector3.up, 90.0f);
				if (!w)
					myWall.transform.RotateAround(transform.position, Vector3.up, 180.0f);
				break;
			case 4:
				if ((n && s) && (e && w))
					myWall = Instantiate(intersection, transform.position, transform.rotation) as GameObject;
				break;

			}
			myWall.transform.parent = transform;

			if (debug)
				Debug.Log("Wall selected: " + myWall.name);
		}
	}

	private void CheckNeighbors () {
		n = Physics.Raycast(transform.position + raycastOffset, Vector3.forward, rayLength, rayMask);
		s = Physics.Raycast(transform.position + raycastOffset, -Vector3.forward, rayLength, rayMask);
		e = Physics.Raycast(transform.position + raycastOffset, Vector3.right, rayLength, rayMask);
		w = Physics.Raycast(transform.position + raycastOffset, -Vector3.right, rayLength, rayMask);
	}

	private bool CheckForChanges () {
		bool ret = false;
		CheckNeighbors();

		if (pN != n)
			ret = true;
		if (pS != s)
			ret = true;
		if (pE != e)
			ret = true;
		if (pW != w)
			ret = true;

		pN = n;
		pS = s;
		pE = e;
		pW = w;

		return ret;
	}

}
