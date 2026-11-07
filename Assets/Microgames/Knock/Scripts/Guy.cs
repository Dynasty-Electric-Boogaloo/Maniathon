using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Microgames.Knock
{
    public class Guy : MonoBehaviour
    {
        private enum State
        {
            Idle,
            DoorOpened,
            Punched
        }
        
        [SerializeField] private Sprite whatSprite;
        [SerializeField] private Sprite happySprite;
        [SerializeField] private Sprite punchedSprite;
        [SerializeField] private AudioClip whatClip;
        [SerializeField] private AudioClip hiClip;
        [SerializeField] private AudioClip[] punchedClip;
        private SpriteRenderer _spriteRenderer;
        private AudioSource _audioSource;
        private State _state;
        private float _timer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();
            _spriteRenderer.sprite = whatSprite;
            _state = State.Idle;
        }

        private void Update()
        {
            switch (_state)
            {
                case State.Idle:
                    break;
                case State.DoorOpened:
                    if (_timer > 0)
                    {
                        _timer -= Time.deltaTime;
                        if (_timer <= 0)
                        {
                            _spriteRenderer.sprite = happySprite;
                            _audioSource.clip = hiClip;
                            _audioSource.Play();
                        }
                    }
                    break;
                case State.Punched:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnDoorOpen()
        {
            _audioSource.clip = whatClip;
            _audioSource.Play();

            _timer = 0.5f;
            _state = State.DoorOpened;
        }

        public void Punch()
        {
            _audioSource.clip = punchedClip[Random.Range(0, punchedClip.Length)];
            _audioSource.Play();
            _spriteRenderer.sprite = punchedSprite;
            _state = State.Punched;
        }
    }
}