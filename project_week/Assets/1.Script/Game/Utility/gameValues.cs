using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace week
{
    public static class gameValues
    {
        /// <summary> 버전 </summary>
        public static readonly ObscuredInt _version = 1;
        /// <summary> 유닉스 시대 </summary>
        public static readonly DateTime epoch = new DateTime(1970, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        /// <summary> 기기 데이터 리스트 키 </summary>
        public static readonly ObscuredString _deviceListKey = "deviceKey";

        // price ==============================
        /// <summary> 어빌리티 포인트 가격 </summary>
        public static readonly ObscuredInt _apPrice = 1000;
        /// <summary> 닉변 가격 </summary>
        public static readonly ObscuredInt _nickPrice = 50;
        // ====================================

        public static readonly ObscuredFloat _firstMopCoin = 1;
        public static readonly ObscuredFloat _mopCoinIncrease = 1.15f;
        public static readonly ObscuredFloat _bossCoinIncrease = 1.2f;

        public static readonly ObscuredInt _firstRoundCoin = 20;
        public static readonly ObscuredFloat _stageCoinIncrease = 1.2f;

        public static readonly ObscuredInt _tileCut = 4;
        public static float _boxSize { get { return 20f / _tileCut; } }

        public static readonly ObscuredFloat _defaultSpeed = 3.2f;
        //public static readonly float _defaultProjSpeed = 5f;
        public static readonly ObscuredFloat _defaultCurvProjSpeed = 2f;

        // 게임 값 ==============================

        public static readonly ObscuredFloat _expIncrease = 1.2f;

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
                float d = (20f - _boxSize) / 2f;
                return new Vector3(-d, d);
            }
        }
    }
}