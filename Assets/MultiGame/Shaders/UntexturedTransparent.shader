Shader "MultiGame/UltraFast/UntexturedTransparent" {
	Properties {
		_Emission ("Emission", Color) = (0,0,0,1)
	    _Color ("Main Color", Color) = (1,1,1,1)
	    _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	    _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	     
	}
	
	SubShader {
	    Tags {"RenderType"="Transparent" "Queue"="Transparent"}
	    // Render into depth buffer only
	    Pass {
	        ColorMask 0
	    }
	    // Render normally
	    Pass {
	        ZWrite Off
	        Blend SrcAlpha OneMinusSrcAlpha
	        ColorMask RGB
	        Material {
				Emission [_Emission]
	            Diffuse [_Color]
	            Ambient [_Color]
            	Shininess [_Shininess]
	            Specular [_SpecColor]
	        }
	        Lighting On
			SeparateSpecular On
	        SetTexture [_] {
	            Combine primary DOUBLE, primary * primary
	        } 
	    }
	}
}