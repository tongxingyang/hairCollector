using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class seasonlySprite : seasonlyBase
    {
        [SerializeField] Sprite[] imgs;
        SpriteRenderer[] render;

        /// <summary> 최초 초기화 - 추가 작업 </summary>
        protected override void whenFixedInit()
        {
            render = GetComponentsInChildren<SpriteRenderer>();
        }

        /// <summary> 재사용 초기화 - 추가 작업 </summary>
        protected override void whenRepeatInit()
        {
            setSeason();
        }

        /// <summary> 계절설정 </summary>
        protected override void setSeason()
        {
            _season = _gs.ClockMng.NowSeason;

            foreach (SpriteRenderer ren in render)
            {
                ren.sprite = imgs[(int)_season];
            }
        }

        protected override void Destroy()
        {
            preDestroy();
        }

        public override void onPause(bool bl)
        {
        }
    }
}