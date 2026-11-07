using System;
using System.Collections;
using Mainframe.Scripts.Microgame;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SwatFly : MonoBehaviour
{
    [SerializeField] private Bounds bounds;
    private SwatMicrogame _swatMicrogame;
    private Vector3 _target;
    private SpriteRenderer _renderer;
    private bool _stop;
    
    private void Start()
    {
        _swatMicrogame = Microgame.GetInstance() as SwatMicrogame;
        _renderer = GetComponent<SpriteRenderer>();

        transform.position = GetRandomPos; 
        
        SetTarget();
    }

    private void Update()
    {
        if(!_stop)
            Move();
    }
    
    private void Move()
    {
        if (_target == transform.position)
            SetTarget();
        else
            transform.position = Vector2.MoveTowards(transform.position, _target, Time.deltaTime);
    }

    private Vector2 GetRandomPos => new (Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    
    private void SetTarget()
    {
        _target = GetRandomPos;
    }
    
    public void OnHit()
    {
        _swatMicrogame.HitFly(this);
        StartCoroutine(Dying());
    }

    private IEnumerator Dying()
    {
        _stop = true;
        _renderer.color = Color.red;
        var endPosition = new Vector3(transform.position.x, -10);
        var t = 0f;
        var timeToMove = Vector3.Distance(transform.position,endPosition);
        while(t < 1)
        {
            transform.position = Vector3.Lerp(transform.position,endPosition,t);
            transform.Rotate(0f, 0f, Mathf.Lerp(-225, 0, -timeToMove * 1.5f));
            t += Time.deltaTime / timeToMove / 10;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
