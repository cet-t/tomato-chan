using UnityEngine;
using Chickenen.Pancreas;
using Cysharp.Threading.Tasks;

namespace Chickenen.Heart
{
    public class ChiliPepper : Object
    {
        protected override void Behavior() { }

        async void OnTriggerEnter2D(Collider2D info)
        {
            if (info.CompareBoth(Constant.Layers.Player, Constant.Tags.Player))
            {
                effects.TryGenerate(transform.position);
                await info.Get<Player>().Die(CuzOfDeath.Venom);
            }
        }
    }
}