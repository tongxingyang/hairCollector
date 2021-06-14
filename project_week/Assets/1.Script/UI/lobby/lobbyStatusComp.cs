using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace week
{
    public class lobbyStatusComp : MonoBehaviour
    {
        #region [ status value ] ===================================

        [Header("STATUS")]
        [SerializeField] Button[] _purchaseBtn;
        [SerializeField] TextMeshProUGUI[] _price;
        [Header("Buttons")]
        [SerializeField] statusButton[] _btns;
        [Space]
        [SerializeField] Transform _icons;
        [Space]
        [SerializeField] Material _material;

        int _upgrade1Price;
        int _upgrade3Price;

        #endregion

        Action _costRefresh;
        Action _questRefresh;
        Action _skinRefresh;

        private void Awake()
        {
            _icons.localScale = Vector3.one * (((float)Screen.width / Screen.height == 0.5625f) ? 0.9f : 1f);
        }

        /// <summary> snowman 초기화 </summary>
        public void Init(Action costRefresh, Action questRefresh, Action skinRefresh)
        {
            _costRefresh = costRefresh;
            _questRefresh = questRefresh;
            _skinRefresh = skinRefresh;

            priceChk();
            _purchaseBtn[0].onClick.AddListener(purchase1Stat);
            _purchaseBtn[1].onClick.AddListener(purchase3Stat);

            statusStart();
        }

        #region [ status ]

        /// <summary> 스탯박스 초기화 </summary>
        void statusStart()
        {
            BaseManager.userGameData.applyLevel();

            for (int i = 0; i < _btns.Length; i++)
            {                
                _btns[i].setInit((statusKeyList)i, Instantiate(_material))
                    .setData(BaseManager.userGameData.StatusLevel[i]);
            }
        }

        /// <summary> 스탯 1번 구매 </summary>
        public void purchase1Stat()
        {
            if (_upgrade1Price <= BaseManager.userGameData.Coin)
            {
                BaseManager.userGameData.Coin -= _upgrade1Price;
                int prize = UnityEngine.Random.Range(0, (int)statusKeyList.skin);
                BaseManager.userGameData.StatusLevel[prize]++;
                if (BaseManager.userGameData.DayQuest[(int)Quest.day_rein] == 0)
                {
                    BaseManager.userGameData.DayQuest[(int)Quest.day_rein]++;
                }

                AuthManager.instance.SaveDataServer(false);

                StartCoroutine(getRandomStat(prize, 1));

                priceChk();
                _skinRefresh?.Invoke();
            }
            else // 돈부족
            {
                WindowManager.instance.Win_message.showMessage("돈이 모잘눈~!");
            }
        }

        /// <summary> 스탯 3번 구매 </summary>
        public void purchase3Stat()
        {
            if (_upgrade3Price <= BaseManager.userGameData.Coin)
            {
                BaseManager.userGameData.Coin -= _upgrade3Price;
                int prize = UnityEngine.Random.Range(0, (int)statusKeyList.skin);
                BaseManager.userGameData.StatusLevel[prize] += 3;
                if (BaseManager.userGameData.DayQuest[(int)Quest.day_rein] == 0)
                {
                    BaseManager.userGameData.DayQuest[(int)Quest.day_rein]++;
                }

                AuthManager.instance.SaveDataServer(false);

                StartCoroutine(getRandomStat(prize, 3));

                priceChk();
                _skinRefresh?.Invoke();
            }
            else // 돈부족
            {
                WindowManager.instance.Win_message.showMessage("돈이 모잘눈~!");
            }
        }

        /// <summary> 랜덤 시늉 </summary>
        IEnumerator getRandomStat(int prize, int mount)
        {
            int num = -1;

            WindowManager.instance.openWin(Windows.win_serverLoad);
            WindowManager.instance.Win_serverWait.setButtonClose(() => {
                WindowManager.instance.Win_purchase.setStatusPurchase((statusKeyList)prize, mount);
            } );

            for (int i = 0; i < 15; i++)
            {
                if (num >= 0)
                {
                    _btns[num].glowOn(false);
                }

                num = UnityEngine.Random.Range(0, (int)statusKeyList.skin);
                _btns[num].glowOn(true);

                SoundManager.instance.PlaySFX(SFX.randomStat);
                yield return new WaitForSeconds(0.1f);            
            }

            _btns[num].glowOn(false);
            yield return new WaitForSeconds(0.15f);

            for (int i = 0; i < 3; i++)
            {
                _btns[prize].glowOn(true);
                yield return new WaitForSeconds(0.25f);
                _btns[prize].glowOn(false);
                yield return new WaitForSeconds(0.25f);
            }

            _btns[prize].setData(BaseManager.userGameData.StatusLevel[prize]);

            WindowManager.instance.Win_serverWait.close();
            WindowManager.instance.Win_purchase.setStatusPurchase((statusKeyList)prize, mount);
        }

        /// <summary> 가격설정 </summary>
        public void priceChk()
        {
            // 총 레벨
            int lvl = 0;            
            for (int i = 0; i < BaseManager.userGameData.StatusLevel.Length; i++)
            {
                lvl += BaseManager.userGameData.StatusLevel[i];
            }

            // 1회 가격
            _upgrade1Price = 0;
            _upgrade1Price = 1000 + 100 * lvl;
            _price[0].text = _upgrade1Price.ToString();

            // 3회 가격
            _upgrade3Price = 0;
            for (int i = 0; i < 3; i++)
            {
                _upgrade3Price += 1000 + 100 * (lvl + i);
            }
            _price[1].text = _upgrade3Price.ToString();
        }

        /// <summary> 선택된 능력치의 설명가져오기 </summary>
        string getExplain(statusKeyList stat, bool next = false)
        {
            float num = (next) ? BaseManager.userGameData.getAddit(stat) : 0;

            switch (stat)
            {
                case statusKeyList.hp:
                    return string.Format("체 {0:0}", BaseManager.userGameData.o_Hp + num);
                case statusKeyList.att:
                    return string.Format("공 {0:0}", BaseManager.userGameData.o_Att + num);
                case statusKeyList.def:
                    return string.Format("{0:0.0}% 감소", (BaseManager.userGameData.o_Def + num) * 100);
                case statusKeyList.hpgen:
                    return string.Format("{0:0.0}/초", BaseManager.userGameData.o_Hpgen + num);
                case statusKeyList.cool:
                    return string.Format("{0:0.00}% 감소", (BaseManager.userGameData.o_Cool + num) * 100);
                case statusKeyList.exp:
                    return string.Format("+{0:0.0}%", (BaseManager.userGameData.o_ExpFactor + num) * 100 - 100);
                case statusKeyList.coin:
                    return string.Format("+{0:0.0}%", (BaseManager.userGameData.o_CoinFactor + num) * 100 - 100);
                //case statusKeyList.skin:
                //    return string.Format("{0:0}강", BaseManager.userGameData.SkinEnhance + ((next) ? 1 : 0));
                default:
                    // Debug.LogError($"잘못된 능력치 요청 : {_selectStat}");
                    return null;
            }
        }

        #region [ 새로고침 ]

        /// <summary> [새로고침] 능력치버튼들 정보 </summary>
        void refresh_StatusBtns()
        {
            for (int i = 0; i < _btns.Length; i++)
            {
                _btns[i].setData(BaseManager.userGameData.StatusLevel[i]);
            }
        }

        /// <summary> [새로고침] 능력치창 </summary>
        public void refresh_Status()
        {
            refresh_StatusBtns();

            //refresh_AboutAp();
        }

        #endregion

        #endregion

    }
}