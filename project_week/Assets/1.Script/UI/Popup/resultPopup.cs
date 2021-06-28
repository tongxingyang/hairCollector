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
        [Header("result")]
        [SerializeField] CanvasGroup _panel;
        [Space]
        [SerializeField] TextMeshProUGUI _bestRecord;
        [SerializeField] TextMeshProUGUI[] _recordTitle;
        [SerializeField] TextMeshProUGUI _record;
        [SerializeField] GameObject _newRecordLight;
        [Header("button")]
        [SerializeField] TextMeshProUGUI _coinTxt;
        [SerializeField] TextMeshProUGUI _gemTxt;
        [SerializeField] GameObject _coinbox;
        [SerializeField] GameObject _gembox;
        [Header("button")]
        [SerializeField] Image _AdsBtn;
        [SerializeField] Image _lobbyBtn;
        [Header("coinper")]
        [SerializeField] GameObject[] _per;

        GameScene _gs;

        float _preCalCoin, _getCoin;
        float _preCalGem, _getGem;
        float _preCalAp, _getAp;

        int _lvl, _mob, _boss, _clearQst;
        float _recordTime;

        bool _isNewRecord;

        private void Start()
        {
            for (int i = 0; i < _per.Length; i++)
            {
                _per[i].SetActive(false);
            }

            _newRecordLight.SetActive(false);
            gameObject.SetActive(false);
        }

        public void Init(GameScene gs)
        {
            _gs = gs;
        }

        public void setRresult()
        {
            // 기록
            _recordTime = _gs.ClockMng.RecordSecond;
            _clearQst = _gs.ClearQst;
            // 신기록 여부
            _isNewRecord = _recordTime > BaseManager.userGameData.TimeRecord(BaseManager.userGameData.NowStageLevel);

            // 보조기록
            _lvl = _gs.Lvl;
            _mob = _gs.MobKill;
            _boss = _gs.BossKill;

            // 재화
            _getCoin = _gs.Coin;
            _getGem = _gs.Gem;

            // 광고 설정
            if (BaseManager.userGameData.RemoveAd)
            {
                _AdsBtn.color = Color.gray;
                _AdsBtn.raycastTarget = false;
            }

            // 버튼 설정
            _AdsBtn.raycastTarget = false;
            _lobbyBtn.raycastTarget = false;

            // 재화 이미지 설정
            _coinbox.SetActive(_getCoin > 0);
            _gembox.SetActive(_getGem > 0);

            gameObject.SetActive(true);
            _panel.alpha = 0;

            StartCoroutine(resultOpenSequence());
        }

        IEnumerator resultOpenSequence()
        {
            //=================[ 창 열기 전 ]==============================================

            RectTransform Rect = (RectTransform)_panel.transform;

            _bestRecord.text = BaseManager.userGameData.getLifeTime(_gs.StageLevel, BaseManager.userGameData.TimeRecord(BaseManager.userGameData.NowStageLevel));
            _record.text = "";

            _coinTxt.text = Convert.ToInt32(_getCoin).ToString();
            _gemTxt.text = Convert.ToInt32(_getGem).ToString();

            _recordTitle[0].text = "이번기록";
            _recordTitle[1].text = "이번기록";

            if (_isNewRecord) // 시즌 신기록
            {
                StartCoroutine(newRecord());            
            }

            if (((int)BaseManager.userGameData.Skin == BaseManager.userGameData.QuestSkin) 
                && (BaseManager.userGameData.QuestSkin == 0))
            {
                BaseManager.userGameData.QuestSkin = 1;
            }

            // 일퀘
            // 일일 스킨 퀘스트
            if (BaseManager.userGameData.DayQuest[(int)Quest.day_skin] == 0
                && BaseManager.userGameData.Skin==(SkinKeyList)BaseManager.userGameData.QuestSkin)
            {
                BaseManager.userGameData.DayQuest[(int)Quest.day_skin] = 1;
            }
            // 일일 부활 퀘스트
            if (BaseManager.userGameData.DayQuest[(int)Quest.day_revive] == 0
                && _gs.RebirthQst > 0)
            {
                BaseManager.userGameData.DayQuest[(int)Quest.day_revive] = 1;
            }
            // 일일 스킬 퀘스트
            for (int i = 0; i < 3; i++)
            {
                if (BaseManager.userGameData.DayQuest[(int)Quest.day_skill_0 + i] == 0
                    && _gs.Player.isHaveSkill(BaseManager.userGameData.QuestSkill(i)))
                {
                    BaseManager.userGameData.DayQuest[(int)Quest.day_skill_0 + i] = 1;
                }
            }            

            // 새 정보
            BaseManager.userGameData.QuestRequest += _clearQst; // 퀘 계산
            BaseManager.userGameData.WholeTimeRecord += Convert.ToInt32(_recordTime); // 총 플탐 계산
            preRewardCalculator();  // 돈계산
            if (_gs.ClockMng.RecordMonth >= 3) // 난이도오픈계산
            {
                BaseManager.userGameData.setLevelOpen(_gs.StageLevel);
            }

            Debug.Log("코인 : " + _preCalCoin);
            //=================[ 창 열기 ]==============================================

            Rect.DOAnchorPosY(950f, 1f);
            _panel.DOFade(1f, 1f);

            yield return new WaitForSeconds(0.5f);

            //=================[ 기록 ]==============================================

            yield return new WaitForSeconds(0.5f);

            _record.text = BaseManager.userGameData.getLifeTime(_gs.StageLevel, _recordTime, false, true);
            _record.transform.localScale = Vector3.one * 2;
            _record.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCirc);

            yield return new WaitForSeconds(0.25f);

            if (BaseManager.userGameData.RemoveAd == false)
            {
                _AdsBtn.raycastTarget = true;
            }
            _lobbyBtn.raycastTarget = true;

            for (mulCoinChkList i = mulCoinChkList.removeAD; i < mulCoinChkList.max; i++)
            {
                if (BaseManager.userGameData.chkMulCoinList(i))
                {
                    yield return new WaitForSeconds(0.5f);
                    _per[(int)i].SetActive(true);
                    yield return StartCoroutine(getMultiReward(gameValues._mulCoinVal[(int)i]));
                }
            }
        }

        /// <summary> 신기록 </summary>
        IEnumerator newRecord()
        {
            // 데이터
            BaseManager.userGameData.setNewSeasonRecord(Convert.ToInt32(_recordTime), _lvl, _boss);
            NanooManager.instance.setSeasonRankingRecord(BaseManager.userGameData.NowStageLevel);

            // 신기록 -> 리뷰창
            if (BaseManager.userGameData.Success_Recommend == false)
            {
                int val = Convert.ToInt32(_recordTime) / 120;
                if (BaseManager._innerData.RecommendDay[(int)BaseManager.userGameData.NowStageLevel] < val)
                {
                    BaseManager._innerData.RecommendDay[(int)BaseManager.userGameData.NowStageLevel] = val;
                    BaseManager._innerData.showRecommend = true;
                    BaseManager.instance.saveDeviceData();
                }
            }

            // 장식
            _newRecordLight.SetActive(true);
            _recordTitle[0].text = "신 기 록";
            _recordTitle[1].text = "신 기 록";

            _bestRecord.text = BaseManager.userGameData.getLifeTime(_gs.StageLevel, Convert.ToInt32(_recordTime));

            _bestRecord.transform.localScale = Vector3.one * 2;
            _bestRecord.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCirc);

            WindowManager.instance.Win_celebrate.whenNewResult();

            // 장식
            Color col = new Color(1f, 0.5f, 0f);// new Color(0.4f, 0.5f, 1f);
            while (true)
            {
                _recordTitle[0].color = Color.white;
                yield return new WaitForSeconds(0.2f);

                _recordTitle[0].color = col;
                yield return new WaitForSeconds(0.2f);
            }
        }

        void doubleReward()
        {
            BaseManager.userGameData.Coin += (int)_preCalCoin;
            BaseManager.userGameData.Gem += (int)_preCalGem;

            BaseManager.userGameData.GameReward = new ObscuredInt[3] { (int)_preCalCoin * 2, (int)_preCalGem * 2, (int)_preCalAp * 2 };

            AuthManager.instance.SaveDataServer(false);

            StartCoroutine(getMultiReward(gameValues._mulCoinVal[(int)mulCoinChkList.removeAD]));

            _AdsBtn.color = Color.gray;
            _AdsBtn.raycastTarget = false;
        }

        /// <summary> [저장] 일단 내가 얻을수있는건 미리 계산 </summary>
        void preRewardCalculator()
        {
            _preCalCoin = _getCoin;
            _preCalGem = _getGem;
            //_preCalAp = _getAp;

            Debug.Log("코인 : " + _preCalCoin);
            for (mulCoinChkList i = mulCoinChkList.removeAD; i < mulCoinChkList.max; i++)
            {
                if (BaseManager.userGameData.chkMulCoinList(i))
                {
                    _preCalCoin *= gameValues._mulCoinVal[(int)i].x;
                    _preCalGem  *= gameValues._mulCoinVal[(int)i].y;
                    //_preCalAp   *= gameValues._mulCoinVal[(int)i].z;

                    if (i == mulCoinChkList.removeAD)
                    {
                        Debug.Log("removeAd : " + BaseManager.userGameData.RemoveAd);
                    }
                }
            }

            BaseManager.userGameData.Coin += (int)_preCalCoin;
            BaseManager.userGameData.Gem += (int)_preCalGem;
            //BaseManager.userGameData.Ap += (int)_preCalAp;

            BaseManager.userGameData.GameReward = new ObscuredInt[3] { (int)_preCalCoin, (int)_preCalGem, (int)_preCalAp };

            AuthManager.instance.SaveDataServer(false);
        }

        /// <summary> 코인얻는거 시연용 (실질적 기능 X) </summary>
        IEnumerator getMultiReward(Vector3 mul)
        {
            // 시작수
            int c = (int)_getCoin;
            int g = (int)_getGem;
            //int a = (int)_getAp;

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

                //if (_getAp * mul.y > a)
                //{
                //    a++;
                //    _apTxt.text = a.ToString();
                //}

                // 띠링
                yield return new WaitForEndOfFrame();
            }

            _getCoin    = (int)(_getCoin * mul.x);
            _getGem     = (int)(_getGem * mul.y);
            //_getAp      = (int)(_getAp * mul.z);

            _coinTxt.text   = ((int)_getCoin).ToString();
            _gemTxt.text    = ((int)_getGem).ToString();
            //_apTxt.text     = ((int)_getAp).ToString();            
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
                doubleReward();
            };
            AdManager.instance.UserChoseToWatchAd();
#endif
        }

        public void goHome()
        {
            WindowManager.instance.Win_celebrate.allClose();
            SoundManager.instance.StopBGM();
            AuthManager.instance.SaveDataServer(false);
            BaseManager.instance.convertScene(SceneNum.GameScene.ToString(), SceneNum.LobbyScene);
        }
    }
}