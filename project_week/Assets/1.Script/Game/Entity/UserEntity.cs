using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

namespace week
{
    public class UserEntity
    {
        #region [ classification ]

        /// <summary> 플레이어 재산 </summary>
        [Serializable]
        public struct property
        {
            /// <summary> 닉넴 </summary>
            [SerializeField] public string _nickName;
            /// <summary> 재화 (코인, 젬, ap) </summary>
            [SerializeField] public int[] _currency;
            /// <summary> 스킨 </summary>
            [SerializeField] public int _hasSkin;
            /// <summary> 장착중인 스킨 </summary>
            [SerializeField] public int _skin;

            public property(string nic, int[] cur, int hasskin, int skin)
            {
                _nickName = nic;
                _currency = cur;
                _hasSkin = hasskin;
                _skin = skin;
            }
        }

        /// <summary> 플레이어 능력치 </summary>
        [Serializable]
        public struct status
        {
            /// <summary> 강화레벨 </summary>
            [SerializeField] public int[] _statusLevel;

            //능력치
            [SerializeField] public int _hp;
            [SerializeField] public float _att;
            [SerializeField] public int _def;
            [SerializeField] public float _hpgen;
            [SerializeField] public float _cool;
            [SerializeField] public float _expFactor;
            [SerializeField] public float _coinFactor;
            [SerializeField] public float _skinEnhance;

            public status(int[] statusLevel, int hp, float att, int def, float hpgen, float cool, float expFactor, float coinFactor, float skinEnhance)
            {
                _statusLevel = statusLevel;
                _hp = hp;
                _att = att;
                _def = def;
                _hpgen = hpgen;
                _cool = cool;
                _expFactor = expFactor;
                _coinFactor = coinFactor;
                _skinEnhance = skinEnhance;
            }
        }

        /// <summary> 퀘스트 </summary>
        [Serializable]
        public struct quest
        {
            // 기록 및 퀘스트
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

            public quest(int[] dayQuest, int questSkin, int timeRecord, int getTimeReward, int bossRecord, int getBossReward, int artifactRecord, int getArtifactReward, int adRecord, int reinRecord)
            {
                _dayQuest = dayQuest;
                _questSkin = questSkin;
                _timeRecord = timeRecord;
                _getTimeReward = getTimeReward;
                _bossRecord = bossRecord;
                _getBossReward = getBossReward;
                _artifactRecord = artifactRecord;
                _getArtifactReward = getArtifactReward;
                _adRecord = adRecord;
                _reinRecord = reinRecord;
            }
        }

        /// <summary> 인앱결제 </summary>
        [Serializable]
        public struct payment
        {
            //인앱결제
            [SerializeField] public bool _addGoods;
            [SerializeField] public float _addGoodsValue;
            [SerializeField] public bool _removeAD;
            [SerializeField] public bool _startPack;
            [SerializeField] public bool _skinPack;

            public payment(bool addGoods, float addGoodsValue, bool removeAD, bool startPack, bool skinPack)
            {
                _addGoods = addGoods;
                _addGoodsValue = addGoodsValue;
                _removeAD = removeAD;
                _startPack = startPack;
                _skinPack = skinPack;
            }
        }

        /// <summary> 옵션 </summary>
        [Serializable]
        public struct option
        {            
            [SerializeField] public float _bgmVol;
            [SerializeField] public float _sfxVol;

            public option(float bgmVol, float sfxVol)
            {
                _bgmVol = bgmVol;
                _sfxVol = sfxVol;
            }
        }

        /// <summary> 게임 기타 변수 </summary>
        [Serializable]
        public struct gameUtility
        {
            [SerializeField] public long _join;             // 최초 가입
            [SerializeField] public bool _isSavedServer;    // 서버에 저장 여부

            [SerializeField] public long _lastSave;         // 마지막 저장

            public gameUtility(long join, bool isSavedServer, long lastSave)
            {
                _join = join;
                _isSavedServer = isSavedServer;
                _lastSave = lastSave;
            }
        }

        #endregion

        #region private

        public property _property;  // 자산
        public status _status;      // 능력치
        public quest _quest;        // 퀘스트
        public payment _payment;    // 결제
        public option _option;      // 옵션
        public gameUtility _util;   // 유틸

        #endregion

        /// <summary> 초기화 생성 </summary>
        public UserEntity()
        {
            _property = new property();
            {
                _property._nickName = "ready_Player_1";                 // 닉                
                _property._currency = new int[3] { 99999999, 2000, 0 }; // 재화                
                _property._hasSkin = 1;                                 // 스킨
            }

            _status = new status();// 스탯
            {
                _status._statusLevel = new int[(int)StatusData.max];

                _status._hp = DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString());
                _status._hpgen = DataManager.GetTable<float>(DataTable.status, "default", StatusData.hpgen.ToString());
                _status._def = DataManager.GetTable<int>(DataTable.status, "default", StatusData.def.ToString());
                _status._att = DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString());
                _status._cool = DataManager.GetTable<float>(DataTable.status, "default", StatusData.cool.ToString());
                _status._expFactor = DataManager.GetTable<float>(DataTable.status, "default", StatusData.exp.ToString());
                _status._coinFactor = DataManager.GetTable<float>(DataTable.status, "default", StatusData.coin.ToString());
                _status._skinEnhance = DataManager.GetTable<float>(DataTable.status, "default", StatusData.skin.ToString());
            }

            _quest = new quest();   // 퀘스트
            {
                _quest._dayQuest = new int[3];

                _quest._timeRecord = 0;
                _quest._getTimeReward = 0;
                _quest._bossRecord = 0;
                _quest._getBossReward = 0;
                _quest._artifactRecord = 0;
                _quest._getArtifactReward = 0;
                _quest._adRecord = 0;
                _quest._reinRecord = 0;
            }

            _payment = new payment();   // 결제
            {
                _payment._addGoods = false;
                _payment._addGoodsValue = 1f;
                _payment._removeAD = false;
            }

            _option = new option();     // 옵션
            {
                _option._bgmVol = 1f;
                _option._sfxVol = 1f;
            }

            _util = new gameUtility(); // 유틸
            {
                _util._join = getLoginTime();
                _util._isSavedServer = false;
                _util._lastSave = _util._join;
            }
        }

        /// <summary> 스탯 레벨 -> 스탯에 적용 </summary>
        public void applyLevel()
        {
            _status._hp             = DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString())
                             + DataManager.GetTable<int>(DataTable.status, "addition", StatusData.hp.ToString()) * _status._statusLevel[(int)StatusData.hp];
            _status._hpgen          = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.hpgen.ToString()) * _status._statusLevel[(int)StatusData.hpgen];
            _status._def            = DataManager.GetTable<int>(DataTable.status, "addition", StatusData.def.ToString()) * _status._statusLevel[(int)StatusData.def];
            _status._att            = DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString())
                             + DataManager.GetTable<float>(DataTable.status, "addition", StatusData.att.ToString()) * _status._statusLevel[(int)StatusData.att];
            _status._cool           = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.cool.ToString()), _status._statusLevel[(int)StatusData.cool]);
            _status._expFactor      = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.exp.ToString()), _status._statusLevel[(int)StatusData.exp]);
            _status._coinFactor     = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.coin.ToString()), _status._statusLevel[(int)StatusData.coin]);
            _status._skinEnhance    = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.skin.ToString()) * _status._statusLevel[(int)StatusData.skin];
        }

        /// <summary> 스탯 레벨 업 </summary>
        public void statusLevelUp(StatusData stat)
        {
            _status._statusLevel[(int)stat]++;

            switch (stat)
            {
                case StatusData.hp:
                    _status._hp = DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString())
                        + DataManager.GetTable<int>(DataTable.status, "addition", StatusData.hp.ToString()) * _status._statusLevel[(int)StatusData.hp];
                    break;
                case StatusData.att:
                    _status._att = DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString())
                        + DataManager.GetTable<float>(DataTable.status, "addition", StatusData.att.ToString()) * _status._statusLevel[(int)StatusData.att];
                    break;
                case StatusData.def:
                    _status._def = DataManager.GetTable<int>(DataTable.status, "addition", StatusData.def.ToString()) * _status._statusLevel[(int)StatusData.def];
                    break;
                case StatusData.hpgen:
                    _status._hpgen = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.hpgen.ToString()) * _status._statusLevel[(int)StatusData.hpgen];
                    break;
                case StatusData.cool:
                    _status._cool = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.cool.ToString()), _status._statusLevel[(int)StatusData.cool]);
                    break;                
                case StatusData.exp:
                    _status._expFactor = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.exp.ToString()), _status._statusLevel[(int)StatusData.exp]);
                    break;
                case StatusData.coin:
                    _status._coinFactor = Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.coin.ToString()), _status._statusLevel[(int)StatusData.coin]);
                    break;
                case StatusData.skin:
                    _status._skinEnhance = DataManager.GetTable<float>(DataTable.status, "addition", StatusData.skin.ToString()) * _status._statusLevel[(int)StatusData.skin];
                    break;
            }
        }

        /// <summary> 데이터 저장 </summary>
        public string saveData()
        {
            return JsonUtility.ToJson(this);
        }

        public long getLoginTime()
        {
            DateTime dateDate = DateTime.Now;
            long result = dateDate.Year * 10000000000 + dateDate.Month * 100000000 + dateDate.Day * 1000000 + dateDate.Hour * 10000 + dateDate.Minute * 100 + dateDate.Second;
            return result;
        }
    }
}