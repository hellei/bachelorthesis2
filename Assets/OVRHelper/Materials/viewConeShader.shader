Shader "OVR/ViewCone" {
 
Properties { 
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (A=Opacity)", 2D) = "" {}
    _DistanceScale ("Distance Scale", Float) = 0.5
    _CameraPos ("Camera Position", Vector) = (0,0,0,0)
}
 
Category { 
	//AlphaTest 0.5
	ZWrite On
 
    SubShader {Pass {
 
        CGPROGRAM
        #pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
 
 		uniform sampler2D _MainTex;
 		uniform float4 _MainTex_ST; 
		uniform float4 _Color;
        uniform float _DistanceScale;
        uniform float4 _CameraPos;
 
		struct appdata
		{
		    float4 vertex : POSITION;
		    float2 texcoord : TEXCOORD0;
		};
		 
		struct v2f 
		{
		    float4 sv : SV_POSITION;
		    float2 uv : TEXCOORD0;
		    float3 pos : TEXCOORD1;
		    
		};        
        
        v2f vert(appdata v) 
        { 
        
            v2f o;
		    o.sv = mul(UNITY_MATRIX_MVP, v.vertex);
        	o.pos = mul(_Object2World, v.vertex).xyz;
            o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
            
            return o;
        }
 
        float4 frag(v2f i) : COLOR
        { 
            float4 fragColor = float4(1, 1, 1, 1);
            fragColor.rgb = _Color.rgb;
            
            float4 mainTex = tex2D(_MainTex, i.uv);
            float mask = mainTex.r - saturate(length(i.pos - _CameraPos.xyz) / _DistanceScale);
            
            clip(mask);
            
            //fragColor.rgb *= saturate(mask);
            
            return fragColor;
        }    
 
        ENDCG
 
    }}
 
 
 
    Fallback Off 
    }}