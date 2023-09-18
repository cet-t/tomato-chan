using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace trrne.Game
{
    public abstract class Enemy : MonoBehaviour
    {
        public bool enable;
        protected abstract void Move();
        protected abstract void DetectPlayer();
        protected abstract void Die();
    }
}
