using System;
using UnityEngine;
using UnityEngine.UI;

namespace Microgames.Dodge
{
    public class PixelRenderer : MonoBehaviour
    {
        [SerializeField] private Camera renderCamera;
        [SerializeField] private RawImage renderImage;
        [SerializeField] private Vector2Int renderSize;
        [SerializeField] private float pixelsPerUnit;
        private RenderTexture _renderTexture;

        private void Awake()
        {
            _renderTexture = new RenderTexture(renderSize.x, renderSize.y, 32);
            _renderTexture.filterMode = FilterMode.Point;
            _renderTexture.antiAliasing = 1;
            _renderTexture.anisoLevel = 0;
            renderCamera.targetTexture = _renderTexture;
            renderCamera.orthographicSize = (renderSize.y / 2f) / pixelsPerUnit;
            renderImage.texture = _renderTexture;
        }
    }
}