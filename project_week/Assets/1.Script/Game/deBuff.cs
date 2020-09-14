using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class BuffEffect
    {
        public eDeBuff _eDB;
        /// <summary> 지속여부 </summary>
        bool _lasted;
        /// <summary> 지속 시간 </summary>
        float _term;
        /// <summary> 값 </summary>
        public float _val;

        /// <summary> 도트뎀 텀 체크 용 시간 </summary>
        float _oneSecond;

        public float Term { get => _term; set => _term = (_lasted == false) ? value : _term; }
        public bool TermOver { get => _term < 0; }

        public BuffEffect(eDeBuff e, bool l, float t, float v)
        {
            _eDB = e;
            _lasted = l;
            _term = t;
            _val = v;
        }

        /// <summary> 도트뎀은 1초간격 </summary>
        public bool chkOne(float del)
        {
            _oneSecond += del;
            if (_oneSecond > 1f)
            {
                _oneSecond = 0;
                return true;
            }
            return false;
        }
    }
}