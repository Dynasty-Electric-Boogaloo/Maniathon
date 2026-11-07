using System;
using System.Collections;
using UnityEngine;

namespace Mainframe.Scripts.Shared
{
    /// <summary>
    /// This component sorts all <c>SpriteSorted</c> tagged <c>SpriteRenderer</c> in the scene so that they all appear
    /// one after the other along the specified <c>sortingAxis</c>.
    /// </summary>
    public class SpriteSorter : MonoBehaviour
    {
        private class Comparer : IComparer
        {
            private Vector3 _sortingAxis;
            private SpriteRenderer[] _spriteRenderers;
            
            public Comparer(Vector3 sortingAxis)
            {
                _sortingAxis = sortingAxis.normalized;
            }

            public void SetSprites(SpriteRenderer[] spriteRenderers)
            {
                _spriteRenderers = spriteRenderers;
            }
            
            public int Compare(object x, object y)
            {
                Debug.Assert(x is int && y is int);
                
                var xDot = Vector3.Dot(_spriteRenderers[(int)x].transform.position, _sortingAxis);
                var yDot = Vector3.Dot(_spriteRenderers[(int)y].transform.position, _sortingAxis);

                return xDot.CompareTo(yDot);
            }
        }
        
        private static SpriteSorter _instance;
        [SerializeField] private Vector3 sortingAxis;
        private SpriteRenderer[] _sprites;
        private int[] _indices;
        private int _spriteCount;
        private Comparer _comparer;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            
            _indices = new int[16];
            _sprites = new SpriteRenderer[16];
            _spriteCount = 0;
            _comparer = new Comparer(sortingAxis);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void Update()
        {
            for (var i = 0; i < _spriteCount; i++)
            {
                _indices[i] = i;
            }
            
            _comparer.SetSprites(_sprites);
            Array.Sort(_indices, 0, _spriteCount, _comparer);

            for (var i = 0; i < _spriteCount; i++)
            {
                _sprites[_indices[i]].sortingOrder = i;
            }
        }
        
        internal static bool AddSprite(SpriteRenderer spriteRenderer)
        {
            if (!_instance)
                return false;
            
            if (_instance._spriteCount >= _instance._sprites.Length)
            {
                Array.Resize(ref _instance._sprites, _instance._spriteCount * 2);
                Array.Resize(ref _instance._indices, _instance._spriteCount * 2);
            }
            
            _instance._sprites[_instance._spriteCount++] = spriteRenderer;
            return true;
        }

        internal static void RemoveSprite(SpriteRenderer spriteRenderer)
        {
            if (!_instance)
                return;

            var index = Array.IndexOf(_instance._sprites, spriteRenderer);
            if (index < 0)
                return;

            _instance._sprites[index] = _instance._sprites[--_instance._spriteCount];
        }
    }
}