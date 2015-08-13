Shader "Custom/CustomSpecular" {
	Properties {
		_Color ("Diffuse Material Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
		//材料表面的光泽程度,根据前文所述，此参数无穷大时，材料完全不会产生镜面反射
		_Shininess ("Shininess", Float) = 10
	}
	SubShader {
		Pass{
		Tags { "LightMode" = "ForwardBase" }

		
		CGPROGRAM

		//定义顶点着色器与片段着色器入口
		#pragma vertex vert 
		#pragma fragment frag
		//获取property中定义的材料颜色
		uniform float4 _Color; 
		uniform float4 _SpecColor;
		uniform float _Shininess;
		
		// 光源的位置或者方向
		//uniform float4 _WorldSpaceLightPos0;
		
		
		// 光源的颜色 (from "Lighting.cginc")
		uniform float4 _LightColor0;

		
		//定义顶点着色器的输入参数结构体 
		//我们只需要每个顶点的位置与对应的法向量
		struct vertexInput {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};
		//定义顶点着色的输出结构体/片段着色的输入结构体
		//已经计算好的颜色
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 col : COLOR;
		};
		

		
		//顶点着色器
		vertexOutput vert (vertexInput input) {
			vertexOutput output;
			//对象坐标系到世界坐标系的变换矩阵
			//_Object2World与_World2Object均为unity提供的内置uniform参数
			float4x4 modelMatrix = _Object2World;
			//世界坐标系到对象坐标系的变换矩阵
			float4x4 modelMatrixInverse = _World2Object;
			
			//法向量N变化至对象坐标系
			float3 normalDirection = normalize(float3(mul(float4(input.normal, 0.0), modelMatrixInverse)));
			
			//平行光源的入射向量L直接由uniform_WorldSpaceLightPos0给出
			float3 lightDirection =normalize(float3(_WorldSpaceLightPos0));
			
			//观察向量V由摄像机坐标与顶点坐标矢量相减
			float3 viewDirection = normalize(float3(float4(_WorldSpaceCameraPos, 1.0)
				- mul(modelMatrix, input.vertex)));
			
			//镜面反射光的计算
			float3 specularReflection=float3(_LightColor0)*float3(_SpecColor)*pow(max(0.0,dot(reflect(-lightDirection, normalDirection),viewDirection)),_Shininess);
			
			
			//前文计算好的漫反射光
			float3 diffuseReflection=float3(_LightColor0) * float3(_Color)* max(0.0, dot(normalDirection, lightDirection));	
			
			//环境光直接获取
			float3 ambientLighting = float3(UNITY_LIGHTMODEL_AMBIENT) * float3(_Color);
			
			
			
			//根据冯氏反射模型将上述3个RGB颜色向量相加,然后补充A:
			
			output.col = float4(ambientLighting + diffuseReflection+ specularReflection, 1.0);
			//国际惯例，顶点变化三步曲
			output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
			
			return output;
		}
		
		//片段着色器，老规矩，把顶点着色器的输出参数作为片段着色器的输入参数
		float4 frag(vertexOutput input): COLOR
		{
			return input.col;
		 
		}
		
		ENDCG
		}
	} 
	FallBack "Diffuse"
}