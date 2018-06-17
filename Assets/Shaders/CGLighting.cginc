//
#ifndef CGLIGHTING
#define CGLIGHTING

//
float3 normalFromColor(float4 color)  
{ 
	#if defined(UNITY_NO_DXT5nm)
		return color.xyz * 2 - 1;
	// W/o RGB => xyz
	// With DXT Compression, RGB => AG => xy
	#else
		float3 normalDecompressed;
		normalDecompressed = float3 (	color.a * 2 - 1,
										color.g * 2 - 1,
										0.0);
		normalDecompressed.z = sqrt(1 - dot(normalDecompressed.xy, normalDecompressed.xy));
		return normalDecompressed;
	#endif
}

float3 WorldNormalFromNormalMap(sampler2D normalMap, float2 normalTexCoord, float3 tangentWorld, float3 binormalWorld, float3 normalWorld)
{
		half4 normalColor = tex2D(normalMap, normalTexCoord);
		//This is a TangentSpaceNormal read from the NormalMap texture
		float3 TSNormal = normalFromColor(normalColor);

		//Calculate TBN matrix based on values passed from VS
		float3x3 TBNWorld = float3x3 (tangentWorld, binormalWorld, normalWorld);
		float3 normalWorldAtPixel = normalize(mul(TSNormal, TBNWorld));

		return normalWorldAtPixel;
}

//
float3 DiffuseLambert(float3 normalVal, float3 lightDir, float3 lightColor, float diffuseFactor, float attenuation)
{
	return lightColor * diffuseFactor * attenuation * max(0, dot(normalVal,lightDir));
}

//
float4 RimLight(float3 normalVal, float3 view, float3 lightColor, float cutoff, float rimWidth)
{
	float A = max(0, 1 - dot(normalVal,view));
	A = clamp(0, 1, A + rimWidth);
	return float4(lightColor * A, A <= cutoff? 0 : 1);
}

//
float3 SpecularBlinnPhong(float3 N, float3 L, float3 V, float3 specularColor, float specularFactor, float attenuation, float specularPower)
{
	//specularColor is readed from SpecularMap
	//specularFactor & specularPower are material properties
	//V is View in world space
	float3 H = normalize(L+V);
	return specularColor * specularFactor * attenuation * pow(max(0, dot(N,H)), specularPower);
}

//
#endif