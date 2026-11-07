using System;
using System.Collections;
using UnityEngine;

namespace Mainframe.Scripts.Game
{
    internal class CartridgeLauncher : MonoBehaviour
    {
        [SerializeField] private Transform cartridge;
        [SerializeField] private Transform launchInitialPosition;
        [SerializeField] private float launchTime;
        [SerializeField] private float launchDistance;
        [SerializeField] private AnimationCurve launchCurve;

        private void Start()
        {
            cartridge.position = new Vector3(10000, 10000, 10000);
        }

        public void Launch()
        {
            StartCoroutine(LaunchCoroutine());
        }

        private IEnumerator LaunchCoroutine()
        {
            cartridge.position = launchInitialPosition.position;
            cartridge.rotation = launchInitialPosition.rotation;
            
            var timer = launchTime;
            var speedMultiplier = Mathf.Pow(2, Game.GetSpeed() / 12.0f);

            while (timer > 0)
            {
                timer -= Time.unscaledDeltaTime * speedMultiplier;
                cartridge.position = launchInitialPosition.position + launchInitialPosition.up *
                    (launchDistance * launchCurve.Evaluate(1 - timer / launchTime));
                yield return null;
            }
        }
    }
}