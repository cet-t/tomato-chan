using System.Collections;
using UnityEngine;

namespace trrne.Box
{
    public class CameraShake : MonoBehaviour
    {
        public void Shake(float duration, float magnitude)
        {
            StartCoroutine(DoShake(duration, magnitude));
        }

        private IEnumerator DoShake(float duration, float magnitude)
        {
            var pos = transform.localPosition;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                var x = pos.x + Random.Range(-1f, 1f) * magnitude;
                var y = pos.y + Random.Range(-1f, 1f) * magnitude;
                transform.localPosition = new(x, y, pos.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = pos;
        }
    }

}