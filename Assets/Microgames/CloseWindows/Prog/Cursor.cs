using System;
using Mainframe.Scripts.Game;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private Bounds bounds;
    private Window _windows; 
    private AudioSource _sound;
    private Animation _animation;
    private RectTransform _rectTransform;
    private bool _hitting;
    private float _timer;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
        _sound = GetComponent<AudioSource>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Update()
    {
        Move(Game.Input.GetJoystick());
            
        if (!Game.Input.GetButtonPressed()) 
            return;
            
        _sound.Play();
        _animation.Play();
        
        if (!_windows) 
            return;
        
        _windows.OnHit();
        _windows = null;
    }

    private void Move(Vector2 movement)
    {
        _rectTransform.position += new Vector3(movement.x, movement.y) * (Time.deltaTime * speed);
        
        _rectTransform.position = new Vector3(Math.Clamp( _rectTransform.position.x, bounds.min.x, bounds.max.x),
            Math.Clamp( _rectTransform.position.y, bounds.min.y, bounds.max.y), 98);
    }
        
    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Window>(out var newWindow))
            _windows = newWindow;
    }
        
    private void OnTriggerExit2D (Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<Window>(out var newWindow)) 
            return;
        
        if (_windows == newWindow)
            _windows = null;
    }
}
