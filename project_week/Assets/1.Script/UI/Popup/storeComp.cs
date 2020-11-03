using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class storeComp : UIBase, UIInterface
    {
        #region [UIBase]
        enum eTr
        {
            s_gem,
            m_gem,
            l_gem,
            s_ap,
            m_ap,
            l_ap,
            s_coin,
            m_coin,
            l_coin
        }

        protected override Enum GetEnumTransform() { return new eTr(); }

        #endregion

        [Header("present")]
        [SerializeField] Image _ad;
        [SerializeField] Image _10p;
        [SerializeField] Image _start;
        [SerializeField] Image _skin;

        [SerializeField] GameObject _soldoutAd;
        [SerializeField] GameObject _soldout10p;
        [SerializeField] GameObject _soldoutStart;
        [SerializeField] GameObject _soldoutSkin;

        [Space]
        [SerializeField] LobbyScene _lobby;

        //Action _costRefresh;

        #region [ 특별 상품 ]

        /// <summary> 광고 제거 </summary>
        public void getRemoveAd()
        {
            Debug.Log("광고 제거");
            BaseManager.userGameData.AddMulCoinList = mulCoinChkList.removeAD;
            BaseManager.userGameData.RemoveAd = true;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.removead.ToString(), productValData.image.ToString()));
            WindowManager.instance.Win_celebrate.whenPurchase();

            _ad.raycastTarget = false;
            AuthManager.instance.AllSaveUserEntity();
            _soldoutAd.SetActive(true);
        }

        /// <summary> 추가보너스 </summary>
        public void getAdd10per()
        {
            Debug.Log("코인 추가 10퍼 겟또다제");
            BaseManager.userGameData.AddMulCoinList = mulCoinChkList.mul_1st_10p;
            BaseManager.userGameData.MulCoin = true;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.bonus.ToString(), productValData.image.ToString()));
            WindowManager.instance.Win_celebrate.whenPurchase();

            _10p.raycastTarget = false;
            AuthManager.instance.AllSaveUserEntity();
            _soldout10p.SetActive(true);
        }

        /// <summary> 추가보너스 정보 </summary>
        public void addPerInfo()
        {
            string str = "적용 범위 : 모험코인" + System.Environment.NewLine + "골드 및 AP 구매";
            WindowManager.instance.showActMessage(str, () => { });
        }

        /// <summary> 스타터팩 </summary>
        public void getStartPack()
        {
            Debug.Log("스타터팩 구매 완료"); 
            BaseManager.userGameData.AddMulCoinList = mulCoinChkList.removeAD;
            BaseManager.userGameData.RemoveAd = true;
            BaseManager.userGameData.StartPack = true;

            bool result = BaseManager.userGameData.RemoveAd;
            if (result == false)
            {
                getRemoveAd();
            }

            int g, a, c;

            g = DataManager.GetTable<int>(DataTable.product, productKeyList.startpack.ToString(), productValData.gem.ToString());
            a = DataManager.GetTable<int>(DataTable.product, productKeyList.startpack.ToString(), productValData.ap.ToString());
            c = DataManager.GetTable<int>(DataTable.product, productKeyList.startpack.ToString(), productValData.coin.ToString());

            if (result)
            {
                g += DataManager.GetTable<int>(DataTable.product, productKeyList.startpack.ToString(), productValData.addgem.ToString());
                a += DataManager.GetTable<int>(DataTable.product, productKeyList.startpack.ToString(), productValData.addap.ToString());
                c += DataManager.GetTable<int>(DataTable.product, productKeyList.startpack.ToString(), productValData.addcoin.ToString());
            }

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.startpack.ToString(), productValData.image.ToString()));
            WindowManager.instance.Win_celebrate.whenPurchase();

            WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.CoinTxt.position, currency.coin, c, 1);
            WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.gem, g, 2);
            WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.CoinTxt.position, currency.ap, a, 3);
            //_costRefresh();

            _start.raycastTarget = false;
            AuthManager.instance.AllSaveUserEntity();
            _soldoutStart.SetActive(true);
        }

        /// <summary> 스킨팩 </summary>
        public void getSkinPack()
        {
            Debug.Log("스킨팩 구매 완료");
            BaseManager.userGameData.SkinPack = true;

            SkinKeyList skl = SkinKeyList.icecreamman;

            bool result = (BaseManager.userGameData.HasSkin & (1 << (int)skl)) > 0;
            if (result == false)
            {
                BaseManager.userGameData.HasSkin |= (1 << (int)skl);
            }

            int i = 30000;
            BaseManager.userGameData.Coin += i;
            i = 300 + ((result) ? 500 : 0);
            BaseManager.userGameData.Gem += i;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.skinpack.ToString(), productValData.image.ToString()));
            WindowManager.instance.Win_celebrate.whenPurchase();

            WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.CoinTxt.position, currency.coin, 30000, 1);
            WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.gem, 300, 2);
            //_costRefresh();

            _skin.raycastTarget = false;
            AuthManager.instance.AllSaveUserEntity();
            _soldoutSkin.SetActive(true);
        }

        public void chkPresent()
        {
            WindowManager.instance.showMessage("해당 상품은 더 이상 구매할 수 없습니눈.");
        }

        #endregion

        #region [ 보석 ]

        public void getSmallGem()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.s_gem.ToString(), productValData.gem.ToString());
            Debug.Log($"{i}보석 겟또다제");
            BaseManager.userGameData.Gem += i;

            WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.s_gem].position, _lobby.GemTxt.position, currency.gem, i, 0, 10);

            AuthManager.instance.AllSaveUserEntity();
        }

        public void getMiddleGem()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.m_gem.ToString(), productValData.gem.ToString());
            Debug.Log($"{i}보석 겟또다제");
            BaseManager.userGameData.Gem += i;

            WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.m_gem].position, _lobby.GemTxt.position, currency.gem, i, 0, 15);

            AuthManager.instance.AllSaveUserEntity();
        }

        public void getLargeGem()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.l_gem.ToString(), productValData.gem.ToString());
            Debug.Log($"{i}보석 겟또다제");
            BaseManager.userGameData.Gem += i;

            WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.l_gem].position, _lobby.GemTxt.position, currency.gem, i, 0, 22);

            AuthManager.instance.AllSaveUserEntity();
        }

        #endregion

        #region [ AP ]

        public void getSmallAp()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.s_ap.ToString(), productValData.ap.ToString());
            Debug.Log($"{i} AP 겟또다제");
            BaseManager.userGameData.Ap += i;

            WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.s_ap].position, _lobby.CoinTxt.position, currency.ap, i, 0, 10);
            
            AuthManager.instance.AllSaveUserEntity();
        }

        public void getMiddleAp()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.m_ap.ToString(), productValData.ap.ToString());
            Debug.Log($"{i} AP 겟또다제");
            BaseManager.userGameData.Ap += i;

            WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.m_ap].position, _lobby.CoinTxt.position, currency.ap, i, 0, 15);

            AuthManager.instance.AllSaveUserEntity();
        }

        public void getLargeAp()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.l_ap.ToString(), productValData.ap.ToString());
            Debug.Log($"{i} AP 겟또다제");
            BaseManager.userGameData.Ap += i;

            WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.l_ap].position, _lobby.CoinTxt.position, currency.ap, i, 0, 22);

            AuthManager.instance.AllSaveUserEntity();
        }

        #endregion

        #region [ 코인 ]

        public void getSmallCoin()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.s_coin.ToString(), productValData.coin.ToString());
            WindowManager.instance.Win_message.showPresentAct($"{i}코인", 
                DataManager.GetTable<Sprite>(DataTable.product, productKeyList.s_coin.ToString(), productValData.image.ToString()), () =>
            {
                int cost = DataManager.GetTable<int>(DataTable.product, productKeyList.s_coin.ToString(), productValData.price.ToString());
                if (BaseManager.userGameData.Gem >= cost)
                {
                    BaseManager.userGameData.Gem -= cost;
                    _lobby.refreshFollowCost();

                    Debug.Log($"{i}코인 겟또다제");
                    BaseManager.userGameData.Coin += i;

                    WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.s_coin].position, _lobby.CoinTxt.position, currency.coin, i, 0, 10);

                    AuthManager.instance.AllSaveUserEntity();
                }
                else
                {
                    WindowManager.instance.showMessage("보석이 모자랍니눈!");
                }
            });
        }

        public void getMiddleCoin()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.m_coin.ToString(), productValData.coin.ToString());
            WindowManager.instance.Win_message.showPresentAct($"{i}코인", 
                DataManager.GetTable<Sprite>(DataTable.product, productKeyList.m_coin.ToString(), productValData.image.ToString()), () =>
            {
                int cost = DataManager.GetTable<int>(DataTable.product, productKeyList.m_coin.ToString(), productValData.price.ToString());
                if (BaseManager.userGameData.Gem >= cost)
                {
                    BaseManager.userGameData.Gem -= cost;
                    _lobby.refreshFollowCost();

                   
                    Debug.Log($"{i}코인 겟또다제");
                    BaseManager.userGameData.Coin += i;

                    WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.m_coin].position, _lobby.CoinTxt.position, currency.coin, i, 0, 15);

                    AuthManager.instance.AllSaveUserEntity();
                }
                else
                {
                    WindowManager.instance.showMessage("보석이 모자랍니눈!");
                }
            });
        }

        public void getLargeCoin()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.l_coin.ToString(), productValData.coin.ToString());
            WindowManager.instance.Win_message.showPresentAct($"{i}코인",
                DataManager.GetTable<Sprite>(DataTable.product, productKeyList.l_coin.ToString(), productValData.image.ToString()), () =>
            {
                int cost = DataManager.GetTable<int>(DataTable.product, productKeyList.l_coin.ToString(), productValData.price.ToString());
                if (BaseManager.userGameData.Gem >= cost)
                {
                    BaseManager.userGameData.Gem -= cost;
                    _lobby.refreshFollowCost();

                    Debug.Log($"{i}코인 겟또다제");
                    BaseManager.userGameData.Coin += i;

                    WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.l_coin].position, _lobby.CoinTxt.position, currency.coin, i, 0, 22);

                    AuthManager.instance.AllSaveUserEntity();
                }
                else
                {
                    WindowManager.instance.showMessage("보석이 모자랍니눈!");
                }
            });
        }

        #endregion

        public void purchaseFail()
        {
            WindowManager.instance.showActMessage("결제에 실패했습눈다", () => { });
        }
        
        public void open()
        {
            _ad.raycastTarget = !BaseManager.userGameData.RemoveAd;
            _soldoutAd.SetActive(BaseManager.userGameData.RemoveAd);
            _10p.raycastTarget = !BaseManager.userGameData.MulCoin;
            _soldout10p.SetActive(BaseManager.userGameData.MulCoin);
            _skin.raycastTarget = !BaseManager.userGameData.SkinPack;
            _soldoutSkin.SetActive(BaseManager.userGameData.SkinPack);

            gameObject.SetActive(true);
        }

        public void close()
        {
            gameObject.SetActive(false);
        }
    }
}