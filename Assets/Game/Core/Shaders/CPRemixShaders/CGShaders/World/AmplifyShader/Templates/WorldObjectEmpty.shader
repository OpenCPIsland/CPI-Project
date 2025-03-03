Shader /*ase_name*/"Hidden/Templates/CPIsland/World"/*end*/
{
    Properties
    {
        /*ase_props*/
        
        [HideInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
        }


        /*ase_pass*/
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            /*ase_pragma*/

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float3 color : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float2 texcoord1  : TEXCOORD2;
                float3 texcoord6 : TEXCOORD6;
                UNITY_VERTEX_OUTPUT_STEREO
                /*ase_interp(3,):sp=sp.xyzw;uv0=tc0.xy;c=c;uv1=tc1.xyzw*/
            };
            
            struct fout
            {
                float4 sv_target : SV_Target0;
            };

            float _ShadowPlaneDim;
            float _ShadowTextureDim;
            float3 _ShadowPlaneWorldPos;
            
            /*ase_globals*/
            
            uniform sampler2D _BlobShadowTex;
            
            v2f vert(appdata_full v /*ase_vert_input*/)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                /*ase_vert_code:v=appdata_t;OUT=v2f*/

                v.vertex.xyz += /*ase_vert_out:Vertex;Float3*/ float3( 0, 0, 0 ) /*end*/;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = mul(unity_ObjectToWorld, v.vertex.xyz);
                OUT.vertex = vPosition;

                OUT.texcoord = v.texcoord;
                OUT.texcoord1 = v.texcoord2;
                
                OUT.worldPosition.xz = OUT.worldPosition.xz - _ShadowPlaneWorldPos.xz;
                OUT.texcoord6.z = OUT.worldPosition.y;
                OUT.worldPosition.y = _ShadowPlaneDim * 0.5;
                OUT.worldPosition.xy = OUT.worldPosition.xz / OUT.worldPosition.yy;
                OUT.worldPosition.z = _ShadowPlaneDim / _ShadowTextureDim;
                OUT.worldPosition.z = OUT.worldPosition.z / _ShadowPlaneDim;
                OUT.worldPosition.xy = OUT.worldPosition.zz + OUT.worldPosition.xy;
                OUT.worldPosition.xy = OUT.worldPosition.xy + float2(1.0, 1.0);
                OUT.texcoord6.xy = OUT.worldPosition.xy * float2(0.5, 0.5);
                
                OUT.color = v.color;
                return OUT;
            }

            fout frag(v2f IN /*ase_frag_input*/)
            {
                fout OUT;
                float2 uv_Lightmap = IN.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                /*ase_frag_code:IN=v2f*/

                OUT.sv_target.xyz =  tex2D(_BlobShadowTex, IN.texcoord6.xy).xxx* UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, uv_Lightmap) * /*ase_frag_out:Frag;Float4*/ IN.color /*end*/;
                OUT.sv_target.w = 1.0;
                return OUT;
            }
        ENDCG
        }
    }
    CustomEditor "ASEMaterialInspector"
}