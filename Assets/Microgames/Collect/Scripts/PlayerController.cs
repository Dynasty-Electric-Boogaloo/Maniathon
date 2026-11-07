using System;
using Mainframe.Scripts.Game;
using UnityEngine;

namespace Microgames.Dodge
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        /*[SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField, Range(0f, 0.9999f)] private float power;*/
        private Rigidbody2D _rigidbody;
        private Vector2 _input;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!Game.IsInMicrogame())
                return;
            
            _input = Game.Input.GetJoystick();
            if (_input.magnitude < .1f)
                _input = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (!Game.IsInMicrogame())
                return;

            /*var targetSpeed = _input * moveSpeed;
            var diff = targetSpeed - _rigidbody.linearVelocity;
            var accel = _input.magnitude == 0f ? deceleration : acceleration;
            var force = new Vector2(Mathf.Pow(Mathf.Abs(diff.x) * accel, power) * Mathf.Sign(diff.x),
                Mathf.Pow(Mathf.Abs(diff.y) * accel, power) * Mathf.Sign(diff.y));*/
            
            _rigidbody.linearVelocity = _input * moveSpeed;
        }
    }
}