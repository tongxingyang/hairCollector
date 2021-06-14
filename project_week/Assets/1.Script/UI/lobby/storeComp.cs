using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class storeComp : UIBase
    {
        #region [UIBase]
        enum eTr
        {
            ad_gem,

            s_coin,
            m_coin,
            l_coin
        }

        protected override Enum GetEnumTransform() { return new eTr(); }

        #endregion

        [Header("present")] 
        [SerializeField] GameObject _ad;
        [SerializeField] GameObject _start;

        [SerializeField] GameObject _vamp;        
        [SerializeField] GameObject _hero;

        [Space]
        [SerializeField] LobbyScene _lobby;
        [SerializeField] Scrollbar _bar;
        [SerializeField] TextMeshProUGUI _adGemCount;
        [SerializeField] GameObject _ad_gemBox;

        [Space] 
        [SerializeField] RectTransform _limit;
        [SerializeField] GameObject _limitSoldout;
        ContentSizeFitter _limitFitter;
        [SerializeField] RectTransform _special;
        [SerializeField] GameObject _specialSoldout;
        ContentSizeFitter _specialFitter;

        Action<bool> _refreshExcla;

        public void Init(Action<bool> refreshExcla)
        {
            _bar.value = 1f;

            //_eventFitter = _event.GetComponent<ContentSizeFitter>();
            _limitFitter = _limit.GetComponent<ContentSizeFitter>();
            _specialFitter = _special.GetComponent<ContentSizeFitter>();

            _refreshExcla = refreshExcla;

            // setEventFitter();
            StartCoroutine(setLimitFitter());
            StartCoroutine(setSpecialFitter());

            setAdGem();
        }        

        #region [ 구매 한정 상품 ]

        /// <summary> 광고 제거 </summary>
        public void getRemoveAd()
        {
            Debug.Log("광고 제거");
            BaseManager.userGameData.AddMulCoinList(mulCoinChkList.removeAD);            
            BaseManager.userGameData.RemoveAd = true;

            WindowManager.instance.Win_purchase.setStorePurchase(D_product.GetEntity(productKeyList.removead.ToString()).f_image, "광고 제거", () => StartCoroutine(setLimitFitter())).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);

            setAnalytics(productKeyList.removead.ToString(), AnalyticsManager.instance.getKey());
        }

        /// <summary> 스타터팩 (광고제거 포함) </summary>
        public void getStartPack()
        {
            Debug.Log("스타팅팩 구매 완료");

            bool result = BaseManager.userGameData.RemoveAd;
            if (result == false)
            {
                getRemoveAd();
            }

            BaseManager.userGameData.AddMulCoinList(mulCoinChkList.removeAD);
            BaseManager.userGameData.RemoveAd = true;
            BaseManager.userGameData.StartPack = true;

            int g = D_product.GetEntity(productKeyList.startpack.ToString()).f_gem;
            int c = D_product.GetEntity(productKeyList.startpack.ToString()).f_coin;

            if (result)
            {
                g += D_product.GetEntity(productKeyList.startpack.ToString()).f_addgem;
                c += D_product.GetEntity(productKeyList.startpack.ToString()).f_addcoin;
            }

            string bonus = calCoinBonus(ref c);

            WindowManager.instance.Win_purchase.setStorePurchase(D_product.GetEntity(productKeyList.startpack.ToString()).f_image,
                "스타팅팩", ()=>StartCoroutine(setLimitFitter())).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + "/스타팅팩 구매" + bonus + $"/{c}/{g}";

            NanooManager.instance.PostboxItemSend(nanooPost.pack, 0, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);

            setAnalytics(productKeyList.startpack.ToString(), key);
        }

        /// <summary> 추가보너스 정보 </summary>
        public void limitInfo()
        {
            string str = "<광고제거>" + System.Environment.NewLine +
                "상점의 [무료보석]을 제외한 게임 내 광고가 제거됩니다." + System.Environment.NewLine + System.Environment.NewLine +
                "<스타팅 팩>" + System.Environment.NewLine +
                "광고제거 후 스타팅팩 구매시" + System.Environment.NewLine +
                "4000코인, 40보석 대체 지급";
                
            WindowManager.instance.showActMessage(str, () => { });
        }

        #endregion

        #region [ 스페셜 팩 ] 

        /// <summary> 뱀파팩 </summary>
        public void getVampPack()
        {
            BaseManager.userGameData.VampPack = true;

            SkinKeyList skl = SkinKeyList.vampireman;
            bool result = (BaseManager.userGameData.HasSkin & (1 << (int)skl)) > 0;
            if (result == false)
            {
                BaseManager.userGameData.HasSkin |= (1 << (int)skl);
            }

            string prod = productKeyList.vamppack.ToString();
            int c = D_product.GetEntity(prod).f_coin;
            int g = D_product.GetEntity(prod).f_gem;

            if (result)
            {
                g += D_product.GetEntity(prod).f_addgem;
                c += D_product.GetEntity(prod).f_addcoin;
            }

            string bonus = calCoinBonus(ref c);

            WindowManager.instance.Win_purchase.setStorePurchase(D_product.GetEntity(prod).f_image, D_product.GetEntity(prod).f_productName, ()=>StartCoroutine(setSpecialFitter())).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + $"/{D_product.GetEntity(prod).f_productName} 구매" + bonus + $"/{c}/{g}";

            NanooManager.instance.PostboxItemSend(nanooPost.pack, (int)SkinKeyList.wildman, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(prod, key);
        }

        /// <summary> 용사팩 </summary>
        public void getHeroPack()
        {
            BaseManager.userGameData.HeroPack = true;
            BaseManager.userGameData.AddMulCoinList(mulCoinChkList.mul_2nd_3p);            

            SkinKeyList skl = SkinKeyList.heroman;
            bool result = (BaseManager.userGameData.HasSkin & (1 << (int)skl)) > 0;
            if (result == false)
            {
                BaseManager.userGameData.HasSkin |= (1 << (int)skl);
            }

            string prod = productKeyList.heropack.ToString();
            int c = D_product.GetEntity(prod).f_coin;
            int g = D_product.GetEntity(prod).f_gem;

            if (result)
            {
                g += D_product.GetEntity(prod).f_addgem;
                c += D_product.GetEntity(prod).f_addcoin;
            }

            string bonus = calCoinBonus(ref c);

            WindowManager.instance.Win_purchase.setStorePurchase(D_product.GetEntity(prod).f_image, D_product.GetEntity(prod).f_productName, () => StartCoroutine(setSpecialFitter())).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + $"/{D_product.GetEntity(prod).f_productName} 구매" + bonus + $"/{c}/{g}";

            NanooManager.instance.PostboxItemSend(nanooPost.pack, (int)SkinKeyList.wildman, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(prod, key);
        }

        /// <summary> 스페셜 팩 정보 </summary>
        public void specialInfo()
        {
            string str = "<뱀파이어사람 팩>" + System.Environment.NewLine +
                "스킨구매 후 스킨팩 구매시" + System.Environment.NewLine +
                "22500코인, 225보석 대체 지급" + System.Environment.NewLine + System.Environment.NewLine +

                "<용사사람 팩>" + System.Environment.NewLine + 
                "스킨구매 후 스킨팩 구매시" + System.Environment.NewLine +
                "40000코인, 400보석 대체 지급" + System.Environment.NewLine +
                "- 3% 코인보너스 -" + System.Environment.NewLine +
                "모험 습득 코인 증가" + System.Environment.NewLine +
                "상점 : 코인 구매량 증가" + System.Environment.NewLine +
                "※다음 코인구매, 모험부터 적용됩니다.";

            WindowManager.instance.showActMessage(str, () => { });
        }

        public void chkPresent()
        {
            WindowManager.instance.showMessage("해당 상품은 더 이상 구매할 수 없습니눈.");
        }

        #endregion

        #region [ 이벤트 (크리스마스) ]

        ///// <summary> 크리스마스 미니세트 </summary>
        //public void getMiniSet()
        //{
        //    Debug.Log("미니세트 : 5퍼 + 20ap");
        //    BaseManager.userGameData.AddMulCoinList = mulCoinChkList.mul_1st_5p;
        //    BaseManager.userGameData.MiniSet = true;

        //    int a = DataManager.GetTable<int>(DataTable.product, productKeyList.miniset.ToString(), productValData.ap.ToString());

        //    WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.miniset.ToString(), productValData.image.ToString()),
        //        "미니세트", setEventFitter).setImgSize();
        //    WindowManager.instance.Win_celebrate.whenPurchase();

        //    string key = AnalyticsManager.instance.getKey();
        //    string postmsg = key + $"/미니세트 구매/0/0/{a}";

        //    NanooManager.instance.PostboxItemSend(nanooPost.pack, 0, postmsg);

        //    NanooManager.instance.getPostCount(_refreshExcla);

        //    BaseManager.userGameData.StoreUseCount++;
        //    AuthManager.instance.SaveDataServer();
        //    setAnalytics(productKeyList.miniset, AnalyticsManager.instance.getKey());
        //}

        ///// <summary> 산타 스킨팩 </summary>
        //public void getSantaSkinSet()
        //{
        //    Debug.Log("산타스킨셋");
        //    BaseManager.userGameData.SantaSet = true;

        //    SkinKeyList skl = SkinKeyList.santaman;

        //    bool result = (BaseManager.userGameData.HasSkin & (1 << (int)skl)) > 0;
        //    if (result == false)
        //    {
        //        BaseManager.userGameData.HasSkin |= (1 << (int)skl);
        //    }

        //    int c = DataManager.GetTable<int>(DataTable.product, productKeyList.santaset.ToString(), productValData.coin.ToString());
        //    int g = DataManager.GetTable<int>(DataTable.product, productKeyList.santaset.ToString(), productValData.gem.ToString());
        //    int a = DataManager.GetTable<int>(DataTable.product, productKeyList.santaset.ToString(), productValData.ap.ToString());

        //    if (result)
        //    {
        //        g += DataManager.GetTable<int>(DataTable.product, productKeyList.santaset.ToString(), productValData.addgem.ToString());
        //        c += DataManager.GetTable<int>(DataTable.product, productKeyList.santaset.ToString(), productValData.addcoin.ToString());
        //        a += DataManager.GetTable<int>(DataTable.product, productKeyList.santaset.ToString(), productValData.addap.ToString());
        //    }

        //    string bonus = calCoinBonus(ref c);

        //    WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.santaset.ToString(), productValData.image.ToString()),
        //        "산타세트", setEventFitter).setImgSize();
        //    WindowManager.instance.Win_celebrate.whenPurchase();

        //    string key = AnalyticsManager.instance.getKey();
        //    string postmsg = key + "/산타세트 구매" + bonus + $"/{c}/{g}/{a}";

        //    NanooManager.instance.PostboxItemSend(nanooPost.pack, (int)SkinKeyList.santaman, postmsg);

        //    NanooManager.instance.getPostCount(_refreshExcla);

        //    BaseManager.userGameData.StoreUseCount++;
        //    AuthManager.instance.SaveDataServer();
        //    setAnalytics(productKeyList.santaset, key);
        //}

        ///// <summary> 이벤트 세트 정보 </summary>
        //public void eventInfo()
        //{
        //    string str = "<미니세트>" + System.Environment.NewLine +
        //        "모험 습득 코인 증가" + System.Environment.NewLine +
        //        "상점 : 코인 구매량 증가" + System.Environment.NewLine +
        //        "※다음 코인구매, 모험부터 적용됩니다." + System.Environment.NewLine + System.Environment.NewLine +
        //        "<산타 세트>" + System.Environment.NewLine +
        //        "스킨구매 후 스킨팩 구매시" + System.Environment.NewLine +
        //        "10000코인, 200보석, 30AP 대체 지급";
        //    WindowManager.instance.showActMessage(str, () => { });
        //}

        #endregion

        #region [ 보석 ]

        void setAdGem()
        {
            if (BaseManager.userGameData.LeftFreeGem > 0)
            {
                _ad_gemBox.SetActive(false);
                _adGemCount.text = $"{BaseManager.userGameData.LeftFreeGem}";
            }
            else
            {
                _ad_gemBox.SetActive(true);
                _adGemCount.text = "0";
            }
        }

        /// <summary> 광고 보석 얻기 </summary>
        public void getAdGem()
        {
            if (BaseManager.userGameData.LeftFreeGem > 0) // 무료보석 남아있음
            {
                BaseManager.userGameData.LeftFreeGem -= 1;
                StartCoroutine(getAdGemRoutine());
            }
            else
            {
                WindowManager.instance.showMessage("오늘의 무료 보석이 소진되었눈!");
            }
        }

        /// <summary> 광고 보석 과정 </summary>
        IEnumerator getAdGemRoutine()
        {
            int g = D_product.GetEntity(productKeyList.ad_gem.ToString()).f_gem;

#if UNITY_EDITOR
            Debug.Log("광고");
            BaseManager.userGameData.Gem += g;
            WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.ad_gem].position, _lobby.GemTxt.position, currency.gem, g, 0, 10);
            
            AuthManager.instance.SaveDataServer(true);
            

#elif UNITY_ANDROID
            if (AuthManager.instance.networkCheck() == false)
            {                
                yield break;
            }


            // 광고 미제거
            if (BaseManager.userGameData.RemoveAd == false)
            {
                bool result = false;

                AdManager.instance.adReward = () =>
                {
                    BaseManager.userGameData.Gem += g;
                    BaseManager.userGameData.LeftFreeGem--;

                    result = true;
                };

                AdManager.instance.UserChoseToWatchAd();

                yield return new WaitUntil(() => result == true);
            }
            else // 광고 제거
            {
                BaseManager.userGameData.Gem += g;
                BaseManager.userGameData.LeftFreeGem--;

                yield return null;
            }

            WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.ad_gem].position, _lobby.GemTxt.position, currency.gem, g, 0, 10);                
            AuthManager.instance.SaveDataServer(true);
# endif
            yield return null;

            setAdGem();
        }

        public void getSmallGem()
        {
            getGem(productKeyList.s_gem.ToString());
        }

        public void getMiddleGem()
        {
            getGem(productKeyList.m_gem.ToString());
        }

        public void getLargeGem()
        {
            getGem(productKeyList.l_gem.ToString());
        }
        public void getBigGem()
        {
            getGem(productKeyList.b_gem.ToString());
        }
        public void getHugeGem()
        {
            getGem(productKeyList.h_gem.ToString());
        }

        void getGem(string prod)
        {
            int g = D_product.GetEntity(prod).f_gem;

            WindowManager.instance.Win_purchase.setStorePurchase(D_product.GetEntity(prod).f_image, D_product.GetEntity(prod).f_productName).setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + $"/{D_product.GetEntity(prod).f_productName}/{0}/{g}";
            NanooManager.instance.PostboxItemSend(nanooPost.gem, g, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(prod, key);
        }

#endregion

#region [ AP ]

        //public void getSmallAp()
        //{
        //    int a = D_product.GetEntity(productKeyList.s_ap.ToString()).f_ap;

        //    WindowManager.instance.Win_purchase.setStorePurchase(D_product.GetEntity(productKeyList.s_ap.ToString()).f_image
        //        , "AP 조금").setImgSize(false);

        //    string key = AnalyticsManager.instance.getKey();
        //    string postmsg = key + "/AP 조금" + $"/{0}/{0}/{a}";
        //    NanooManager.instance.PostboxItemSend(nanooPost.ap, a, postmsg);

        //    NanooManager.instance.getPostCount(_refreshExcla);

        //    BaseManager.userGameData.StoreUseCount++;
        //    AuthManager.instance.SaveDataServer(true);
        //    setAnalytics(productKeyList.s_ap, key);
        //}

        //public void getMiddleAp()
        //{
        //    int a = D_product.GetEntity(productKeyList.m_ap.ToString()).f_ap;

        //    WindowManager.instance.Win_purchase.setStorePurchase(D_product.GetEntity(productKeyList.m_ap.ToString()).f_image
        //        , "AP 뭉치").setImgSize(false);

        //    string key = AnalyticsManager.instance.getKey();
        //    string postmsg = key + "/AP 뭉치" + $"/{0}/{0}/{a}";
        //    NanooManager.instance.PostboxItemSend(nanooPost.ap, a, postmsg);

        //    NanooManager.instance.getPostCount(_refreshExcla);

        //    BaseManager.userGameData.StoreUseCount++;
        //    AuthManager.instance.SaveDataServer(true);
        //    setAnalytics(productKeyList.m_ap, key);
        //}

        //public void getLargeAp()
        //{
        //    int a = D_product.GetEntity(productKeyList.l_ap.ToString()).f_ap;

        //    WindowManager.instance.Win_purchase.setStorePurchase(D_product.GetEntity(productKeyList.l_ap.ToString()).f_image
        //        , "AP 가방").setImgSize(false);

        //    string key = AnalyticsManager.instance.getKey();
        //    string postmsg = key + "/AP 가방" + $"/{0}/{0}/{a}";
        //    NanooManager.instance.PostboxItemSend(nanooPost.ap, a, postmsg);

        //    NanooManager.instance.getPostCount(_refreshExcla);

        //    BaseManager.userGameData.StoreUseCount++;
        //    AuthManager.instance.SaveDataServer(true);
        //    setAnalytics(productKeyList.l_ap, key);
        //}

#endregion

#region [ 코인 ]

        public void getSmallCoin()
        {
            getCoin(productKeyList.s_coin.ToString());
        }

        public void getMiddleCoin()
        {
            getCoin(productKeyList.m_coin.ToString());
        }

        public void getLargeCoin()
        {
            getCoin(productKeyList.l_coin.ToString());
        }

        void getCoin(string prod)
        {
            int i = D_product.GetEntity(prod).f_coin;
            WindowManager.instance.Win_message.showPresentAct($"{i}코인", D_product.GetEntity(prod).f_image, () =>
            {
                int cost = D_product.GetEntity(prod).f_price;
                if (BaseManager.userGameData.Gem >= cost)
                {
                    BaseManager.userGameData.Gem -= cost;
                    _lobby.refreshFollowCost();

                    calCoinBonus(ref i);

                    //Debug.Log($"{i}코인 겟또다제");
                    BaseManager.userGameData.Coin += i;

                    eTr e = EnumHelper.StringToEnum<eTr>(prod);
                    WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)e].position, _lobby.CoinTxt.position, currency.coin, i, 0, 15);

                    BaseManager.userGameData.StoreUseCount++;
                    AuthManager.instance.SaveDataServer(true);
                }
                else
                {
                    WindowManager.instance.showMessage("보석이 모자랍니눈!");
                }
            });
        }

#endregion

        /// <summary> 결제 실패 </summary>
        public void purchaseFail()
        {
            WindowManager.instance.showActMessage("결제에 실패했습눈다", () => { });
        }

        //void setEventFitter()
        //{
        //    _mini.SetActive(BaseManager.userGameData.MiniSet == false);
        //    _icecream.SetActive(BaseManager.userGameData.SantaSet == false);

        //    if (BaseManager.userGameData.MiniSet == true && BaseManager.userGameData.SantaSet == true)
        //    {
        //        _eventFitter.enabled = false;
        //        _event.sizeDelta = new Vector2(1350f, 150f);
        //    }
        //}

        /// <summary> 한정 상품 창 </summary>
        IEnumerator setLimitFitter()
        {
            _ad.SetActive(BaseManager.userGameData.RemoveAd == false);
            _start.SetActive(BaseManager.userGameData.StartPack == false);

            if (BaseManager.userGameData.RemoveAd == true && BaseManager.userGameData.StartPack == true)
            {
                _limitSoldout.SetActive(true);
                //_limitFitter.enabled = false;
                //_limit.sizeDelta = new Vector2(1350f, 150f);
            }

            _limit.gameObject.SetActive(false);
            yield return null;
            _limit.gameObject.SetActive(true);
        }

        /// <summary> 스페셜 상품 창 </summary>
        IEnumerator setSpecialFitter()
        {
            _vamp.SetActive(BaseManager.userGameData.VampPack == false);
            _hero.SetActive(BaseManager.userGameData.HeroPack == false);

            if (BaseManager.userGameData.VampPack && BaseManager.userGameData.HeroPack)
            {
                _specialSoldout.SetActive(true);
                //_specialFitter.enabled = false;
                //_special.sizeDelta = new Vector2(1350f, 150f);
            }

            _special.gameObject.SetActive(false);
            yield return null;
            _special.gameObject.SetActive(true);
        }

        /// <summary> 파베 Analytic 데이터 세팅 </summary>
        void setAnalytics(string pKey, string key)
        {
            Context context = new Context(key, analyticsWhere.store.ToString())
                    .setProduct(D_product.GetEntity(pKey).f_name, 0, 0);
            AnalyticsManager.instance.Send(pKey, context, null);
        }

#region

        /// <summary> 보너스 코인 계산 </summary>
        string calCoinBonus(ref int c)
        {
            string bonus = "";
            for (mulCoinChkList mul = mulCoinChkList.mul_1st_10p; mul < mulCoinChkList.max; mul++)
            {
                if (BaseManager.userGameData.chkMulCoinList(mul))
                {
                    c = Convert.ToInt32(c * gameValues._bonusCoinVal[(int)mul]);
                    bonus = " (코인 보너스)";
                }
            }

            return bonus;
        }

#endregion
    }
}