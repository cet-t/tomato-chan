using trrne.Bag;
using UnityEngine;

namespace trrne.Arm
{
    public class WoodenTree : MonoBehaviour
    {
        // プレイヤーを検知できる範囲
        readonly float detection = 4;

        (CircleCollider2D collider, SpriteRenderer sr) self;
        GameObject _player;
        (Transform transform, int order) player;
        int defaultOrder;

        void Start()
        {
            self.collider = GetComponent<CircleCollider2D>();
            self.sr = GetComponent<SpriteRenderer>();
            defaultOrder = self.sr.sortingOrder;

            _player = Gobject.Find(Constant.Tags.Player);
            player.transform = _player.transform;
            player.order = _player.GetComponent<SpriteRenderer>().sortingOrder;
        }

        void Update()
        {
            // プレイやーが葉っぱの部分にいたらorderInLayerをいじって木が前に来るように
            var distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= detection)
            {
                // プレイヤーの方が高い位置にいたら自分を前に低い位置なら後ろに
                self.sr.sortingOrder = transform.position.y - 1.84f <= player.transform.position.y ?
                        player.order + 1 : defaultOrder;
            }
        }
    }
}