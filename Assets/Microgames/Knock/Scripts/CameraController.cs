using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Microgames.Knock
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private float travelTime;
        [SerializeField] private AnimationCurve travelCurve;
        private float _shakeAmount;

        private void Update()
        {
            _shakeAmount = Mathf.Max(_shakeAmount - Time.deltaTime, 0);
            transform.position = endPoint.position + (Vector3)(Random.insideUnitCircle * (_shakeAmount / 2));
        }

        public void Approach()
        {
            StartCoroutine(ApproachCoroutine());
        }

        public void Shake()
        {
            _shakeAmount = .2f;
        }

        private IEnumerator ApproachCoroutine()
        {
            var timer = 0f;

            while (timer < travelTime)
            {
                timer += Time.deltaTime;
                var factor = travelCurve.Evaluate(timer / travelTime);
                transform.position = Vector3.Lerp(startPoint.position, endPoint.position, factor);
                transform.position += Vector3.up * Mathf.Abs(Mathf.Sin(Mathf.PI * factor * 3) * .1f);
                yield return null;
            }

            transform.position = endPoint.position;
        }
    }
}