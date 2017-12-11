using UnityEditor;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	[CustomEditor(typeof(BezierNode))]
	public class BezierCurveInspector : Editor {

		private const int lineSteps = 10;
		private const float directionScale = 0.5f;

		private BezierNode node;
		private Transform handleTransform;
		private Quaternion handleRotation;

		private void OnSceneGUI () {
			node = target as BezierNode;
			handleTransform = node.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ?
				handleTransform.rotation : Quaternion.identity;
			
			Vector3 p0 = ShowPoint(0);
			Vector3 p1 = ShowPoint(1);
			Vector3 p2 = ShowPoint(2);
			Vector3 p3 = ShowPoint(3);
			
			Handles.color = Color.gray;
			Handles.DrawLine(p0, p1);
			Handles.DrawLine(p2, p3);
			
			ShowDirections();
			Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
		}

		private void ShowDirections () {
			Handles.color = Color.green;
			Vector3 point = node.GetPoint(0f);
			Handles.DrawLine(point, point + node.GetDirection(0f) * directionScale);
			for (int i = 1; i <= lineSteps; i++) {
				point = node.GetPoint(i / (float)lineSteps);
				Handles.DrawLine(point, point + node.GetDirection(i / (float)lineSteps) * directionScale);
			}
		}

		private Vector3 ShowPoint (int index) {
			Vector3 point = handleTransform.TransformPoint(node.points[index]);
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(node, "Move Point");
				EditorUtility.SetDirty(node);
				node.points[index] = handleTransform.InverseTransformPoint(point);
			}
			return point;
		}
	}
}