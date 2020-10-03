using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class SsnowballCtrl : SprojCtrl
    {
        [SerializeField] SpriteRenderer _render;
        [SerializeField] Sprite[] _snowballs;

        public void setSprite(snowballType sbt)
        {
            _render.sprite = _snowballs[(int)sbt];
        }
    }
}