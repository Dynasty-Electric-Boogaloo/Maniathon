using System;
using TMPro;
using UnityEngine;

namespace Mainframe.Scripts.Game
{
    public class InstructionDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textDisplay;
        [SerializeField] private bool screenSpace;
        private bool _visible;

        private void Update()
        {
            textDisplay.enabled = (!Game.IsInMicrogame() ^ screenSpace) && _visible;
        }
        
        public void SetInstruction(string instruction)
        {
            textDisplay.text = instruction;
        }

        public void SetInstructionVisible(bool visible)
        {
            _visible = visible;
        }
    }
}