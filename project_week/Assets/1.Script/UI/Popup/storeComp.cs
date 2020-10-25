using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class storeComp : MonoBehaviour, UIInterface
    {
        [Header("coinEff")]
        [SerializeField] coingenerator _coinGen;
        [Header("present")]
        [SerializeField] Image _ad;
        [SerializeField] Image _10p;
        [SerializeField] Image _start;
        [SerializeField] Image _skin;

        [SerializeField] GameObject _soldoutAd;
        [SerializeField] GameObject _soldout10p;
        [SerializeField] GameObject _soldoutStart;
        [SerializeField] GameObject _soldoutSkin;

        Action _costRefresh;
        TweenAnim _tweenAnim;


        void Awake()
        {
            _tweenAnim = GetComponentInChildren<TweenAnim>();
            Debug.Log(_tweenAnim.name);
            _tweenAnim.gameObject.SetActive(false);
        }

        #region [ 특별 상품 ]

        /// <summary> 광고 제거 </summary>
        public void getRemoveAd()
        {
            Debug.Log("광고 제거");
            BaseManager.userGameData.RemoveAD = true;

            _ad.raycastTarget = false;
            _soldoutAd.SetActive(true);
        }

        /// <summary> 추가보너스 </summary>
        public void getAdd10per()
        {
            Debug.Log("코인 추가 10퍼 겟또다제");
            BaseManager.userGameData.AddGoods = true;
            BaseManager.userGameData.AddGoodsValue = 1.1f;

            _10p.raycastTarget = false;
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
            BaseManager.userGameData.StartPack = true;

            bool result = BaseManager.userGameData.RemoveAD;
            if (result == false)
            {
                getRemoveAd();
            }

            int i = (int)(10000 * ((result) ? 1.9f : 1f));
            BaseManager.userGameData.Coin += i;
            i = (int)(100 * ((result) ? 1.9f : 1f));
            BaseManager.userGameData.Gem += i;
            i = (int)(10 * ((result) ? 1.9f : 1f));
            BaseManager.userGameData.Ap += i;

            _coinGen.getCurrent(transform.position, currency.coin, 10000, 1);
            _coinGen.getCurrent(transform.position, currency.gem, 100, 2);
            _coinGen.getCurrent(transform.position, currency.ap, 10, 3);
            //_costRefresh();

            _tweenAnim.gameObject.SetActive(true);

            //_start.raycastTarget = false;
            //_soldoutStart.SetActive(true);
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

            _costRefresh();

            _skin.raycastTarget = false;
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
            int i = 40;
            Debug.Log($"{i}보석 겟또다제");
            BaseManager.userGameData.Gem += i;
            _costRefresh();
        }

        public void getMiddleGem()
        {
            int i = 250;
            Debug.Log($"{i}보석 겟또다제");
            BaseManager.userGameData.Gem += i;
            _costRefresh();
        }

        public void getLargeGem()
        {
            int i = 600;
            Debug.Log($"{i}보석 겟또다제");
            BaseManager.userGameData.Gem += i;
            _costRefresh();
        }

        #endregion

        #region [ AP ]

        public void getSmallAp()
        {
            int i = 40;
            Debug.Log($"{i} AP 겟또다제");
            BaseManager.userGameData.Gem += i;
            _costRefresh();
        }

        public void getMiddleAp()
        {
            int i = 40;
            Debug.Log($"{i} AP 겟또다제");
            BaseManager.userGameData.Gem += i;
            _costRefresh();
        }

        public void getLargeAp()
        {
            int i = 40;
            Debug.Log($"{i} AP 겟또다제");
            BaseManager.userGameData.Gem += i;
            _costRefresh();
        }

        #endregion

        #region [ 코인 ]

        public void getSmallCoin()
        {
            int cost = 50;
            if (BaseManager.userGameData.Gem >= cost)
            {
                int i = 5000;
                Debug.Log($"{i}코인 겟또다제");
                BaseManager.userGameData.Coin += i;
                _costRefresh();
            }
            else
            {
                WindowManager.instance.showMessage("보석이 모자랍니눈!");
            }
        }

        public void getMiddleCoin()
        {
            int cost = 175;
            if (BaseManager.userGameData.Gem >= cost)
            {
                int i = 20000;
                Debug.Log($"{i}코인 겟또다제");
                BaseManager.userGameData.Coin += i;
                _costRefresh();
            }
            else
            {
                WindowManager.instance.showMessage("보석이 모자랍니눈!");
            }
        }

        public void getLargeCoin()
        {
            int cost = 400;
            if (BaseManager.userGameData.Gem >= cost)
            {
                int i = 50000;
                Debug.Log($"{i}코인 겟또다제");
                BaseManager.userGameData.Coin += i;
                _costRefresh();
            }
            else
            {
                WindowManager.instance.showMessage("보석이 모자랍니눈!");
            }
        }

        #endregion

        public void purchaseFail()
        {
            WindowManager.instance.showActMessage("결제에 실패했습니눈", () => { });
        }
        
        public void open()
        {
            _ad.raycastTarget = !BaseManager.userGameData.RemoveAD;
            _soldoutAd.SetActive(BaseManager.userGameData.RemoveAD);
            _10p.raycastTarget = !BaseManager.userGameData.AddGoods;
            _soldout10p.SetActive(BaseManager.userGameData.AddGoods);
            _skin.raycastTarget = !BaseManager.userGameData.SkinPack;
            _soldoutSkin.SetActive(BaseManager.userGameData.SkinPack);

            gameObject.SetActive(true);
        }

        public void close()
        {
            gameObject.SetActive(false);
        }

        /// <summary> 코인 새로고침 받아오기 </summary>
        public void costRefresh(Action act)
        {
            _costRefresh = null;
            _costRefresh = act;
        }
    }
}