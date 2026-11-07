using UnityEngine;

namespace Microgames.Greet.Scripts
{
    public class Flipbook : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer renderer;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private float fps;
        private float _timer;
        private int _frameIndex;

        private void Update()
        {
            _timer += Time.deltaTime;
            var frames = Mathf.FloorToInt(_timer * fps);
            _timer -= frames / fps;
            _frameIndex = (_frameIndex + frames) % sprites.Length;
            renderer.sprite = sprites[_frameIndex];
        }
    }
}