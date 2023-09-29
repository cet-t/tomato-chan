﻿using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using trrne.Bag;
using Cysharp.Threading.Tasks;
using System.Collections;

namespace trrne.Body
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        Text velT;

        [SerializeField]
        GameObject diefx;

        public bool Ctrlable { get; set; }
        public bool Jumpable { get; set; }
        public bool Walkable { get; set; }

        (float Recharge, float progress, bool able) respawn = (0.5f, 0f, false);
        public float Progress => respawn.progress;

        /// <summary>
        /// 氷の上に乗っていたらtrue
        /// </summary>
        bool onIce;

        /// <summary>
        /// 死亡処理中はtrue
        /// </summary>
        public bool IsDieProcessing { get; set; }

        readonly (float basis, float max, float freduction, float reduction) speed = (20, 10, 0.1f, 0.9f);

        /// <summary>
        /// 始点<br/>
        /// 長さ<br/>
        /// 対象のレイヤー
        /// </summary>
        (Vector3 offset, float distance, int layers) rayconf = (new(), 0.25f, Constant.Layers.Ground | Constant.Layers.Object);

        (bool during, float power, float hitbox) jump = (false, 2f, 0.9f);

        bool isFloating;
        /// <summary>
        /// 地に足がついていなかったらtrue
        /// </summary>
        public bool IsFloating => isFloating;

        float velocity;
        /// <summary>
        /// 移動速度
        /// </summary>
        public float Velocity => velocity;

        // SpriteRenderer sr;
        Rigidbody2D rb;
        new BoxCollider2D collider;

        PlayerJumpFlag pjf;

        readonly float tolerance = 0.33f;

        Cam cam;

        Vector3 checkpoint = Vector3.zero;
        /// <summary>
        /// チェックポイントを設定する
        /// </summary>
        public void SetCheckpoint(Vector3 point) => checkpoint = point;

        /// <summary>
        /// チェックポイントに戻す
        /// </summary>
        public void Return2CP() => transform.SetPosition(checkpoint);

        void Start()
        {
            pjf = transform.GetFromChild<PlayerJumpFlag>(0);

            collider = GetComponent<BoxCollider2D>();

            rb = GetComponent<Rigidbody2D>();
            rb.mass = 60f;

            cam = Gobject.GetWithTag<Cam>(Constant.Tags.MainCamera);
            cam.Followable = true;
        }

        void FixedUpdate()
        {
            Move();
        }

        void Update()
        {
            Jump();
            Flip();
            Respawn();
        }

        void LateUpdate()
        {
            // 速度表示
            velT.SetText(rb.velocity);
        }

        /// <summary>
        /// スペースでリスポーンする
        /// </summary>
        void Respawn() => Runner.Simple(Inputs.Down(KeyCode.Space), () => Return2CP());

        float h;
        void Flip()
        {
            if (!Ctrlable || !Input.GetButtonDown(Constant.Keys.Horizontal))
            {
                return;
            }

            var scale = Mathf.Sign(transform.localScale.x);
            switch (Mathf.Sign(Input.GetAxisRaw(Constant.Keys.Horizontal)))
            {
                case 0: break;

                case 1:
                    if (scale != 1)
                    {
                        print(1);
                        transform.localScale *= new Vector2(-1, 1);
                    }
                    break;

                case -1:
                    if (scale != -1)
                    {
                        print(-1);
                        transform.localScale *= new Vector2(-1, 1);
                    }
                    break;
            }
        }

        void Jump()
        {
            if (!Ctrlable) { return; }

            // (Vector2 origin, Vector2 size) hitbox = (
            //     (Vector2)transform.position + collider.offset - collider.size.y / 2 * (Vector2)Coordinate.y,
            //     new(collider.bounds.size.x, 0.2f));

            // if (isFloating = Gobject.BoxCast2D(out _, hitbox.origin, hitbox.size, layer: Fixed.Layers.Object | Fixed.Layers.Ground)
            //      && Inputs.Pressed(Fixed.Keys.Jump))
            if (pjf.Hit && Inputs.Down(Constant.Keys.Jump))
            {
                // ジャンプ
                rb.velocity += jump.power * 3 * (Vector2)Coordinate.y;
            }
        }

        /// <summary>
        /// 移動
        /// </summary>
        void Move()
        {
            if (!Ctrlable) { return; }

            Vector2 move = Input.GetAxisRaw(Constant.Keys.Horizontal) * Coordinate.x;

            // 入力がtolerance以下、氷に乗っていない、浮いていない
            if (move.magnitude <= tolerance && !onIce && !isFloating)
            {
                // x軸の速度をspeed.reduction倍
                rb.SetVelocityX(rb.velocity.x * speed.reduction);
            }

            // 速度を制限
            rb.velocity = new(onIce ?
                Mathf.Clamp(rb.velocity.x, -speed.max * 2, speed.max * 2) :
                Mathf.Clamp(rb.velocity.x, -speed.max, speed.max),
                rb.velocity.y);

            // 浮いていたら移動速度低下
            rb.velocity += Time.fixedDeltaTime * (isFloating ? speed.basis * speed.freduction : speed.basis) * move;
        }

        /// <summary>
        /// 成仏
        /// </summary>
        public async UniTask Die()
        {
            if (IsDieProcessing) { return; }

            IsDieProcessing = true;
            Ctrlable = false;
            cam.Followable = false;

            // TODO below
            // エフェクト生成
            // var fx = dieFx.Generate(transform.position);

            // // エフェクトの長さ分だけちと待機
            // await UniTask.Delay(Numeric.Cutail(diefx.FxDuration()));

            // // 1/5フレーム待機
            // await UniTask.DelayFrame(Numeric.Cutail(App.fps / 5));
            await UniTask.Delay(1000);

            StartCoroutine(AfterDelay());
        }

        IEnumerator AfterDelay()
        {
            yield return null;

            // 座標リセット
            Return2CP();

            // うごいていいよ
            cam.Followable = true;
            Ctrlable = true;
            IsDieProcessing = false;
        }

        void OnCollisionEnter2D(Collision2D info)
        {
            onIce = info.Compare(Constant.Tags.Ice);
        }

        void OnCollisionExit2D(Collision2D info)
        {
            onIce = info.Compare(Constant.Tags.Ice);
        }
    }
}