using Mainframe.Scripts.Microgame;
using UnityEngine;

namespace Microgames.Dodge
{
    public class Corn : MonoBehaviour
    {
        public void Collect()
        {
            Microgame.GetInstance<CollectMicrogame>().CollectCorn(this);
            Destroy(gameObject);
        }
    }
}