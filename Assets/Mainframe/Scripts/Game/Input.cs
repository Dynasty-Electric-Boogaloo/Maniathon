using UnityEngine;
using UnityEngine.Events;

namespace Mainframe.Scripts.Game
{
    public abstract class Input
    {
        internal enum Button
        {
            Main,
            Back
        }
        
        /// <summary>
        /// This event is invoked every time the joystick's value is changed.
        /// </summary>
        public UnityEvent<Vector2> OnJoystickPerformed = new ();
        /// <summary>
        /// This event is invoked every time the button's state is changed.
        /// </summary>
        public UnityEvent<bool> OnButtonPerformed = new ();
        
        internal abstract void Setup();
        internal abstract void Update();
        internal abstract void Cleanup();

        internal void ClearCallbacks()
        {
            OnJoystickPerformed.RemoveAllListeners();
            OnButtonPerformed.RemoveAllListeners();
        }

        internal abstract bool GetButtonPressed(Button button);
        internal abstract bool GetButtonHeld(Button button);
        internal abstract bool GetButtonReleased(Button button);
        
        /// <returns>The current state of the joystick</returns>
        public abstract Vector2 GetJoystick();
        
        /// <returns>Whether the main action button was pressed down this frame.</returns>
        public abstract bool GetButtonPressed();
        /// <returns>Whether the main action button is currently being held down.</returns>
        public abstract bool GetButtonHeld();
        /// <returns>Whether the main action button was released this frame.</returns>
        public abstract bool GetButtonReleased();
    }
}