using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldDataVoxelizer : MonoBehaviour {

	public bool showEditorVoxels = true;
	public enum VoxelizationModes {ManualOnly, OnStart, EditorOnly, Both};
	public VoxelizationModes voxelizationMode = VoxelizationModes.Both;
	private bool voxelsDirty = false;
	private VInt originVoxel;

	#region dataTypes
	public class MultiVoxel {
		public bool filled;
		public bool isBorder;
		public VInt pos;

		public MultiVoxel () {
			filled = false;
			isBorder = false;
			pos = new VInt(0,0,0);
		}

		public MultiVoxel(int _x,int _y,int _z, bool _filled, bool _isBorder) {
			filled = false;
			isBorder = false;
			pos = new VInt(_x,_y,_z);
		}
	}

	public class VInt {
		public int x;
		public int y;
		public int z;

		public VInt () {
			x = 0;
			y = 0;
			z = 0;
		}

		public VInt (int _x, int _y, int _z) {
			x = _x;
			y = _y;
			z = _z;
		}
	}
	#endregion

	[HideInInspector]
	public MultiVoxel[,,] voxels;

	public Vector3 volumeSize = new Vector3(500f,500f,500f);
	[System.NonSerialized]
	public Vector3 adjustedVolumeSize;
	public int voxelsPerWorldUnit = 1;
	[System.NonSerialized]
	public float voxelSize = 1f;
	public LayerMask collisionMask;

	public void OnDrawGizmosSelected () {
		Gizmos.DrawWireCube(transform.position, volumeSize);
	}


	public void OnValidate () {
		if (voxelsPerWorldUnit < 1)
			voxelsPerWorldUnit = 1;

		voxelSize = 1f/(float)voxelsPerWorldUnit;

		if (voxelizationMode == VoxelizationModes.Both || voxelizationMode == VoxelizationModes.EditorOnly)
			voxelsDirty = true;
	}

	public void InitializeVoxelData () {
		UpdateAdjustedVolumeSize();
		voxels = new MultiVoxel[(int)(adjustedVolumeSize.x * voxelsPerWorldUnit), (int)(adjustedVolumeSize.y * voxelsPerWorldUnit), (int)(adjustedVolumeSize.z * voxelsPerWorldUnit)];

		for (int i = 0; i < (int)volumeSize.x; i++) {
			for (int j = 0; j < (int)volumeSize.y; j++) {
				for (int k = 0; k < (int)volumeSize.z; k++) {
					voxels[i,j,k] = new MultiVoxel(i,j,k, Physics.CheckSphere( VoxelSpaceToWorld(new VInt(i,j,k)), voxelSize, collisionMask),false);

				}
			}
		}

		voxelsDirty = false;
	}

	void UpdateAdjustedVolumeSize () {
		adjustedVolumeSize = new Vector3( Mathf.RoundToInt(volumeSize.x), Mathf.RoundToInt(volumeSize.y), Mathf.RoundToInt (volumeSize.z));
	}

	void UpdateOriginVoxel () {
		originVoxel = new VInt(Mathf.RoundToInt(voxels.GetUpperBound(0)/2),Mathf.RoundToInt(voxels.GetUpperBound(1)/2),Mathf.RoundToInt(voxels.GetUpperBound(2)/2));
	}

	public Vector3 VoxelSpaceToWorld (VInt _position) {
		UpdateAdjustedVolumeSize();
		Vector3 voxelSpaceZero = new Vector3(transform.position.x - adjustedVolumeSize.x/2, transform.position.y - adjustedVolumeSize.y/2, transform.position.z - adjustedVolumeSize.z/2);

		return new Vector3(voxelSpaceZero.x + (_position.x * voxelSize), voxelSpaceZero.y + (_position.y * voxelSize), voxelSpaceZero.z + (_position.z * voxelSize));
	}

	//returns null if the position is outside of the volume
	public VInt WorldSpaceToVoxel (Vector3 _position) {
		UpdateAdjustedVolumeSize();
		if (_position.x < transform.position.x - adjustedVolumeSize.x/2 || _position.x > transform.position.x + adjustedVolumeSize.x/2)
			return null;
		if (_position.y < transform.position.y - adjustedVolumeSize.y/2 || _position.y > transform.position.y + adjustedVolumeSize.y/2)
			return null;
		if (_position.z < transform.position.z - adjustedVolumeSize.z/2 || _position.z > transform.position.z + adjustedVolumeSize.z/2)
			return null;

		//TODO: return the voxel space coordinate transformed from world space
//		int potentialX = _position.x ;
//		int potentialY;
//		int potentialZ;

		voxels.GetUpperBound(0);

		return new VInt();
	}

	public VInt GetOriginVoxelPosition () {
		UpdateAdjustedVolumeSize();
		VInt _pos;

		_pos = new VInt( Mathf.RoundToInt(adjustedVolumeSize.x/2), Mathf.RoundToInt(adjustedVolumeSize.y/2), Mathf.RoundToInt(adjustedVolumeSize.z/2));

		return _pos;
	}

	public void UpdateVoxel (VInt _position) {

	}

}
