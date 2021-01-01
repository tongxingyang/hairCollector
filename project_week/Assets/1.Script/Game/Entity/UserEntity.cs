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
            
            /// <summary> 레이더 장착여부 </summary>
            [SerializeField] public ObscuredBool _isSetRader;
            /// <summary> 레이더 마지막 대여시간 </summary>
            [SerializeField] public ObscuredLong _lastRaderTime;

            public property(ObscuredString nic, ObscuredInt[] cur, ObscuredInt hasskin, ObscuredInt skin, ObscuredBool isSetRader, ObscuredLong lastRaderTime)
            {
                _nickName = nic;
                _currency = cur;
                _hasSkin = hasskin;
                _skin = skin;
                _isSetRader = isSetRader;
                _lastRaderTime = lastRaderTime;
            }
        }

        /// <summary> 플레이어 통계 데이터 </summary>
        [Serializable]
        public struct statistics
        {
            /// <summary> 전체 접속 시간 </summary>
            [SerializeField] public ObscuredInt _wholeAccessTime;
            /// <summary> 모험 플레이 횟수 </summary>
            [SerializeField] public ObscuredInt _playCount;
            /// <summary> 상점 이용 횟수 (전체, 일반상품) </summary>
            [SerializeField] public ObscuredInt _storeUseCount;

            public statistics(ObscuredInt wholeAccessTime, ObscuredInt playCount, ObscuredInt storeUseCount)
            {
                _wholeAccessTime = wholeAccessTime;
                _playCount = playCount;
                _storeUseCount = storeUseCount;
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
            [SerializeField] public ObscuredString _nowSeasonRankKey;
            [SerializeField] public ObscuredInt _seasonTimeRecord;
            [SerializeField] public ObscuredInt _recordSeasonSkin; // 시즌 신기록 당시의 스킨

            [SerializeField] public ObscuredInt _allTimeRecord;
            [SerializeField] public ObscuredInt _recordAllSkin; // 전체 신기록 당시의 스킨

            [SerializeField] public ObscuredInt _wholeTimeRecord;
            [SerializeField] public ObscuredInt _bossRecord;
            [SerializeField] public ObscuredInt _artifactRecord;
            [SerializeField] public ObscuredInt _adRecord;
            [SerializeField] public ObscuredInt _reinRecord;

            public record(ObscuredString nowSeasonRankKey, ObscuredInt seasonTimeRecord, ObscuredInt recordssSkin, ObscuredInt allTimeRecord, ObscuredInt recordallSkin,
                ObscuredInt wholeTimeRecord, ObscuredInt bossRecord, ObscuredInt artifactRecord, ObscuredInt adRecord, ObscuredInt reinRecord)
            {
                _nowSeasonRankKey = nowSeasonRankKey;

                _seasonTimeRecord = seasonTimeRecord;
                _recordSeasonSkin = recordssSkin;
                _allTimeRecord = allTimeRecord;
                _recordAllSkin = recordallSkin;

                _wholeTimeRecord = wholeTimeRecord;
                _bossRecord = bossRecord;
                _artifactRecord = artifactRecord;
                _adRecord = adRecord;
                _reinRecord = reinRecord;
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
            // [SerializeField] public ObscuredInt _getArtifactReward;

            public quest(ObscuredInt[] dayQuest, ObscuredInt questSkin, ObscuredInt getTimeReward, ObscuredInt getBossReward)
            {
                _dayQuest = dayQuest;
                _questSkin = questSkin;
                _getTimeReward = getTimeReward;
                _getBossReward = getBossReward;
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

            /// <summary> 레이더 마지막 대여시간 </summary>
            [SerializeField] public ObscuredLong _nextAdGemTime;

            public payment(ObscuredInt mullist, ObscuredInt chklist, ObscuredLong nextAdGemTime)
            {
                _mulCoinList = mullist;
                _chkList = chklist;
                _nextAdGemTime = nextAdGemTime;
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

        [SerializeField] public property _property;     // 자산
        [SerializeField] public statistics _statistics; // 통계
        [SerializeField] public status _status;         // 능력치
        [SerializeField] public record _record;         // 기록
        [SerializeField] public quest _quest;           // 퀘스트
        [SerializeField] public payment _payment;       // 결제
        [SerializeField] public gameUtility _util;      // 유틸

        #endregion

        /// <summary> 초기화 생성 </summary>
        public UserEntity()
        {
            Debug.Log("entity 초기화");

            // 유저 기본정보
            _property = new property(    
                nic     : "ready_Player_1",                     // 닉                
                cur     : new ObscuredInt[3] { 1000, 10, 1 },   // 재화       
                hasskin : 1,                                    // 보유스킨
                skin    : 0,                                    // 스킨
                isSetRader      : false,                        // 레이더 대여 여부
                lastRaderTime   : 0                             // 레이더 마지막 대여시간
            );

            _statistics = new statistics(
                wholeAccessTime : 0,    // 전체 플탐
                playCount       : 0,    // 플 횟수
                storeUseCount   : 0     // 상점이용횟수
                );

            // 스탯
            _status = new status(
                statusLevel : new ObscuredInt[(int)StatusData.max]
            );

            // 기록
            _record = new record(
                nowSeasonRankKey: "non",

                seasonTimeRecord: 0,
                recordssSkin    : 1,
                allTimeRecord   : 0,
                recordallSkin   : 1,

                wholeTimeRecord : 0,
                bossRecord      : 0,
                artifactRecord  : 0,
                adRecord        : 0,
                reinRecord      : 0
            );

            // 업적
            _quest = new quest(
                dayQuest            : new ObscuredInt[3],
                questSkin           : 0,

                getTimeReward       : 0,
                getBossReward       : 0
            );

            // 인앱결제
            _payment = new payment(
                mullist : 0,
                chklist : 0,
                nextAdGemTime: 0
            );

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