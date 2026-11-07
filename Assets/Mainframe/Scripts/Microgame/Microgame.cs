using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Mainframe.Scripts.Microgame
{
    /// <summary>
    /// Inherit from and implement this abstract class to define the core logic of your <c>Microgame</c>.
    /// This is the interface between the game and the rest of your microgame.
    /// It handles some global signals like starting and ending the microgame, defines how long the microgame
    /// should last, whether the microgame should be cut off at any point and whether the player successfully completed the goal.
    /// </summary>
    public abstract class Microgame : MonoBehaviour
    {
        [Tooltip("The camera used to render the Microgame.")] [SerializeField]
        private Camera camera;
        [Tooltip("The Microgame's audio listener")] [SerializeField]
        private AudioListener listener;

        protected static Microgame Instance;
        private Light[] _lights;
        private Volume[] _volumes;
        
        /// <summary>
        /// This event is called the moment the <c>Microgame</c> begins.
        /// </summary>
        public UnityEvent onBegin;

        /// <summary>
        /// This event is called the moment the <c>Microgame</c> ends.
        /// </summary>
        public UnityEvent onEnd;
        
        /// <summary>
        /// Invoke this event <b>once</b> when the player completes the <c>Microgame</c>'s goal.
        /// Objects in your microgame can listen to this event to change their behavior or make feedbacks when it is invoked.
        /// </summary>
        public UnityEvent onWin;
        
        /// <summary>
        /// Invoke this event <b>once</b> if the player irreversibly fails the <c>Microgame</c>'s goal.
        /// Objects in your microgame can listen to this event to change their behavior or make feedbacks when it is invoked.
        /// </summary>
        public UnityEvent onLose;
        
        protected virtual void Awake()
        {
            if (Instance == null)
                Instance = this;

            _lights = FindObjectsByType<Light>(FindObjectsInactive.Exclude);
            _volumes = FindObjectsByType<Volume>(FindObjectsInactive.Exclude);
        }

        protected void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// This function is called by the <c>Game</c> as soon as the <c>Microgame</c> is loaded.
        /// Should you need to disable some objects before the <c>Microgame</c> starts to prevent your logic from executing too
        /// early, you can override this function (whilst still calling <c>base.Setup()</c>)
        /// </summary>
        public virtual void Setup()
        {
            camera.enabled = false;
            listener.enabled = false;
            SetLightsVisible(false);
        }

        /// <summary>
        /// This function is called by the <c>Game</c> right before unloading the <c>Microgame</c>.
        /// Should you need to clean up some things you've set up for your <c>Microgame</c>, you can override this function.
        /// </summary>
        public virtual void Cleanup()
        {
            
        }

        /// <summary>
        /// This function is called by the <c>Game</c> as the <c>Microgame</c> starts playing properly.
        /// This can be used to signal other objects in the <c>Microgame</c>.
        /// You can alternatively check for <c>Game.GetState() == Microgame</c>.
        /// </summary>
        public virtual void Begin()
        {
            camera.enabled = true;
            listener.enabled = true;
            SetLightsVisible(true);
            SetVolumesActive(true);
            onBegin?.Invoke();
        }

        /// <summary>
        /// This function is called by the <c>Game</c> as the <c>Microgame</c> finishes playing.
        /// This can be used to signal other objects in the <c>Microgame</c>.
        /// You can alternatively check for <c>Game.GetState() != Microgame</c>.
        /// The game will be unloaded shortly after this function is called.
        /// </summary>
        public virtual void End()
        {
            camera.enabled = false;
            listener.enabled = false;
            SetLightsVisible(false);
            SetVolumesActive(false);
            onEnd?.Invoke();
        }

        /// <summary>
        /// This function is called by the <c>Game</c> to check if the <c>Microgame</c> is a boss
        /// microgame.
        /// </summary>
        /// <returns>Whether the <c>Microgame</c> is a boss microgame</returns>
        public virtual bool IsBoss()
        {
            return false;
        }

        /// <summary>
        /// The return value of this function is used by <c>Game</c> before the start of the <c>Microgame</c> to display
        /// to the player the instruction for playing your <c>Microgame</c>. This instruction should be a very short phrase
        /// containing a strong action verb, and optionally a few additional words for more context.
        /// </summary>
        /// <returns>The instruction text to display to the player.</returns>
        public abstract string GetInstruction();

        /// <summary>
        /// The return value of this function is used by <c>Game</c> to determine how long the <c>Microgame</c>
        /// can last. If <see cref="IsFinished"/> doesn't return true by that time, the <c>Microgame</c> will be cut off,
        /// and <see cref="IsSuccess"/> will be used to determine whether the player completed the goal.
        /// Should the maximum duration of the <c>Microgame</c> change due to the game's current context, such as difficulty,
        /// or cutscenes you can implement that logic in this function.
        /// For regular <c>Microgame</c>s, please keep the total duration of a <c>Microgame</c> between around 3 to 6 seconds.
        /// Keep in mind the game regularly ramps up the execution speed and thus time passes faster.
        /// </summary>
        /// <returns>The maximum duration of the <c>Microgame</c></returns>
        public abstract float GetDuration();
        
        /// <summary>
        /// The return value of this function is used by <c>Game</c> to determine whether the <c>Microgame</c> has finished early.
        /// For instance, if your microgame is supposed to last 6 seconds, but the player has completed the goal in 4 seconds,
        /// you can return true from this function to cut the <c>Microgame</c> short and save 2 seconds of waiting time.
        /// </summary>
        /// <returns>Whether the <c>Microgame</c> is completed and can be cut off</returns>
        public abstract bool IsFinished();
        
        /// <returns>Whether the player has completed the <c>Microgame</c>'s goal</returns>
        public abstract bool IsSuccess();
        
        /// <returns>The base typed instance of your <c>Microgame</c></returns>
        public static Microgame GetInstance()
        {
            return Instance;
        }
        
        /// <typeparam name="T">The specific type of your <c>Microgame</c></typeparam>
        /// <returns>The specified typed instance of your <c>Microgame</c></returns>
        public static T GetInstance<T>() where T : Microgame
        {
            return Instance as T;
        }

        private void SetLightsVisible(bool visible)
        {
            foreach (var light in _lights)
            {
                if (light.type == LightType.Directional)
                    light.enabled = visible;
            }
        }

        private void SetVolumesActive(bool active)
        {
            foreach (var volume in _volumes)
            {
                volume.enabled = active;
            }
        }
    }
}