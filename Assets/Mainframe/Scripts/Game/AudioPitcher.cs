using System.Collections.Generic;
using UnityEngine;

namespace Mainframe.Scripts.Game
{
    /// <summary>
    /// This component controls the pitch of <c>AudioSource</c> components in
    /// the same <c>GameObject</c>.
    /// </summary>
    public class AudioPitcher : MonoBehaviour
    {
        [SerializeField, Range(-1.5f, 1.5f)] private float pitch = 1;
        public List<AudioSource> sources = new ();

        /// <summary>
        /// This value controls how "fast" or "slow" associated AudioSources will play.
        /// The value can go from <c>-1.5f</c> to <c>1.5f</c>.
        /// </summary>
        public float Pitch
        {
            get => pitch;
            set => pitch = Mathf.Clamp(value, -1.5f, 1.5f);
        }
        
        private void Update()
        {
            UpdatePitch();
        }

        private void UpdatePitch()
        {
            foreach (var source in sources)
            {
                source.pitch = pitch * Mathf.Pow(2, Game.GetSpeed() / 12f) * (Game.IsInMicrogame() ? 1 : 0);
            }
        }

        private void OnValidate()
        {
            UpdatePitch();
        }
    }
}