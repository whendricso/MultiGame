using UnityEngine;
using System.Collections;
using MultiGame;
using UnityEditor;

namespace MultiGame {

	public class ProceduralSimpleton : MultiModule {

		public HelpInfo help = new HelpInfo("A procedurally-generated parametric object helper.");


		public static void CreateNewPrefab (GameObject _sourceObj, string _prefabFolderPath) {
			Object pf = PrefabUtility.CreateEmptyPrefab(_prefabFolderPath + _sourceObj.name + ".prefab"); 
			
			//make sure we never deal with "(clone)" objects
			if (_sourceObj.GetComponent<CloneFlagRemover>() == null)
				_sourceObj.AddComponent<CloneFlagRemover>();
			
			PrefabUtility.ReplacePrefab(_sourceObj, pf, ReplacePrefabOptions.ConnectToPrefab);
		}

	}
}