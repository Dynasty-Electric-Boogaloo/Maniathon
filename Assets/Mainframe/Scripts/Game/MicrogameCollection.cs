using Mainframe.Scripts.Microgame;
using UnityEngine;

namespace Mainframe.Scripts.Game
{
    [CreateAssetMenu(menuName = "Maniathon/Microgame Collection", fileName = "Microgame Collection")]
    public class MicrogameCollection : ScriptableObject
    {
        public string displayName;
        public Color color;
        public MicrogameInfo[] microgames;
        public int campaignPlayCount;
        public int speedUpStride;
    }
}