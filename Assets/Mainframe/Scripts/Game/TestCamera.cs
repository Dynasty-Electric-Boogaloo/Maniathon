using System;
using UnityEngine;

namespace Mainframe.Scripts.Game
{
    public class TestCamera : MonoBehaviour
    {
        [SerializeField] private Camera trackedCamera;
        [SerializeField] private Transform upPosition;
        [SerializeField] private Transform downPosition;
        //[SerializeField] private float upFov;
        //[SerializeField] private float downFov;
        [SerializeField] private float speed;
        [SerializeField] private AnimationCurve positionCurve;
        [SerializeField] private AnimationCurve rotationCurve;
        //[SerializeField] private AnimationCurve fovCurve;
        private bool _up;
        private float _position;
        
        private void Update()
        {
            _up = Game.GetState() switch
            {
                Runner.State.Intermission => false,
                Runner.State.BossIntermission => false,
                Runner.State.StartMicrogame => true,
                Runner.State.Microgame => true,
                Runner.State.EndMicrogame => false,
                Runner.State.SpeedUp => false,
                _ => throw new ArgumentOutOfRangeException()
            };

            _position = Mathf.MoveTowards(_position, _up ? 1 : 0, speed * Time.unscaledDeltaTime * Mathf.Pow(2, Game.GetSpeed() / 12.0f));
            trackedCamera.transform.position = Vector3.Lerp(downPosition.position, upPosition.position, positionCurve.Evaluate(_position));
            trackedCamera.transform.rotation = Quaternion.Lerp(downPosition.rotation, upPosition.rotation, rotationCurve.Evaluate(_position));
            //trackedCamera.fieldOfView = Mathf.Lerp(downFov, upFov, fovCurve.Evaluate(_position));
        }
    }
}