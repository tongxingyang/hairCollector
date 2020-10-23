using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

namespace week
{
    public class UserEntity
    {
        #region private

        /// <summary> 닉넴 </summary>
        [SerializeField] public string _nickName;

        /// <summary> 코인 </summary>
        [SerializeField] public int _coin;
        /// <summary> 보석 </summary>
        [SerializeField] public int _gem;
        /// <summary> 어빌리티 포인트 </summary>
        [SerializeField] public int _ap;
        /// <summary> 장착중인 스킨 </summary>
        [SerializeField] public int _skin;

        // 기록 및 퀘스트
        [SerializeField] public DateTime _lastLogin;
        // 일일
        [SerializeField] public int[] _dayQuest;
        [SerializeField] public int _questSkin;
        // 전체        
        [SerializeField] public int _timeRecord;
        [SerializeField] public int _getTimeReward;
        [SerializeField] public int _bossRecord;
        [SerializeField] public int _getBossReward;
        [SerializeField] public int _artifactRecord;
        [SerializeField] public int _getArtifactReward;
        [SerializeField] public int _adRecord;
        [SerializeField] public int _reinRecord;

        /// <summary> 강화레벨 </summary>
        [SerializeField] public int[] _statusLevel;
        /// <summary> 스킨 </summary>
        [SerializeField] public int _hasSkin;

        //인앱결제
        [SerializeField] public bool _addGoods;
        [SerializeField] public float _addGoodsValue;
        [SerializeField] public bool _removeAD;
        [SerializeField] public bool _skinPack;

        //능력치
        [SerializeField] public int _hp;
        [SerializeField] public float _att;
        [SerializeField] public int _def;
        [SerializeField] public float _hpgen;
        [SerializeField] public float _cool;
        [SerializeField] public float _expFactor;
        [SerializeField] public float _coinFactor;
        [SerializeField] public float _skinEnhance;

        //옵션
        [SerializeField] public float _bgmVol;
        [SerializeField] public float _sfxVol;        

        #endregion

        #region [properties]
        


        #endregion

        /// <summary> 초기화 </summary>
        public UserEntity()
        {
            _nickName = "ready_Player_1";

            // 재화
            _coin = 99999999;
            _gem = 2000;
            _ap = 0;

            // 스탯
            _statusLevel = new int[(int)StatusData.max];

            _hp             = DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString());
            _hpgen          = DataManager.GetTable<float>(DataTable.status, "default", StatusData.hpgen.ToString());
            _def            = DataManager.GetTable<int>(DataTable.status, "default", StatusData.def.ToString());
            _att            = DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString());
            _cool           = DataManager.GetTable<float>(DataTable.status, "default", StatusData.cool.ToString());
            _expFactor      = DataManager.GetTable<float>(DataTable.status, "default", StatusData.exp.ToString());
            _coinFactor     = DataManager.GetTable<float>(DataTable.status, "default", StatusData.coin.ToString());
            _skinEnhance    = DataManager.GetTable<float>(DataTable.status, "default", StatusData.skin.ToString());

            // 퀘스트
            _dayQuest = new int[3];

            _timeRecord = 0;
            _getTimeReward = 0;
            _bossRecord = 0;
            _getBossReward = 0;
            _artifactRecord = 0;
            _getArtifactReward = 0;
            _adRecord = 0;
            _reinRecord = 0;

            // 옵션
            _bgmVol = 1f;
            _sfxVol = 1f;

            // 결제
            _addGoods = false;
            _addGoodsValue = 1f;
            _removeAD = false;
        }

        /// <summary> 스탯 레벨 -> 스탯에 적용 </summary>
        public void applyLevel()
        {
            _hp             = DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString())
                             + DataManager.GetTable<int>(DataTable.status, "addition", StatusData.hp.ToString()) * _statusLevel[(int)StatusData.hp];
            _hpgen          = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.hpgen.ToString()) * _statusLevel[(int)StatusData.hpgen];
            _def            = DataManager.GetTable<int>(DataTable.status, "addition", StatusData.def.ToString()) * _statusLevel[(int)StatusData.def];
            _att            = DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString())
                             + DataManager.GetTable<float>(DataTable.status, "addition", StatusData.att.ToString()) * _statusLevel[(int)StatusData.att];
            _cool           = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.cool.ToString()), _statusLevel[(int)StatusData.cool]);
            _expFactor      = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.exp.ToString()), _statusLevel[(int)StatusData.exp]);
            _coinFactor     = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.coin.ToString()), _statusLevel[(int)StatusData.coin]);
            _skinEnhance    = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.skin.ToString()) * _statusLevel[(int)StatusData.skin];
        }

        /// <summary> 스탯 레벨 업 </summary>
        public void statusLevelUp(StatusData stat)
        {
            _statusLevel[(int)stat]++;

            switch (stat)
            {
                case StatusData.hp:
                    _hp = DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString())
                        + DataManager.GetTable<int>(DataTable.status, "addition", StatusData.hp.ToString()) * _statusLevel[(int)StatusData.hp];
                    break;
                case StatusData.att:
                    _att = DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString())
                        + DataManager.GetTable<float>(DataTable.status, "addition", StatusData.att.ToString()) * _statusLevel[(int)StatusData.att];
                    break;
                case StatusData.def:
                    _def = DataManager.GetTable<int>(DataTable.status, "addition", StatusData.def.ToString()) * _statusLevel[(int)StatusData.def];
                    break;
                case StatusData.hpgen:
                    _hpgen = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.hpgen.ToString()) * _statusLevel[(int)StatusData.hpgen];
                    break;
                case StatusData.cool:
                    _cool = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.cool.ToString()), _statusLevel[(int)StatusData.cool]);
                    break;                
                case StatusData.exp:
                    _expFactor = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.exp.ToString()), _statusLevel[(int)StatusData.exp]);
                    break;
                case StatusData.coin:
                    _coinFactor = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.coin.ToString()), _statusLevel[(int)StatusData.coin]);
                    break;
                case StatusData.skin:
                    _skinEnhance = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.skin.ToString()) * _statusLevel[(int)StatusData.skin];
                    break;
            }
        }

        /// <summary> 데이터 저장 </summary>
        public string saveData()
        {
            return JsonUtility.ToJson(this);
        }
    }
}