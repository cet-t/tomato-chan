using UnityEngine;

namespace trrne.Core
{
    public class MovingFloorFlag : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D info)
        {
            if (info.TryGetComponent(out Player _) && info.transform.parent != transform)
            {
                info.transform.parent = transform;
            }
        }

        void OnTriggerExit2D(Collider2D info)
        {
            if (info.TryGetComponent(out Player _) && info.transform.parent != null)
            {
                info.transform.parent = null;
            }
        }
    }
}
