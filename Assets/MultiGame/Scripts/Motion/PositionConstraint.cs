using UnityEngine;
using System.Collections;

public class PositionConstraint : MultiModule {

	public bool constrainX = false;
	public float maxX = 0;
	public float minX = 0;
	public bool constrainY = false;
	public float maxY = 0;
	public float minY = 0;
	public bool constrainZ = false;
	public float maxZ = 0;
	public float minZ = 0;

	public HelpInfo help = new HelpInfo("This component forces a non-rigidbody into certain position constraints. Useful for 2.5D games.");

	void LateUpdate () {
		float _x = transform.position.x;
		float _y = transform.position.y;
		float _z = transform.position.z;

		if (constrainX)
			Mathf.Clamp(_x, minX, maxX);
		if (constrainY)
			Mathf.Clamp(_y, minY, maxY);
		if (constrainZ)
			Mathf.Clamp(_z, minZ, maxZ);

		transform.position = new Vector3(_x, _y, _z);
	}
}
