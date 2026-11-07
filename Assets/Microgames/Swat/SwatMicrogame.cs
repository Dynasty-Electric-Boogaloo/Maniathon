using System;
using System.Collections.Generic;
using Mainframe.Scripts.Game;
using Mainframe.Scripts.Microgame;
using UnityEngine;

public class SwatMicrogame : Microgame
{
    [Header("Background")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Sprite[] colors;
    
    [Header("GamePlay Element")] 
    [SerializeField] private SwatFly fly;    
    private readonly HashSet<SwatFly> _flies = new HashSet<SwatFly>();
    private float timer = 2;
    private bool isWon; 

    public override string GetInstruction()
    {
        return "Swat 'em!";
    }

    public override float GetDuration()
    {
        return 4;
    }

    public override bool IsFinished()
    {
        return _flies.Count <= 0 && Game.IsInMicrogame() && timer <= 0;
    }

    public override bool IsSuccess()
    {
        return _flies.Count <= 0 && Game.IsInMicrogame();
    }

    public override void Begin()
    {
        base.Begin();
          
        for (var i = 0; i < Game.GetDifficulty() + 1; i++)
        {
            var current = Instantiate(fly);
            _flies.Add(current);
        }
    }

    private void Update()
    {
        if (isWon)
            timer -= Time.deltaTime; 
    }

    public override void Setup()
    {
        base.Setup();
        backgroundRenderer.sprite = colors[Math.Clamp(Game.GetDifficulty(), 0, colors.Length-1)];
    }
    
    public void HitFly(SwatFly currentFly)
    {
        _flies.Remove(currentFly);
        
        if (_flies.Count > 0) 
            return;
        
        onWin?.Invoke();
        isWon = true;
    }

}