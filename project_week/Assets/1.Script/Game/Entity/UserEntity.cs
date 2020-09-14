using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class UserEntity
    {
        #region private

        /// <summary> 닉넴 </summary>
        string _nickName;

        /// <summary> 코인 </summary>
        int _Coin;

        /// <summary> 체력 </summary>
        float _hp           = 20f;
        /// <summary> 체력젠 </summary>
        float _hpgen        = 0f;
        /// <summary> 공격 기본값 </summary>
        float _attFactor    = 10f;
        /// <summary> 전체 쿨감 </summary>
        float _subAttSpeed  = 0;
        /// <summary> 경험치 배수 </summary>
        float _expFactor    = 1f;
        /// <summary> 코인 배수 </summary>
        float _coinFactor   = 1f;
        /// <summary> 방어력 </summary>
        float _def          = 1f;
        /// <summary> 스킬 범위 증가 </summary>
        float _skillRange   = 1f;
        /// <summary> 처치 몬스터 수 감소 </summary>
        int _subKillAmount  = 0;

        #endregion

        public string NickName { get => _nickName; set => _nickName = value; }

        public int Coin { get => _Coin; set => _Coin = value; }

        public float Hp { get => _hp; set => _hp = value; }
        public float Hpgen { get => _hpgen; set => _hpgen = value; }
        public float AttFactor { get => _attFactor; set => _attFactor = value; }
        public float SubAttSpeed { get => _subAttSpeed; set => _subAttSpeed = value; }
        public float ExpFactor { get => _expFactor; set => _expFactor = value; }
        public float CoinFactor { get => _coinFactor; set => _coinFactor = value; }
        public float Def { get => _def; set => _def = value; }
        public float SkillRange { get => _skillRange; set => _skillRange = value; }
        public int SubKillAmount { get => _subKillAmount; set => _subKillAmount = value; }

        public UserEntity()
        {
            _nickName = "ready_Player_1";

            _Coin = 0;

            _hp = 20f;
            _hpgen = 0f;
            _attFactor = 10f;
            _subAttSpeed = 0;
            _expFactor = 1f;
            _coinFactor = 1f;
            _def = 1f;
            _skillRange = 1f;
            _subKillAmount = 0;
        }        
    }
}