Shader "MultiGame/UltraFast/UntexturedLitSpecularVertexColor" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Spec Color", Color) = (1,1,1,1)
		_Emission("Emmisive Color", Color) = (0,0,0,0)
		_Shininess("Shininess", Range(0.01, 1)) = 0.7
	}

	SubShader{
		Pass{
			Material{
				Shininess[_Shininess]
				Specular[_SpecColor]
				Emission[_Emission]
			}
			ColorMaterial AmbientAndDiffuse
			Lighting On
			SeparateSpecular On
			SetTexture[_]{
				Combine primary DOUBLE, primary * primary
			}
			//SetTexture[_]{
				//constantColor[_Color]
				//Combine previous * constant DOUBLE, previous * constant
			//}
		}
	}

		Fallback " VertexLit", 1
} 