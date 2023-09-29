using System;
using System.Collections;
using trrne.Bag;
using UnityEngine;
using UnityEngine.UI;

namespace trrne.Body
{
    public enum FadeType { CutIn, CutOut }

    public class FadingPanel : MonoBehaviour
    {
        [SerializeField]
        float fadingSpeed = 1;

        float alfa;
        public float Alpha => panel.GetAlpha();

        RectTransform recT;
        (Vector2 panel, Vector2 screen) size;

        Image panel;

        bool isFading = false;
        public bool IsFading => isFading;

        void Start()
        {
            panel = GetComponent<Image>();
            panel.color = Color.black;
            panel.SetAlpha(0);

            recT = GetComponent<RectTransform>();
        }

        void Update()
        {
            SyncScreen();
        }

        /// <summary>
        /// パネルとシーンのサイズを同期
        /// </summary>
        void SyncScreen()
        {
            size = (recT.sizeDelta, new(Screen.width, Screen.height));

            if (size.panel != size.screen)
            {
                size.panel = size.screen;
            }
        }

        IEnumerator Fader(FadeType cut)
        {
            // 補正
            alfa = cut == FadeType.CutOut ? 0 : 1;
            panel.SetAlpha(alfa);

            // alfaが0-1の間ループ
            while (true)
            {
                yield return null;

                switch (cut)
                {
                    case FadeType.CutIn:
                        panel.SetAlpha(alfa -= fadingSpeed * Time.unscaledDeltaTime);
                        break;

                    case FadeType.CutOut:
                        panel.SetAlpha(alfa += fadingSpeed * Time.unscaledDeltaTime);
                        break;
                }

                if (alfa >= 1 || alfa <= 0) { break; }
            }

            print("Faded.");
        }

        /// <summary>
        /// パネルの
        /// </summary>
        /// <param name="cut">cut in or out</param>
        public void Fade(FadeType cut) => StartCoroutine(Fader(cut));

        /// <summary>
        /// 
        /// </summary>
        public void Fade2(FadeType cut)
        {
            while (isFading)
            {
                switch (cut)
                {
                    case FadeType.CutIn:
                        break;
                    case FadeType.CutOut:
                        break;
                }
            }
        }
    }
}
