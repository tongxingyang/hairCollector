using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class BuffEffect
    {
        public enum buffTermType { term, season, infinity }
        /// <summary> 버프 타입 </summary>
        snowStt _stt;
        /// <summary> 계절 </summary>
        buffTermType _isTerm;
        /// <summary> 지속 시간 </summary>
        float? _term;
        /// <summary> 값 </summary>
        float _val;

        public snowStt Stt { get => _stt; }
        public buffTermType IsSeason { get => _isTerm; }
        public float? Term { get => _term; set => _term = (_isTerm == buffTermType.term) ? value : null; }
        public bool TermOver { get => _term < 0; }
        public float Val { get => _val; }

        public BuffEffect(snowStt stt, float term, float val, buffTermType isterm = buffTermType.term)
        {
            _stt = stt;
            _isTerm = isterm;

            if (isterm != buffTermType.term)
                _term = null;
            else
                _term = term;

            _val = val;
        }
    }

    public class dotDmg
    {
        bool _used;

        float _dmgRate = 1.5f;
        float _duration;

        float _time;
        float _duTime;

        public void setDotDmg(float dmgRate, float duration)
        {
            if (_used == false)
            {
                _used = true;
                _duration = duration;
                _dmgRate = dmgRate;
            }
            else
            {
                _duration = duration;
                _dmgRate = dmgRate;
                _duTime = 0;
            }
        }

        public float dotDmging(float delTime)
        {
            if (_used == false)
                return 0;

            _time += delTime;
            _duTime += delTime;

            if (_duTime > _duration)
            {
                _used = false;
                _duTime = 0;
            }
            else if (_time > 1f)
            {
                _time = 0;
                return _dmgRate;
            }

            return 0;
        }
    }
}