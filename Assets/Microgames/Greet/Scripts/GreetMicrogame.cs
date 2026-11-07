using System;
using System.Collections;
using System.Collections.Generic;
using Mainframe.Scripts.Game;
using Mainframe.Scripts.Microgame;
using Microgames.Greet.Scripts;
using UnityEngine;

public class GreetMicrogame : Microgame
{
    [SerializeField] private Transform otherGuyTransform;
    [SerializeField] private Transform otherGuyStart;
    [SerializeField] private Transform otherGuyEnd;
    [SerializeField] private Transform guyShoulder;
    [SerializeField] private Transform otherShoulder;
    [SerializeField] private SpriteRenderer guyHand;
    [SerializeField] private SpriteRenderer otherHand;
    [SerializeField] private SpriteRenderer dapHand;
    [SerializeField] private AnimationCurve dapCurve;
    [SerializeField] private float margin;
    private bool _success;
    
    public override void Begin()
    {
        base.Begin();
        
        guyShoulder.gameObject.SetActive(false);
        otherShoulder.gameObject.SetActive(false);
        guyHand.enabled = true;
        otherHand.enabled = true;
        dapHand.enabled = false;
        
        StartCoroutine(Microgame());
    }

    private IEnumerator Microgame()
    {
        yield return Arrive();
        yield return WindUp();

        if (_success)
            yield return DapSuccess();
        else
            yield return DapFail();
    }

    private IEnumerator Arrive()
    {
        var timer = 0f;
        
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            otherGuyTransform.position = Vector3.Lerp(otherGuyStart.position, otherGuyEnd.position, timer) + Vector3.up * (Mathf.Abs(Mathf.Sin(timer * Mathf.PI * 4)) * .5f);
            yield return null;
        }

        otherGuyTransform.position = otherGuyEnd.position;
    }

    private IEnumerator WindUp()
    {
        var timer = 0f;
        
        guyShoulder.gameObject.SetActive(true);
        otherShoulder.gameObject.SetActive(true);

        _success = false;

        while (timer < .4f + margin)
        {
            timer += Time.deltaTime;
            var factor = dapCurve.Evaluate(Mathf.Clamp01(timer / (0.4f + margin / 2)));
            guyShoulder.eulerAngles = new Vector3(0, Mathf.LerpUnclamped(-106, 0, factor), 0);
            otherShoulder.eulerAngles = new Vector3(0, Mathf.LerpUnclamped(-60, 0, factor), 0);

            if (timer > .4f && Game.Input.GetButtonPressed())
            {
                _success = true;
                guyShoulder.eulerAngles = Vector3.zero;
                otherShoulder.eulerAngles = Vector3.zero;
                yield break;
            }

            yield return null;
        }

        guyShoulder.eulerAngles = Vector3.zero;
        otherShoulder.eulerAngles = Vector3.zero;
    }

    private IEnumerator DapSuccess()
    {
        onWin?.Invoke();
        
        guyHand.enabled = false;
        otherHand.enabled = false;
        dapHand.enabled = true;
        
        yield return null;
    }

    private IEnumerator DapFail()
    {
        onLose?.Invoke();
        yield return null;
    }

    public override string GetInstruction()
    {
        return "Dap up!";
    }

    public override float GetDuration()
    {
        return 3;
    }

    public override bool IsFinished()
    {
        return false;
    }

    public override bool IsSuccess()
    {
        return _success;
    }
}