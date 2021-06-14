using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class seasonlyGo : seasonlyBase
    {
        [SerializeField] GameObject[] objs;

        /// <summary> 최초 초기화 - 추가 작업 </summary>
        protected override void whenFixedInit()
        {
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

            for (season i = season.spring; i < season.max; i++)
            {
                objs[(int)i].SetActive(i == _season);
            }
        }

        public override void onPause(bool bl)
        {
        }
    }
}