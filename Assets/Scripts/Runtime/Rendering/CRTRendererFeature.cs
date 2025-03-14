using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LoJam.Rendering
{
	public sealed class CRTRendererFeature : ScriptableRendererFeature
	{
        [Header("References")]
        [SerializeField] private Shader _shader;

        [Header("Options")]
        [SerializeField] private RenderPassEvent _injectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;

        [Header("Screen Parameters")]
        [SerializeField] private Vector2 _screenResolution = new Vector2(256.0f, 256.0f);
        [SerializeField, Min(0)] private float _screenBend = 3.0f;

        [Header("Vignette Parameters")]
        [SerializeField, Range(0, 1)] private float _screenRoundness = 1.0f;
        [SerializeField, Range(0, 1)] private float _vignetteOpacity = 1.0f;

        [Header("Scanlines Parameters")]
        [SerializeField, Range(0, 1)] private float _scanLineVerticalOpacity = 3.0f;
        [SerializeField, Range(0, 1)] private float _scanLineHoriztionalOpacity = 0.1f;
        [SerializeField, Range(0, 1)] private float _scanLineVerticalSpeed = 0.1f;
        [SerializeField, Range(0, 1)] private float _scanLineHoriztionalSpeed = 3.0f;
        [SerializeField, Range(0, 1)] private float _scanLineStrength;

        [Header("Noise Parameters")]
        [SerializeField, Range(0, 1)] private float _noiseScale = 3f;
        [SerializeField, Range(0, 1)] private float _noiseSpeed = 0.1f;
        [SerializeField, Range(0, 1)] private float _noiseFade = 0.3f;

        [Header("Chromatic Aberration Parameters")]
        [SerializeField, Range(0, 1)] private float _chromaticOffset = 0.0f;
        [SerializeField, Range(0, 1)] private float _chromaticOffsetSpeed = 0.1f;

        [Header("Display Parameters")]
        [SerializeField, Range(-1, 1)] private float _brightness = 0.5f;
        [SerializeField, Range(-5, 5)] private float _contrast = 0.5f;
        [SerializeField, Range(-5, 5)] private float _saturation = 0.5f;
        [SerializeField, Range(0, 1)] private float _hue = 0.0f;
        [SerializeField, Range(0, 1)] private float _redShift = 0.0f;
        [SerializeField, Range(0, 1)] private float _blueShift = 0.0f;
        [SerializeField, Range(0, 1)] private float _greenShift = 0.0f;
        [SerializeField] private bool _isMonochrome = false;

        private Material _mat;
        private CRTRendererPass _rp;

        public void SetHSBSettings(float brightness, float contrast, float hue)
        {
            _brightness = brightness;
            _contrast = contrast;
            _hue = hue;
        }

        public void SetScreenBend(float screenBend)
        {
            _screenBend = screenBend;
        }

        public void SetNoiseLevel(float noiseLevel)
        {
            _noiseFade = noiseLevel;
        }

        private void SetParameters()
        {
            _mat.SetFloat("m_screenBend", _screenBend);
            _mat.SetFloat("m_screenRoundness", _screenRoundness);
            _mat.SetVector("m_screenResolution", _screenResolution);
            _mat.SetFloat("m_vignetteOpacity", _vignetteOpacity);
            _mat.SetVector("m_scanLineOpacity", new Vector2(_scanLineVerticalOpacity, _scanLineHoriztionalOpacity));
            _mat.SetVector("m_scanLineSpeed", new Vector2(_scanLineVerticalSpeed, _scanLineHoriztionalSpeed));
            _mat.SetFloat("m_scanLineStrength", _scanLineStrength);

            _mat.SetFloat("m_noiseSpeed", _noiseSpeed);
            _mat.SetFloat("m_noiseScale", _noiseScale);
            _mat.SetFloat("m_noiseFade", _noiseFade);

            _mat.SetFloat("m_chromaticOffset", _chromaticOffset);
            _mat.SetFloat("m_chromaticSpeed", _chromaticOffsetSpeed);

            _mat.SetFloat("m_brightness", _brightness);
            _mat.SetFloat("m_contrast", _contrast);
            _mat.SetFloat("m_saturation", _saturation);
            _mat.SetFloat("m_hue", _hue);

            _mat.SetFloat("m_redShift", _redShift);
            _mat.SetFloat("m_blueShift", _blueShift);
            _mat.SetFloat("m_greenShift", _greenShift);

            _mat.SetInt("m_isMonochrome", _isMonochrome ? 1 : 0);

        }

        public override void Create()
        {
            if (_shader == null)
            {
                _shader = Shader.Find("Hidden/CRTFilter");
            }

            if (_mat == null)
            {
                _mat = CoreUtils.CreateEngineMaterial(_shader);
            }

            if (_rp == null)
            {
                _rp = new CRTRendererPass(_mat);
                _rp.renderPassEvent = _injectionPoint;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (_mat != null)
            {
                CoreUtils.Destroy(_mat);
                _mat = null;
            }

            if (_rp != null)
            {
                _rp.Destroy();
                _rp = null;
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_mat == null || _rp == null) return;

            SetParameters();

            _rp.ConfigureInput(ScriptableRenderPassInput.Color);
            renderer.EnqueuePass(_rp);
        }
    }
}
