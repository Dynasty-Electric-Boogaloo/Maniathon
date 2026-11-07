using System;
using Mainframe.Scripts.Game;
using Mainframe.Scripts.Microgame;
using UnityEngine;

namespace Microgames.Dodge
{
    public class PlayerEating : MonoBehaviour
    {
        private AudioSource _eatSource;
        private AudioPitcher _eatPitcher;

        private void Awake()
        {
            _eatSource = GetComponent<AudioSource>();
            _eatPitcher = GetComponent<AudioPitcher>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Corn>(out var corn))
            {
                corn.Collect();
                _eatPitcher.Pitch = Mathf.Pow(2, Microgame.GetInstance<CollectMicrogame>().GetCollectedCount() / 12f) / 2;
                _eatSource.Play();
            }
        }
    }
}