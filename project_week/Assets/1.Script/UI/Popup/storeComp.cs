using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class storeComp : UIBase
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
        [SerializeField] GameObject _ad;
        [SerializeField] GameObject _10p;
        [SerializeField] GameObject _start;
        [SerializeField] GameObject _skin;

        [Space]
        [SerializeField] LobbyScene _lobby;
        [SerializeField] Scrollbar _bar;

        [Space]
        [SerializeField] RectTransform _limit;
        ContentSizeFitter _limitFitter;
        [SerializeField] RectTransform _special;
        ContentSizeFitter _specialFitter;
        //Action _costRefresh;


        public void Init()
        {
            _bar.value = 1f;

            _limitFitter = _limit.GetComponent<ContentSizeFitter>();
            _specialFitter = _special.GetComponent<ContentSizeFitter>();

            setLimitFitter();
            setSpecialFitter();
        }

        #region [ 특별 상품 ]

        /// <summary> 광고 제거 </summary>
        public void getRemoveAd()
        {
            Debug.Log("광고 제거");
            BaseManager.userGameData.AddMulCoinList = mulCoinChkList.removeAD;
            BaseManager.userGameData.RemoveAd = true;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.removead.ToString(), productValData.image.ToString()),
                "광고 제거", setLimitFitter).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();
            
            AuthManager.instance.SaveUserEntity();

            setAnalitics(productKeyList.removead, AnalyticsManager.instance.getKey());
        }

        /// <summary> 추가보너스 </summary>
        public void getAdd10per()
        {
            Debug.Log("코인 추가 10퍼 겟또다제");
            BaseManager.userGameData.AddMulCoinList = mulCoinChkList.mul_1st_10p;
            BaseManager.userGameData.MulCoin = true;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.bonus10.ToString(), productValData.image.ToString()),
                "추가 10%코인", setLimitFitter).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();
            
            AuthManager.instance.SaveUserEntity();
            setAnalitics(productKeyList.bonus10, AnalyticsManager.instance.getKey());
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
            Debug.Log("스타팅팩 구매 완료");            

            bool result = BaseManager.userGameData.RemoveAd;
            if (result == false)
            {
                getRemoveAd();
            }

            BaseManager.userGameData.AddMulCoinList = mulCoinChkList.removeAD;
            BaseManager.userGameData.RemoveAd = true;
            BaseManager.userGameData.StartPack = true;

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

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.startpack.ToString(), productValData.image.ToString()),
                "스타팅팩", () => { setLimitFitter(); setSpecialFitter(); }).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            string key = AnalyticsManager.instance.getKey();
            string msg = key + "/스타팅팩 구매";

            NanooManager.instance.PostboxItemSend(nanooPost.gem, g, msg);
            NanooManager.instance.PostboxItemSend(nanooPost.coin, c, msg);
            NanooManager.instance.PostboxItemSend(nanooPost.ap, a, msg);
            
            AuthManager.instance.SaveUserEntity();

            setAnalitics(productKeyList.startpack, key);
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

            int c = 30000 + ((result) ? 25000 : 0);            
            int g = 300 + ((result) ? 250 : 0);

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.wildskinpack.ToString(), productValData.image.ToString()),
                "야수사람팩", setSpecialFitter).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            string key = AnalyticsManager.instance.getKey();
            string msg = key + "/야수팩 구매";

            NanooManager.instance.PostboxItemSend(nanooPost.gem, g, msg);
            NanooManager.instance.PostboxItemSend(nanooPost.coin, c, msg);

            AuthManager.instance.SaveUserEntity();
            setAnalitics(productKeyList.wildskinpack, key);
        }

        public void chkPresent()
        {
            WindowManager.instance.showMessage("해당 상품은 더 이상 구매할 수 없습니눈.");
        }

        #endregion

        #region [ 보석 ]

        public void getSmallGem()
        {
            int g = DataManager.GetTable<int>(DataTable.product, productKeyList.s_gem.ToString(), productValData.gem.ToString());
            // Debug.Log($"{g}보석 겟또다제");
            // BaseManager.userGameData.Gem += g;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.s_gem.ToString(), productValData.image.ToString())
                , "보석 조금").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string msg = key + "/보석 조금";

            NanooManager.instance.PostboxItemSend(nanooPost.gem, g, msg);

            AuthManager.instance.SaveUserEntity();
            setAnalitics(productKeyList.s_gem, key);
        }

        public void getMiddleGem()
        {
            int g = DataManager.GetTable<int>(DataTable.product, productKeyList.m_gem.ToString(), productValData.gem.ToString());
            //Debug.Log($"{g}보석 겟또다제");
            //BaseManager.userGameData.Gem += g;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.m_gem.ToString(), productValData.image.ToString())
                , "보석 가방").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string msg = key + "/보석 가방";

            NanooManager.instance.PostboxItemSend(nanooPost.gem, g, msg);

            AuthManager.instance.SaveUserEntity();
            setAnalitics(productKeyList.m_gem, key);
        }

        public void getLargeGem()
        {
            int g = DataManager.GetTable<int>(DataTable.product, productKeyList.l_gem.ToString(), productValData.gem.ToString());
            //Debug.Log($"{i}보석 겟또다제");
            //BaseManager.userGameData.Gem += i;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.l_gem.ToString(), productValData.image.ToString())
                , "보석 금고").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string msg = key + "/보석 금고";

            NanooManager.instance.PostboxItemSend(nanooPost.gem, g, msg);

            AuthManager.instance.SaveUserEntity();
            setAnalitics(productKeyList.l_gem, key);
        }

        #endregion

        #region [ AP ]

        public void getSmallAp()
        {
            int a = DataManager.GetTable<int>(DataTable.product, productKeyList.s_ap.ToString(), productValData.ap.ToString());
            //Debug.Log($"{a} AP 겟또다제");
            //BaseManager.userGameData.Ap += a;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.s_ap.ToString(), productValData.image.ToString())
                , "AP 조금").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string msg = key + "/AP 조금";

            NanooManager.instance.PostboxItemSend(nanooPost.ap, a, msg);

            AuthManager.instance.SaveUserEntity();
            setAnalitics(productKeyList.s_ap, key);
        }

        public void getMiddleAp()
        {
            int a = DataManager.GetTable<int>(DataTable.product, productKeyList.m_ap.ToString(), productValData.ap.ToString());
            //Debug.Log($"{a} AP 겟또다제");
            //BaseManager.userGameData.Ap += a;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.m_ap.ToString(), productValData.image.ToString())
                , "AP 뭉치").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string msg = key + "/AP 뭉치";
            //str = str.replaceAll(" ", "&quot;");

            NanooManager.instance.PostboxItemSend(nanooPost.ap, a, msg);

            AuthManager.instance.SaveUserEntity(); 
            setAnalitics(productKeyList.m_ap, key);
        }

        public void getLargeAp()
        {
            int a = DataManager.GetTable<int>(DataTable.product, productKeyList.l_ap.ToString(), productValData.ap.ToString());
            //Debug.Log($"{a} AP 겟또다제");
            //BaseManager.userGameData.Ap += a;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.l_ap.ToString(), productValData.image.ToString())
                , "AP 가방").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string msg = key + "/AP 가방";

            NanooManager.instance.PostboxItemSend(nanooPost.ap, a, msg);

            AuthManager.instance.SaveUserEntity();
            setAnalitics(productKeyList.l_ap, key);
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

                    AuthManager.instance.SaveUserEntity();
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

                    AuthManager.instance.SaveUserEntity();
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

                    AuthManager.instance.SaveUserEntity();
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

        void setLimitFitter()
        {
            _ad.SetActive(BaseManager.userGameData.RemoveAd == false);
            _10p.SetActive(BaseManager.userGameData.MulCoin == false);

            if (BaseManager.userGameData.RemoveAd == true && BaseManager.userGameData.MulCoin == true)
            {
                _limitFitter.enabled = false;
                _limit.sizeDelta = new Vector2(1350f, 150f);
                //_limitFitter.
            }
        }

        void setSpecialFitter()
        {
            _start.SetActive(BaseManager.userGameData.StartPack == false);
            _skin.SetActive(BaseManager.userGameData.SkinPack == false);

            if (BaseManager.userGameData.StartPack == true && BaseManager.userGameData.SkinPack == true)
            {
                _specialFitter.enabled = false;
                _special.sizeDelta = new Vector2(1350f, 150f);
                //_limitFitter.
            }
        }

        void setAnalitics(productKeyList pKey, string key)
        {
            Context context = new Context(key, analyticsWhere.store.ToString())
                    .setProduct(DataManager.GetTable<string>(DataTable.product, pKey.ToString(), productValData.name.ToString()), 0);
            AnalyticsManager.instance.Send(pKey.ToString(), context, null);
        }
    }
}