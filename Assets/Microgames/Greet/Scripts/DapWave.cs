using System;
using UnityEngine;
using UnityEngine.UI;

namespace Microgames.Greet.Scripts
{
    public class DapWave : MonoBehaviour
    {
        private static readonly int WaveRadius = Shader.PropertyToID("_WaveRadius");
        private static readonly int WaveThickness = Shader.PropertyToID("_WaveThickness");
        private static readonly int WaveAmplitude = Shader.PropertyToID("_WaveAmplitude");
        private static readonly int WaveOffset = Shader.PropertyToID("_WaveOffset");

        [SerializeField] private RawImage postProcessImage;
        [SerializeField] private float dapTime;
        [SerializeField] private Transform targetObject;
        [SerializeField] private Camera renderCamera;
        private Material _material;
        private float _dapTimer;

        private void Awake()
        {
            _dapTimer = -1;
            _material = new Material(postProcessImage.material);
            postProcessImage.material = _material;
            _material.SetFloat(WaveRadius, -1);
        }

        private void Update()
        {
            if (_dapTimer > 0)
            {
                Vector4 offset = renderCamera.WorldToViewportPoint(targetObject.position);
                offset *= 2;
                offset -= Vector4.one;
                _material.SetVector(WaveOffset, offset);
                
                _dapTimer -= Time.deltaTime;
                var factor = 1 - (_dapTimer / dapTime);
                _material.SetFloat(WaveRadius, 2 * factor);
                _material.SetFloat(WaveThickness, .05f + .15f * factor);
                _material.SetFloat(WaveAmplitude, .4f * _dapTimer / dapTime);
            }
        }

        public void Dap()
        {
            _dapTimer = dapTime;
        }
    }
}