using UnityEngine;
using System.Collections;

public class MessageSpawner : MonoBehaviour {

	public GameObject item;
	public GameObject spawnPoint;
	public bool inheritVelocity = true;
	public bool debug = false;

	void Start () {
		if (spawnPoint == null)
			spawnPoint = gameObject;
	}

	public void Activate () {
		Spawn ();
	}

	public void Spawn () {
		if(!enabled)
			return;
		if (debug)
			Debug.Log("Message Spawner " + gameObject.name + " spawned an " + item.name);
	
		GameObject _new = Instantiate(item,spawnPoint.transform.position,spawnPoint.transform.rotation) as GameObject;
		Rigidbody _body = GetComponent<Rigidbody>();
		Rigidbody _newBody = _new.GetComponent<Rigidbody>();
		if(inheritVelocity && (_body != null && _newBody != null))
			_newBody.AddForce( _body.velocity, ForceMode.VelocityChange);
	}


}
