﻿using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using trrne.Box;
using trrne.Brain;

namespace trrne.Core
{
    public enum CuzOfDeath
    {
        Caps,   // 唐辛死
        Fallen, // 落下死
        None,   // 不審死
    }

    public enum EffectType
    {
        Mirror,     // 操作左右反転
        Chain,      // ジャンプ不可
        Fetters,    // 移動速度低下
    }

    public class Player : MonoBehaviour, ICreature
    {
        [SerializeField]
        GameObject diefx;

        /// <summary>
        /// 操作可能か
        /// </summary>
        public bool Controllable { get; set; }

        /// <summary>
        /// キーが入力されているか
        /// </summary>
        public bool IsMoveKeyDetecting { get; private set; }

        /// <summary>
        /// テレポート処理中か
        /// </summary>
        public bool IsTeleporting { get; set; }

        /// <summary>
        /// 死亡中か
        /// </summary>
        public bool IsDying { get; private set; } = false;

        /// <summary>
        /// 与えるエフェクト
        /// </summary>
        int fxidx = -1;

        /// <summary>
        /// エフェクトを与えられるか
        /// </summary>
        public bool[] Effectables { get; private set; }

        /// <summary>
        /// 与えられてるエフェクト
        /// </summary>
        public bool[] EffectFlags { get; set; }

        /// <summary>
        /// 移動速度
        /// </summary>
        readonly (float basis, float max) speed = (20, 10);

        /// <summary>
        /// 減速比
        /// </summary>
        readonly (float fetters, float floating, float move) red = (0.5f, 0.95f, 0.9f);

        /// <summary>
        /// 重力
        /// </summary>
        readonly (float floating, float basis) gscale = (2f, 1f);

        /// <summary>
        /// ジャンプ力
        /// </summary>
        const float JumpPower = 10f;

        /// <summary>
        /// 地に足がついていなかったらtrue
        /// </summary>
        public bool IsFloating { get; private set; }

        /// <summary>
        /// 移動速度
        /// </summary>
        public Vector2 Velocity { get; private set; }

        Rigidbody2D rb;
        Animator animator;
        PlayerFlag flag;
        Cam cam;
        PauseMenu menu;
        BoxCollider2D hitbox;

        /// <summary>
        /// プレイヤーのへそあたりの座標
        /// </summary>
        public Vector3 CoreOffset { get; private set; }

        /// <summary>
        /// 入力の許容値
        /// </summary>
        const float InputTolerance = 0.33f;

        /// <summary>
        /// 現在のチェックポイントを取得:public<br/>更新:private
        /// </summary>
        public Vector2 Checkpoint { get; private set; }

        /// <summary>
        /// チェックポイントを設定する
        /// </summary>
        public void SetCheckpoint(Vector2 position) => Checkpoint = position;

        /// <summary>
        /// チェックポイントに戻る
        /// </summary>
        public void ReturnToCheckpoint() => transform.position = Checkpoint;

        Vector2 Reverse => new(-1, 1);

        void Start()
        {
            Checkpoint = Vector2.zero;

            menu = Gobject.GetComponentWithTag<PauseMenu>(Constant.Tags.Manager);
            flag = transform.GetComponentFromChild<PlayerFlag>();

            cam = Gobject.GetComponentWithTag<Cam>(Constant.Tags.MainCamera);

            animator = GetComponent<Animator>();
            hitbox = GetComponent<BoxCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = gscale.basis;

            int length = new EffectType().EnumLength();
            EffectFlags = new bool[length];
            Effectables = new bool[length];
            for (int i = 0; i < EffectFlags.Length; i++)
            {
                EffectFlags[i] = false;
                Effectables[i] = true;
            }
        }

        void FixedUpdate()
        {
            Move();
        }

        void Update()
        {
            Def();
            Jump();
            Flip();
            Respawn();
            PunishFlagsUpdater();
        }

        void Def()
        {
            CoreOffset = transform.position + new Vector3(0, hitbox.bounds.size.y / 2);
            rb.isKinematic = IsTeleporting;
            IsFloating = !flag.OnGround;
            rb.gravityScale = IsFloating ? gscale.floating : gscale.basis;
        }

        /// <summary>
        /// スペースでリスポーンする
        /// </summary>
        void Respawn()
        {
            if (IsDying)
                return;

            if (Inputs.Down(Constant.Keys.Respawn))
                ReturnToCheckpoint();
        }

        public IEnumerator Punishment(float duration, EffectType type)
        {
            if (fxidx == -1 && Effectables[fxidx = (int)type])
            {
                EffectFlags[fxidx] = true;
                yield return new WaitForSeconds(duration);
                EffectFlags[fxidx] = false;
                fxidx = -1;
            }
        }

        void PunishFlagsUpdater()
        {
            for (int i = 0; i < EffectFlags.Length; i++)
                Effectables[i] = !EffectFlags[i];
        }

        void Flip()
        {
            if (!Controllable || menu.IsPausing || IsTeleporting)
                return;

            string horizontal = EffectFlags[(int)EffectType.Mirror] ? Constant.Keys.ReversedHorizontal : Constant.Keys.Horizontal;
            if (Input.GetButtonDown(horizontal))
            {
                int current = Maths.Sign(transform.localScale.x);
                switch (Mathf.Sign(Input.GetAxisRaw(horizontal)))
                {
                    case 1:
                        Shorthand.If(current != 1, () => transform.localScale *= Reverse);
                        break;
                    case -1:
                        Shorthand.If(current != -1, () => transform.localScale *= Reverse);
                        break;
                }
            }
        }

        void Jump()
        {
            if (!Controllable || EffectFlags[(int)EffectType.Chain] || IsTeleporting)
                return;

            if (!flag.OnGround)
            {
                animator.SetBool(Constant.Animations.Jump, true);
                return;
            }

            if (Inputs.Down(Constant.Keys.Jump))
                rb.velocity += JumpPower * Coordinate.V2Y;
            animator.SetBool(Constant.Animations.Jump, false);
        }

        /// <summary>
        /// 移動
        /// </summary>
        void Move()
        {
            if (!Controllable || IsTeleporting)
                return;

            bool walkAnimation = Input.GetButton(Constant.Keys.Horizontal) && flag.OnGround;
            animator.SetBool(Constant.Animations.Walk, walkAnimation);

            string horizontal = EffectFlags[(int)EffectType.Mirror] ?
                Constant.Keys.ReversedHorizontal : Constant.Keys.Horizontal;
            Vector2 move = Input.GetAxisRaw(horizontal) * Coordinate.V2X;

            // 入力がtolerance以下、氷に乗っていない、浮いていない
            if (move.magnitude <= InputTolerance && !flag.OnIce)
                // x軸の速度をspeed.reduction倍
                rb.SetVelocity(x: rb.velocity.x * red.move);

            float limit = Shorthand.L1ne(() =>
            {
                // 足枷がついていたら速度制限をFettersRed倍する
                if (EffectFlags[(int)EffectType.Fetters])
                    return speed.max * red.fetters;
                // 氷の上にいたら速度制限を2倍にする
                else if (flag.OnIce)
                    return speed.max * 2;
                return speed.max;
            });

            // x軸の速度を制限
            rb.ClampVelocity(x: (-limit, limit));

            // 浮いていたら移動速度低下
            float velocity = IsFloating ? speed.basis * red.floating : speed.basis;
            rb.velocity += Time.fixedDeltaTime * velocity * move;
        }

        /// <summary>
        /// 成仏
        /// </summary>
        public async UniTask Die(CuzOfDeath cause = CuzOfDeath.None)
        {
            if (IsDying)
                return;

            IsDying = true;
            Controllable = false;
            cam.Followable = false;

            diefx.TryInstantiate(transform.position);

            switch (cause)
            {
                case CuzOfDeath.None: return;
                case CuzOfDeath.Caps:
                    rb.velocity = Vector2.zero;
                    animator.Play(Constant.Animations.Venomed);
                    break;
                case CuzOfDeath.Fallen:
                    animator.Play(Constant.Animations.Die);
                    break;
                default: break;
            }

            await UniTask.Delay(1250);

            // エフェクトが付与されていたら消す
            for (int i = 0; i < EffectFlags.Length; i++)
            {
                if (!EffectFlags[i])
                    continue;
                EffectFlags[i] = false;
            }

            // 落とし穴を修繕する
            Gobject.Finds<Carrot>().ForEach(c => c.Mendable.If(c.Mend));

            ReturnToCheckpoint();
            animator.StopPlayback();

            cam.Followable = true;
            Controllable = true;
            IsDying = false;
        }

        public async UniTask Die() => await Die(CuzOfDeath.None);
    }
}