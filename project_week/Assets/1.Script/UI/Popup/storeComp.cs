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
        //[SerializeField] GameObject _mini;
        //[SerializeField] GameObject _icecream;
        [SerializeField] GameObject _ad;
        [SerializeField] GameObject _start;

        [SerializeField] GameObject _bonus;        
        [SerializeField] GameObject _skin;

        [Space]
        [SerializeField] LobbyScene _lobby;
        //[SerializeField] TextMeshProUGUI _leftTime;
        [SerializeField] Scrollbar _bar;
        [SerializeField] TextMeshProUGUI _timer;
        [SerializeField] Image _ad_gemBox;

        [Space] 
        //[SerializeField] RectTransform _event;
        //ContentSizeFitter _eventFitter;
        [SerializeField] RectTransform _limit;
        ContentSizeFitter _limitFitter;
        [SerializeField] RectTransform _special;
        ContentSizeFitter _specialFitter;

        Action<bool> _refreshExcla;
        bool _adUsable;

        public void Init(Action<bool> refreshExcla)
        {
            _bar.value = 1f;
            _adUsable = true;

            //_eventFitter = _event.GetComponent<ContentSizeFitter>();
            _limitFitter = _limit.GetComponent<ContentSizeFitter>();
            _specialFitter = _special.GetComponent<ContentSizeFitter>();

            _refreshExcla = refreshExcla;

            // setEventFitter();
            setLimitFitter();
            setSpecialFitter();

            StartCoroutine(timeCheckUpdate());
        }

        IEnumerator timeCheckUpdate()
        {
            BaseManager.userGameData.NextAdGemTime = 0;
            BaseManager.instance.PlayTimeMng.setAdGem();
            BaseManager.instance.PlayTimeMng.setStoreCheck(BaseManager.userGameData.LastSave);

            while (true)
            {
                //if (_10p.activeSelf)
                //{
                //    if (BaseManager.instance.PlayTimeMng.LeftTime.Days > 0)
                //    {
                //        _leftTime.text = $"{BaseManager.instance.PlayTimeMng.LeftTime.Days}일 {BaseManager.instance.PlayTimeMng.LeftTime.Hours}시간 남음";
                //    }
                //    else if (BaseManager.instance.PlayTimeMng.LeftTime.Hours > 0)
                //    {
                //        _leftTime.text = $"{BaseManager.instance.PlayTimeMng.LeftTime.Hours}시간 {BaseManager.instance.PlayTimeMng.LeftTime.Minutes}분 남음";
                //    }
                //    else
                //    {
                //        _leftTime.text = $"{BaseManager.instance.PlayTimeMng.LeftTime.Minutes}분 남음";
                //    }
                //}

                if (BaseManager.instance.PlayTimeMng.AdGem_LeftTime.TotalSeconds > 0)
                {
                    _timer.text = $"{BaseManager.instance.PlayTimeMng.AdGem_LeftTime.Minutes}:{BaseManager.instance.PlayTimeMng.AdGem_LeftTime.Seconds} 남음";
                    _ad_gemBox.color = Color.gray;
                    // _ad_gemBox.raycastTarget = false;
                }
                else
                {
                    _timer.text = "광고보기";
                    _ad_gemBox.color = Color.white;
                    // _ad_gemBox.raycastTarget = true;
                }

                yield return new WaitForSecondsRealtime(1f);
            }
        }

        #region [ 구매 한정 상품 ]

        /// <summary> 광고 제거 </summary>
        public void getRemoveAd()
        {
            Debug.Log("광고 제거");
            BaseManager.userGameData.AddMulCoinList(mulCoinChkList.removeAD);            
            BaseManager.userGameData.RemoveAd = true;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.removead.ToString(), productValData.image.ToString()),
                "광고 제거", setLimitFitter).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.removead, AnalyticsManager.instance.getKey());
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

            string bonus = calCoinBonus(ref c);

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.startpack.ToString(), productValData.image.ToString()),
                "스타팅팩", setLimitFitter).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + "/스타팅팩 구매" + bonus + $"/{c}/{g}/{a}";

            NanooManager.instance.PostboxItemSend(nanooPost.pack, 0, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.startpack, key);
        }

        /// <summary> 추가보너스 정보 </summary>
        public void limitInfo()
        {
            string str = "<광고제거>" + System.Environment.NewLine +
                "상점의 [무료보석]을 제외한 게임 내 광고가 제거됩니다." + System.Environment.NewLine + System.Environment.NewLine +
                "<스타팅 팩>" + System.Environment.NewLine +
                "광고제거 후 스타팅팩 구매시" + System.Environment.NewLine +
                "4000코인, 40보석, 4AP 대체 지급";
                
            WindowManager.instance.showActMessage(str, () => { });
        }

        #endregion

        #region [ 스페셜 팩 ] 

        /// <summary> 추가보너스 </summary>
        public void getAdd3per()
        {
            Debug.Log("코인 추가 3퍼 겟또다제");
            BaseManager.userGameData.AddMulCoinList(mulCoinChkList.mul_1st_3p);
            BaseManager.userGameData.MulCoin3p = true;

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.bonus_3_0.ToString(), productValData.image.ToString()),
                "추가 10%코인", setSpecialFitter).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.bonus_3_0, AnalyticsManager.instance.getKey());
        }

        /// <summary> 스킨팩 </summary>
        public void getSkinPack()
        {
            Debug.Log("스킨팩 구매 완료");
            BaseManager.userGameData.SkinPack = true;

            SkinKeyList skl = SkinKeyList.wildman;

            bool result = (BaseManager.userGameData.HasSkin & (1 << (int)skl)) > 0;
            if (result == false)
            {
                BaseManager.userGameData.HasSkin |= (1 << (int)skl);
            }
                        
            int c = DataManager.GetTable<int>(DataTable.product, productKeyList.wildskinpack.ToString(), productValData.coin.ToString());
            int g = DataManager.GetTable<int>(DataTable.product, productKeyList.wildskinpack.ToString(), productValData.gem.ToString());

            if (result)
            {
                g += DataManager.GetTable<int>(DataTable.product, productKeyList.wildskinpack.ToString(), productValData.addgem.ToString());
                c += DataManager.GetTable<int>(DataTable.product, productKeyList.wildskinpack.ToString(), productValData.addcoin.ToString());
            }

            string bonus = calCoinBonus(ref c);

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.wildskinpack.ToString(), productValData.image.ToString()),
                "야수사람팩", setSpecialFitter).setImgSize();
            WindowManager.instance.Win_celebrate.whenPurchase();

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + "/야수팩 구매" + bonus + $"/{c}/{g}/{0}";

            NanooManager.instance.PostboxItemSend(nanooPost.pack, (int)SkinKeyList.wildman, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.wildskinpack, key);
        }

        /// <summary> 스페셜 팩 정보 </summary>
        public void specialInfo()
        {
            string str = "<코인보너스>" + System.Environment.NewLine +
                "모험 습득 코인 증가" + System.Environment.NewLine +
                "상점 : 코인 구매량 증가" + System.Environment.NewLine +
                "※다음 코인구매, 모험부터 적용됩니다." + System.Environment.NewLine + System.Environment.NewLine +
                "<스킨 팩>" + System.Environment.NewLine + 
                "스킨구매 후 스킨팩 구매시" + System.Environment.NewLine +
                "25000코인, 250보석 대체 지급";

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

        public void getAdGem()
        {
            if (_adUsable)
            {
                _adUsable = false;
                StartCoroutine(getAdGemRoutine());
            }
        }

        IEnumerator getAdGemRoutine()
        {
            if (BaseManager.instance.PlayTimeMng.AdGem_LeftTime.TotalSeconds > 0)
            {
                WindowManager.instance.showMessage("조금만 더 기다려주세눈!");
                _adUsable = true;
                yield break;
            }

            int result = 0;
            
            NanooManager.instance.getTimeStamp(() =>
            {
                if (new TimeSpan(BaseManager.userGameData.NextAdGemTime - BaseManager.userGameData.LastSave).TotalSeconds < 5f)
                {
                    result += 1;
                }

                result += 1;
            });

            yield return new WaitUntil(() => result > 0);

            if (result == 1)
            {
                _adUsable = true;
                yield break;
            }

            int g = DataManager.GetTable<int>(DataTable.product, productKeyList.ad_gem.ToString(), productValData.gem.ToString());

#if UNITY_EDITOR
            Debug.Log("광고");
            BaseManager.userGameData.Gem += g;
            WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.ad_gem].position, _lobby.GemTxt.position, currency.gem, g, 0, 10);
            BaseManager.instance.PlayTimeMng.adGemRefresh();
            AuthManager.instance.SaveDataServer(true);

            _adUsable = true;
#elif UNITY_ANDROID
            if (AuthManager.instance.networkCheck() == false)
            {                
                yield break;
            }

            AdManager.instance.adReward = () =>
            {                
                BaseManager.userGameData.AdRecord++;
                if (BaseManager.userGameData.DayQuestAd == 0)
                    BaseManager.userGameData.DayQuestAd = 1;

                BaseManager.userGameData.Gem += g;
                WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.ad_gem].position, _lobby.GemTxt.position, currency.gem, g, 0, 10);
                BaseManager.instance.PlayTimeMng.adGemRefresh();
                AuthManager.instance.SaveDataServer();
                
                _adUsable = true;
            };

            AdManager.instance.UserChoseToWatchAd();
#endif
            //WindowManager.instance.showMessage("보석이 모자랍니눈!");
        }

        public void getSmallGem()
        {
            int g = DataManager.GetTable<int>(DataTable.product, productKeyList.s_gem.ToString(), productValData.gem.ToString());

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.s_gem.ToString(), productValData.image.ToString())
                , "보석 조금").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + "/보석 조금" + $"/{0}/{g}/{0}";
            NanooManager.instance.PostboxItemSend(nanooPost.gem, g, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.s_gem, key);
        }

        public void getMiddleGem()
        {
            int g = DataManager.GetTable<int>(DataTable.product, productKeyList.m_gem.ToString(), productValData.gem.ToString());

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.m_gem.ToString(), productValData.image.ToString())
                , "보석 가방").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + "/보석 가방" + $"/{0}/{g}/{0}";
            NanooManager.instance.PostboxItemSend(nanooPost.gem, g, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.m_gem, key);
        }

        public void getLargeGem()
        {
            int g = DataManager.GetTable<int>(DataTable.product, productKeyList.l_gem.ToString(), productValData.gem.ToString());

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.l_gem.ToString(), productValData.image.ToString())
                , "보석 금고").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + "/보석 금고" + $"/{0}/{g}/{0}";
            NanooManager.instance.PostboxItemSend(nanooPost.gem, g, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.l_gem, key);
        }

        #endregion

        #region [ AP ]

        public void getSmallAp()
        {
            int a = DataManager.GetTable<int>(DataTable.product, productKeyList.s_ap.ToString(), productValData.ap.ToString());

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.s_ap.ToString(), productValData.image.ToString())
                , "AP 조금").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + "/AP 조금" + $"/{0}/{0}/{a}";
            NanooManager.instance.PostboxItemSend(nanooPost.ap, a, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.s_ap, key);
        }

        public void getMiddleAp()
        {
            int a = DataManager.GetTable<int>(DataTable.product, productKeyList.m_ap.ToString(), productValData.ap.ToString());

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.m_ap.ToString(), productValData.image.ToString())
                , "AP 뭉치").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + "/AP 뭉치" + $"/{0}/{0}/{a}";
            NanooManager.instance.PostboxItemSend(nanooPost.ap, a, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.m_ap, key);
        }

        public void getLargeAp()
        {
            int a = DataManager.GetTable<int>(DataTable.product, productKeyList.l_ap.ToString(), productValData.ap.ToString());

            WindowManager.instance.Win_purchase.setOpen(DataManager.GetTable<Sprite>(DataTable.product, productKeyList.l_ap.ToString(), productValData.image.ToString())
                , "AP 가방").setImgSize(false);

            string key = AnalyticsManager.instance.getKey();
            string postmsg = key + "/AP 가방" + $"/{0}/{0}/{a}";
            NanooManager.instance.PostboxItemSend(nanooPost.ap, a, postmsg);

            NanooManager.instance.getPostCount(_refreshExcla);

            BaseManager.userGameData.StoreUseCount++;
            AuthManager.instance.SaveDataServer(true);
            setAnalytics(productKeyList.l_ap, key);
        }

        #endregion

        #region [ 코인 ]

        public void getSmallCoin()
        {
            int i = DataManager.GetTable<int>(DataTable.product, productKeyList.s_coin.ToString(), productValData.coin.ToString());
            WindowManager.instance.Win_message.showPresentAct(
                $"{i}코인",
                DataManager.GetTable<Sprite>(DataTable.product, productKeyList.s_coin.ToString(), productValData.image.ToString()), 
                () => {
                int cost = DataManager.GetTable<int>(DataTable.product, productKeyList.s_coin.ToString(), productValData.price.ToString());
                if (BaseManager.userGameData.Gem >= cost)
                {
                    BaseManager.userGameData.Gem -= cost;
                    _lobby.refreshFollowCost();

                    calCoinBonus(ref i);

                    Debug.Log($"{i}코인 겟또다제");
                    BaseManager.userGameData.Coin += i;

                    WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.s_coin].position, _lobby.CoinTxt.position, currency.coin, i, 0, 10);

                    BaseManager.userGameData.StoreUseCount++;
                    AuthManager.instance.SaveDataServer(true);
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
            WindowManager.instance.Win_message.showPresentAct(
                $"{i}코인", 
                DataManager.GetTable<Sprite>(DataTable.product, productKeyList.m_coin.ToString(), productValData.image.ToString()), 
                () => {
                int cost = DataManager.GetTable<int>(DataTable.product, productKeyList.m_coin.ToString(), productValData.price.ToString());
                if (BaseManager.userGameData.Gem >= cost)
                {
                    BaseManager.userGameData.Gem -= cost;
                    _lobby.refreshFollowCost();

                    calCoinBonus(ref i);

                    Debug.Log($"{i}코인 겟또다제");
                    BaseManager.userGameData.Coin += i;

                    WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.m_coin].position, _lobby.CoinTxt.position, currency.coin, i, 0, 15);

                    BaseManager.userGameData.StoreUseCount++;
                    AuthManager.instance.SaveDataServer(true);
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
            WindowManager.instance.Win_message.showPresentAct(
                $"{i}코인",
                DataManager.GetTable<Sprite>(DataTable.product, productKeyList.l_coin.ToString(), productValData.image.ToString()), 
                () => {
                int cost = DataManager.GetTable<int>(DataTable.product, productKeyList.l_coin.ToString(), productValData.price.ToString());
                if (BaseManager.userGameData.Gem >= cost)
                {
                    BaseManager.userGameData.Gem -= cost;
                    _lobby.refreshFollowCost();

                    calCoinBonus(ref i);

                    Debug.Log($"{i}코인 겟또다제");
                    BaseManager.userGameData.Coin += i;

                    WindowManager.instance.Win_coinGenerator.getWealth2Point(mTrs[(int)eTr.l_coin].position, _lobby.CoinTxt.position, currency.coin, i, 0, 22);

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
        void setLimitFitter()
        {
            _ad.SetActive(BaseManager.userGameData.RemoveAd == false);
            _start.SetActive(BaseManager.userGameData.StartPack == false);

            if (BaseManager.userGameData.RemoveAd == true && BaseManager.userGameData.StartPack == true)
            {
                _limitFitter.enabled = false;
                _limit.sizeDelta = new Vector2(1350f, 150f);
            }
        }

        /// <summary> 스페셜 상품 창 </summary>
        void setSpecialFitter()
        {
            _bonus.SetActive(BaseManager.userGameData.MulCoin3p == false);
            _skin.SetActive(BaseManager.userGameData.SkinPack == false);

            if (BaseManager.userGameData.MulCoin3p == true && BaseManager.userGameData.SkinPack == true)
            {
                _specialFitter.enabled = false;
                _special.sizeDelta = new Vector2(1350f, 150f);
            }
        }

        /// <summary> 파베 Analytic 데이터 세팅 </summary>
        void setAnalytics(productKeyList pKey, string key)
        {
            Context context = new Context(key, analyticsWhere.store.ToString())
                    .setProduct(DataManager.GetTable<string>(DataTable.product, pKey.ToString(), productValData.name.ToString()), 0, 0, 0);
            AnalyticsManager.instance.Send(pKey.ToString(), context, null);
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