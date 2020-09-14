using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class seasonlySprite : seasonlyBase
    {
        [SerializeField] SpriteRenderer render;
        [SerializeField] Sprite[] imgs;

        public override void FixedInit()
        {
        }

        public override void setSeason(season ss)
        {
            render.sprite = imgs[(int)ss];
        }
    }
}