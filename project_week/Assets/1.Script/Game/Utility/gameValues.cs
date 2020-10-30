using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public static class gameValues
    {
        /// <summary> 어빌리티 포인트 </summary>
        public static readonly int _apPrice = 1000;

        public static readonly float _firstMopCoin = 1;
        public static readonly float _mopCoinIncrease = 1.15f;
        public static readonly float _bossCoinIncrease = 1.2f;

        public static readonly int _firstRoundCoin = 20;
        public static readonly float _stageCoinIncrease = 1.2f;

        public static readonly int _tileCut = 4;
        public static float _boxSize { get { return 20.48f / _tileCut; } }

        public static readonly float _defaultSpeed = 3.2f;
        //public static readonly float _defaultProjSpeed = 5f;
        public static readonly float _defaultCurvProjSpeed = 2f;

        public static  Vector3 _firstbox
        {
            get
            {
                float d = (20.48f - _boxSize) / 2f;
                return new Vector3(-d, d);
            }
        }
    }
}