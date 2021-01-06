using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using CodeStage.AntiCheat.ObscuredTypes;

namespace week
{
    public class resultPopup : MonoBehaviour
    {
        [Header("top")]
        [SerializeField] CanvasGroup _top;
        [Space]
        [SerializeField] TextMeshProUGUI _bestRecord;
        [SerializeField] Image _titleBackImg;
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
        [Header("button")]
        [SerializeField] Image _AdsBtn;
        [SerializeField] Image _lobbyBtn;
        [Header("coinper")]
        [SerializeField] GameObject[] _per;

        float _preCalCoin, _getCoin;
        float _preCalGem, _getGem;
        float _preCalAp, _getAp;

        int _boss;

        int _isNewRecord; // 0: 신기록X, 1: 시즌기록 달성, 2: 전체기록 달성

        private void Start()
        {
            for (int i = 0; i < _per.Length; i++)
            {
                _per[i].SetActive(false);
            }

            gameObject.SetActive(false);
        }

        public void resultInit(float time, int coin, int gem, int ap, int mob, int boss, int arti)
        {
            if (BaseManager.userGameData.RemoveAd)
            {
                _AdsBtn.color = Color.gray;
                _AdsBtn.raycastTarget = false;
            }

            _AdsBtn.raycastTarget = false;
            _lobbyBtn.raycastTarget = false;

            _isNewRecord = (time > BaseManager.userGameData.AllTimeRecord) ? 2 : (time > BaseManager.userGameData.SeasonTimeRecord) ? 1 : 0;

            _coin.SetActive(coin > 0);
            _gem.SetActive(gem > 0);
            _ap.SetActive(ap > 0);

            gameObject.SetActive(true);
            _top.alpha = 0;
            _bot.alpha = 0;

            _getCoin = coin;
            _getGem = gem;
            _getAp = ap;

            _boss = boss;

            StartCoroutine(resultOpenSequence(time));
        }

        IEnumerator resultOpenSequence(float time)
        {
            //=================[ 창 열기 전 ]==============================================

            RectTransform topRect = (RectTransform)_top.transform;
            RectTransform botRect = (RectTransform)_bot.transform;

            _bestRecord.text = BaseManager.userGameData.getLifeTime(BaseManager.userGameData.SeasonTimeRecord, false);
            _record.text = "";// BaseManager.userEntity.getLifeTime(time, true);

            _coinTxt.text = _getCoin.ToString();
            _gemTxt.text = _getGem.ToString();
            _apTxt.text = _getAp.ToString();

            _recordTitle.text = "기  록";

            if (_isNewRecord > 0) // 시즌 신기록
            {
                BaseManager.userGameData.setNewSeasonRecord(Convert.ToInt32(time));
                NanooManager.instance.setSeasonRankingRecord(_boss);

                if (_isNewRecord == 2) // 전체도 신기록
                {
                    BaseManager.userGameData.setNewAllRecord(Convert.ToInt32(time));
                    NanooManager.instance.setAllRankingRecord(_boss);
                }

                StartCoroutine(newRecord(_isNewRecord == 2));            
            }

            if (((int)BaseManager.userGameData.Skin == BaseManager.userGameData.QuestSkin) 
                && (BaseManager.userGameData.DayQuestSkin == 0))
            {
                BaseManager.userGameData.DayQuestSkin = 1;
            }

            BaseManager.userGameData.WholeTimeRecord += Convert.ToInt32(time);
            preRewardCalculator();

            Debug.Log("코인 : " + _preCalCoin);
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

            if (_isNewRecord > 0)
            {            
                _bestRecord.text = BaseManager.userGameData.getLifeTime(BaseManager.userGameData.SeasonTimeRecord, false);

                _bestRecord.transform.localScale = Vector3.one * 2;
                _bestRecord.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCirc);

                WindowManager.instance.Win_celebrate.whenNewResult();
            }

            if (BaseManager.userGameData.RemoveAd == false)
            {
                _AdsBtn.raycastTarget = true;
            }
            _lobbyBtn.raycastTarget = true;

            for (mulCoinChkList i = mulCoinChkList.removeAD; i < mulCoinChkList.max; i++)
            {
                if (BaseManager.userGameData.chkMulCoinList(i))
                {
                    Debug.Log(i.ToString());
                    yield return new WaitForSeconds(0.5f);
                    _per[(int)i].SetActive(true);
                    yield return StartCoroutine(getMultiReward(gameValues._mulCoinVal[(int)i]));
                }
            }
        }

        IEnumerator newRecord(bool allRecord = false)
        {
            _titleBackImg.gameObject.SetActive(true);
            _recordTitle.text = "신 기 록";

            Color col = (allRecord) ? new Color(1f, 0.5f, 0f) : new Color(0.4f, 0.5f, 1f);

            while (true)
            {
                _recordTitle.color = Color.white;
                yield return new WaitForSeconds(0.2f);

                _recordTitle.color = col;
                yield return new WaitForSeconds(0.2f);
            }
        }

        void doubleReward()
        {
            BaseManager.userGameData.Coin += (int)_preCalCoin;
            BaseManager.userGameData.Gem += (int)_preCalGem;
            BaseManager.userGameData.Ap += (int)_preCalAp;

            BaseManager.userGameData.GameReward = new ObscuredInt[3] { (int)_preCalCoin * 2, (int)_preCalGem * 2, (int)_preCalAp * 2 };

            AuthManager.instance.SaveDataServer();

            StartCoroutine(getMultiReward(gameValues._mulCoinVal[(int)mulCoinChkList.removeAD]));

            _AdsBtn.color = Color.gray;
            _AdsBtn.raycastTarget = false;
        }

        /// <summary> 일단 내가 얻을수있는건 미리 계산 </summary>
        void preRewardCalculator()
        {
            _preCalCoin = _getCoin;
            _preCalGem = _getGem;
            _preCalAp = _getAp;

            Debug.Log("코인 : " + _preCalCoin);
            for (mulCoinChkList i = mulCoinChkList.removeAD; i < mulCoinChkList.max; i++)
            {
                if (BaseManager.userGameData.chkMulCoinList(i))
                {
                    _preCalCoin *= gameValues._mulCoinVal[(int)i].x;
                    _preCalGem  *= gameValues._mulCoinVal[(int)i].y;
                    _preCalAp   *= gameValues._mulCoinVal[(int)i].z;

                    if (i == mulCoinChkList.removeAD)
                    {
                        Debug.Log("removeAd : " + BaseManager.userGameData.RemoveAd);
                        BaseManager.userGameData.AdRecord++;
                        if (BaseManager.userGameData.DayQuestAd == 0)
                            BaseManager.userGameData.DayQuestAd = 1;
                    }
                }
            }

            BaseManager.userGameData.Coin += (int)_preCalCoin;
            BaseManager.userGameData.Gem += (int)_preCalGem;
            BaseManager.userGameData.Ap += (int)_preCalAp;

            BaseManager.userGameData.GameReward = new ObscuredInt[3] { (int)_preCalCoin, (int)_preCalGem, (int)_preCalAp };

            AuthManager.instance.SaveDataServer();
        }

        /// <summary> 코인얻는거 시연용 (실질적 기능 X) </summary>
        IEnumerator getMultiReward(Vector3 mul)
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
                if (_getCoin * mul.x > c)
                {
                    c += coinup;
                    _coinTxt.text = c.ToString();
                }
                else
                {
                    going = false;
                }

                if (_getGem * mul.y > g)
                {
                    g++;
                    _gemTxt.text = g.ToString();
                }

                if (_getAp * mul.y > a)
                {
                    a++;
                    _apTxt.text = a.ToString();
                }

                // 띠링
                yield return new WaitForEndOfFrame();
            }

            _getCoin    = (int)(_getCoin * mul.x);
            _getGem     = (int)(_getGem * mul.y);
            _getAp      = (int)(_getAp * mul.z);

            _coinTxt.text   = ((int)_getCoin).ToString();
            _gemTxt.text    = ((int)_getGem).ToString();
            _apTxt.text     = ((int)_getAp).ToString();            
        }

        public void doubleCoin()
        {
#if UNITY_EDITOR
            doubleReward();

#elif UNITY_ANDROID
            if (AuthManager.instance.networkCheck() == false)
            {                
                return;
            }

            AdManager.instance.adReward = () =>
            {                
                BaseManager.userGameData.AdRecord++;
                if (BaseManager.userGameData.DayQuestAd == 0)
                    BaseManager.userGameData.DayQuestAd = 1;
                doubleReward();
            };
            AdManager.instance.UserChoseToWatchAd();
#endif
        }

        public void goHome()
        {
            WindowManager.instance.Win_celebrate.allClose();
            SoundManager.instance.StopBGM();
            AuthManager.instance.SaveDataServer();
            BaseManager.instance.convertScene(SceneNum.GameScene.ToString(), SceneNum.LobbyScene);
        }
    }
}