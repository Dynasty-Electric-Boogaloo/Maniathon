using Eflatun.SceneReference;
using UnityEngine;

namespace Mainframe.Scripts.Microgame
{
    [CreateAssetMenu(menuName = "Maniathon/Microgame Info", fileName = "Microgame Info")]
    public class MicrogameInfo : ScriptableObject
    {
        [Tooltip("The microgame's scene.")]
        public SceneReference scene;
        [Tooltip("The microgame's creator credits. Only include names or pseudos.")]
        public string[] creatorNames;
        [Tooltip("An image to represent the microgame when the player is browsing individual microgames.")]
        public Sprite thumbnail;
    }
}