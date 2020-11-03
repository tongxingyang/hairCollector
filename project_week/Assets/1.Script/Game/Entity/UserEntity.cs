using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

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

        /// <summary> 기록 </summary>
        [Serializable]
        public struct record
        {
            [SerializeField] public int _timeRecord;
            [SerializeField] public int _bossRecord;
            [SerializeField] public int _artifactRecord;
            [SerializeField] public int _adRecord;
            [SerializeField] public int _reinRecord;
            [SerializeField] public int _recordSkin; // 신기록 당시의 스킨

            public record(int timeRecord, int bossRecord, int artifactRecord, int adRecord, int reinRecord, int recordSkin)
            {
                _timeRecord = timeRecord;
                _bossRecord = bossRecord;
                _artifactRecord = artifactRecord;
                _adRecord = adRecord;
                _reinRecord = reinRecord;
                _recordSkin = recordSkin;
            }
        }

        /// <summary> 퀘스트 </summary>
        [Serializable]
        public struct quest
        {
            // 퀘스트
            // 일일
            [SerializeField] public int[] _dayQuest;
            [SerializeField] public int _questSkin;
            // 전체        
            [SerializeField] public int _getTimeReward;
            [SerializeField] public int _getBossReward;
            [SerializeField] public int _getArtifactReward;

            public quest(int[] dayQuest, int questSkin, int getTimeReward, int getBossReward, int getArtifactReward)
            {
                _dayQuest = dayQuest;
                _questSkin = questSkin;
                _getTimeReward = getTimeReward;
                _getBossReward = getBossReward;
                _getArtifactReward = getArtifactReward;
            }
        }

        /// <summary> 인앱결제 </summary>
        [Serializable]
        public struct payment
        {
            /// <summary> 코인 추가 배율 </summary>
            [SerializeField] public int _mulCoinList;
            /// <summary> 인앱결제 - 일회성 구매 상품 체크리스트 </summary>
            [SerializeField] public int _chkList;

            public payment(int mullist, int chklist)
            {
                _mulCoinList = mullist;
                _chkList = chklist;
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
            [SerializeField] public long _lastSave;         // 마지막 저장
 
            /// <summary> 유틸 체크리스트 </summary>
            [SerializeField] public int _chkList;

            public gameUtility(long join, long lastSave, int chklist)
            {
                _join = join;
                _lastSave = lastSave;
                _chkList = chklist;
            }
        }

        #endregion

        #region private

        public property _property;  // 자산
        public status _status;      // 능력치
        public record _record;      // 기록
        public quest _quest;        // 퀘스트
        public payment _payment;    // 결제
        public option _option;      // 옵션
        public gameUtility _util;   // 유틸

        #endregion

        /// <summary> 초기화 생성 </summary>
        public UserEntity()
        {
            // 유저 기본정보
            _property = new property(    
                nic     : "ready_Player_1",                     // 닉                
                cur     : new int[3] { 1000, 10, 1 },           // 재화       
                hasskin : 1,                                    // 보유스킨
                skin    : 0                                     // 스킨
            );

            // 스탯
            _status = new status(
                statusLevel : new int[(int)StatusData.max],
                hp          : DataManager.GetTable<int>(DataTable.status, "default", StatusData.hp.ToString()),
                hpgen       : DataManager.GetTable<float>(DataTable.status, "default", StatusData.hpgen.ToString()),
                def         : DataManager.GetTable<int>(DataTable.status, "default", StatusData.def.ToString()),
                att         : DataManager.GetTable<float>(DataTable.status, "default", StatusData.att.ToString()),
                cool        : DataManager.GetTable<float>(DataTable.status, "default", StatusData.cool.ToString()),
                expFactor   : DataManager.GetTable<float>(DataTable.status, "default", StatusData.exp.ToString()),
                coinFactor  : DataManager.GetTable<float>(DataTable.status, "default", StatusData.coin.ToString()),
                skinEnhance : DataManager.GetTable<float>(DataTable.status, "default", StatusData.skin.ToString())
            );

            // 기록
            _record = new record(
                timeRecord      : 0,
                bossRecord      : 0,
                artifactRecord  : 0,
                adRecord        : 0,
                reinRecord      : 0,
                recordSkin      : 1
            );

            // 업적
            _quest = new quest(
                dayQuest            : new int[3],
                questSkin           : 0,

                getTimeReward       : 0,
                getBossReward       : 0,
                getArtifactReward   : 0
            );

            // 인앱결제
            _payment = new payment(
                mullist : 0,
                chklist : 0
            );

            // 옵션
            _option = new option(
                bgmVol  : 1f,
                sfxVol  : 1f
            );

            // 유틸
            _util = new gameUtility(
                join        : (AuthManager.instance.networkCheck()) ? AuthManager.instance.LastLogin : 0,                
                lastSave    : (AuthManager.instance.networkCheck()) ? AuthManager.instance.LastLogin : 0,
                chklist     : 0
            );
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
    }
}