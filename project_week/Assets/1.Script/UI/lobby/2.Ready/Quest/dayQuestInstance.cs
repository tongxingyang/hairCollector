using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class dayQuestInstance : MonoBehaviour
    {
        [SerializeField] Image _questImg;
        [SerializeField] TextMeshProUGUI _questText;
        [Space]
        [SerializeField] GameObject _reward;
        [SerializeField] Image _rewardImg;
        [SerializeField] TextMeshProUGUI _rewardText;
        [Space]
        [SerializeField] Image _dark;
        [Space]
        [SerializeField] Sprite[] rwds;

        LobbyScene _lobby;
        Action _refresh_CheckQuest;

        Quest _qst;
        currency _cur;
        int _curMount;

        Color _orange = new Color(1f, 0.575f, 0f, 1f);

        public void Init(LobbyScene lobby, Quest qst, Action refresh_CheckQuest)
        {
            _lobby = lobby;
            _refresh_CheckQuest = refresh_CheckQuest;

            questReInit(qst);
        }

        public void questReInit(Quest qst)
        {
            _qst = qst;

            switch (_qst)
            {
                case Quest.day_rein:
                    _questText.text = $"강화 1회!";
                    break;
                case Quest.day_skin:
                    _questText.text = $"{D_skin.GetEntity(((SkinKeyList)BaseManager.userGameData.QuestSkin).ToString()).f_skinname}으로"
                                    + System.Environment.NewLine + "1회 플레이!";
                    _questImg.sprite = DataManager.SkinSprite[(SkinKeyList)BaseManager.userGameData.QuestSkin];
                    break;
                case Quest.day_revive:
                    _questText.text = $"부활 1회!";
                    break;
                case Quest.day_skill_0:
                    _questText.text = $"{D_skill.GetEntity(BaseManager.userGameData.QuestSkill(0).ToString()).f_skill_name} 습득!";
                    _questImg.sprite = DataManager.Skillicon[BaseManager.userGameData.QuestSkill(0)];
                    break;
                case Quest.day_skill_1:
                    _questText.text = $"{D_skill.GetEntity(BaseManager.userGameData.QuestSkill(1).ToString()).f_skill_name} 습득!";
                    _questImg.sprite = DataManager.Skillicon[BaseManager.userGameData.QuestSkill(1)];
                    break;
                case Quest.day_skill_2:
                    _questText.text = $"{D_skill.GetEntity(BaseManager.userGameData.QuestSkill(2).ToString()).f_skill_name} 습득!";
                    _questImg.sprite = DataManager.Skillicon[BaseManager.userGameData.QuestSkill(2)];
                    break;
            }
            _reward.SetActive(true);

            _cur = EnumHelper.StringToEnum<currency>(D_quest.GetEntity(_qst.ToString()).f_currency);
            _rewardImg.sprite = rwds[(int)_cur];

            _curMount = D_quest.GetEntity(_qst.ToString()).f_reward;
            _rewardText.text = $"x{_curMount}";
        }

        public bool progress()
        {
            bool chk = false;
            int day = (AuthManager.instance.networkCheck()) ? (int)BaseManager.userGameData.DayQuest[(int)_qst] : -1;

            _dark.color = new Color(0, 0, 0, 0f);
            if (day == 0) // 새거 미달성
            {
                _reward.SetActive(true);
                _reward.GetComponent<Image>().color = Color.white;
            }
            else if (day == 1) // 달성
            {
                _reward.SetActive(true);
                _reward.GetComponent<Image>().color = _orange;
                chk = true;
            }
            else if (day >= 2) // 보상 완료
            {
                _dark.color = new Color(0, 0, 0, 0.5f);
                _reward.SetActive(false);
            }

            return chk;
        }

        /// <summary> [button] 일일퀘스트 클리어 </summary>
        public void clearDayQuest()
        {
            if (BaseManager.userGameData.DayQuest[(int)_qst] == 1)
            {
                BaseManager.userGameData.DayQuest[(int)_qst] = 2;

                _questText.text = "완 료";
                // mImgs[(int)eImg.qstReinTxt + i].raycastTarget = false;
                //mImgs[(int)eImg.qstReinTxt + i].color = _transparent;
                //mImgs[(int)eImg.dqr_black + i].color = _dayBlack;

                Debug.Log(_questImg.transform.position);
                WindowManager.instance.Win_coinGenerator.getWealth2Point(_questImg.transform.position, _lobby.CoinTxt.position, _cur, _curMount);

                if (_cur == currency.coin)
                    BaseManager.userGameData.Coin += _curMount;
                else
                    BaseManager.userGameData.Gem += _curMount;

                progress();
                _refresh_CheckQuest?.Invoke();

                AuthManager.instance.SaveDataServer(true);
            }
        }
    }
}