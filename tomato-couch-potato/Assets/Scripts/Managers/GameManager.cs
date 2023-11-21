using trrne.Core;
using trrne.Box;
using UnityEngine;
using UnityEngine.UI;

namespace trrne.Brain
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        Text timeT;

        Cam cam;
        TimeManager time;
        PauseMenu menu;

        void Start()
        {
            time = GetComponent<TimeManager>();
            time.Start();

            menu = GetComponent<PauseMenu>();
            menu.Inactive();

            Physics2D.gravity = Coordinate.Gravity;

            cam = Gobject.GetComponentWithTag<Cam>(Constant.Tags.MainCamera);
            cam.Followable = true;

            GameObject[] enemies = Gobject.GetGameObjectsWithTag(Constant.Tags.Enemy);
            enemies.ForEach(enemy => enemy.GetComponent<Creature>().Enable = true);
        }

        void Update()
        {
            timeT.SetText(time.CurrentStr);
        }
    }
}
