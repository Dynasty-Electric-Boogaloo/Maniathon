using System;
using System.Collections.Generic;
using Mainframe.Scripts.Microgame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Microgames.Dodge
{
    public class CollectMicrogame : Microgame
    {
        [Serializable]
        private struct Layout
        {
            public List<Transform> points;
        }
        
        [SerializeField] private Corn cornPrefab;
        [SerializeField] private Layout[] layouts;
        private HashSet<Corn> _corns;
        private int _cornCount;
        private float _timer;
        
        public override void Begin()
        {
            base.Begin();

            _corns = new HashSet<Corn>();

            var layout = Random.Range(0, layouts.Length);
            _cornCount = layouts[layout].points.Count;
            foreach (var point in layouts[layout].points)
            {
                var corn = Instantiate(cornPrefab, point.position, Quaternion.identity);
                _corns.Add(corn);
            }

            _timer = -1;
        }

        private void Update()
        {
            if (_timer > 0)
                _timer -= Time.deltaTime;
        }

        public override string GetInstruction()
        {
            return "Collect!";
        }

        public override float GetDuration()
        {
            return 6;
        }

        public override bool IsFinished()
        {
            return IsSuccess() && _timer < 0;
        }

        public override bool IsSuccess()
        {
            return _corns.Count == 0;
        }

        public void CollectCorn(Corn corn)
        {
            _corns.Remove(corn);

            if (_corns.Count == 0)
            {
                _timer = 1;
                onWin?.Invoke();
            }
        }

        public int GetCollectedCount()
        {
            return _cornCount - _corns.Count;
        }
    }
}