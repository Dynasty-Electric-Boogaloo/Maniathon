using System;
using System.Collections;
using UnityEngine;

namespace Mainframe.Scripts.Game
{
    public class CartridgeInserter : MonoBehaviour
    {
        [SerializeField] private Transform cartridge;
        [SerializeField] private Transform offscreenPosition;
        [SerializeField] private Transform abovePosition;
        [SerializeField] private Transform insertPosition;
        [SerializeField] private float appearTime;
        [SerializeField] private float insertTime;
        [SerializeField] private AnimationCurve appearCurve;
        [SerializeField] private AnimationCurve insertCurve;

        private void Update()
        {
            if (Game.IsInMicrogame())
                cartridge.position = Vector3.one * 10000;
        }

        public void Insertion()
        {
            StartCoroutine(InsertionCoroutine());
        }

        private IEnumerator InsertionCoroutine()
        {
            yield return AppearCoroutine();
            yield return InsertCoroutine();
            
            cartridge.position = insertPosition.position;
            cartridge.rotation = insertPosition.rotation;
        }

        private IEnumerator AppearCoroutine()
        {
            cartridge.position = offscreenPosition.position;
            cartridge.rotation = offscreenPosition.rotation;

            var timer = 0f;
            
            while (timer < appearTime)
            {
                timer += Time.unscaledDeltaTime * Mathf.Pow(2, Game.GetSpeed() / 12.0f);

                var factor = appearCurve.Evaluate(timer / appearTime);
                cartridge.position = Vector3.Lerp(offscreenPosition.position, abovePosition.position, factor);
                cartridge.rotation = Quaternion.Lerp(offscreenPosition.rotation, abovePosition.rotation, factor);
                
                yield return null;
            }
        }

        private IEnumerator InsertCoroutine()
        {
            cartridge.position = abovePosition.position;
            cartridge.rotation = abovePosition.rotation;

            var timer = 0f;
            
            while (timer < insertTime)
            {
                timer += Time.unscaledDeltaTime * Mathf.Pow(2, Game.GetSpeed() / 12.0f);

                var factor = insertCurve.Evaluate(timer / insertTime);
                cartridge.position = Vector3.LerpUnclamped(abovePosition.position, insertPosition.position, factor);
                cartridge.rotation = Quaternion.LerpUnclamped(abovePosition.rotation, insertPosition.rotation, factor);
                
                yield return null;
            }
        }
    }
}