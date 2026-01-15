Shader "Custom/DoubleSidedWater"
{
    Properties
    {
        _Tint ("Water Tint", Color) = (0.1,0.4,0.9,0.3)   // 水の色と濃さ
        _Distort ("Distortion", Range(0,0.05)) = 0.02   // 歪みの強さ
        _Speed ("Speed", Range(0,5)) = 1                // 波の流れる速さ
    }

    SubShader
    {
        // 透明オブジェクトとして描画
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            // アルファブレンド
            Blend SrcAlpha OneMinusSrcAlpha

            // 深度を書かない（奥の景色が見える）
            ZWrite Off

            // 両面描画（内側からも見える）
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // URPの基本関数
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // URPが用意する「画面のコピー」
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float4 _Tint;
            float _Distort;
            float _Speed;

            // メッシュから来る頂点データ
            struct Attributes
            {
                float4 positionOS : POSITION;   // オブジェクト空間の頂点
            };

            // ピクセルシェーダーに渡すデータ
            struct Varyings
            {
                float4 positionHCS : SV_POSITION; // 画面上の座標
                float4 screenPos : TEXCOORD0;     // スクリーンUV用
            };

            // 頂点シェーダー
            Varyings vert(Attributes v)
            {
                Varyings o;

                // オブジェクト → 画面座標に変換
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);

                // スクリーン座標（後でUVに変換）
                o.screenPos = ComputeScreenPos(o.positionHCS);

                return o;
            }

            // ピクセルシェーダー
            half4 frag(Varyings i) : SV_Target
            {
                // スクリーン座標を 0～1 のUVに変換
                float2 uv = i.screenPos.xy / i.screenPos.w;

                // サイン波で横方向に揺らす
                float wave = sin((uv.y + _Time.y * _Speed) * 30) * _Distort;
                uv.x += wave;

                // 歪んだUVで背景をサンプリング
                half4 col = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv);

                // 水の色を混ぜる
                col.rgb = lerp(col.rgb, _Tint.rgb, _Tint.a);

                return col;
            }
            ENDHLSL
        }
    }
}
