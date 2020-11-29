using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace week
{
    public class Context
    {
        public string Id;
        public string Where;
        public string Product;
        public int Amount;

        public Context(string id, string where)
        {
            Id = id;
            Where = where;
        }

        public Context setProduct(string product, int amount)
        {
            Product = product;
            Amount = amount;
            return this;
        }
    }

    public static class gameValues
    {
        /// <summary> 버전 </summary>
        public static readonly ObscuredInt _version = 1;
        /// <summary> 유닉스 시대 </summary>
        public static readonly DateTime epoch = new DateTime(1970, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        /// <summary> 기기 데이터 리스트 키 </summary>
        public static readonly ObscuredString _offlineKey = "offlineKey";

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

        // 게임 값 ==============================
        
        /// <summary> 플레이어 시작 경험치 </summary>
        public static readonly ObscuredInt _startExp = 25;
        /// <summary> 몹 시작 경험치 </summary>
        public static readonly ObscuredInt _startMobExp = 2;
        /// <summary> 보스 시작 경험치 </summary>
        public static readonly ObscuredInt _startBobExp = 50;

        /// <summary> 기본 속도(눈사람 기준) </summary>
        public static readonly ObscuredFloat _defaultSpeed = 3.2f;
        /// <summary> 기본 투사체 속도 </summary>
        public static readonly ObscuredFloat _defaultProjSpeed = 3.7f;
        /// <summary> 기본 곡사체 속도 </summary>
        public static readonly ObscuredFloat _defaultCurvProjSpeed = 2f;

        /// <summary> max경험치 증가 값 </summary>
        public static readonly ObscuredFloat _expIncrease = 1.2f;
        /// <summary> 힐팩량(5%) </summary>
        public static readonly ObscuredFloat _healpackVal = 0.05f;

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