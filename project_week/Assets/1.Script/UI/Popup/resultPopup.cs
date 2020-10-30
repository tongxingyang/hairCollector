using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace week
{
    public class resultPopup : MonoBehaviour
    {
        [Header("top")]
        [SerializeField] CanvasGroup _top;
        [Space]
        [SerializeField] TextMeshProUGUI _bestRecord;
        [SerializeField] Image _newImg;
        [Header("bot")]
        [SerializeField] CanvasGroup _bot;
        [Space]
        [SerializeField] TextMeshProUGUI _recordTitle;
        [SerializeField] TextMeshProUGUI _record;
        [SerializeField] TextMeshProUGUI _coinTxt;
        [SerializeField] TextMeshProUGUI _gemTxt;
        [SerializeField] TextMeshProUGUI _apTxt;

        [SerializeField] GameObject _coin;
        [SerializeField] GameObject _gem;
        [SerializeField] GameObject _ap;

        [SerializeField] Image _AdsBtn;

        float _preCalCoin, _getCoin;
        float _preCalGem, _getGem;
        float _preCalAp, _getAp;

        bool _isNewRecord;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void resultInit(float time, int coin, int gem, int ap, int mob, int boss, int arti)
        {
            if (BaseManager.userGameData.RemoveAD)
            {
                _AdsBtn.color = Color.gray;
                _AdsBtn.raycastTarget = false;
            }

            _isNewRecord = time > BaseManager.userGameData.TimeRecord;

            _coin.SetActive(coin > 0);
            _gem.SetActive(gem > 0);
            _ap.SetActive(ap > 0);

            gameObject.SetActive(true);
            _top.alpha = 0;
            _bot.alpha = 0;

            _getCoin = coin;
            _getGem = gem;
            _getAp = ap;

            StartCoroutine(resultOpenSequence(time));
        }

        IEnumerator resultOpenSequence(float time)
        {
            //=================[ 창 열기 전 ]==============================================

            RectTransform topRect = (RectTransform)_top.transform;
            RectTransform botRect = (RectTransform)_bot.transform;

            _bestRecord.text = BaseManager.userGameData.getLifeTime(BaseManager.userGameData.TimeRecord, false);
            _record.text = "";// BaseManager.userEntity.getLifeTime(time, true);

            _coinTxt.text = _getCoin.ToString();
            _gemTxt.text = _getGem.ToString();
            _apTxt.text = _getAp.ToString();

            _newImg.gameObject.SetActive(false);
            if (_isNewRecord)
            {
                StartCoroutine(newRecord());
            }

            //=================[ 창 열기 ]==============================================

            botRect.DOAnchorPosY(-175f, 1f);
            _bot.DOFade(1f, 1f);

            yield return new WaitForSeconds(0.5f);

            topRect.DOAnchorPosY(875f, 1f);
            _top.DOFade(1f, 1f);

            //=================[ 기록 ]==============================================

            yield return new WaitForSeconds(0.5f);
            
            _record.text = BaseManager.userGameData.getLifeTime(time, true);
            _record.transform.localScale = Vector3.one * 2;
            _record.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCirc);

            yield return new WaitForSeconds(0.25f);

            if (_isNewRecord)
            {
                BaseManager.userGameData.TimeRecord = (int)time;

                _bestRecord.text = BaseManager.userGameData.getLifeTime(BaseManager.userGameData.TimeRecord, false);

                _bestRecord.transform.localScale = Vector3.one * 2;
                _bestRecord.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCirc);

                _newImg.transform.localScale = Vector3.one * 3;
                _newImg.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.InCirc);

                WindowManager.instance.Win_celebrate.whenNewResult();
            }

            preRewardCalculator();

            // 10퍼 추가
            if (BaseManager.userGameData.AddGoods)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(getMultiReward(BaseManager.userGameData.AddGoodsValue, 1f, 1f));
            }

            // 광고 제거
            if (BaseManager.userGameData.RemoveAD)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(getMultiReward(2f, 2f, 2f));
            }
        }

        IEnumerator newRecord()
        {
            // _newImg.gameObject.SetActive(true);
            _recordTitle.text = "신 기 록";

            while (true)
            {
                _recordTitle.color = Color.white;
                yield return new WaitForSeconds(0.2f);
                _recordTitle.color = new Color(1f, 0.5f, 0f);
                yield return new WaitForSeconds(0.2f);
            }
        }

        void doubleReward()
        {
            BaseManager.userGameData.Coin += (int)_preCalCoin;
            BaseManager.userGameData.Gem += (int)_preCalGem;
            BaseManager.userGameData.Ap += (int)_preCalAp;

            BaseManager.userGameData.GameReward = new int[3] { (int)_preCalCoin * 2, (int)_preCalGem * 2, (int)_preCalAp * 2 };

            BaseManager.userGameData.saveDataToLocal();

            StartCoroutine(getMultiReward(2f, 2f, 2f)); 

            _AdsBtn.color = Color.gray;
            _AdsBtn.raycastTarget = false;
        }

        /// <summary> 일단 내가 얻을수있는건 미리 계산 </summary>
        void preRewardCalculator()
        {
            _preCalCoin = _getCoin;
            _preCalGem = _getGem;
            _preCalAp = _getAp;

            if (BaseManager.userGameData.AddGoods)
            {
                _preCalCoin *= 1.1f;
                _preCalGem *= 1.1f;
                _preCalAp *= 1.1f;
            }

            // 광고 제거
            if (BaseManager.userGameData.RemoveAD)
            {
                _preCalCoin *= 2f;
                _preCalGem *= 2f;
                _preCalAp *= 2f;
            }

            BaseManager.userGameData.Coin += (int)_preCalCoin;
            BaseManager.userGameData.Gem += (int)_preCalGem;
            BaseManager.userGameData.Ap += (int)_preCalAp;

            BaseManager.userGameData.GameReward = new int[3] { (int)_preCalCoin, (int)_preCalGem, (int)_preCalAp };

            BaseManager.userGameData.saveDataToLocal();
        }

        IEnumerator getMultiReward(float m_c,float m_g,float m_a)
        {
            // 시작수
            int c = (int)_getCoin;
            int g = (int)_getGem;
            int a = (int)_getAp;

            bool going = true;

            // 코인 배수 설정
            int coinup = 1;
            if (_getCoin > 100)
            {
                coinup = (int)(_getCoin * 0.1f);
            }

            yield return new WaitForSeconds(1f);

            // 문자변경
            while (going)
            {
                if (_getCoin * m_c > c)
                {
                    c += coinup;
                    _coinTxt.text = c.ToString();
                }
                else
                {
                    going = false;
                }

                if (_getGem * m_g > g)
                {
                    g++;
                    _gemTxt.text = g.ToString();
                }

                if (_getAp * m_a > a)
                {
                    a++;
                    _apTxt.text = a.ToString();
                }

                // 띠링
                yield return new WaitForEndOfFrame();
            }

            _getCoin    = (int)(_getCoin * m_c);
            _getGem     = (int)(_getGem * m_g);
            _getAp      = (int)(_getAp * m_a);

            _coinTxt.text   = ((int)_getCoin).ToString();
            _gemTxt.text    = ((int)_getGem).ToString();
            _apTxt.text     = ((int)_getAp).ToString();            
        }

        public void doubleCoin()
        {
#if UNITY_EDITOR
            doubleReward();
#elif UNITY_ANDROID
            AdManager.instance.adReward = () =>
            {
                doubleReward();
            };
            AdManager.instance.UserChoseToWatchAd();
#endif

        }

        public void goHome()
        {
            WindowManager.instance.Win_celebrate.allClose();
            SoundManager.instance.StopBGM();
            BaseManager.instance.convertScene(SceneNum.GameScene.ToString(), SceneNum.LobbyScene);
        }
    }
}