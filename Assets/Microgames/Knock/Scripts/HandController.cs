using System;
using System.Collections;
using Mainframe.Scripts.Game;
using Mainframe.Scripts.Microgame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Microgames.Knock
{
    public class HandController : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_BaseMap");
        
        [SerializeField] private float hiddenHeight;
        [SerializeField] private float shownHeight;
        [SerializeField] private float appearTime;
        [SerializeField] private AnimationCurve appearCurve;
        [SerializeField] private AudioSource knockSource;
        [SerializeField] private AudioPitcher pitcher;
        private MeshRenderer _meshRenderer;
        private Material _material;
        private bool _appeared;
        private float _height;

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _material = new Material(_meshRenderer.sharedMaterial);
            _meshRenderer.sharedMaterial = _material;
            
            _material.SetTextureOffset(MainTex, new Vector2(0.5f, 0));
        }

        private void Update()
        {
            if (!Game.IsInMicrogame() || !_appeared)
                return;

            var microgame = Microgame.GetInstance<KnockMicrogame>();

            if (microgame.IsDoorOpen())
                _height -= Time.deltaTime * .7f;
            
            if (Game.Input.GetButtonPressed())
            {
                if (!microgame.IsDoorOpen())
                {
                    pitcher.Pitch = Random.Range(0.9f, 1.2f);
                    knockSource.Play();
                }
                
                if (microgame.IsDoorOpen())
                    microgame.Knock();
            }
            
            if (Game.Input.GetButtonReleased() && !microgame.IsDoorOpen())
                microgame.Knock();

            var held = Game.Input.GetButtonHeld();
            _height = held ? shownHeight : _height;
            _material.SetTextureOffset(MainTex, new Vector2(held ? 0.5f : 0f, 0));
            transform.localPosition = new Vector3(0, _height, held ? 0.825f : 0.68f);
        }

        public void Appear()
        {
            StartCoroutine(AppearCoroutine());
        }

        private IEnumerator AppearCoroutine()
        {
            var timer = 0f;

            while (timer < appearTime)
            {
                timer += Time.deltaTime;
                var factor = appearCurve.Evaluate(timer / appearTime);
                transform.localPosition = new Vector3(0, Mathf.Lerp(hiddenHeight, shownHeight, factor), 0.68f);
                yield return null;
            }

            transform.localPosition = new Vector3(0, shownHeight, 0.68f);
            _height = shownHeight;
            _appeared = true;
        }
    }
}