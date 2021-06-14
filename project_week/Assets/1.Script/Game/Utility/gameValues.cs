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
        public int Coin;
        public int Gem;

        public Context(string id, string where)
        {
            Id = id;
            Where = where;
        }

        public Context setProduct(string product, int c, int g)
        {
            Product = product;
            Coin = c;
            Gem = g;

            return this;
        }
    }

    public class statusRankData
    {
        public readonly string _name;
        public readonly Color _color;

        public statusRankData(string name, Color color)
        {
            _name = name;
            _color = color;
        }
    }

    public static class gameValues
    {
        /// <summary> 버전 </summary>
        // public static readonly ObscuredInt _version = 1;
        /// <summary> 유닉스 시대 </summary>
        public static readonly DateTime epoch = new DateTime(1970, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        /// <summary> 기기 데이터 리스트 키 </summary>
        public static readonly ObscuredString _offlineKey = "offlineKey";

        // price ==============================
        /// <summary> 닉변 가격 </summary>
        public static readonly ObscuredInt _nickPrice = 50;
        /// <summary> 스킨 강화 가격 </summary>
        public static readonly ObscuredInt[] _skinEnhancePrice = new ObscuredInt[10] { 25, 50, 125, 250, 500, 1250, 2500, 5000, 12500, 25000 };
        // ====================================

        public static readonly ObscuredFloat _firstMopCoin = 1;
        //public static readonly ObscuredFloat _mopCoinIncrease = 1.15f;
        //public static readonly ObscuredFloat _bossCoinIncrease = 1.2f;

        // public static readonly ObscuredInt _firstRoundCoin = 20;
        //public static readonly ObscuredFloat _stageCoinIncrease = 1.2f;

        public static readonly ObscuredInt _tileCut = 4;
        public static float _boxSize { get { return 20f / _tileCut; } }

        // 게임 값 ==============================
        
        /// <summary> 플레이어 시작 경험치 </summary>
        public static readonly ObscuredInt _startExp = 25;
        /// <summary> 몹 시작 경험치 </summary>
        public static readonly ObscuredInt _startMobExp = 2;
        /// <summary> 보스 시작 경험치 </summary>
        public static readonly ObscuredInt _startBobExp = 50;

        /// <summary> 기본 속도 </summary>
        public static readonly ObscuredFloat _defaultSpeed = 3.2f;
        /// <summary> 눈사람 속도 ( 기본속도 95% ) </summary>
        public static float SnowmanSpeed { get => _defaultSpeed * 0.95f; }
        /// <summary> 기본 투사체 속도 </summary>
        public static readonly ObscuredFloat _defaultProjSpeed = 3.7f;
        /// <summary> 기본 곡사체 속도 </summary>
        public static readonly ObscuredFloat _defaultCurvProjSpeed = 2f;

        /// <summary> max경험치 증가 값 </summary>
        public static readonly ObscuredFloat _expIncrease = 1.15f;
        /// <summary> max경험치 증가 값 </summary>
        public static readonly ObscuredFloat _expRate = 1.1f;
        /// <summary> 힐팩량(5%) </summary>
        public static readonly ObscuredFloat _healpackVal = 0.06f;

        /// <summary> 몹 소환 최대치 </summary>
        public static readonly ObscuredFloat _mobMaxCount = 80;
        /// <summary> 몹 강화량(하루) </summary>
        public static readonly ObscuredFloat _mobIncrease = 1.14f;
        /// <summary> 몹 강화량(1년) </summary>
        public static readonly ObscuredFloat _mobIncrease2 = 0.75f;

        /// <summary> shot 스킬 도탄 거리 </summary>
        public static readonly ObscuredFloat _ricocheRange = 3f;

        /// <summary> 기본 스킬 리스트 </summary>
        public static readonly SkillKeyList[] _baseSkill 
            = new SkillKeyList[] { SkillKeyList.SnowBall, SkillKeyList.IceBall, SkillKeyList.IceBat, SkillKeyList.Shield, SkillKeyList.Field, SkillKeyList.Summon };

        // 게임 외부 값 ==============================
        /// <summary> 알림창 시간 </summary>
        public static readonly ObscuredFloat _notiDuration = 3f;

        /// <summary> 등급컷 </summary>
        public static readonly int[] _tier = new int[5] { 0, 20, 50, 80, 100 };
        /// <summary> 등급별 정보(이름, 색) </summary>
        public static readonly statusRankData[] _statusRankData = new statusRankData[5] 
        {
            new statusRankData( "브론즈",   new Color(0.9f, 0.3f, 0f)),   
            new statusRankData( "실버",     new Color(0.9f, 0.9f, 0.9f)), 
            new statusRankData( "골드",     new Color(1f, 0.8f, 0.05f)),  
            new statusRankData( "플래티넘", new Color(0f, 1f, 0.85f)),    
            new statusRankData( "미정1",    new Color(0.57f, 0.22f, 1f))  
        };

        /// <summary> 스킨등급별 정보(색) </summary>
        public static readonly Color[] _skinRankColor = new Color[] {
            new Color(0.9f, 0.9f, 0.9f),
            new Color(0.3f, 0.45f, 1f),
            new Color(0.7f, 0.23f, 0.7f),
            new Color(1f, 0.25f, 0.25f),
            new Color(1f, 0.8f, 0.06f)
        };

        // 추가코인 ==============================

        public static readonly Vector3[] _mulCoinVal = new Vector3[] {
            new Vector3(2f,2f,2f),          // 광고제거
            new Vector3(1.1f,1f,1f),        // 10퍼
            new Vector3(1.05f,1f,1f),       // 5퍼
            new Vector3(1.05f,1.03f,1.03f), // 3퍼
            new Vector3(1.05f,1.03f,1.03f), // 3퍼
        };

        public static readonly float[] _bonusCoinVal = new float[5] { 1f, 1.1f, 1.05f, 1.03f, 1.03f };

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