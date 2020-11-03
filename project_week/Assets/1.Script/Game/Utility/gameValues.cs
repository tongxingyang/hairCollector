﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public static class gameValues
    {
        /// <summary> 버전 </summary>
        public static readonly int _version = 1;
        /// <summary> 유닉스 시대 </summary>
        public static readonly DateTime epoch = new DateTime(1970, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        /// <summary> 어빌리티 포인트 가격 </summary>
        public static readonly int _apPrice = 1000;
        /// <summary> 닉변 가격 </summary>
        public static readonly int _nickPrice = 50;

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

        // 추가코인 ==============================
        public static readonly Vector3[] _mulCoinVal = new Vector3[2] {
            new Vector3(2f,2f,2f),  // 광고제거
            new Vector3(1.1f,1f,1f) // 10퍼
        };
        // ======================================

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