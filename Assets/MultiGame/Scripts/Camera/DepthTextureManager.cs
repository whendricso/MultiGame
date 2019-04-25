using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class DepthTextureManager : MultiModule  {

		[Header("Modality")]
		[Tooltip("Default depth texture mode for this camera")]
		public DepthTextureMode depthMode = DepthTextureMode.None;
		Camera cam;

		public HelpInfo help = new HelpInfo("Depth Texture Manager allows control over the depth texture status of a camera via the editor and/or messages. If you don't know what these are, " +
			"you probably don't need them.");

		void OnEnable () {
			if (cam == null)
				cam = GetComponentInChildren<Camera>();
			if (cam != null) {
				cam.depthTextureMode = depthMode;
			} else {
				Debug.LogError("Depth Texture Manager " + gameObject.name + " could not find an attached camera!");
			}
		}

		[Header("Available Messages")]
		public MessageHelp enableDepthTexturesHelp = new MessageHelp("EnableDepthTextures","Causes any camera attached to this object (or it's children) to begin writing depth textures which can be used for some effects");
		public void EnableDepthTextures () {
			cam.depthTextureMode = DepthTextureMode.Depth;
		}

		public MessageHelp enableDepthNormalTexturesHelp = new MessageHelp("EnableDepthNormalTextures","Causes any camera attached to this object (or it's children) to begin writing depth textures and normals which can be used for some effects");
		public void EnableDepthNormalTextures () {
			cam.depthTextureMode = DepthTextureMode.DepthNormals;
		}

		public MessageHelp disableDepthTexturesHelp = new MessageHelp("DisableDepthTextures","Causes any camera attached to this object (or it's children) to stop writing all depth/normal data");
		public void DisableDepthTextures () {
			cam.depthTextureMode = DepthTextureMode.None;
		}

	}
}