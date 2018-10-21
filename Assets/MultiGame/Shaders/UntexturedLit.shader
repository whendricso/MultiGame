Shader "MultiGame/UltraFast/UntexturedLit" {
	Properties {
		//_Color ("Color", Color) = (1,1,1,1)
		_Emission ("Emission", Color) = (0,0,0,1)
		_Ambient ("Ambient", Color) = (0,0,0,1)
		_Diffuse ("Diffuse", Color) = (1,1,1,1)
	}
	SubShader {
		
		Lighting On
		Material {
			Emission [_Emission]
			Ambient [_Ambient]
			Diffuse[_Diffuse]
		}
		
		Pass {
			Color[_Diffuse]
			SetTexture[_] {
				Combine primary DOUBLE
			}
		}
	} 
	//FallBack "Custom/UntexturedColor"
}
