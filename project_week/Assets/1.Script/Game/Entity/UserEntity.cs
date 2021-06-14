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
        public class property
        {
            /// <summary> 닉넴 </summary>
            [SerializeField] public ObscuredString _nickName;
            /// <summary> 재화 (코인, 젬, ap) </summary>
            [SerializeField] public ObscuredInt[] _currency;
            /// <summary> 스킨 </summary>
            [SerializeField] public ObscuredInt _hasSkin;
            /// <summary> 보유 스킨 레벨 </summary>
            [SerializeField] public ObscuredInt[] _skinLevel;
            /// <summary> 장착중인 스킨 </summary>
            [SerializeField] public ObscuredInt _skin;
            
            /// <summary> 현재 난이도 </summary>
            [SerializeField] public ObscuredInt _nowStageLevel;
            /// <summary> 난이도 오픈여부 </summary>
            [SerializeField] public ObscuredInt _isLevelOpen;

            public property()
            {
                _nickName = "ready_Player_1";                     // 닉                
                _currency = new ObscuredInt[3] { 1000, 10, 1 };   // 재화       
                _hasSkin = 1;                                    // 보유스킨
                _skinLevel = new ObscuredInt[(int)SkinKeyList.max];
                _skin = 0;                                    // 스킨
                _nowStageLevel = 0;                            // 스테이지 레벨
                _isLevelOpen = 1;   
            }
            public property(ObscuredString nic, ObscuredInt[] cur, ObscuredInt hasskin, ObscuredInt[] skinLevel, ObscuredInt skin, 
                ObscuredInt nowStageLevel, ObscuredBool isSetRader, ObscuredLong lastRaderTime, ObscuredInt isLevelOpen)
            {
                _nickName = nic;
                _currency = cur;
                _hasSkin = hasskin;
                _skinLevel = skinLevel;
                _skin = skin;
                _nowStageLevel = nowStageLevel;
                _isLevelOpen = isLevelOpen;
            }
        }

        /// <summary> 플레이어 통계 데이터 </summary>
        [Serializable]
        public class statistics
        {
            /// <summary> 전체 접속 시간 </summary>
            [SerializeField] public ObscuredInt _wholeAccessTime;
            /// <summary> 모험 플레이 횟수 </summary>
            [SerializeField] public ObscuredInt _playCount;
            /// <summary> 상점 이용 횟수 (전체, 일반상품) </summary>
            [SerializeField] public ObscuredInt _storeUseCount;

            public statistics()
            {
                _wholeAccessTime = 0;
                _playCount = 0;
                _storeUseCount = 0;
            }
            public statistics(ObscuredInt wholeAccessTime, ObscuredInt playCount, ObscuredInt storeUseCount)
            {
                _wholeAccessTime = wholeAccessTime;
                _playCount = playCount;
                _storeUseCount = storeUseCount;
            }
        }

        /// <summary> 플레이어 능력치 </summary>
        [Serializable]
        public class status
        {
            /// <summary> 강화레벨 </summary>
            [SerializeField] public ObscuredInt[] _statusLevel;

            public status()
            {
                _statusLevel = new ObscuredInt[(int)StatusData.max];
            }
            public status(ObscuredInt[] statusLevel)
            {
                _statusLevel = statusLevel;
            }
        }        

        /// <summary> 기록 </summary>
        [Serializable]
        public class record
        {
            //[SerializeField] public levelRecord[] _levelRecord; // 난이도별 기록
            [SerializeField] public ObscuredInt[] _season_TimeRecord;     // 시즌 기록
            [SerializeField] public ObscuredInt[] _season_RecordSkin;     // 시즌 신기록 당시의 스킨
            [SerializeField] public ObscuredInt[] _season_RecordLevel;    // 시즌 신기록 당시 레벨
            [SerializeField] public ObscuredInt[] _season_RecordBoss;     // 보스킬

            [SerializeField] public ObscuredInt _requestRecord; // 의뢰 해결
            [SerializeField] public ObscuredInt _reinRecord; // 강화 횟수

            [SerializeField] public ObscuredInt _wholeTimeRecord; // 전체 플레이 타임

            public record() 
            {
                _season_TimeRecord= new ObscuredInt[(int)levelKey.max];
                _season_RecordSkin= new ObscuredInt[(int)levelKey.max];
                _season_RecordLevel= new ObscuredInt[(int)levelKey.max];
                _season_RecordBoss= new ObscuredInt[(int)levelKey.max];
                
                _requestRecord= 0;
                _reinRecord= 0;
                _wholeTimeRecord= 0; 
            }

            public record(ObscuredInt[] season_TimeRecord, ObscuredInt[] season_RecordSkin, ObscuredInt[] season_RecordLevel, ObscuredInt[] season_RecordBoss, 
                ObscuredInt requestRecord, ObscuredInt reinRecord, ObscuredInt wholeTimeRecord)
            {
                _season_TimeRecord = season_TimeRecord;
                _season_RecordSkin = season_RecordSkin;
                _season_RecordLevel = season_RecordLevel;
                _season_RecordBoss = season_RecordBoss;

                _requestRecord = requestRecord;
                _reinRecord = reinRecord;
                _wholeTimeRecord = wholeTimeRecord;
            }

            /// <summary> 시즌 기록 초기화 </summary>
            public void initSeasonRecord()
            {
                for (int i = 0; i < 3; i++)
                {
                    _season_TimeRecord[i] = 0;
                    _season_RecordSkin[i] = 1;
                    _season_RecordLevel[i] = 0;
                    _season_RecordBoss[i] = 0;
                }
            }
        }

        /// <summary> 퀘스트 </summary>
        [Serializable]
        public class quest
        {
            // 일일퀘스트            
            [SerializeField] public ObscuredInt[] _questChk;    // 1~3
            [SerializeField] public ObscuredInt _questSkin;
            [SerializeField] public ObscuredInt[] _questSkill;  // 4~6
            // 반복퀘스트
            [SerializeField] public ObscuredInt _questRein;
            [SerializeField] public ObscuredInt _questRequest;
            // 난이도별 퀘스트
            [SerializeField] public ObscuredInt[] _lvlTimeReward;
            [SerializeField] public ObscuredInt[] _lvlBossReward;

            public quest()
            {
                _questChk = new ObscuredInt[(int)Quest.day_max];
                _questSkin = 0;
                _questSkill = new ObscuredInt[3];

                _questRein = 0;
                _questRequest = 0;

                _lvlTimeReward = new ObscuredInt[(int)levelKey.max];
                _lvlBossReward = new ObscuredInt[(int)levelKey.max];
            }

            public quest( ObscuredInt[] questChk, ObscuredInt questSkin, ObscuredInt[] questSkill,
                            ObscuredInt questRein, ObscuredInt questRequest,
                            ObscuredInt[] lvlTimeReward, ObscuredInt[] lvlBossReward)
            {
                _questChk = questChk;

                _questSkin = questSkin;
                _questSkill = questSkill;

                _questRein = questRein;
                _questRequest = questRequest;

                _lvlTimeReward = lvlTimeReward;
                _lvlBossReward = lvlBossReward;
            }

            /// <summary> (하루경과) 일일퀘스트 재설정 - [플레이시 한번 체크] </summary>
            public void setNextDay()
            {
                _questChk = new ObscuredInt[(int)Quest.day_max];

                // 스킨
                List<SkinKeyList> skins = new List<SkinKeyList>();
                for (int i = 0; i < (int)SkinKeyList.max; i++)
                {
                    if ((BaseManager.userGameData.HasSkin & (1 << i)) > 0)
                    {
                        skins.Add((SkinKeyList)i);
                    }
                }
                int sk = UnityEngine.Random.Range(0, skins.Count);
                _questSkin = (int)skins[sk];                            // 스킨 설정

                // 스킬

                // 스킬0 (능력치) 돌림판 -> 선택
                do
                {
                    _questSkill[0] = UnityEngine.Random.Range(0, (int)SkillKeyList.SnowBall);
                } while (_questSkill[0] == (int)SkillKeyList.BOSS || _questSkill[0] == (int)SkillKeyList.DODGE);

                // 스킬1,2 돌림판
                List<SkillKeyList>[] skills = new List<SkillKeyList>[2];
                for (int i = 0; i < 2; i++)
                {
                    skills[i] = new List<SkillKeyList>();
                }
                // 스킬1,2 돌림판
                for (SkillKeyList skl = SkillKeyList.SnowBall; skl < SkillKeyList.Present; skl++)
                {
                    int num = D_skill.GetEntity(skl.ToString()).f_rank % 10;
                    if (num > 0 && num < 3)
                    {
                        skills[num - 1].Add(skl);
                    }
                }
                // 스킬1,2 선택
                for (int i = 0; i < 2; i++)
                {
                    int j = UnityEngine.Random.Range(0, skills[i].Count);
                    _questSkill[i + 1] = (int)skills[i][j];                 // 스킬 설정
                }
            }
        }

        /// <summary> 인앱결제 </summary>
        [Serializable]
        public class payment
        {
            ///// <summary> 광고제거 </summary>
            //[SerializeField] public ObscuredBool _removeAd;            
            /// <summary> 코인 추가 배율 </summary>
            [SerializeField] public ObscuredInt _mulCoinList;

            /// <summary> </summary>
            [SerializeField] public ObscuredInt _chkList;

            /// <summary> 이번 시즌 상품 2 - (뱀파세트) </summary>
            [SerializeField] public ObscuredBool _vampPack;
            /// <summary> 이번 시즌 상품 1 - (용사세트) </summary>
            [SerializeField] public ObscuredBool _heroPack;

            /// <summary> 남은 무료 보석 </summary>
            [SerializeField] public ObscuredInt _leftFreeGem;

            public payment()
            {
                _mulCoinList = 0;
                _chkList = 0;
                _vampPack = false;
                _heroPack = false;
                _leftFreeGem = 3;
            }
            public payment(ObscuredInt mulCoinList, ObscuredInt chkList, ObscuredBool vampPack, ObscuredBool heroPack, ObscuredInt leftFreeGem)
            {
                _mulCoinList = mulCoinList;
                _chkList = chkList;
                _vampPack = vampPack;
                _heroPack = heroPack;
                _leftFreeGem = leftFreeGem;
            }
        }

        /// <summary> 게임 기타 변수 </summary>
        [Serializable]
        public class gameUtility
        {
            [SerializeField] public ObscuredLong _lastSave;         // 마지막 저장

            /// <summary> 유틸 체크리스트 </summary>
            [SerializeField] public ObscuredInt _chkList;
            /// <summary> 퀘스트등 날짜 체크용 </summary>
            [SerializeField] public ObscuredInt _publishDate;

            public gameUtility()
            {
                _lastSave = 0;
                _chkList = 0;
                _publishDate = 0;
            }
            public gameUtility(long lastSave, ObscuredInt chklist, ObscuredInt publishDate)
            {
                _lastSave = lastSave;
                _chkList = chklist;
                _publishDate = publishDate;
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
            _property = new property();

            _statistics = new statistics();

            // 스탯
            _status = new status();

            // 기록
            _record = new record();

            // 업적
            _quest = new quest();

            // 인앱결제
            _payment = new payment();

            // 유틸
            _util = new gameUtility();
        }        

        /// <summary> 데이터 저장 </summary>
        public string saveData()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new ObscuredValueConverter());
        }
    }
}