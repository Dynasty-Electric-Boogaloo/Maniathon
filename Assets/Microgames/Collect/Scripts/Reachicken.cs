using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Microgames.Dodge
{
    public class Reachicken : MonoBehaviour
    {
        [SerializeField] private Vector2 hiddenPosition;
        [SerializeField] private Vector2 shownPosition;
        [SerializeField] private float appearTime;
        private RectTransform _rectTransform;
        private float _appearTimer;
        private Vector2 _position;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _position = hiddenPosition;
            transform.localPosition = _position;
            _appearTimer = -1;
        }

        private void Update()
        {
            _rectTransform.anchoredPosition = _position + Random.insideUnitCircle;
            
            if (_appearTimer < 0)
                return;

            _appearTimer -= Time.deltaTime;
            _position = Vector2.Lerp(shownPosition, hiddenPosition, _appearTimer / appearTime);
        }

        public void Appear()
        {
            _appearTimer = appearTime;
        }
    }
}