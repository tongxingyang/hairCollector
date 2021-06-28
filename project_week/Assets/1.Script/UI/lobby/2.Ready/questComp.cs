using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeStage.AntiCheat.ObscuredTypes;

namespace week
{
    public class questComp : UIBase, UIInterface
    {
        #region [UIBase]

        enum dayQuestButton { dayRein, daySkin, dayRevive, daySkill0, daySkill1, daySkill2 }

        enum eTmp
        {           
            qstReinTxt,
            qstRequestTxt,

            qstEasyTimeTxt,
            qstNormalTimeTxt,
            qstHardTimeTxt,

            qstEasyBossTxt,
            qstNormalBossTxt,
            qstHardBossTxt
        }

        enum eImg
        {
            qstReinBtn,
            qstRequestBtn,

            qstEasyTimeBtn,
            qstNormalTimeBtn,
            qstHardTimeBtn,

            qstEasyBossBtn,
            qstNormalBossBtn,
            qstHardBossBtn
        }

        // 일퀘 버튼
        public dayQuestInstance[] mDayQstBtns;
        protected Enum GetEnumDayQuest() { return new dayQuestButton(); }
        // Tmp
        public TextMeshProUGUI[] mTmps;
        protected Enum GetEnumTmp() { return new eTmp(); }
        // 이미지
        protected override Enum GetEnumImage() { return new eImg(); }
        

        protected override void OtherSetContent()
        {
            // 일퀘 버튼
            if (GetEnumDayQuest() != null)
            {
                mDayQstBtns = SetComponent<dayQuestInstance>(GetEnumDayQuest());
            }
            // Tmp
            if (GetEnumTmp() != null)
            {
                mTmps = SetComponent<TextMeshProUGUI>(GetEnumTmp());
            }            
        }

        #endregion

        LobbyScene _lobby;
        Action<bool> _exclamation; // 알림
        bool[] _getable;

        Color _transparent = new Color(0, 0, 0, 0);
        Color _enableColor = new Color(1f, 0.3f, 0.4f, 1f);

        Color _dayStandard = new Color(0.8f, 0.8f, 0.8f, 1f);
        Color _dayBlack = new Color(0, 0, 0, 0.5f);

        Color _qstBlack = new Color(0.5f, 0.5f, 0.5f, 1f);

        #region [interface]
        public void open()
        {
            refresh_CheckQuest();
            gameObject.SetActive(true);
        }
        public void close()
        {
            refresh_CheckQuest();
            gameObject.SetActive(false);

            AuthManager.instance.SaveDataServer(true);
        }

        #endregion

        #region [init]

        void Awake()
        {
            _getable = new bool[(int)Quest.day_max];

            for (eImg i = 0; i <= eImg.qstHardBossBtn; i++)
            {
                int num = (int)i;
                mImgs[num].GetComponent<Button>().onClick.AddListener(()=>clearQuest(num));
            }
        }

        /// <summary> 초기화 </summary>
        public void Init(LobbyScene lobby, Action<bool> exclamation)
        {
            _lobby = lobby;
            _exclamation = exclamation;

            // 일퀘 버튼 초기화
            for (Quest qst = 0; qst < Quest.day_max; qst++)
            {
                mDayQstBtns[(int)qst].Init(_lobby, qst, refresh_CheckQuest);
            }

            refresh_CheckQuest();
        }

        /// <summary> 수령하지않은 완료퀘스트가 있는지 체크 </summary>
        public void refresh_CheckQuest()
        {
            bool chk = false;

            chk |= refreshDayQuest();

            chk |= refreshQuest();            

            _exclamation?.Invoke(chk);
        }

        /// <summary> 일일퀘스트 달성여부 새로고침 </summary>
        bool refreshDayQuest()
        {
            bool check = false;

            for (Quest qst = 0; qst < Quest.day_max; qst++)
            {
                check |= mDayQstBtns[(int)qst].progress();
            }

            return check;
        }

        /// <summary> 일반퀘스트 새로고침 </summary>
        bool refreshQuest()
        {
            bool _chk = false;
            int _Val;

            _Val = D_quest.GetEntity(Quest.rein.ToString()).f_val;
            mTmps[(int)eTmp.qstReinTxt].text = $"능력치 강화 ({BaseManager.userGameData.ReinRecord}/{_Val}회)";
            _chk = (BaseManager.userGameData.ReinRecord >= _Val);
            mImgs[(int)eImg.qstReinBtn].color = (_chk) ? _enableColor : _qstBlack;
            mImgs[(int)eImg.qstReinBtn].raycastTarget = _chk;

            _Val = D_quest.GetEntity(Quest.request.ToString()).f_val;
            mTmps[(int)eTmp.qstRequestTxt].text = $"의뢰 ({BaseManager.userGameData.RequestRecord}/{_Val}회 해결)";
            _chk = (BaseManager.userGameData.RequestRecord >= _Val);
            mImgs[(int)eImg.qstRequestBtn].color = (_chk) ? _enableColor : _qstBlack;
            mImgs[(int)eImg.qstRequestBtn].raycastTarget = _chk;

            for (levelKey i = 0; i < levelKey.max; i++)
            {
                int lvl = (int)i;

                // 시간
                mTmps[(int)eTmp.qstEasyTimeTxt + lvl].text = $"({D_level.GetEntity(i.ToString()).f_trans}) {BaseManager.userGameData.dayToTimeRecord(i, BaseManager.userGameData.LvlTimeReward[lvl] + 1)}까지 버티기";
                _chk = (BaseManager.userGameData.LvlTimeReward[lvl] < BaseManager.userGameData.TimeRecord(i) / 120);
                mImgs[(int)eImg.qstEasyTimeBtn + lvl].color = (_chk) ? _enableColor : _qstBlack;
                mImgs[(int)eImg.qstEasyTimeBtn + lvl].raycastTarget = _chk;

                // 보스
                _Val = D_quest.GetEntity((Quest.easy_boss + lvl).ToString()).f_val;
                mTmps[(int)eTmp.qstEasyBossTxt + lvl].text = $"({D_level.GetEntity(i.ToString()).f_trans}) 한번의 모험에서 보스 {BaseManager.userGameData.LvlBossReward[lvl] + 1}마리 처치";
                _chk = (BaseManager.userGameData.RecordBoss(i) - BaseManager.userGameData.LvlBossReward[lvl] >= _Val);
                mImgs[(int)eImg.qstEasyBossBtn + lvl].color = (_chk) ? _enableColor : _qstBlack;
                mImgs[(int)eImg.qstEasyBossBtn + lvl].raycastTarget = _chk;
            }

            return _chk;
        }

        /// <summary> 일반퀘스트 클리어 </summary>
        public void clearQuest(int i)
        {
            Quest qst = Quest.rein + i;
            int _Val = D_quest.GetEntity(qst.ToString()).f_val;
            currency cur = EnumHelper.StringToEnum<currency>(D_quest.GetEntity(qst.ToString()).f_currency);
            int curMount = D_quest.GetEntity(qst.ToString()).f_reward;

            switch (qst)
            {
                case Quest.rein:
                    BaseManager.userGameData.ReinRecord -= _Val;
                    break;
                case Quest.request:
                    BaseManager.userGameData.RequestRecord -= _Val;
                    break;
                case Quest.easy_time:
                    BaseManager.userGameData.LvlTimeReward[(int)levelKey.easy] += 1;
                    break;
                case Quest.normal_time:
                    BaseManager.userGameData.LvlTimeReward[(int)levelKey.normal] += 1;
                    break;
                case Quest.hard_time:
                    BaseManager.userGameData.LvlTimeReward[(int)levelKey.hard] += 1;
                    break;
                case Quest.easy_boss:
                    BaseManager.userGameData.LvlBossReward[(int)levelKey.easy] += 1;
                    break;
                case Quest.normal_boss:
                    BaseManager.userGameData.LvlBossReward[(int)levelKey.normal] += 1;
                    break;
                case Quest.hard_boss:
                    BaseManager.userGameData.LvlBossReward[(int)levelKey.hard] += 1;
                    break;
                default:
                    break;                
            }

            Debug.Log(mImgs[(int)eImg.qstReinBtn + i].transform.position);
            WindowManager.instance.Win_coinGenerator.getWealth2Point(mImgs[(int)eImg.qstReinBtn + i].transform.position, _lobby.GemTxt.position, currency.gem, 10);
            BaseManager.userGameData.Gem += D_quest.GetEntity(qst.ToString()).f_reward;

            refreshQuest();

            AuthManager.instance.SaveDataServer(false);
        }

        #endregion

        /// <summary> (하루경과) 일일퀘스트 재설정 - [플레이시 한번 체크] </summary>
        public void setNextDay(int v)
        {
            for (Quest q = 0; q < Quest.day_max; q++)
            {
                mDayQstBtns[(int)q].questReInit(q);
            }

            refreshDayQuest();
        }
    }
}