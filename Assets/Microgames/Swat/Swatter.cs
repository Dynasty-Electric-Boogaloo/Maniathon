using System;
using System.Collections;
using System.Collections.Generic;
using Mainframe.Scripts.Game;
using UnityEngine;

namespace Microgames.Swat
{
    public class Swatter : MonoBehaviour
    {
        [SerializeField] private float speed = 5;
        [SerializeField] private Bounds bounds;
        private readonly HashSet<SwatFly> _overlappingFlies = new HashSet<SwatFly>(); 
        private AudioSource _sound;
        private Animation _animation;
        private bool _hitting;
        private float _timer;

        private void Awake()
        {
            _animation = GetComponent<Animation>();
            _sound = GetComponent<AudioSource>();
        }

        public void Update()
        {
            Move(Game.Input.GetJoystick());
            
            if (!Game.Input.GetButtonPressed()) 
                return;
            
            _sound.Play();
            _animation.GetClip("swatter");
            _animation.Play();
            
            foreach (var fly in _overlappingFlies)
                fly.OnHit();
            
            _overlappingFlies.Clear();
        }

        private void Move(Vector2 movement)
        {
            transform.position += new Vector3(movement.x, movement.y) * (Time.deltaTime * speed);
            transform.position = new Vector3(Math.Clamp(transform.position.x, bounds.min.x, bounds.max.x),
                Math.Clamp(transform.position.y, bounds.min.y, bounds.max.y));
        }
        
        private void OnTriggerEnter2D (Collider2D other)
        {
            if (other.gameObject.TryGetComponent<SwatFly>(out var fly))
                _overlappingFlies.Add(fly);
        }
        
        private void OnTriggerExit2D (Collider2D other)
        {
            if (other.gameObject.TryGetComponent<SwatFly>(out var fly))
                _overlappingFlies.Remove(fly);
        }
    }
}
