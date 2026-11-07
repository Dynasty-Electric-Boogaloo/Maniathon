// Wave Shader made by Matys "Geek_Joystick" Guéroult

Shader "Greet/Wave Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveRadius ("Wave Radius", Float) = 0
        _WaveThickness ("Wave Thickness", Float) = 0
        _WaveAmplitude ("Wave Amplitude", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveRadius;      // Where is the peak of the wave from the center of the impact
            float _WaveThickness;   // How thick the wave is around the peak of the wave
            float _WaveAmplitude;   // How much the wave should displace the screen at the peak of the wave
            float2 _WaveOffset;     // Where is the center of the wave

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv = (uv - .5) * 2;                                 //Transform UV so that it goes from (-1; -1) to (1; 1)
                uv -= _WaveOffset;                                  //Offset UV to center the wave.
                
                float aspect = _ScreenParams.x / _ScreenParams.y;   //Compute the screen's aspect ratio
                uv.x *= aspect;                                     //Make the UV space non normalized (AKA Circle, not Ellipse)
                
                float2 direction = normalize(uv);                   //Get the direction of the wave
                float radius = length(uv);                          //Get the current pixel's distance from the center of the wave
                
                //Compute how much the pixel should be displaced by the wave.
                //https://easings.net/#easeInOutSine
                float bump = -(cos(UNITY_PI * max(1 - abs(_WaveRadius - radius) / _WaveThickness, 0)) - 1) / 2;
                
                uv += direction * (bump * _WaveAmplitude);          //Offset the UV by the displacement force alongside the wave direction.
                uv.x /= _ScreenParams.x / _ScreenParams.y;          //Re-normalize the UV space
                uv += _WaveOffset;
                uv = (uv + 1) / 2;                                  //Transform UV back to (0; 0) to (1; 1) space
                
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}