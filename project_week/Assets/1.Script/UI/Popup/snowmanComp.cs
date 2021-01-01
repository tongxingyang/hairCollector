using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace week
{
    public class snowmanComp : UIBase
    {
        #region [ UI base ]
        enum eImage
        {
            statImg,
            upgradeBtn,
            purchaseBtn,
            purchaseBtnMax
        }

        enum eTmp
        {
            prevVal,
            nextVal,
            prevLvl,
            nextLvl,
            upgradePrice,
            nowSkinName,

            apPriceMax
        }

        public TextMeshProUGUI[] mTmps;
        protected Enum GetEnumTmp() { return new eTmp(); }
        protected override Enum GetEnumImage() { return new eImage(); }

        protected override void OtherSetContent()
        {
            if (GetEnumTmp() != null)
            {
                mTmps = SetComponent<TextMeshProUGUI>(GetEnumTmp());
            }
        }

        #endregion

        #region [ snowman value ]

        [Header("SNOW")]
        [SerializeField] TextMeshProUGUI _changeText;

        #endregion

        #region [ status value ]

        [Header("STATUS")]
        [SerializeField] GameObject _statusPanel;
        [SerializeField] GameObject upGradePanel;
        [SerializeField] Sprite[] statImg;
        [SerializeField] statusButton[] _btns;
        [SerializeField] TextMeshProUGUI _apText;

        StatusData _selectStat;
        int _upgradePrice;

        #endregion

        #region [ skin value ]

        [Header("SKIN")]
        [SerializeField] GameObject _skinPanel;
        [SerializeField] Image _snowman;
        [SerializeField] TextMeshProUGUI _hpTxt;
        [SerializeField] TextMeshProUGUI _attTxt;
        [SerializeField] TextMeshProUGUI _defTxt;
        [SerializeField] TextMeshProUGUI _hpgenTxt;
        [SerializeField] TextMeshProUGUI _effTxt;
        [Space]
        [SerializeField] Transform _skinBoxParent;
        [SerializeField] GameObject _skinBox;

        Dictionary<SkinKeyList, skinBox> _skinBoxies;
        SkinKeyList _selectSkin;

        #endregion

        bool _isStat;
        Action _costRefresh;
        Action _skinRefresh;
        Action _refreshExcla;

        #region [ snowman ]

        /// <summary> snowman 초기화 </summary>
        public void Init(Action costRefresh, Action skinRefresh, Action refreshExcla)
        {
            _costRefresh = costRefresh;
            _skinRefresh = skinRefresh;
            _refreshExcla = refreshExcla;

            statusStart();
            skinStart();

            _isStat = false;
            OnClickChangeBtn();
        }

        /// <summary> 스탯~스킨 창 변환 </summary>
        public void OnClickChangeBtn()
        {
            _isStat = !_isStat;
            _statusPanel.SetActive(_isStat);
            _skinPanel.SetActive(!_isStat);

            if (_isStat)
            {
                _changeText.text = "스킨";
            }
            else
            {
                _changeText.text = "능력";
                skinBoxRefresh();
            }
        }

        /// <summary> snowman 능력치 보여주기 (새로고침) </summary>
        void showSnowmanInfo()
        {
            _snowman.sprite = DataManager.SkinSprite[_selectSkin];

            string str;
            float f;

            str = BaseManager.userGameData.o_Hp.ToString();
            f = (BaseManager.userGameData.AddStats[0] > 0) ? (BaseManager.userGameData.AddStats[0] - 1) : 0;
            str += $" (<color=green>+ {Convert.ToInt32(BaseManager.userGameData.o_Hp * f)}</color>)";
            _hpTxt.text = str;

            str = BaseManager.userGameData.o_Att.ToString();
            f = (BaseManager.userGameData.AddStats[1] > 0) ? (BaseManager.userGameData.AddStats[1] - 1) : 0;
            str += $" (<color=green>+ {Convert.ToInt32(BaseManager.userGameData.o_Att * f)}</color>)";
            _attTxt.text = str;

            str = (BaseManager.userGameData.o_Def * 100).ToString("0.00");
            f = (BaseManager.userGameData.AddStats[2] > 0) ? (BaseManager.userGameData.AddStats[2] - 1) : 0;
            str += string.Format("% (<color=green>+ {0:0.00}%</color>)", Convert.ToInt32(BaseManager.userGameData.o_Def * f));
            _defTxt.text = str;

            str = string.Format("{0:0.0}", BaseManager.userGameData.o_Hpgen);
            f = (BaseManager.userGameData.AddStats[3] > 0) ? (BaseManager.userGameData.AddStats[3] - 1) : 0;
            str += string.Format("/초 (<color=green>+ {0:0.0}</color>)", BaseManager.userGameData.o_Hpgen * f);
            _hpgenTxt.text = str;

            _effTxt.text = BaseManager.userGameData.getSkinExplain(_selectSkin);
        }

        #endregion

        #region [ status ]

        /// <summary> 스탯 초기화 </summary>
        void statusStart()
        {
            BaseManager.userGameData.applyLevel();
            // statusBtnRefresh();
            for (int i = 0; i < _btns.Length; i++)
            {
                StatusData sd = (StatusData)i;
                   _btns[i].setInit(getStatName(sd), statImg[i])
                    .setData(BaseManager.userGameData.StatusLevel[i], getExplain(sd), getPrice(sd));
            }

            apPurchaseBtnRefresh();

            upGradePanel.SetActive(false);
        }

        /// <summary> 선택된 능력치의 설명가져오기 </summary>
        string getExplain(StatusData stat, bool next = false)
        {
            float num = (next) ? BaseManager.userGameData.getAddit(stat) : 0;

            switch (stat)
            {
                case StatusData.hp:                    
                    return string.Format("체 {0:0}", BaseManager.userGameData.o_Hp + num);
                case StatusData.att:
                    return string.Format("공 {0:0}", BaseManager.userGameData.o_Att + num);
                case StatusData.def:
                    return string.Format("{0:0.0}% 감소", (BaseManager.userGameData.o_Def + num) * 100);
                case StatusData.hpgen:
                    return string.Format("{0:0.0}/초", BaseManager.userGameData.o_Hpgen + num);
                case StatusData.cool:
                    return string.Format("{0:0.00}% 감소", (BaseManager.userGameData.o_Cool + num) * 100);
                case StatusData.exp:
                    return string.Format("+{0:0.0}%", (BaseManager.userGameData.o_ExpFactor + num) * 100 - 100);
                case StatusData.coin:
                    return string.Format("+{0:0.0}%", (BaseManager.userGameData.o_CoinFactor + num) * 100 - 100);
                case StatusData.skin:
                    return string.Format("{0:0}강", BaseManager.userGameData.SkinEnhance + ((next) ? 1 : 0));
                default:
                    Debug.LogError($"잘못된 능력치 요청 : {_selectStat}");
                    return null;
            }
        }

        string getStatName(StatusData stat)
        {
            switch (stat)
            {
                case StatusData.hp:
                    return "체력";
                case StatusData.att:
                    return "공격력";
                case StatusData.def:
                    return "방어력";
                case StatusData.hpgen:
                    return "초당 회복량";
                case StatusData.cool:
                    return "공격속도";
                case StatusData.exp:
                    return "경험치";
                case StatusData.coin:
                    return "코인";
                case StatusData.skin:
                    return "스킨 강화율";
                default:
                    Debug.LogError($"잘못된 능력치 요청 : {_selectStat}");
                    return null;
            }
        }

        /// <summary> 가격 가져오기 </summary>
        int getPrice(StatusData status)
        {
            int stat = (int)status;
            int div = DataManager.GetTable<int>(DataTable.status, statusKeyList.costTerm.ToString(), status.ToString());
            int rate = DataManager.GetTable<int>(DataTable.status, statusKeyList.cost.ToString(), status.ToString());

            return ((int)(BaseManager.userGameData.StatusLevel[stat] / div) + 1) * rate;
        }

        /// <summary> 능력치버튼 정보 새로고침 </summary>
        void statusBtnRefresh()
        {
            for (int i = 0; i < _btns.Length; i++)
            {
                _btns[i].setData(BaseManager.userGameData.StatusLevel[i], getExplain((StatusData)i), getPrice((StatusData)i));
            }
        }

        #region [ AP ]

        /// <summary> ap 구매 </summary>
        public void purchaseAp()
        {
            if (BaseManager.userGameData.Coin < gameValues._apPrice)
            {
                return;
            }

            BaseManager.userGameData.Coin -= gameValues._apPrice;

            BaseManager.userGameData.Ap++;
            _apText.text = $"{BaseManager.userGameData.Ap}";

            apPurchaseBtnRefresh();

            _costRefresh?.Invoke();
            AuthManager.instance.SaveDataServer();
        }

        /// <summary> ap 구매 max </summary>
        public void purchaseApMax()
        {
            if (BaseManager.userGameData.Coin < gameValues._apPrice)
            {
                return;
            }

            int max = BaseManager.userGameData.Coin / gameValues._apPrice;
            BaseManager.userGameData.Coin -= gameValues._apPrice * max;

            BaseManager.userGameData.Ap += max;
            _apText.text = $"{BaseManager.userGameData.Ap}";

            apPurchaseBtnRefresh();

            _costRefresh?.Invoke();
            AuthManager.instance.SaveDataServer();
        }

        /// <summary> ap 새로고침 </summary>
        void ApTxtRefresh()
        {
            _apText.text = $"{BaseManager.userGameData.Ap}";
        }

        /// <summary> ap 구매버튼2개 새로고침 </summary>
        public void apPurchaseBtnRefresh()
        {
            ApTxtRefresh();

            // max 버튼 새로고침
            int max = BaseManager.userGameData.Coin / gameValues._apPrice;
            mTmps[(int)eTmp.apPriceMax].text = $"{gameValues._apPrice * max}";

            purchaseBtnRefresh();
        }

        public void purchaseBtnRefresh()
        {
            bool bl = BaseManager.userGameData.Coin >= gameValues._apPrice;

            mImgs[(int)eImage.purchaseBtn].raycastTarget = bl;
            mImgs[(int)eImage.purchaseBtn].color = (bl) ? Color.white : Color.grey;
            mImgs[(int)eImage.purchaseBtnMax].raycastTarget = bl;
            mImgs[(int)eImage.purchaseBtnMax].color = (bl) ? Color.white : Color.grey;
        }

        #endregion

        #region [능력치 업그레이드 창]

        /// <summary> 능력치 창 오픈 </summary>
        public void openUpgradePanel(int num)
        {
            _selectStat = (StatusData)num;

            int max = DataManager.GetTable<int>(DataTable.status, statusKeyList.max.ToString(), _selectStat.ToString());
            if (max == -1 || BaseManager.userGameData.StatusLevel[num] < max)
            {
                upgradePanelRefresh();

                upGradePanel.SetActive(true);
            }
            else
            {
                WindowManager.instance.Win_message.showMessage("강화 한도에 도달했습니다.");
            }            
        }

        /// <summary> 업그레이드 버튼 눌름 </summary>
        public void pressUpgradeBtn()
        {
            if (BaseManager.userGameData.Ap >= _upgradePrice)
            {
                BaseManager.userGameData.Ap -= _upgradePrice;

                BaseManager.userGameData.statusLevelUp(_selectStat);                               

                BaseManager.userGameData.ReinRecord += 1;
                if (BaseManager.userGameData.DayQuestRein == 0)
                    BaseManager.userGameData.DayQuestRein++;

                AuthManager.instance.SaveDataServer();

                ApTxtRefresh();
                statusBtnRefresh();
                upgradePanelRefresh();
                _refreshExcla?.Invoke();
                showSnowmanInfo();

                int max = DataManager.GetTable<int>(DataTable.status, statusKeyList.max.ToString(), _selectStat.ToString());
                if (max != -1 && BaseManager.userGameData.StatusLevel[(int)_selectStat] >= max)
                {
                    closeUpgradePanel();
                    WindowManager.instance.Win_message.showMessage("강화 한도에 도달했습니다.");
                }
            }
            else
            {
                Debug.Log("AP가 모자릅니다.");
            }
        }

        /// <summary> 업그레이드 창 닫기 </summary>
        public void closeUpgradePanel()
        {
            AuthManager.instance.SaveDataServer();

            upGradePanel.SetActive(false);
        }

        /// <summary> 업그레이드 창 새로고침 </summary>
        void upgradePanelRefresh()
        {
            int stat = (int)_selectStat;
            _upgradePrice = getPrice(_selectStat);

            bool bl = (BaseManager.userGameData.Ap >= _upgradePrice);
            mTmps[(int)eTmp.upgradePrice].text = _upgradePrice.ToString();

            mImgs[(int)eImage.statImg].sprite = statImg[stat];
            mTmps[(int)eTmp.prevVal].text = getExplain(_selectStat, false);
            mTmps[(int)eTmp.nextVal].text = getExplain(_selectStat, true);
            mTmps[(int)eTmp.prevLvl].text = $"Lv.{BaseManager.userGameData.StatusLevel[stat]}";
            mTmps[(int)eTmp.nextLvl].text = $"Lv.{BaseManager.userGameData.StatusLevel[stat] + 1}";

            bool chk = BaseManager.userGameData.Ap >= _upgradePrice;
            mImgs[(int)eImage.upgradeBtn].color = (chk) ? new Color(0.78f, 0.13f, 0.3f) : Color.gray;
        }

        #endregion

        #endregion

        #region [ skin ]

        /// <summary> skin 초기화 </summary>
        public void skinStart()
        {
            _selectSkin = (SkinKeyList)BaseManager.userGameData.Skin;
            showSnowmanInfo();

            _skinBoxies = new Dictionary<SkinKeyList, skinBox>();
            for (SkinKeyList i = SkinKeyList.snowman; i < SkinKeyList.max; i++)
            {
                if (DataManager.GetTable<bool>(DataTable.skin, i.ToString(), SkinValData.enable.ToString()))
                {
                    skinBox sb = Instantiate(_skinBox).GetComponent<skinBox>();

                    sb.transform.SetParent(_skinBoxParent);
                    sb.transform.localScale = Vector3.one;
                    sb.setSkinBox((SkinKeyList)i);
                    sb.setAction(changeSkin, _costRefresh, _skinRefresh);

                    _skinBoxies.Add(i, sb);
                }
            }

            changeSkin(_selectSkin);
        }

        void changeSkin(SkinKeyList newSkin)
        {
            _selectSkin = newSkin;
            mTmps[(int)eTmp.nowSkinName].text = DataManager.GetTable<string>(DataTable.skin, newSkin.ToString(), SkinValData.skinname.ToString());
            BaseManager.userGameData.Skin = newSkin;
            BaseManager.userGameData.applySkin();
            showSnowmanInfo();

            skinBoxRefresh();
        }

        void skinBoxRefresh()
        {
            foreach (skinBox sb in _skinBoxies.Values)
            {
                sb.chkState();
            }
        }

        #endregion
    }
}