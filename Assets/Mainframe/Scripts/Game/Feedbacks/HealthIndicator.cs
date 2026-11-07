using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mainframe.Scripts.Game
{
    public class HealthIndicator : MonoBehaviour
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        [SerializeField] private int lifeIndex;
        [Header("Display")]
        [SerializeField] private MeshRenderer display;
        [SerializeField, ColorUsage(true, true)] private Color aliveColor;
        [SerializeField, ColorUsage(true, true)] private Color deadColor;
        [SerializeField, ColorUsage(true, true)] private Color winColor;
        [SerializeField, ColorUsage(true, true)] private Color looseColor;
        [Header("Hand")]
        [SerializeField] private Transform hand;
        [SerializeField] private float aliveSpringForce;
        [SerializeField] private float aliveSpringDamper;
        [SerializeField] private float deadSpringForce;
        [SerializeField] private float deadSpringDamper;
        
        private Material _material;
        private float _time;
        private float _targetAngle;
        private float _velocity;
        private float _angle;
        private Vector3 _eulerAngles;

        private void Awake()
        {
            var materials = display.materials;
            _material = new Material(materials[1]);
            materials[1] = _material;
            display.SetMaterials(new List<Material>(materials));
            _angle = -140;
            _eulerAngles = hand.localEulerAngles;
        }

        private void Update()
        {
            _time += Time.unscaledDeltaTime * Mathf.Pow(2, Game.GetSpeed() / 12.0f);
            _time %= 1f;
            
            switch (Game.GetState())
            {
                case Runner.State.Intermission:
                    UpdateIntermission();
                    break;
                case Runner.State.BossIntermission:
                    UpdateIntermission();
                    break;
                case Runner.State.StartMicrogame:
                    UpdateIntermission();
                    break;
                case Runner.State.Microgame:
                    break;
                case Runner.State.EndMicrogame:
                    UpdateEndMicrogame();
                    break;
                case Runner.State.SpeedUp:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateHand();
        }

        public void OnEndMicrogame()
        {
            _time = 0;
        }

        private void UpdateEndMicrogame()
        {
            if (!IsAlive() || (HasDied() && (_time % .3f) > .15f))
            {
                _material.SetColor(EmissionColor, deadColor);
                return;
            }

            _material.SetColor(EmissionColor, Game.HasWonLastMicrogame() ? winColor : looseColor);
            _targetAngle = Mathf.Sin(_time * Mathf.PI * 6) * (Game.HasWonLastMicrogame() ? 50 : 20) + (Game.HasWonLastMicrogame() ? -90 : -120);
        }

        private void UpdateIntermission()
        {
            _material.SetColor(EmissionColor, IsAlive() ? aliveColor : deadColor);
            _targetAngle = IsAlive() ? -50 + Mathf.Sin(_time * Mathf.PI * 6) * 10 : -140;
        }

        private void UpdateHand()
        {
            var force = IsAlive() ? aliveSpringForce : deadSpringForce;
            var damper = IsAlive() ? aliveSpringDamper : deadSpringDamper;
            
            var diff = _targetAngle - _angle;
            var acceleration = (diff / Time.unscaledDeltaTime) * force - _velocity * damper;
            _velocity += acceleration * Time.unscaledDeltaTime;
            _angle += _velocity * Time.unscaledDeltaTime;
            hand.localEulerAngles = new Vector3(_angle, _eulerAngles.y, _eulerAngles.z);
        }

        private bool IsAlive()
        {
            return lifeIndex < Game.GetLives();
        }

        private bool HasDied()
        {
            return lifeIndex == Game.GetLives() - 1 && !Game.HasWonLastMicrogame();
        }
    }
}