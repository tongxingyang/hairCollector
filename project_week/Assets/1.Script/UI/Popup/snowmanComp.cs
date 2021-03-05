using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace week
{
    public class snowmanComp : MonoBehaviour
    {       
        #region [ snowman value ] ===================================

        [Header("SNOW")]
        [SerializeField] TextMeshProUGUI _changeText;
        [SerializeField] TextMeshProUGUI _nowSkinName;

        #endregion

        #region [ status value ] ===================================

        [Header("STATUS")]
        [SerializeField] GameObject _statusPanel;

        [SerializeField] apMountPopup _setApMountPanel;

        [SerializeField] Sprite[] statImg;
        [SerializeField] statusButton[] _btns;
        [SerializeField] TextMeshProUGUI _apText;

        [SerializeField] TextMeshProUGUI _reservApTxt;
        [SerializeField] TextMeshProUGUI _reservCoinTxt;

        int[] _virtualUsedAp;
        int[] _addStatusLevel;

        public int _reservAp = 1;

        statusKeyList _selectStat;
        int _upgradePrice;

        #endregion

        #region [ skin value ] ===================================

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
            refresh_StatusInfo();

            _effTxt.text = BaseManager.userGameData.getSkinExplain(_selectSkin);
        }

        void refresh_StatusInfo()
        {
            BaseManager.userGameData.applyLevel();

            string str = BaseManager.userGameData.o_Hp.ToString();
            float f = (BaseManager.userGameData.AddStats[0] > 0) ? (BaseManager.userGameData.AddStats[0] - 1) : 0;
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
        }

        #endregion

        #region [ status ]

        /// <summary> 스탯 초기화 </summary>
        void statusStart()
        {
            _reservAp = 1;
            _addStatusLevel = new int[8];
            _virtualUsedAp = new int[8];

            BaseManager.userGameData.applyLevel();

            for (int i = 0; i < _btns.Length; i++)
            {
                statusKeyList sd = (statusKeyList)i;
                   _btns[i].setInit(sd, statImg[i])
                    .setData(BaseManager.userGameData.StatusLevel[i], ref _addStatusLevel[i]);
            }

            refresh_AboutAp();

            _setApMountPanel.Init(this);
        }

        /// <summary> 가상능력치 증가 </summary>
        public void upgradePreview(int num)
        {
            Debug.Log(num);
            _selectStat = (statusKeyList)num;

            if (BaseManager.userGameData.StatusLevel[num] < 100)
            {
                if (cal_UsedAp() < BaseManager.userGameData.Ap)
                {
                    _addStatusLevel[num]++;
                }
                refresh_Status();
            }
            else
            {
                WindowManager.instance.Win_message.showMessage("강화 한도에 도달했습니다.");
            }
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
                case statusKeyList.skin:
                    return string.Format("{0:0}강", BaseManager.userGameData.SkinEnhance + ((next) ? 1 : 0));
                default:
                    Debug.LogError($"잘못된 능력치 요청 : {_selectStat}");
                    return null;
            }
        }

        /// <summary> 적용 </summary>
        public void confirmStatus()
        { 
            for(int i = 0; i < (int)statusKeyList.max; i++)
            {
                BaseManager.userGameData.StatusLevel[i] += _addStatusLevel[i];
            }
            BaseManager.userGameData.Ap -= cal_UsedAp();

            for (int i = 0; i < (int)statusKeyList.max; i++)
            {
                _addStatusLevel[i] = 0;
                _virtualUsedAp[i] = 0;
            }

            refresh_Status();
            refresh_StatusInfo();

            AuthManager.instance.SaveDataServer(true);
        }

        /// <summary> 취소 </summary>
        public void cancel_Upgrade()
        {
            for (int i = 0; i < (int)statusKeyList.max; i++)
            {
                _addStatusLevel[i] = 0;
                _virtualUsedAp[i] = 0;
            }

            refresh_Status();
        }

        /// <summary> 강화 예상 가격 계산 </summary>
        int cal_UsedAp()
        {
            int cal = 0;
            for (int i = 0; i < (int)statusKeyList.max; i++)
            {
                cal += _virtualUsedAp[i]; ;
            }
            return cal;
        }

        #region [ AP ]

        /// <summary> ap 구매 </summary>
        public void purchase_Ap()
        {
            if (BaseManager.userGameData.Coin < gameValues._apPrice)
            {
                return;
            }

            if (BaseManager.userGameData.Coin >= _reservAp * gameValues._apPrice)
            {
                BaseManager.userGameData.Coin -= _reservAp * gameValues._apPrice;
                BaseManager.userGameData.Ap += _reservAp;

                refresh_AboutAp();
                _costRefresh?.Invoke();

                AuthManager.instance.SaveDataServer(true);
            }
            else
            {
                Debug.LogError("Error : 구매불가");
                return;
            }
        }

        #endregion

        #region [ap 구매 설정창]

        /// <summary> ap 구매 설정창 오픈 </summary>
        public void open_setApMountPanel()
        {
            _setApMountPanel.setOpen();
        }

        ///// <summary> 업그레이드 버튼 눌름 </summary>
        //public void pressUpgradeBtn()
        //{
        //    if (BaseManager.userGameData.Ap >= _upgradePrice)
        //    {
        //        BaseManager.userGameData.Ap -= _upgradePrice;

        //        BaseManager.userGameData.statusLevelUp(_selectStat);                               

        //        BaseManager.userGameData.ReinRecord += 1;
        //        if (BaseManager.userGameData.DayQuestRein == 0)
        //            BaseManager.userGameData.DayQuestRein++;

        //        AuthManager.instance.SaveDataServer(true);

        //        refresh_ApTxt();
        //        refresh_StatusBtns();
        //        upgradePanelRefresh();
        //        _refreshExcla?.Invoke();
        //        showSnowmanInfo();

        //        int max = DataManager.GetTable<int>(DataTable.status, statusKeyList.max.ToString(), _selectStat.ToString());
        //        if (max != -1 && BaseManager.userGameData.StatusLevel[(int)_selectStat] >= max)
        //        {
        //            close_setApMountPanel();
        //            WindowManager.instance.Win_message.showMessage("강화 한도에 도달했습니다.");
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("AP가 모자릅니다.");
        //    }
        //}

        ///// <summary> 업그레이드 창 새로고침 </summary>
        //void upgradePanelRefresh()
        //{
        //    //int stat = (int)_selectStat;
        //    //_upgradePrice = getPrice(_selectStat);

        //    //bool bl = (BaseManager.userGameData.Ap >= _upgradePrice);
        //    //mTmps[(int)eTmp.upgradePrice].text = _upgradePrice.ToString();

        //    //mImgs[(int)eImage.statImg].sprite = statImg[stat];
        //    //mTmps[(int)eTmp.prevVal].text = getExplain(_selectStat, false);
        //    //mTmps[(int)eTmp.nextVal].text = getExplain(_selectStat, true);
        //    //mTmps[(int)eTmp.prevLvl].text = $"Lv.{BaseManager.userGameData.StatusLevel[stat]}";
        //    //mTmps[(int)eTmp.nextLvl].text = $"Lv.{BaseManager.userGameData.StatusLevel[stat] + 1}";

        //    //bool chk = BaseManager.userGameData.Ap >= _upgradePrice;
        //    //mImgs[(int)eImage.upgradeBtn].color = (chk) ? new Color(0.78f, 0.13f, 0.3f) : Color.gray;
        //}

        #endregion

        #region [ 새로고침 ]

        /// <summary> [새로고침] 능력치버튼들 정보 </summary>
        void refresh_StatusBtns()
        {
            for (int i = 0; i < _btns.Length; i++)
            {
                _virtualUsedAp[i] = _btns[i].setData(BaseManager.userGameData.StatusLevel[i], ref _addStatusLevel[i]);
            }
        }

        /// <summary> [새로고침] 능력치창 </summary>
        public void refresh_Status()
        {
            refresh_StatusBtns();

            refresh_AboutAp();
        }

        /// <summary> [ 새로고침 ] ap </summary>
        void refresh_ApTxt()
        {
            _apText.text = $"{BaseManager.userGameData.Ap - cal_UsedAp()}";
        }

        /// <summary> [ 새로고침 ] ap 관련 </summary>
        public void refresh_AboutAp()
        {
            refresh_ApTxt();

            bool bl = BaseManager.userGameData.Coin >= gameValues._apPrice;

            _reservApTxt.text = _reservAp.ToString();
            _reservCoinTxt.text = (_reservAp * gameValues._apPrice).ToString();

            if (_reservAp > BaseManager.userGameData.Coin / gameValues._apPrice)
            {
                _reservAp = Convert.ToInt32(BaseManager.userGameData.Coin / gameValues._apPrice);
            }
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
            _nowSkinName.text = DataManager.GetTable<string>(DataTable.skin, newSkin.ToString(), SkinValData.skinname.ToString());
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