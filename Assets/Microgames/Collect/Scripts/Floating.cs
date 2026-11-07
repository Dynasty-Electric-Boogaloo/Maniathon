using System;
using UnityEngine;

namespace Microgames.Dodge
{
    public class Floating : MonoBehaviour
    {
        [SerializeField] private float frequency;
        [SerializeField] private float amplitude;
        private Vector3 _origin;
        private float _time;

        private void Awake()
        {
            _origin = transform.localPosition;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            transform.localPosition = _origin + new Vector3(0, 1, -1) * (Mathf.Sin(_time * frequency * Mathf.PI) * amplitude);
        }
    }
}