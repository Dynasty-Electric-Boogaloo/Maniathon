using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mainframe.Scripts.Game.InputProviders
{
    public class InputSystemProvider : Input
    {
        private PlayerInputs _playerInputs;
        private InputAction _stick;
        private InputAction _mainButton;
        private InputAction _backButton;
        
        internal override void Setup()
        {
            _playerInputs = new PlayerInputs();
            _playerInputs.Enable();
            _stick = _playerInputs.FindAction("Stick");
            _mainButton = _playerInputs.FindAction("Main");
            _backButton = _playerInputs.FindAction("Back");

            _stick.performed += StickPerformed;
            _mainButton.performed += ButtonPerformed;
            _mainButton.canceled += ButtonPerformed;
        }

        internal override void Update()
        {
            
        }

        internal override void Cleanup()
        {
            _stick.performed -= StickPerformed;
            _mainButton.performed -= ButtonPerformed;
            
            _playerInputs.Disable();
        }

        internal override bool GetButtonPressed(Button button)
        {
            return button switch
            {
                Button.Main => _mainButton.WasPressedThisFrame(),
                Button.Back => _backButton.WasPressedThisFrame(),
                _ => throw new NotImplementedException()
            };
        }

        internal override bool GetButtonHeld(Button button)
        {
            return button switch
            {
                Button.Main => _mainButton.IsPressed(),
                Button.Back => _backButton.IsPressed(),
                _ => throw new NotImplementedException()
            };
        }

        internal override bool GetButtonReleased(Button button)
        {
            return button switch
            {
                Button.Main => _mainButton.WasReleasedThisFrame(),
                Button.Back => _backButton.WasReleasedThisFrame(),
                _ => throw new NotImplementedException()
            };
        }
        
        public override Vector2 GetJoystick()
        {
            return Vector2.ClampMagnitude(_stick.ReadValue<Vector2>(), 1);
        }

        public override bool GetButtonPressed()
        {
            return _mainButton.WasPressedThisFrame();
        }

        public override bool GetButtonHeld()
        {
            return _mainButton.IsPressed();
        }

        public override bool GetButtonReleased()
        {
            return _mainButton.WasReleasedThisFrame();
        }

        private void StickPerformed(InputAction.CallbackContext ctx)
        {
            OnJoystickPerformed?.Invoke(Vector2.ClampMagnitude(ctx.ReadValue<Vector2>(), 1));
        }

        private void ButtonPerformed(InputAction.CallbackContext ctx)
        {
            OnButtonPerformed?.Invoke(ctx.performed);
        }
    }
}