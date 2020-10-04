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

        Color _transparent = new Color(0, 0, 0, 0);
        Color _orange = new Color(1f, 0.575f, 0f, 1f);

        Color _dayStandard = new Color(0.8f, 0.8f, 0.8f, 1f);
        Color _dayBlack = new Color(0, 0, 0, 0.5f);

        Color _qstBlack = new Color(0.5f, 0.5f, 0.5f, 1f);

        #region [interface]
        public void open()
        {
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
        
        }

        void Init()
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

            if (BaseManager.userGameData.TimeRecord - BaseManager.userGameData.GetTimeReward >= DataManager.GetTable<int>(DataTable.quest, ((int)Quest.time).ToString(), QuestValData.val.ToString()))
            { }
            mTmps[(int)eTmp.qstTimeTxt].text = "";
            mImgs[(int)eImg.qstTimeBtn].color = _qstBlack;

        }

        #endregion
    }
}