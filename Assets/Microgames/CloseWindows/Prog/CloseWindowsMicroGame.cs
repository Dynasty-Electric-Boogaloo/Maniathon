using System;
using System.Collections.Generic;
using Mainframe.Scripts.Game;
using Mainframe.Scripts.Microgame;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CloseWindowsMicroGame : Microgame
{
    [SerializeField] private Window window;
    [SerializeField] private Transform parent;
    [SerializeField] private Bounds bounds;
    private readonly HashSet<Window> _windows = new HashSet<Window>();
    [SerializeField] private Image backgroundRenderer;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private Sprite[] colors;
    private float timer = 2;
    private bool isWon; 
    
    public override string GetInstruction()
    {
        return "Close it!";
    }

    public override float GetDuration()
    {
        return 4;
    }

    public override bool IsFinished()
    {
        return _windows.Count <= 0 && Game.IsInMicrogame() && timer <= 0;
    }

    public override bool IsSuccess()
    {
        return  _windows.Count <= 0;
    }

    public override void Begin()
    {
        base.Begin();
        var current = Instantiate(window, new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), 98),Quaternion.identity , parent );
        _windows.Add(current); 
    }
    
    
    private void Update()
    {
        if (isWon)
            timer -= Time.deltaTime; 
    }
    
    public override void Setup()
    {
        base.Setup();
        endScreen.SetActive(Game.GetDifficulty() >= 5); 
        backgroundRenderer.sprite = colors[Math.Clamp(Game.GetDifficulty(), 0, colors.Length-1)];
    }
    
    public void CloseWindow(Window currentWindow)
    {
        _windows.Remove(currentWindow);
        
        if (_windows.Count > 0) 
            return;
        
        endScreen.SetActive(false);
        backgroundRenderer.sprite = colors[0];
        onWin?.Invoke();
        isWon = true;
    }
}