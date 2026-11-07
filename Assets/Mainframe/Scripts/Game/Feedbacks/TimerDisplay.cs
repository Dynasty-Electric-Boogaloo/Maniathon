using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mainframe.Scripts.Game
{
    public class TimerDisplay : MonoBehaviour
    {
        [Header("Global")]
        [SerializeField] private GameObject timer;
        [SerializeField] private float timeOffset;
        [SerializeField] private float timerTime;
        [SerializeField] private float tapeCutTime;
        [SerializeField] private float tapeThickness;
        [SerializeField] private AnimationCurve motionCurve;
        [SerializeField] private TMP_Text timeText;
        
        [Header("Top reel")]
        [SerializeField] private RectTransform topCore;
        [SerializeField] private RectTransform[] topTapes;
        [SerializeField] private RectTransform topOutline;
        
        [Header("Bottom reel")]
        [SerializeField] private RectTransform bottomCore;
        [SerializeField] private RectTransform[] bottomTapes;
        [SerializeField] private RectTransform bottomOutline;

        [Header("Tape")]
        [SerializeField] private RectTransform tapeTransform;
        [SerializeField] private Image tapeImage;
        [SerializeField] private Image tapeOutlineImage;
        [SerializeField] private Color[] tapeColors;

        private void Update()
        {
            var time = Game.GetRemainingTime();
            var shouldDisplay = Game.IsInMicrogame() && time < timerTime + timeOffset;
            timer.SetActive(shouldDisplay);

            if (!shouldDisplay)
                return;

            time -= timeOffset;
            
            topCore.localRotation = Quaternion.Euler(0, 0, time * 360 * 6);
            bottomCore.localRotation = Quaternion.Euler(0, 0, -time * 360 * 6);

            var timeFactor = (time - tapeCutTime) / (timerTime - tapeCutTime);
            var moveFactor = motionCurve.Evaluate(Mathf.Clamp01(timeFactor));
            
            topOutline.sizeDelta = Vector2.one * (55 + tapeThickness * moveFactor);
            bottomOutline.sizeDelta = Vector2.one * (80 + tapeThickness * (1 - moveFactor));
            tapeTransform.anchoredPosition = new Vector2(Mathf.Lerp(-23, 15.5f, moveFactor), 17.41f);
            tapeImage.color = tapeColors[Mathf.Clamp(Mathf.FloorToInt(moveFactor * 3), 0, 2)];
            timeText.text = (Mathf.Clamp(Mathf.FloorToInt(motionCurve.Evaluate(timeFactor) * 3) + 1, 0, 3)).ToString();
            
            for (var i = 0; i < 3; i++)
            {
                var underSize = tapeThickness / 3 * i;
                var size = tapeThickness / 3 * (i + 1);
                var topSize = Mathf.LerpUnclamped(50 + underSize, 50 + size, Mathf.Min(moveFactor * 3 - i, 1));
                var bottomSize = Mathf.LerpUnclamped(75 + size, 75 + underSize, Mathf.Max(moveFactor * 3 - (2 - i), 0));
                
                topTapes[i].sizeDelta = Vector2.one * topSize;
                bottomTapes[i].sizeDelta = Vector2.one * bottomSize;
            }

            var tapeCutFactor = time / tapeCutTime;
            tapeImage.fillAmount = tapeCutFactor;
            tapeOutlineImage.fillAmount = tapeCutFactor;
        }
    }
}