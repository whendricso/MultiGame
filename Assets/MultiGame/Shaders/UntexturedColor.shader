Shader "MultiGame/UltraFast/UntexturedColor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		
		Pass {
			Color [_Color]
		
		}
	} 
}
