Shader "BO/BillboardGrass" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Rotation("Rotation", Range(0,1)) = 0
		_AnimAmp("Anim Intencity", float) = 1
		_Freq("Anim Freq", float) = 1
		_SpatialFreq("Anim Spatial Freq (X, Y). Z & W not used.", Vector) = (1,1,1,1)
		_Cutoff ("Cutoff", Range(0,1)) = 0.5
	}
	SubShader {
		Tags
		{
			"Queue"="Geometry-55"
			"RenderType"="Opaque"
		}
		LOD 200

		Cull Off

		CGPROGRAM
		
		#pragma surface surf Grass vertex:VertexGrass alphatest:_Cutoff addshadow halfasview nolightmap noforwardadd
		
		sampler2D _MainTex;
		float     _Rotation;
		float     _AnimAmp;
		float     _Freq;
		float4    _SpatialFreq;
        
        float g_WindSpeed;
        float g_WindFreq;

		struct Input {
			float2 uv_MainTex;
			float4 color;
			float3 worldPos;
		};

		void VertexGrass(inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color;
			
			float3 rightAxis = UNITY_MATRIX_IT_MV[0].xyz;
			rightAxis += _Rotation * (2 * v.color.w - 1) * float3(UNITY_MATRIX_IT_MV[2].x, 0, UNITY_MATRIX_IT_MV[2].z);
			rightAxis = normalize(rightAxis);
			
			float3 upAxis = UNITY_MATRIX_IT_MV[1].xyz;
			
			//fixed baseLength = v.vertex.z;

			float4 worldPos = mul(_Object2World, v.vertex);
			fixed windFreq = pow(1, 5) * 5;// * 2; //10 to compensate for _Time.x being time scaled down by 20
			float animValue = _AnimAmp * sin(_Time * _Freq * windFreq + dot(_SpatialFreq.xy, worldPos.xz)) * (1 + (sin(_Time.y + 0.5f) + 1) / 2);
			
			animValue = animValue / 2 + 0.5;
			//animValue *= 2;

			animValue *= 0.5 * 0.1f;
            
			v.vertex.xyz += v.texcoord1.x * (animValue * v.texcoord.y + (v.texcoord.x - 0.5)) * rightAxis;
			v.vertex.xyz += v.texcoord1.y * v.texcoord.y * upAxis;
			//v.vertex.xyz = clamp(v.vertex.xyz, 0, baseLength);
		}
		
		half4 LightingGrass(SurfaceOutput s, half3 lightDir, half atten) 
		{
			half NdotL = lightDir.y;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
			c.a = s.Alpha;
			return c;
		}


		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
