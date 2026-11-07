using System;
using UnityEngine;
using UnityEngine.UI;

namespace Microgames.Greet.Scripts
{
    public class PostProcessRenderer : MonoBehaviour
    {
        [SerializeField] private Camera postProcessCamera;
        [SerializeField] private Camera renderCamera;
        [SerializeField] private RawImage renderImage;
        private RenderTexture _renderTexture;

        private void Awake()
        {
            _renderTexture = new RenderTexture(postProcessCamera.pixelWidth, postProcessCamera.pixelHeight, 32);
            renderCamera.targetTexture = _renderTexture;
            renderImage.texture = _renderTexture;
        }
    }
}