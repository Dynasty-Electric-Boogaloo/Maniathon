using System;
using Mainframe.Scripts.Game;
using Mainframe.Scripts.Microgame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Microgames.Knock
{
    public class KnockMicrogame : Microgame
    {
        [SerializeField] private int knockCountMin;
        [SerializeField] private int knockCountMax;
        [SerializeField] private Door door;
        [SerializeField] private Guy guy;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private HandController handController;
        private int _knockCounter;
        private float _finishTimer;
        
        public override void Begin()
        {
            base.Begin();
            _knockCounter = Random.Range(knockCountMin, knockCountMax);
            cameraController.Approach();
            handController.Appear();
        }

        private void Update()
        {
            _finishTimer -= Time.deltaTime;
        }

        public override string GetInstruction()
        {
            return "Knock!";
        }

        public override float GetDuration()
        {
            // Prevent the game from being cut off when the door is opened and the guy talks.
            // This relies on IsFinished to return true.
            // Do not make a Microgame last forever without making absolutely sure IsFinished will return true and the
            // Microgame roughly lasts 3 to 6 seconds
            return _knockCounter > 0 ? 5 : Mathf.Infinity;
        }

        public override bool IsFinished()
        {
            return _knockCounter <= 0 && _finishTimer < 0;
        }

        public override bool IsSuccess()
        {
            return _knockCounter == 0;
        }

        public void Knock()
        {
            _knockCounter--;
            if (_knockCounter <= 0)
            {
                if (_knockCounter < 0)
                {
                    if (_knockCounter == -1)
                        onLose?.Invoke();
                    
                    guy.Punch();
                    cameraController.Shake();
                }
                else
                {
                    guy.OnDoorOpen();
                    _finishTimer = 1.5f;
                }

                door.Open();
            }
        }

        public bool IsDoorOpen()
        {
            return _knockCounter <= 0;
        }
    }
}