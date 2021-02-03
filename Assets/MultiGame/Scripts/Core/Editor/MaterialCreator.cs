using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using MultiGame;

namespace MultiGame {

	public class MaterialCreator : MGEditor {
#if UNITY_EDITOR
		//private static Texture2D paintIcon;
#endif
		public static Material mat;
		public static string matName;
		public static Color color = new Color(1,1,1,1);
		public XKCDDesignerColors designerColor = XKCDDesignerColors.EggShell;
		public static Gradient gradient = new Gradient();
		/// <summary>
		/// Used for the scale of circles and size of boxes in gradients
		/// </summary>
		public static float radius = 1;
		public enum GradientDirections { Up, Down, Left, Right, Radial, DiagonalR, DiagonalL, Diamond, Square };
		public static GradientDirections gradientDirection = GradientDirections.Down;
		public static Texture2D albedo;
		public static NoiseSample noiseSample;

		private const int MAX_PREVIEW_SIZE = 512;
		private static int previewSize = 512;

		//noise settings
		[System.Serializable]
		public class NoiseSample {
			public float xOrigin;
			public float yOrigin;
			public float xAmplitude;
			public float yAmplitude;


			public NoiseSample(float _xOrigin, float _yOrigin, float _xAmplitude, float _yAmplitude) {
				xOrigin = _xOrigin;
				yOrigin = _yOrigin;
				xAmplitude = _xAmplitude;
				yAmplitude = _yAmplitude;
			}

			public float GetNoiseValue() {
				return Mathf.PerlinNoise(xOrigin * xAmplitude, yOrigin * yAmplitude);
			}

			public float GetNoiseValue(float xOffset, float yOffset) {
				return Mathf.PerlinNoise((xOrigin+xOffset) * xAmplitude, (yOrigin+yOffset) * yAmplitude);
			}
		}

		//public Texture2D normal;
		//public Texture2D height;
		//public Texture2D occlusion;

		public enum MaterialTypes { FlatColor, Gradient, Noise, Skybox};
		public MaterialTypes materialType = MaterialTypes.FlatColor;
		public enum Sizes { _4,_256,_512,_1024,_2048,_4096};
		public Sizes size = Sizes._512;

		[MenuItem("MultiGame/Material Creator")]
		static void ShowWindow() {
			EditorWindow creator = EditorWindow.GetWindow(typeof(MaterialCreator),true);
			creator.minSize = new Vector2(320, 320);
			//paintIcon = AssetDatabase.LoadAssetAtPath("Assets/MultiGame/Editor/Icons/Paintbrush.png", typeof(Texture2D)) as Texture2D;

			noiseSample = new NoiseSample(Random.Range(0,10000), Random.Range(0, 10000), .004f, .004f);

			//create a default gradient
			if (gradient == null || gradient.colorKeys.Length < 1) {
				gradient.SetKeys(
					new GradientColorKey[] {
						new GradientColorKey(color,0),
						new GradientColorKey(color,1)
					},
					new GradientAlphaKey[] {
						new GradientAlphaKey(1,0),
						new GradientAlphaKey(0, 1)
					}
				);
			}

		}

		void OnGUI() {
			mat = EditorGUILayout.ObjectField(mat, typeof(Material), false) as Material;
			matName = EditorGUILayout.TextField("Material Name",matName);
			previewSize = Mathf.Min(512, SizeFromSizes(size));
			if (string.IsNullOrEmpty(matName))
				matName = "Material" + Random.Range(0, 10000);
			EditorGUILayout.Space();
			if (GUILayout.Button("New Material")) {
				CreateMaterial();
			}
			EditorGUILayout.Separator();
			materialType = (MaterialTypes)EditorGUILayout.EnumPopup("Material Type", materialType);
			size = (Sizes)EditorGUILayout.EnumPopup("Texture Size",size);
			EditorGUILayout.Separator();
			if (albedo != null)
				EditorGUI.DrawPreviewTexture(//draw the texture in the window with a max size of 512
					EditorGUILayout.GetControlRect( GUILayout.Width(previewSize),GUILayout.Height(previewSize) ),//location where we draw and size
					albedo//the texture we're drawing
				);

			GUILayout.FlexibleSpace();

			ShowModalGUI();

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Refresh",GUILayout.Height(32), GUILayout.ExpandWidth(true)))
				WriteTextures();
		}


		void ShowModalGUI() {
			switch (materialType) {
				case MaterialTypes.FlatColor:
					color = EditorGUILayout.ColorField("Color", color);
					break;
				case MaterialTypes.Gradient:
					gradient = EditorGUILayout.GradientField("Gradient", gradient);
					gradientDirection = (GradientDirections)EditorGUILayout.EnumPopup("Gradient Direction", gradientDirection);
					if (gradientDirection == GradientDirections.Radial)
						radius = EditorGUILayout.FloatField("Circle Density", radius);
					if (gradientDirection == GradientDirections.Square || gradientDirection == GradientDirections.Diamond)
						radius = EditorGUILayout.FloatField("Box Density", radius);

					break;
				case MaterialTypes.Noise:
					if (noiseSample == null)
						noiseSample = new NoiseSample(Random.Range(0,10000), Random.Range(0, 10000), .004f, .004f);

					noiseSample.xOrigin = EditorGUILayout.FloatField("X Seed",noiseSample.xOrigin);
					noiseSample.yOrigin = EditorGUILayout.FloatField("Y Seed",noiseSample.yOrigin);
					noiseSample.xAmplitude = EditorGUILayout.FloatField("X Amplitude",noiseSample.xAmplitude);
					noiseSample.yAmplitude = EditorGUILayout.FloatField("Y Amplitude",noiseSample.yAmplitude);
					gradient = EditorGUILayout.GradientField("Color Ramp", gradient);
					break;
				case MaterialTypes.Skybox:
					gradient = EditorGUILayout.GradientField("Gradient", gradient);
					break;
			}
		}

		void WriteTextures() {
			Debug.Log("Writing textures for material " + matName);

			//first, populate the maps in RAM
			/*albedo = new Texture2D(SizeFromSizes(size), SizeFromSizes(size), TextureFormat.ARGB32, false);
			albedo.name = matName + "_abledo";
			*/
			//Next, draw on them
			switch (materialType) {
				case MaterialTypes.FlatColor:
					DrawFlatColor(color);
					break;
				case MaterialTypes.Gradient:
					DrawGradient(gradient);
					break;
				case MaterialTypes.Noise:
					DrawNoise();
					break;
				case MaterialTypes.Skybox:
					DrawSkybox();
					break;
				default:
					DrawFlatColor(color);
					break;
			}

			//Then, write the data to disk
			byte[] albedoBytes = albedo.EncodeToPNG();
			File.WriteAllBytes(AssetDatabase.GetAssetPath(mat) + "_albedo.png", albedoBytes);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mat) + "_albedo.png");
			mat.mainTexture = albedo;
			
		}

		void DrawSkybox() {
			float _yPartial = 0f;
			for (int x = 0; x < SizeFromSizes(size); x++) {
				for (int y = 0; y < SizeFromSizes(size); y++) {
					_yPartial = (float)y;
					albedo.SetPixel(x, y, gradient.Evaluate(_yPartial / (float)SizeFromSizes(size)));
				}
			}
			albedo.Apply();
		}

		void DrawFlatColor(Color _color) {
			for (int x = 0; x < SizeFromSizes(size); x++) {
				for (int y = 0; y < SizeFromSizes(size); y++) {
					albedo.SetPixel(x, y, _color);
				}
			}
			albedo.Apply();
		}

		void DrawGradient(Gradient _gradient) {
			//a squared + b squared = c squared
			//Thanks, Pythagoras! Radial gradients would be impossible without you!

			float _center = ((float)SizeFromSizes(size)) * 0.5f;
			float _xDistance = 0;//a
			float _yDistance = 0;//b

			float _xPartial = 0f;//floating point members, to prevent the compiler from casting to int
			float _yPartial = 0f;

			for (int x = 0; x < SizeFromSizes(size); x++) {
				for (int y = 0; y < SizeFromSizes(size); y++) {

					_xPartial = (float)x;
					_yPartial = (float)y;

					switch (gradientDirection) {
						default:
							albedo.SetPixel(x, y, _gradient.Evaluate(_yPartial/(float)SizeFromSizes(size)));
							break;
						case GradientDirections.Up:
							albedo.SetPixel(x, y,_gradient.Evaluate(1f - _yPartial / (float)SizeFromSizes(size)));
							break;
						case GradientDirections.Left:
							albedo.SetPixel(x, y, _gradient.Evaluate(_xPartial/ (float)SizeFromSizes(size)));
							break;
						case GradientDirections.Right:
							albedo.SetPixel(x, y,_gradient.Evaluate(1f - _xPartial / (float)SizeFromSizes(size)));
							break;
						case GradientDirections.Radial:
							_xDistance = Mathf.Abs(_xPartial - _center) * radius;
							_yDistance = Mathf.Abs(_yPartial - _center) * radius;
							albedo.SetPixel(x, y, _gradient.Evaluate( Mathf.Sqrt(Mathf.Pow(_xDistance,2) + Mathf.Pow( _yDistance,2))/(float)SizeFromSizes(size) ));//c
							break;
						case GradientDirections.DiagonalR:
							_xDistance = _xPartial / (float)SizeFromSizes(size);
							_yDistance = _yPartial / (float)SizeFromSizes(size);
							albedo.SetPixel(x, y, _gradient.Evaluate((_xDistance+_yDistance)*.5f));
							break;
						case GradientDirections.DiagonalL:
							_xDistance = _xPartial / (float)SizeFromSizes(size);
							_yDistance = _yPartial / (float)SizeFromSizes(size);
							albedo.SetPixel(x, y, _gradient.Evaluate( ((1-_xDistance) + (_yDistance)) * .5f) );
							break;
						case GradientDirections.Diamond:
							_xDistance = Mathf.Abs(_xPartial - _center) * radius;
							_yDistance = Mathf.Abs(_yPartial - _center) * radius;
							albedo.SetPixel(x, y, _gradient.Evaluate(   ((_xDistance + _yDistance) * .5f) / (float)SizeFromSizes(size)   ));
							break;
						case GradientDirections.Square:
							_xDistance = Mathf.Abs(_xPartial - _center) / (float)SizeFromSizes(size) * radius;
							_yDistance = Mathf.Abs(_yPartial - _center) / (float)SizeFromSizes(size) * radius;
							albedo.SetPixel(x,y, _gradient.Evaluate(Mathf.Max(_xDistance, _yDistance)));
							break;
					}
				}
			}
			albedo.Apply();
		}

		void DrawNoise() {
			for (int x = 0; x < SizeFromSizes(size); x++) {
				for (int y = 0; y < SizeFromSizes(size); y++) {
					albedo.SetPixel(x,y,gradient.Evaluate( noiseSample.GetNoiseValue(x,y)));
				}
			}
			albedo.Apply();
		}

		/// <summary>
		/// Get an integer value from the Sizes enum
		/// </summary>
		/// <param name="_size">the 'Sizes' value we wish to convert to integer.</param>
		/// <returns></returns>
		int SizeFromSizes(Sizes _size) {
			switch (_size) {
				case Sizes._4:
					return 4;
				case Sizes._256:
					return 256;
				case Sizes._1024:
					return 1024;
				case Sizes._2048:
					return 2048;
				case Sizes._4096:
					return 4096;
				default:
					return 512;
			}
		}

		void CreateMaterial() {
			if (!Directory.Exists(Application.dataPath + "/Generated/TexMat/"))
				Directory.CreateDirectory(Application.dataPath + "/Generated/TexMat/");

			if (materialType == MaterialTypes.Skybox) {
				mat = new Material(Shader.Find("Skybox/Panoramic"));
			} else
				mat = new Material(Shader.Find("Standard"));
			mat.name = matName;
			albedo = new Texture2D(SizeFromSizes(size), SizeFromSizes(size), TextureFormat.ARGB32, false);
			albedo.name = matName + "_abledo";

			byte[] albedoBytes = albedo.EncodeToPNG();
			AssetDatabase.CreateAsset(mat, "Assets/Generated/TexMat/" + matName + ".mat");
			Debug.Log("Creating Albedo texture " + AssetDatabase.GetAssetPath(mat) + "_albedo.png");

			File.WriteAllBytes(AssetDatabase.GetAssetPath(mat) + "_albedo.png", albedoBytes);

			AssetDatabase.SaveAssets();
			//AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mat) + "_albedo.png");
		}
	}
}