using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class PhotonBullet : Photon.MonoBehaviour {

	[System.NonSerialized]
	public Rigidbody rigid;
	
	void Start () {
		rigid = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		
	}
}
