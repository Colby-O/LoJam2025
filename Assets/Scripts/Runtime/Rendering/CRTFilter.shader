Shader "Hidden/CRTFilter"
{
    Properties
    {
        _MainTex("InputTex", 2D) = "white" {}
     }

     SubShader
     {
        Cull Off ZWrite Off ZTest Always

        Pass 
        {
            Name "CRT"
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag

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

            sampler2D   _MainTex;
            uniform float m_time;

            uniform int m_isMonochrome;

            uniform float m_noiseSpeed;
            uniform float m_noiseScale;
            uniform float m_noiseFade;

            uniform float m_screenBend;
            uniform float m_screenRoundness;
            uniform float2 m_screenResolution;
            uniform float m_vignetteOpacity;

            uniform float2 m_scanLineOpacity;
            uniform float2 m_scanLineSpeed;
            uniform float m_scanLineStrength;

            uniform float m_brightness;
            uniform float m_contrast;
            uniform float m_saturation;
            uniform float m_hue;
            uniform float m_redShift;
            uniform float m_blueShift;
            uniform float m_greenShift;

            uniform float m_chromaticOffset;
            uniform float m_chromaticSpeed;

            inline half3 GammaToLinearSpace(half3 sRGB)
            {
                return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);
            }

            inline half3 LinearToGammaSpace(half3 linRGB)
            {
                linRGB = max(linRGB, half3(0.h, 0.h, 0.h));
                return max(1.055h * pow(linRGB, 0.416666667h) - 0.055h, 0.h);
            }

            float2 curve_remap(float2 uv) 
            {
                uv = uv * 2.0 - 1.0f;
                float2 offset = abs(uv.yx) / float2(m_screenBend, m_screenBend);
                uv = uv + uv * offset * offset;
                uv = uv * 0.5 + 0.5;
                return uv;
            }

            float calc_round_factor(float2 uv) 
            {
                uv *=  1.0 - uv.yx;
                float vig = uv.x * uv.y * 100.0 * m_vignetteOpacity;
                return clamp(pow(vig, m_screenRoundness), 0.0, 1.0);
            } 

            float4 create_round_view(float4 col, float2 uv) 
            {
                col *= calc_round_factor(uv.x);
                col *= calc_round_factor(uv.y);
                return col;
            }

            float4 create_scanlne(float4 col, float2 uv) 
            {
                float2 intensity = float2(sin(uv.x * m_screenResolution.x + m_time * m_scanLineSpeed.x * 100.0), cos(uv.y * m_screenResolution.y + m_time * m_scanLineSpeed.y * 100.0)) * m_scanLineStrength;
                intensity = (0.5 * intensity) + 0.5;
                float val = dot(intensity, m_scanLineOpacity);
                return col * val * 2.0 / (m_scanLineOpacity.x + m_scanLineOpacity.y);
            }

            float4 sample_from_texture(float2 uv) 
            {
                return tex2D(_MainTex, uv);
            }

            float random(float2 seed) 
            {
                return frac(sin(dot(seed ,float2(12.9898,78.233)*2.0)) * 43758.5453 + sin(m_time * m_noiseSpeed));
            }

            float gradient_noise(float2 uv) 
            {
                float2 ip = floor(uv);
                float2 fp = frac(uv);

                float a = random(ip);
                float b = random(ip + float2(1.0, 0.0));
                float c = random(ip + float2(0.0, 1.0));
                float d = random(ip + float2(1.0, 1.0));

                float2 e = smoothstep(0.0, 1.0, fp);

                return lerp(a, b, e.x) + (c - a) * e.y * (1.0 - e.x) + (d - b) * e.x * e.y;
            }

            float4 apply_chromatic_aberration(float4 col, float2 uv) 
            {
                col.r = sample_from_texture(float2(uv.x - m_chromaticOffset * cos(m_time * m_chromaticSpeed) / 10.0, uv.y)).r;
                col.b = sample_from_texture(float2(uv.x, uv.y)).b;
                col.g = sample_from_texture(float2(uv.x + m_chromaticOffset * cos(m_time * m_chromaticSpeed) / 10.0, uv.y)).g;
                return col;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 rbg_to_gray(float4 col) 
            {
                return 0.2125 * col.r + 0.0721 * col.b + 0.7154 * col.g;
            }

            inline half3 apply_hue(half3 aColor, float aHue)
            {
                half angle = radians(aHue);
                half3 k = half3(0.57735, 0.57735, 0.57735);
                half cosAngle = cos(angle);
                //Rodrigues' rotation formula
                return aColor * cosAngle + cross(k, aColor) * sin(angle) + k * dot(k, aColor) * (1 - cosAngle);
            }
            
            half3 adjust_contrast(half3 color) {
            #if !UNITY_COLORSPACE_GAMMA
                color = LinearToGammaSpace(color);
            #endif
                color = saturate(lerp(half3(0.5, 0.5, 0.5), color, m_contrast));
            #if !UNITY_COLORSPACE_GAMMA
                color = GammaToLinearSpace(color);
            #endif
                return color;
            }

            inline half3 apply_hsb_effect(half3 startColor)
            {
            #if !UNITY_COLORSPACE_GAMMA
                startColor = LinearToGammaSpace(startColor);
            #endif

                float hue = 360 * m_hue;
                float brightness = m_brightness * 2 - 1;
                float contrast = m_contrast * 2;
                float saturation = m_saturation  * 2;
            
                half3 outputColor = startColor;
                outputColor = apply_hue(outputColor, hue);
                outputColor = (outputColor - 0.5f) * contrast + 0.5f;;
                outputColor = outputColor + brightness;      
                half3 intensity = dot(outputColor, half3(0.299,0.587,0.114));
                outputColor = lerp(intensity, outputColor, saturation);
            
            #if !UNITY_COLORSPACE_GAMMA
                outputColor = GammaToLinearSpace(outputColor);
            #endif

                return outputColor;
            }

            float4 frag(v2f IN) : SV_Target
            {
                float2 uv = curve_remap(IN.uv); 
                
                if (uv.x < 0.0 || uv.y < 0.0 || uv.x > 1.0 || uv.y > 1.0) 
                {
                    return float4(0.0, 0.0, 0.0, 1.0);
                } 

                float4 baseColor = sample_from_texture(uv);
                baseColor = apply_chromatic_aberration(baseColor, uv);
                baseColor.rbg = apply_hsb_effect(baseColor.rbg);
                baseColor = create_scanlne(baseColor, uv); 
                baseColor = create_round_view(baseColor, uv);
                
                baseColor.r *= 1.0 + m_redShift;
                baseColor.b *= 1.0+ m_blueShift;
                baseColor.g *= 1.0 + m_greenShift;
                baseColor = lerp(baseColor, baseColor * gradient_noise(uv * m_noiseScale * 1000.0), m_noiseFade);
                
                if (m_isMonochrome == 1) 
                {
                    baseColor = rbg_to_gray(baseColor);
                }

                //baseColor.r = clamp(baseColor.r, 0.0, 1.0);
                //baseColor.b = clamp(baseColor.b, 0.0, 1.0);
                //baseColor.g = clamp(baseColor.g, 0.0, 1.0);

                return baseColor;
            }

            ENDHLSL
        }
    }
}
