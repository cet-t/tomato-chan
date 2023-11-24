using DG.Tweening;
using trrne.Box;
using UnityEngine;

namespace trrne.Core
{
    public class Portal : Object
    {
        [SerializeField]
        PortalGoal portalGoal;

        [SerializeField]
        float teleportSpeed = 1.5f;

        [SerializeField]
        float framesAlpha = .75f;

        const float SpeedRange = 30;

        [SerializeField]
        GameObject[] frames;
        float[] speeds;
        float myspeed;
        bool warping = false;
        int children;

        protected override void Start()
        {
            base.Start();

            // パーティクルを除くため-1
            children = frames.Length;

            speeds = new float[children];
            for (int i = 0; i < children; i++)
            {
                frames[i].GetComponent<SpriteRenderer>().SetAlpha(framesAlpha);
                speeds[i] = Randoms.Single(-SpeedRange, SpeedRange);
            }
            myspeed = Randoms.Single(-SpeedRange, SpeedRange);
        }

        protected override void Behavior()
        {
#if DEBUG
            // draw line to goal
            Debug.DrawLine(transform.position, portalGoal.Goal);
#endif

            // フレームを回転させる
            for (int i = 0; i < children; i++)
                frames[i].transform.Rotate(Time.deltaTime * speeds[i] * Coordinate.V3Z);

            // ついでに中心も回転させる
            transform.Rotate(Time.deltaTime * myspeed * Coordinate.V3Z);
        }

        void OnTriggerEnter2D(Collider2D info)
        {
            if (warping && !info.CompareTag(Constant.Tags.Player))
                return;

            if (info.TryGetComponent(out Player player) && !player.IsDying)
                info.transform.DOMove(portalGoal.Goal, teleportSpeed)
                    .SetEase(Ease.OutCubic)
                    .OnPlay(() =>
                    {
                        warping = true;
                        player.IsTeleporting = true;
                        effects.TryInstantiate(transform.position);
                    })
                    .OnComplete(() =>
                    {
                        player.IsTeleporting = false;
                        warping = false;
                    });
        }
    }
}
