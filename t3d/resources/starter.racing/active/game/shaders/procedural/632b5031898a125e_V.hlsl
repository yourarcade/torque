//*****************************************************************************
// Torque -- HLSL procedural shader
//*****************************************************************************

// Dependencies:
#include "shaders/common/torque.hlsl"

// Features:
// Vert Position
// Diffuse Color
// Reflect Cube
// HDR Output

struct VertData
{
   float3 position        : POSITION;
   float3 normal          : NORMAL;
   float3 T               : TANGENT;
   float2 texCoord        : TEXCOORD0;
};


struct ConnectData
{
   float4 hpos            : POSITION;
   float3 reflectVec      : TEXCOORD0;
};


//-----------------------------------------------------------------------------
// Main
//-----------------------------------------------------------------------------
ConnectData main( VertData IN,
                  uniform float4x4 modelview       : register(C0),
                  uniform float4x4 objTrans        : register(C4),
                  uniform float3   eyePosWorld     : register(C8)
)
{
   ConnectData OUT;

   // Vert Position
   OUT.hpos = mul(modelview, float4(IN.position.xyz,1));
   
   // Diffuse Color
   
   // Reflect Cube
   float3 cubeVertPos = mul(objTrans, float4(IN.position,1)).xyz;
   float3 cubeNormal = normalize( mul(objTrans, float4(normalize(IN.normal),0.0)).xyz );
   float3 eyeToVert = cubeVertPos - eyePosWorld;
   OUT.reflectVec = reflect(eyeToVert, cubeNormal);
   
   // HDR Output
   
   return OUT;
}
