
#ifndef PREVIEW_INCLUDED
#define PREVIEW_INCLUDED

float3 preview_WorldSpaceCameraPos;
float4x4 preview_WorldToObject;
float4x4 preview_ObjectToWorld;
float4x4 preview_MatrixV;
float4x4 preview_MatrixInvV;

inline float3 PreviewFragmentPositionOS( float2 uv )
{
	float2 xy = 2 * uv - 1;
	float z = -sqrt( 1 - saturate( dot( xy, xy ) ) );
	return float3( xy, z );
}

inline float3 PreviewFragmentNormalOS( float2 uv, bool normalized = true )
{
	float3 positionOS = PreviewFragmentPositionOS( uv );
	float3 normalOS = positionOS;
	if ( normalized )
	{
		normalOS = normalize( normalOS );
	}
	return normalOS;
}

inline float3 PreviewFragmentTangentOS( float2 uv, bool normalized = true )
{
	float3 positionOS = PreviewFragmentPositionOS( uv );
	float3 tangentOS = float3( -positionOS.z, positionOS.y * 0.01, positionOS.x );
	if ( normalized )
	{
		tangentOS = normalize( tangentOS );
	}
	return tangentOS;
}

inline float3 PreviewWorldSpaceViewDir( in float3 worldPos, bool normalized )
{
	float3 vec = preview_WorldSpaceCameraPos.xyz - worldPos;
	if ( normalized )
	{
		vec = normalize( vec );
	}
	return vec; 
}

inline float3 PreviewWorldToObjectDir( in float3 dir, const bool normalized )
{
	float3 vec = mul( ( float3x3 )preview_WorldToObject, dir );	
	if ( normalized )
	{
		vec = normalize( vec );
	}
	return vec;
}

inline float3 PreviewObjectToWorldDir( in float3 dir, const bool normalized )
{
	float3 vec = mul( ( float3x3 )preview_ObjectToWorld, dir );
	if ( normalized )
	{
		vec = normalize( vec );
	}
	return vec;
}

inline float3 PreviewWorldToViewDir( in float3 dir, const bool normalized )
{
	float3 vec = mul( ( float3x3 )preview_MatrixV, dir );
	if ( normalized )
	{
		vec = normalize( vec );
	}
	return vec;
}

inline float3 PreviewViewToWorldDir( in float3 dir, const bool normalized )
{
	float3 vec = mul( ( float3x3 )preview_MatrixInvV, dir );
	if ( normalized )
	{
		vec = normalize( vec );
	}
	return vec;
}

#endif // PREVIEW_INCLUDED