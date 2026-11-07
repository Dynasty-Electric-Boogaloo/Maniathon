using System;
using Mainframe.Scripts.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Microgames.Dodge
{
    public class PlayerFeedback : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private Rigidbody2D rigidbody;
        [SerializeField] private Sprite[] runAnimation;
        [SerializeField] private float fps;
        [Header("Audio")]
        [SerializeField] private float maxWalkVelocity;
        [SerializeField] private float maxWalkVolume;
        private SpriteRenderer _spriteRenderer;
        private AudioSource _audioSource;
        private AudioPitcher _audioPitcher;
        private float _timer;
        private int _frame;
        private float _pitchTime;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();
            _audioPitcher = GetComponent<AudioPitcher>();
        }

        private void Update()
        {
            if (Mathf.Abs(rigidbody.linearVelocity.x) > 0.1f)
                _spriteRenderer.flipX = rigidbody.linearVelocity.x < 0;

            _timer += Time.deltaTime;
            _pitchTime = (_pitchTime + Time.deltaTime) % (2 * Mathf.PI);
            var frames = Mathf.FloorToInt(_timer * fps);

            _audioSource.volume = Mathf.Clamp((rigidbody.linearVelocity.magnitude - .1f) / (maxWalkVelocity / maxWalkVolume), 0, maxWalkVolume);
            _audioPitcher.Pitch = .5f + (_audioSource.volume / maxWalkVolume) * .5f + Mathf.Sin(_pitchTime * 8) * .05f;
            
            for (var i = 0; i < frames; i++)
                UpdateFrame();

            _timer %= 1 / fps;
            _spriteRenderer.sprite = runAnimation[_frame];
        }

        private void UpdateFrame()
        {
            if (rigidbody.linearVelocity.magnitude < 0.1f)
            {
                _frame = 0;
                return;
            }

            _frame = (_frame + 1) % runAnimation.Length;
        }
    }
}