using System;
using UnityEngine;

namespace Mainframe.Scripts.Shared
{
    /// <summary>
    /// Attach this component to any <c>GameObject</c> containing <c>SpriteRenderer</c>s you want the <c>SpriteSorter</c> to sort.
    /// </summary>
    public class SpriteSorted : MonoBehaviour
    {
        private SpriteRenderer[] _sprites;
        private bool _sorted;

        private void Awake()
        {
            _sprites = GetComponents<SpriteRenderer>();
        }

        private void Start()
        {
            if (_sorted)
                return;
            
            _sorted = true;
            foreach (var sprite in _sprites)
            {
                _sorted &= SpriteSorter.AddSprite(sprite);
            }
        }

        private void OnEnable()
        {
            if (_sorted)
                return;
            
            _sorted = true;
            foreach (var sprite in _sprites)
            {
                _sorted &= SpriteSorter.AddSprite(sprite);
            }
        }

        private void OnDisable()
        {
            if (!_sorted)
                return;
            
            foreach (var sprite in _sprites)
            {
                SpriteSorter.RemoveSprite(sprite);
            }

            _sorted = false;
        }
    }
}