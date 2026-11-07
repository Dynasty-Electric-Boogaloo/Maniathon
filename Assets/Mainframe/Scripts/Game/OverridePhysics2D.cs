using System;
using UnityEngine;

namespace Mainframe.Scripts.Game
{
    /// <summary>
    /// This component allows microgames to override the 2D gravity and layer collision mask for their duration.
    /// This is a singleton component, you only need to place it once in your scene.
    /// </summary>
    public class OverridePhysics2D : MonoBehaviour
    {
        private static OverridePhysics2D _instance;
        
        [SerializeField] private Vector2 gravity = new Vector3(0, -9.81f);
        [SerializeField] private int maxLayers = 1;
        [SerializeField] private uint[] collisionLayerMatrix = {
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF,
            0xFFFFFFFF
        };

        private Vector2 _defaultGravity;
        private readonly uint[] _defaultCollisionLayerMatrix = new uint[32];

        /// <summary>
        /// Acceleration due to gravity.
        /// </summary>
        public static Vector2 Gravity
        {
            get
            {
                if (!_instance)
                {
                    Debug.LogError("No instance of OverridePhysics2D exists! Make sure to add one to your Microgame. Defaulting to Physics.gravity.");
                    return Physics2D.gravity;
                }
                
                return _instance.gravity;
            }
            set
            {
                if (!_instance)
                {
                    Debug.LogError("No instance of OverridePhysics2D exists! Make sure to add one to your Microgame. Defaulting to Physics2D.gravity.");
                    Physics2D.gravity = value;
                    return;
                }
                
                _instance.gravity = value;
                
                if (Game.IsInMicrogame())
                    _instance.ApplyGravity();
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }

            _instance = this;
        }

        private void Start()
        {
            Microgame.Microgame.GetInstance().onBegin.AddListener(OnBegin);
            Microgame.Microgame.GetInstance().onEnd.AddListener(OnEnd);
        }

        private void OnBegin()
        {
            BackupDefaults();
            ApplyGravity();
            ApplyCollisionLayerMatrix();
        }
        
        private void OnEnd()
        {
            RestoreDefaults();
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        /// <summary>
        /// Checks whether collisions between the specified layers be ignored or not.
        /// </summary>
        /// <param name="layer1">ID of the first layer.</param>
        /// <param name="layer2">ID of the second layer.</param>
        /// <returns>Whether collisions between the specified layers be ignored or not.</returns>
        public static bool GetIgnoreLayerCollision(int layer1, int layer2)
        {
            if (!_instance)
            {
                Debug.LogError("No instance of OverridePhysics2D exists! Make sure to add one to your Microgame. Defaulting to Physics2D.GetIgnoreLayerCollision.");
                return Physics2D.GetIgnoreLayerCollision(layer1, layer2);
            }
            
            if (layer1 > layer2)
                (layer1, layer2) = (layer2, layer1);

            return (_instance.collisionLayerMatrix[layer1] & (1U << layer2)) == 0;
        }

        /// <summary>
        /// Set collisions between a specified pair of layers to be ignored.
        /// Note that IgnoreLayerCollision will reset the trigger state of affected colliders, so you might receive OnTriggerExit and OnTriggerEnter messages in response to calling this.
        /// </summary>
        /// <param name="layer1">ID of the first layer.</param>
        /// <param name="layer2">ID of the second layer.</param>
        public static void IgnoreLayerCollision(int layer1, int layer2)
        {
            if (!_instance)
            {
                Debug.LogError("No instance of OverridePhysics2D exists! Make sure to add one to your Microgame. Defaulting to Physics2D.IgnoreLayerCollision.");
                Physics2D.IgnoreLayerCollision(layer1, layer2);
                return;
            }
            
            if (layer1 > layer2)
                (layer1, layer2) = (layer2, layer1);

            _instance.collisionLayerMatrix[layer1] &= ~(1U << layer2);
            
            if (Game.IsInMicrogame())
                _instance.ApplyCollisionLayerMatrix();
        }
        
        /// <summary>
        /// Choose whether to detect or ignore collisions between a specified pair of layers.
        /// Note that IgnoreLayerCollision will reset the trigger state of affected colliders, so you might receive OnTriggerExit and OnTriggerEnter messages in response to calling this.
        /// </summary>
        /// <param name="layer1">ID of the first layer.</param>
        /// <param name="layer2">ID of the second layer.</param>
        /// <param name="ignore">Should collisions between these layers be ignored?</param>
        public static void IgnoreLayerCollision(int layer1, int layer2, bool ignore)
        {
            if (!_instance)
            {
                Debug.LogError("No instance of OverridePhysics2D exists! Make sure to add one to your Microgame. Defaulting to Physics2D.IgnoreLayerCollision.");
                Physics2D.IgnoreLayerCollision(layer1, layer2, ignore);
                return;
            }
            
            if (layer1 > layer2)
                (layer1, layer2) = (layer2, layer1);
            
            _instance.collisionLayerMatrix[layer1] &= ~(1U << layer2);
            _instance.collisionLayerMatrix[layer1] |= ignore ? 0 : (1U << layer2);
            
            if (Game.IsInMicrogame())
                _instance.ApplyCollisionLayerMatrix();
        }

        private void BackupDefaults()
        {
            _defaultGravity = Physics2D.gravity;

            for (var i = 0; i < 31; i++)
            {
                _defaultCollisionLayerMatrix[i] = 0;
                for (var j = i; j < 31; j++)
                { 
                    _defaultCollisionLayerMatrix[i] |= (Physics2D.GetIgnoreLayerCollision(i, j) ? 0U : 1U) << j;
                }
            }
        }

        private void RestoreDefaults()
        {
            Physics2D.gravity = _defaultGravity;

            for (var i = 0; i < 31; i++)
            {
                for (var j = i; j < 31; j++)
                {
                    Physics2D.IgnoreLayerCollision(i, j, (_defaultCollisionLayerMatrix[i] & (1 << j)) == 0);
                }
            }
        }

        private void ApplyGravity()
        {
            Physics2D.gravity = gravity;
        }

        private void ApplyCollisionLayerMatrix()
        {
            for (var i = 0; i < 31; i++)
            {
                for (var j = i; j < 31; j++)
                {
                    Physics2D.IgnoreLayerCollision(i, j, (collisionLayerMatrix[i] & (1 << j)) == 0);
                }
            }
        }
    }
}