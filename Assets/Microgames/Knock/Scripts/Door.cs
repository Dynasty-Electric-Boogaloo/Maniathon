using UnityEngine;

namespace Microgames.Knock
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private float openAngle;
        
        public void Open()
        {
            transform.localEulerAngles = new Vector3(0, openAngle, 0);
        }
    }
}