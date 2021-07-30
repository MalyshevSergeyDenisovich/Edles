Shader "Custom/Terrain"
{
    Properties
    {
        testTexture("Texture", 2D) = "white"{}
        testScale("Scale", Float)  = 1 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows


        #pragma target 3.0

        const static int maxLayerCount = 8;
        const static float epsilon = 1E-4;
        
        int layerCount;
        float3 baseColours[maxLayerCount];
        float baseStartHeights[maxLayerCount];
        float baseBlends[maxLayerCount];
        float baseColorStrength[maxLayerCount];
        float baseTextureScales[maxLayerCount];
                
        float minHeight;
        float maxHeight;

        sampler2D testTexture;
        float testScale;

        UNITY_DECLARE_TEX2DARRAY(baseTextures);
        
        struct Input {
            float3 worldPos;
            float3 worldNormal;
        };

        float inverse_lerp(float a, float b, float value) {
            return saturate((value - a) / (b-a));
        }


        float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex)
        {
            float3 scaleWorldPos = worldPos / scale;
            
            float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaleWorldPos.y, scaleWorldPos.z, textureIndex)) * blendAxes.x;
            float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaleWorldPos.x, scaleWorldPos.z, textureIndex)) * blendAxes.y;
            float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaleWorldPos.x, scaleWorldPos.y, textureIndex)) * blendAxes.z;
            return xProjection + yProjection + zProjection;
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            float heightPercent = inverse_lerp(minHeight, maxHeight, IN.worldPos.y);
            float3 blendAxes = abs(IN.worldNormal);

            blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z; 
            for (int i = 0; i< layerCount; i++)
            {
                float drawStrength = inverse_lerp(-baseBlends[i] / 2, baseBlends[i] / 2 - epsilon, heightPercent - baseStartHeights[i]);
                float3 baseColour = baseColours[i] * baseColorStrength[i];
                float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColorStrength[i]); 
                
                o.Albedo = o.Albedo * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
            }





        }
        
        ENDCG
    }
    FallBack "Diffuse"
}
