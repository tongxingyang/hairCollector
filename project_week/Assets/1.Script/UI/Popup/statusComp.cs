using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace week
{
    public class statusComp : UIBase, UIInterface
    {
        #region [UIBase]

        enum eTmp
        {
            hpEx,
            hpgenEx,
            defEx,
            attEx,
            coolEx,
            expEx,
            coinEx,
            skinEx,

            hpLevel,
            attLevel,
            defLevel,
            hpgenLevel,
            coolLevel,
            expLevel,
            coinLevel,
            skinLevel,

            prevVal,
            nextVal,
            prevLvl,
            nextLvl,

            ApTxt,
            apPrice,
            apPriceMax,
            upgradePrice
        }

        enum eImage
        {
            upgradeBg,
            statImg,
            upgradeBtn,

            hp,
            att,
            def,
            hpgen,
            cool,
            exp,
            coin,
            skin,

            purchaseBtn,
            purchaseBtnMax
            //applyBtn,
            //cancelBtn
        }

        protected override Enum GetEnumImage() { return new eImage(); }
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

        [SerializeField] Sprite[] statImg;

        int _upgradePrice;

        StatusData _selectStat;
        int _cost;

        Action _costRefresh;        

        #region [초기화]

        // Start is called before the first frame update
        void Awake()
        {
            //upList = new int[(int)StatusData.max];

            statusPopupStart();

            mImgs[(int)eImage.upgradeBg].gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        /// <summary> 능력치 강화창 초기화 </summary>
        public void statusPopupStart()
        {
            mTmps[(int)eTmp.hpEx].text          = string.Format("체력 +{0:0}", BaseManager.userGameData.o_Hp);
            mTmps[(int)eTmp.attEx].text         = string.Format("공격력 +{0:0}", BaseManager.userGameData.o_Att);
            mTmps[(int)eTmp.defEx].text         = string.Format("방어력 +{0:0}", BaseManager.userGameData.o_Def);
            mTmps[(int)eTmp.hpgenEx].text       = string.Format("체력회복 {0:0.00}", BaseManager.userGameData.o_Hpgen);
            mTmps[(int)eTmp.coolEx].text        = string.Format("공격속도 {0:0.00}", BaseManager.userGameData.o_Cool);
            mTmps[(int)eTmp.expEx].text         = string.Format("경험치획득량 x{0:0.00}", BaseManager.userGameData.o_ExpFactor);
            mTmps[(int)eTmp.coinEx].text        = string.Format("코인추가획득 x{0:0.00}", BaseManager.userGameData.o_CoinFactor);
            mTmps[(int)eTmp.skinEx].text        = string.Format("스킨 강화율 {0:0.0}%", BaseManager.userGameData.SkinEnhance);
        }

        /// <summary> 창 오픈시 </summary>
        public void open()
        {
            ApTxtRefresh();
            apMaxRefresh();

            apPurchaseBtnRefresh();

            statusPanelRefresh();

            gameObject.SetActive(true);
        }

        /// <summary> 창 닫기 </summary>
        public void close()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region [새로고침]

        /// <summary> 코인 새로고침 받아오기 </summary>
        public void costRefresh(Action act)
        {
            _costRefresh = null;
            _costRefresh = act;
        }

        /// <summary> ap 새로고침 </summary>
        void ApTxtRefresh()
        {
            mTmps[(int)eTmp.ApTxt].text = $"{BaseManager.userGameData.Ap}";
        }

        /// <summary> 능력치 창 설명 새로고침 </summary>
        void statusPanelRefresh()
        {
            for (int i = 0; i < (int)StatusData.max; i++)
            {
                mTmps[(int)eTmp.hpEx + i].text = getExplain((StatusData)i, false); 
                mTmps[(int)eTmp.hpLevel + i].text = BaseManager.userGameData.StatusLevel[i].ToString();
            }
        }

        /// <summary> 업그레이드 창 새로고침 </summary>
        void upgradePanelRefresh()
        { 
            int stat = (int)_selectStat;
            _upgradePrice = (int)(BaseManager.userGameData.StatusLevel[stat] / 5) + 1;

            bool bl = (BaseManager.userGameData.Ap >= _upgradePrice);
            mTmps[(int)eTmp.upgradePrice].text = _upgradePrice.ToString();

            mImgs[(int)eImage.statImg].sprite = statImg[stat];
            mTmps[(int)eTmp.prevVal].text = getExplain(_selectStat, false);
            mTmps[(int)eTmp.nextVal].text = getExplain(_selectStat ,true);
            mTmps[(int)eTmp.prevLvl].text = $"Lv.{BaseManager.userGameData.StatusLevel[stat]}";
            mTmps[(int)eTmp.nextLvl].text = $"Lv.{BaseManager.userGameData.StatusLevel[stat] + 1}";

            bool chk = BaseManager.userGameData.Ap >= _upgradePrice;
            mImgs[(int)eImage.upgradeBtn].color = (chk) ? new Color(0.78f, 0.13f, 0.3f) : Color.gray;
        }

        /// <summary> 구매버튼 새로고침 </summary>
        void apPurchaseBtnRefresh()
        {
            bool bl = BaseManager.userGameData.Coin >= gameValues._apPrice;

            mImgs[(int)eImage.purchaseBtn].raycastTarget = bl;
            mImgs[(int)eImage.purchaseBtn].color = (bl) ? Color.white : Color.grey;
            mImgs[(int)eImage.purchaseBtnMax].raycastTarget = bl;
            mImgs[(int)eImage.purchaseBtnMax].color = (bl) ? Color.white : Color.grey;
        }

        /// <summary> ap최대치 가격 새로고침 </summary>
        void apMaxRefresh()
        {
            int max = BaseManager.userGameData.Coin / gameValues._apPrice;
            mTmps[(int)eTmp.apPriceMax].text = $"{gameValues._apPrice * max}";
        }

        #endregion

        //string getExplain()
        //{
        //    float num;

        //    bool next = (upList[_selectStat] > 0);

        //    switch (_selectStat)
        //    {
        //        case 0:
        //            num = (next) ? DataManager.GetTable<int>(DataTable.status, "addition", StatusData.hp.ToString()) * upList[_selectStat] : 0;
        //            return string.Format("체력 +{0:0}", BaseManager.userGameData.Hp + num);
        //        case 1:
        //            num = (next) ? DataManager.GetTable<float>(DataTable.status, "addition", StatusData.att.ToString()) * upList[_selectStat] : 0;
        //            return string.Format("공격력 +{0:0}", BaseManager.userGameData.AttFactor + num);                
        //        case 2:
        //            num = (next) ? DataManager.GetTable<int>(DataTable.status, "addition", StatusData.def.ToString()) * upList[_selectStat] : 0;
        //            return string.Format("방어력 +{0:0}", BaseManager.userGameData.Def + num);
        //        case 3:
        //            num = (next) ? DataManager.GetTable<float>(DataTable.status, "addition", StatusData.hpgen.ToString()) * upList[_selectStat] : 0;
        //            return string.Format("체력회복 {0:0.00}", BaseManager.userGameData.Hpgen + num);
        //        case 4:
        //            num = (next) ? Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.cool.ToString()), upList[_selectStat])  : 1;
        //            return string.Format("공격속도 {0:0.00}", BaseManager.userGameData.Cool * num);
        //        case 5:
        //            num = (next) ? Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.exp.ToString()), upList[_selectStat]) : 1;
        //            return string.Format("경험치획득량 x{0:0.00}", BaseManager.userGameData.ExpFactor * num);
        //        case 6:
        //            num = (next) ? Mathf.Pow(DataManager.GetTable<float>(DataTable.status, "addition", StatusData.coin.ToString()), upList[_selectStat]) : 1;
        //            return string.Format("코인추가획득 x{0:0.00}", BaseManager.userGameData.CoinFactor * num);
        //        case 7:
        //            num = (next) ? DataManager.GetTable<float>(DataTable.status, "addition", StatusData.skin.ToString()) * upList[_selectStat] : 0;
        //            return string.Format("스킨 강화율 {0:0.0}%", BaseManager.userGameData.SkinEnhance + num);
        //        default:
        //            Debug.LogError($"잘못된 능력치 : {_selectStat}");
        //            return null;
        //    }
        //}

        #region [능력치 업그레이드 창]

        /// <summary> 능력치 창 오픈 </summary>
        public void openUpgradePanel(int num)
        {
            _selectStat = (StatusData)num;

            upgradePanelRefresh();

            mImgs[(int)eImage.upgradeBg].gameObject.SetActive(true);
        }
        
        /// <summary> 선택된 능력치의 설명가져오기 </summary>
        string getExplain(StatusData stat, bool next = false)
        {
            float num;

            switch (stat)
            {
                case StatusData.hp:
                    num = (next) ? DataManager.GetTable<int>(DataTable.status, "addition", StatusData.hp.ToString()) : 0;
                    return string.Format("체력 +{0:0}", BaseManager.userGameData.o_Hp + num);
                case StatusData.att:
                    num = (next) ? DataManager.GetTable<float>(DataTable.status, "addition", StatusData.att.ToString()) : 0;
                    return string.Format("공격력 +{0:0}", BaseManager.userGameData.o_Att + num);
                case StatusData.def:
                    num = (next) ? DataManager.GetTable<int>(DataTable.status, "addition", StatusData.def.ToString()) : 0;
                    return string.Format("방어력 +{0:0}", BaseManager.userGameData.o_Def + num);
                case StatusData.hpgen:
                    num = (next) ? DataManager.GetTable<float>(DataTable.status, "addition", StatusData.hpgen.ToString()) : 0;
                    return string.Format("체력회복 {0:0.00}", BaseManager.userGameData.o_Hpgen + num);
                case StatusData.cool:
                    num = (next) ? DataManager.GetTable<float>(DataTable.status, "addition", StatusData.cool.ToString()) : 1;
                    return string.Format("공격속도 {0:0.00}", BaseManager.userGameData.o_Cool * num);
                case StatusData.exp:
                    num = (next) ? DataManager.GetTable<float>(DataTable.status, "addition", StatusData.exp.ToString()) : 1;
                    return string.Format("경험치획득량 x{0:0.00}", BaseManager.userGameData.o_ExpFactor * num);
                case StatusData.coin:
                    num = (next) ? DataManager.GetTable<float>(DataTable.status, "addition", StatusData.coin.ToString()) : 1;
                    return string.Format("코인추가획득 x{0:0.00}", BaseManager.userGameData.o_CoinFactor * num);
                case StatusData.skin:
                    num = (next) ? DataManager.GetTable<float>(DataTable.status, "addition", StatusData.skin.ToString()) : 0;
                    return string.Format("스킨 강화율 {0:0.0}%", BaseManager.userGameData.SkinEnhance + num);
                default:
                    Debug.LogError($"잘못된 능력치 : {_selectStat}");
                    return null;
            }
        }

        /// <summary> 업그레이드 버튼 눌름 </summary>
        public void pressUpgradeBtn()
        {
            if (BaseManager.userGameData.Ap >= _upgradePrice)
            {
                BaseManager.userGameData.Ap -= _upgradePrice;

                BaseManager.userGameData.statusLevelUp(_selectStat);

                BaseManager.userGameData.saveUserEntity();

                BaseManager.userGameData.ReinRecord += 1;

                ApTxtRefresh();
                statusPanelRefresh();
                upgradePanelRefresh();
            }
            else
            {
                Debug.Log("ap 모자름");
            }
        }

        /// <summary> 업그레이드 창 닫기 </summary>
        public void closeUpgradePanel()
        {
            mImgs[(int)eImage.upgradeBg].gameObject.SetActive(false);
        }

        #endregion

        /// <summary> ap 구매 </summary>
        public void purchaseAp()
        {
            BaseManager.userGameData.Coin -= gameValues._apPrice;
            _costRefresh();

            BaseManager.userGameData.Ap++;
            mTmps[(int)eTmp.ApTxt].text = $"{BaseManager.userGameData.Ap}";
            ApTxtRefresh();

            apPurchaseBtnRefresh();
            apMaxRefresh();

            BaseManager.userGameData.saveUserEntity();
        }

        /// <summary> ap 구매 max </summary>
        public void purchaseApMax()
        {
            int max = BaseManager.userGameData.Coin / gameValues._apPrice;
            BaseManager.userGameData.Coin -= gameValues._apPrice * max;
            _costRefresh();

            BaseManager.userGameData.Ap += max;
            mTmps[(int)eTmp.ApTxt].text = $"{BaseManager.userGameData.Ap}";
            ApTxtRefresh();

            apPurchaseBtnRefresh();
            apMaxRefresh();

            BaseManager.userGameData.saveUserEntity();
        }
    }
}