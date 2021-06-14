using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class baseRuinTrap : MonoBehaviour
    {
        obstacleKeyList _type;
        protected GameScene _gs;
        protected PlayerCtrl _player;
        protected clockManager _clm;

        /// <summary> (계산 전)기본 공격력 </summary>
        protected float _att;
        /// <summary> 계산 후 공격력 </summary>
        protected float Att;
        protected float _increase;

        protected float _dmgRate;

        /// <summary> 함정 생성 초기화 </summary>
        public baseRuinTrap FixedInit(GameScene gs, obstacleKeyList type)
        {
            _gs = gs;
            _player = _gs.Player;
            _clm = _gs.ClockMng;

            _type = type;

            _dmgRate = D_level.GetEntity(_gs.StageLevel.ToString()).f_mobrate;

            whenFixedInit();

            return RepeatInit();
        }
        /// <summary> 함정 생성 초기화 - 추가작업 필요시 </summary>
        protected abstract void whenFixedInit();

        /// <summary> 함정 재사용 초기화 </summary>
        public baseRuinTrap RepeatInit()
        {
            _increase = Mathf.Pow(1.12f, _clm.RecordDay);
            whenRepeatInit();

            return this;
        }

        /// <summary> 함정 재사용 초기화 - 추가작업 필요시 </summary>
        protected abstract void whenRepeatInit();
        public void Play()
        {
            StartCoroutine(trapPlay());
        }
        /// <summary> 함정 작동 </summary>
        protected abstract IEnumerator trapPlay();

        /// <summary> 일시정지시 </summary>
        public virtual void onPause(bool bl) { }

    }
}