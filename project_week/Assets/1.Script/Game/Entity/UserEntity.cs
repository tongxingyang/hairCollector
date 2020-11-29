using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;

namespace week
{
    [Serializable]
    public class UserEntity
    {
        #region [ classification ]

        /// <summary> 플레이어 재산 </summary>
        [Serializable]
        public struct property
        {
            /// <summary> 닉넴 </summary>
            [SerializeField] public ObscuredString _nickName;
            /// <summary> 재화 (코인, 젬, ap) </summary>
            [SerializeField] public ObscuredInt[] _currency;
            /// <summary> 스킨 </summary>
            [SerializeField] public ObscuredInt _hasSkin;
            /// <summary> 장착중인 스킨 </summary>
            [SerializeField] public ObscuredInt _skin;
            /// <summary> 전체 접속 시간 </summary>
            [SerializeField] public ObscuredInt _wholeAccessTime;

            public property(ObscuredString nic, ObscuredInt[] cur, ObscuredInt hasskin, ObscuredInt skin, ObscuredInt wholeAccessTime)
            {
                _nickName = nic;
                _currency = cur;
                _hasSkin = hasskin;
                _skin = skin;
                _wholeAccessTime = wholeAccessTime;
            }
        }

        /// <summary> 플레이어 능력치 </summary>
        [Serializable]
        public struct status
        {
            /// <summary> 강화레벨 </summary>
            [SerializeField] public ObscuredInt[] _statusLevel;

            public status(ObscuredInt[] statusLevel)
            {
                _statusLevel = statusLevel;
            }
        }

        /// <summary> 기록 </summary>
        [Serializable]
        public struct record
        {
            [SerializeField] public ObscuredInt _timeRecord;
            [SerializeField] public ObscuredInt _wholeTimeRecord;
            [SerializeField] public ObscuredInt _bossRecord;
            [SerializeField] public ObscuredInt _artifactRecord;
            [SerializeField] public ObscuredInt _adRecord;
            [SerializeField] public ObscuredInt _reinRecord;
            [SerializeField] public ObscuredInt _recordSkin; // 신기록 당시의 스킨

            public record(ObscuredInt timeRecord, ObscuredInt wholeTimeRecord, ObscuredInt bossRecord, ObscuredInt artifactRecord, ObscuredInt adRecord, ObscuredInt reinRecord, ObscuredInt recordSkin)
            {
                _timeRecord = timeRecord;
                _wholeTimeRecord = wholeTimeRecord;
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
            [SerializeField] public ObscuredInt[] _dayQuest;
            [SerializeField] public ObscuredInt _questSkin;
            // 전체        
            [SerializeField] public ObscuredInt _getTimeReward;
            [SerializeField] public ObscuredInt _getBossReward;
            [SerializeField] public ObscuredInt _getArtifactReward;

            public quest(ObscuredInt[] dayQuest, ObscuredInt questSkin, ObscuredInt getTimeReward, ObscuredInt getBossReward, ObscuredInt getArtifactReward)
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
            [SerializeField] public ObscuredInt _mulCoinList;
            /// <summary> 인앱결제 - 일회성 구매 상품 체크리스트 </summary>
            [SerializeField] public ObscuredInt _chkList;

            public payment(ObscuredInt mullist, ObscuredInt chklist)
            {
                _mulCoinList = mullist;
                _chkList = chklist;
            }
        }

        /// <summary> 게임 기타 변수 </summary>
        [Serializable]
        public struct gameUtility
        {
            [SerializeField] public ObscuredLong _lastSave;         // 마지막 저장

            /// <summary> 유틸 체크리스트 </summary>
            [SerializeField] public ObscuredInt _chkList;

            public gameUtility(long lastSave, ObscuredInt chklist)
            {
                _lastSave = lastSave;
                _chkList = chklist;
            }
        }

        #endregion

        #region private

        [SerializeField] public property _property;  // 자산
        [SerializeField] public status _status;      // 능력치
        [SerializeField] public record _record;      // 기록
        [SerializeField] public quest _quest;        // 퀘스트
        [SerializeField] public payment _payment;    // 결제
        [SerializeField] public gameUtility _util;   // 유틸

        #endregion

        /// <summary> 초기화 생성 </summary>
        public UserEntity()
        {
            Debug.Log("entity 초기화");

            // 유저 기본정보
            _property = new property(    
                nic     : "ready_Player_1",                     // 닉                
                cur     : new ObscuredInt[3] { 1000, 10, 1 },           // 재화       
                hasskin : 1,                                    // 보유스킨
                skin    : 0,                                    // 스킨
                wholeAccessTime : 0                             // 접속시간
            );

            // 스탯
            _status = new status(
                statusLevel : new ObscuredInt[(int)StatusData.max]
            );

            // 기록
            _record = new record(
                timeRecord      : 0,
                wholeTimeRecord : 0,
                bossRecord      : 0,
                artifactRecord  : 0,
                adRecord        : 0,
                reinRecord      : 0,
                recordSkin      : 1
            );

            // 업적
            _quest = new quest(
                dayQuest            : new ObscuredInt[3],
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

            //const string strPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //문자 생성 풀
            //char[] chRandom = new char[8];
            //for (int i = 0; i < 8; i++ ) 
            //{
            //    chRandom[i] = strPool[UnityEngine.Random.Range(0, strPool.Length)];
            //} 
            //string strRet = ((network) ? AuthManager.instance.LastLogin.ToString() : "000") + new String(chRandom); 

            // 유틸
            _util = new gameUtility(
                lastSave    : 0,
                chklist     : 0
            );;
        }        

        /// <summary> 데이터 저장 </summary>
        public string saveData()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new ObscuredValueConverter());
        }
    }
}