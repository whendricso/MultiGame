using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class MessageMaterializer : MultiModule {
		public List<Material> materials = new List<Material>();
		public List<Renderer> meshRenderers = new List<Renderer>();

		private List<Recolorable> recolorables = new List<Recolorable>();
		public class Recolorable {
			public Renderer meshRenderer;
			public List<Material> originals = new List<Material>();

			public Recolorable(Renderer _renderer, Material[] _materials) {
				meshRenderer = _renderer;
				originals.AddRange(_materials);
			}
		}

		private void Awake() {
			if (materials.Count < 1) {
				Debug.LogError("MessageMaterializer " + gameObject.name + " doesn't have any available materials assigned!");
				enabled = false;
			}

			List<Renderer> objRends = new List<Renderer>();
			objRends.AddRange(GetComponentsInChildren<Renderer>());

			foreach (Renderer _rend in objRends) {
				recolorables.Add(new Recolorable(_rend, _rend.sharedMaterials));
			}

			if (meshRenderers.Count < 1)
				meshRenderers.AddRange( GetComponentsInChildren<MeshRenderer>());
			if (meshRenderers.Count < 1) {
				Debug.LogError("MessageMaterializer " + gameObject.name+ " doesn't have any targeted Mesh Renderer components and can't find any in it's heirarchy.");
				enabled = false;
			}
		}
	}
}