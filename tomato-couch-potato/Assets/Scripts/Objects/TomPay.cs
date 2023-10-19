using Chickenen.Pancreas;
using UnityEngine;

namespace Chickenen.Heart
{
    public class TomPay : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("決済時の音")]
        AudioClip sound;

        bool isPaid = false;
        public bool IsPaid => isPaid;

        Bank bank;

        void Start()
        {
            bank = Gobject.GetWithTag<Bank>(Constant.Tags.Player);
        }

        void OnTriggerEnter2D(Collider2D info)
        {
            if (info.CompareTag(Constant.Tags.Player))
            {
                bank.Fluc(12);
                isPaid = true;
            }
        }
    }
}