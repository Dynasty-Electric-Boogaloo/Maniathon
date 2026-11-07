using System;
using TMPro;
using UnityEngine;

namespace Mainframe.Scripts.Game
{
    public class RoundDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textDisplay;
        private bool _visible;

        private void Update()
        {
            textDisplay.text = Game.GetScore().ToString();
            textDisplay.enabled = !Game.IsInMicrogame() && _visible;
        }

        public void SetRoundVisible(bool visible)
        {
            _visible = visible;
        }
    }
}