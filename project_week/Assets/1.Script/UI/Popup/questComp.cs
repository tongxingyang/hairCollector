using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace week
{
    public class questComp : UIBase, UIInterface
    {
        #region [UIBase]

        enum eTmp
        {
            reinQstTxt,
            skinQstTxt,
            adQstTxt,

            qstTimeTxt,
            qstBossTxt,
            qstArtifactTxt,
            qstAdTxt,
            qstReinTxt
        }

        enum eImg
        {
            dayQst_Rein,
            dayQst_Skin,
            dayQst_Ad,

            dqr_black,
            dqs_black,
            dqa_black,

            qstTimeBtn,
            qstBossBtn,
            qstArtifactBtn,
            qstAdBtn,
            qstReinBtn
        }

        protected override Enum GetEnumImage() { return new eImg(); }
        public TextMeshProUGUI[] mTmps;
        protected Enum GetEnumTmp() { return new eTmp(); }

        protected override void OtherSetContent()
        {
            if (GetEnumTmp() != null)
            {
                mTmps = SetComponent<TextMeshProUGUI>(GetEnumTmp());
            }
        }

        #endregion

        Action _costRefresh;
        bool[] _getable;

        Color _transparent = new Color(0, 0, 0, 0);
        Color _orange = new Color(1f, 0.575f, 0f, 1f);

        Color _dayStandard = new Color(0.8f, 0.8f, 0.8f, 1f);
        Color _dayBlack = new Color(0, 0, 0, 0.5f);

        Color _qstBlack = new Color(0.5f, 0.5f, 0.5f, 1f);

        #region [interface]
        public void open()
        {
            Init();
            gameObject.SetActive(true);
        }
        public void close()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region [init]

        void Awake()
        {
            _getable = new bool[5];
        }

        void Init()
        {
            refreshDayQuest();

            for (int i = 0; i < 5; i++)
            {
                refreshQuest(i);
            }
        }

        /// <summary> 일일퀘스트 새로고침 </summary>
        void refreshDayQuest()
        {
            int day = 0;
            for (int i = 0; i < 3; i++)
            {
                day = BaseManager.userGameData.DayQuest[i];
                if (day == 0) // 새거 미달성
                {
                    mImgs[(int)eImg.dayQst_Rein + i].color = _dayStandard;
                    mImgs[(int)eImg.dqr_black + i].color = _transparent;
                }
                else if (day == 1) // 달성
                {
                    mImgs[(int)eImg.dayQst_Rein + i].color = _orange;
                    mImgs[(int)eImg.dqr_black + i].color = _transparent;
                }
                else // 보상 완료
                {
                    mTmps[(int)eTmp.reinQstTxt + i].text = "완료";
                    mImgs[(int)eImg.dayQst_Rein + i].color = _transparent; 
                    mImgs[(int)eImg.dqr_black + i].color = _dayBlack;
                }
            }
        }

        /// <summary> 일반퀘스트 새로고침 </summary>
        void refreshQuest(int i)
        {
            int _Val = 0;
            bool _chk = false;
            switch (i)
            {
                case 0:
                    mTmps[(int)eTmp.qstTimeTxt].text = BaseManager.userGameData.getTimeRecordToString(BaseManager.userGameData.GetTimeReward + 120) + "까지 버티기";
                    _Val = DataManager.GetTable<int>(DataTable.quest, Quest.time.ToString(), QuestValData.val.ToString());
                    _chk = (BaseManager.userGameData.TimeRecord - BaseManager.userGameData.GetTimeReward >= _Val);
                    break;
                case 1:
                    mTmps[(int)eTmp.qstBossTxt].text = $"한번의 모험에서 보스 {BaseManager.userGameData.GetBossReward + 1}마리 처치";
                    _Val = DataManager.GetTable<int>(DataTable.quest, Quest.boss.ToString(), QuestValData.val.ToString());
                    _chk = (BaseManager.userGameData.BossRecord - BaseManager.userGameData.GetBossReward >= _Val);
                    break;
                case 2:
                    mTmps[(int)eTmp.qstArtifactTxt].text = $"한번의 모험에서 유물 {BaseManager.userGameData.GetBossReward + 1}개 습득";
                    _Val = DataManager.GetTable<int>(DataTable.quest, Quest.artifact.ToString(), QuestValData.val.ToString());
                    _chk = (BaseManager.userGameData.ArtifactRecord - BaseManager.userGameData.GetArtifactReward >= _Val);
                    break;
                case 3:
                    _Val = DataManager.GetTable<int>(DataTable.quest, Quest.ad.ToString(), QuestValData.val.ToString());
                    mTmps[(int)eTmp.qstAdTxt].text = $"광고 처치 ({BaseManager.userGameData.AdRecord}/{_Val}회)";
                    _chk = (BaseManager.userGameData.AdRecord >= _Val);
                    break;
                case 4:
                    _Val = DataManager.GetTable<int>(DataTable.quest, Quest.rein.ToString(), QuestValData.val.ToString());
                    mTmps[(int)eTmp.qstReinTxt].text = $"능력치 강화 ({BaseManager.userGameData.ReinRecord}/{_Val}회)";
                    _chk = (BaseManager.userGameData.ReinRecord >= _Val);
                    break;
            }

            mImgs[(int)eImg.qstTimeBtn + i].color = (_chk) ? _orange : _qstBlack;
            // mImgs[(int)eImg.qstTimeBtn + i].raycastTarget = _chk;
            _getable[i] = _chk;
        }

        /// <summary> 일일퀘스트 클리어 </summary>
        public void clearDayQuest(int i)
        {
            if (BaseManager.userGameData.DayQuest[i] == 1)
            {
                BaseManager.userGameData.DayQuest[i] = 2;

                mTmps[(int)eTmp.reinQstTxt + i].text = "완료";
                mImgs[(int)eImg.dayQst_Rein + i].raycastTarget = false;
                mImgs[(int)eImg.dayQst_Rein + i].color = _transparent;
                mImgs[(int)eImg.dqr_black + i].color = _dayBlack;

                refreshDayQuest();
            }
        }

        /// <summary> 일반퀘스트 클리어 </summary>
        public void clearQuest(int i)
        {
            if (_getable[i])
            {
                int _Val = 0;
                switch (i)
                {
                    case 0:
                        _Val = DataManager.GetTable<int>(DataTable.quest, Quest.time.ToString(), QuestValData.val.ToString());
                        BaseManager.userGameData.GetTimeReward += _Val;
                        break;
                    case 1:
                        _Val = DataManager.GetTable<int>(DataTable.quest, Quest.boss.ToString(), QuestValData.val.ToString());
                        BaseManager.userGameData.GetBossReward += _Val;
                        break;
                    case 2:
                        _Val = DataManager.GetTable<int>(DataTable.quest, Quest.artifact.ToString(), QuestValData.val.ToString());
                        BaseManager.userGameData.GetArtifactReward += _Val;
                        break;
                    case 3:
                        _Val = DataManager.GetTable<int>(DataTable.quest, Quest.ad.ToString(), QuestValData.val.ToString());
                        BaseManager.userGameData.AdRecord -= _Val;
                        break;
                    case 4:
                        _Val = DataManager.GetTable<int>(DataTable.quest, Quest.rein.ToString(), QuestValData.val.ToString());
                        BaseManager.userGameData.ReinRecord -= _Val;
                        break;
                }

                refreshQuest(i);
            }
        }

        #endregion

        /// <summary> 코인 새로고침 받아오기 </summary>
        public void costRefresh(Action act)
        {
            _costRefresh = null;
            _costRefresh = act;
        }
    }
}