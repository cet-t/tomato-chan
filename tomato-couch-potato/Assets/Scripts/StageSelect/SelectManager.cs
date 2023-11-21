﻿using DG.Tweening;
using trrne.Brain;
using trrne.Box;
using UnityEngine;
using UnityEngine.UI;

namespace trrne.Arm
{
    public class SelectManager : MonoBehaviour
    {
        [SerializeField]
        Text centerT;

        [SerializeField]
        Sprite[] backs;

        [SerializeField]
        RectTransform[] buttons;

        [SerializeField]
        RectTransform core;

        const string PrefabPrefix = "stage";
        const float Offset = 18.96f;

        bool isScrolling = false;
        const float ButtonScrollSpeed = 0.5f;

        float horizon;

        void Start()
        {
            core.SetPosition(Coordinate.V3X * Offset);
        }

        void Update()
        {
#if DEBUG
            centerT.text = CenterButton() != null ? CenterButton().name : "null";
#endif
            Scroll();
            Transition();
        }

        void Transition()
        {
            if (CenterButton() != null
                && int.TryParse(Typing.Delete(CenterButton().name, PrefabPrefix), out int idx))
            {
                if (idx <= Recorder.Instance.Done && Inputs.Down(Constant.Keys.Button))
                {
                    print(Constant.Scenes.Prefix + idx);
                    Scenes.Load(Constant.Scenes.Prefix + idx);
                }
            }
        }

        /// <summary>
        /// xが一番0に近いボタンを取得
        /// </summary>
        GameObject CenterButton()
        {
            foreach (var button in buttons)
                if (Maths.CutailedTwins(button.transform.position.x, 0f))
                    return button.gameObject;
            return null;
        }

        void Scroll()
        {
            if (Maths.Twins(horizon = Input.GetAxisRaw(Constant.Keys.Horizontal), 0f))
                return;

            switch (horizon.Sign())
            {
                case 1:
                    if (CenterButton() != buttons[^1].gameObject)
                        Scroller(core.position.x - Offset);
                    break;
                default:
                    if (CenterButton() != buttons[0].gameObject)
                        Scroller(core.position.x + Offset);
                    break;
            }
        }

        void Scroller(float targetX)
        {
            if (isScrolling)
                return;

            isScrolling = true;
            core.DOMoveX(targetX, ButtonScrollSpeed)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() => isScrolling = false);
        }
    }
}