using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class lobbyComp : MonoBehaviour
    {
        [Header("snowman")]
        [SerializeField] Image _snowman;
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] TextMeshProUGUI _record;

        [Header("btns")]
        [SerializeField] Button _rankBtn;
        [SerializeField] Button _questBtn;
        [SerializeField] Button _postBtn;
        [SerializeField] Button _startBtn;

        [Header("level")]
        [SerializeField] Button _levelBtn;
        [SerializeField] Button _levelDown;
        [SerializeField] Button _levelUp;
        [SerializeField] TextMeshProUGUI _level;
        [SerializeField] Image _levelImg;

        LobbyScene _ls;

        public void Init(LobbyScene ls)
        {
            _ls = ls;

            _rankBtn.onClick.AddListener(open_RankPanel);
            _questBtn.onClick.AddListener(open_quest);
            _postBtn.onClick.AddListener(open_postPanel);

            _levelBtn.onClick.AddListener(open_levelPanel);
            _levelDown.onClick.AddListener(down_level); 
            _levelUp.onClick.AddListener(up_level);

            _startBtn.onClick.AddListener(gameStart);

            refresh_SnowImg();
            show_levelRecord();
        }

        public void setStage()
        {
            _name.text = BaseManager.userGameData.NickName;

            if (BaseManager.userGameData.TimeRecord(BaseManager.userGameData.NowStageLevel) == 0)
            {
                _record.text = "응애 나 아기눈사람";
            }
            else
            {
                _record.text = $"{BaseManager.userGameData.getLifeTime(BaseManager.userGameData.NowStageLevel, BaseManager.userGameData.TimeRecord(BaseManager.userGameData.NowStageLevel))}";
            }
        }

        public void refresh_SnowImg()
        {
            _snowman.sprite = DataManager.SkinSprite[BaseManager.userGameData.Skin];
        }

        /// <summary> 랭크창 열기 </summary>
        public void open_RankPanel()
        {
            _ls.openRank();
        }

        /// <summary> 일퀘창 열기 </summary>
        public void open_quest()
        {
            _ls.openQuest();
        }

        /// <summary> 우편함 열기 </summary>
        public void open_postPanel()
        {
            _ls.openPost();
        }

        /// <summary> 게임시작 </summary>
        public void gameStart()
        {
            _ls.PlayGame();
        }

        /// <summary> 난이도창 열기 </summary>
        public void open_levelPanel()
        {
            _ls.openLevel();
        }

        /// <summary> 난이도 텍스트 변환 (난이도 변경시 시행) </summary>
        public void show_levelRecord()
        {
            _level.text = (BaseManager.userGameData.NowStageLevel).ToString();
            _levelImg.sprite = _ls.Lvls[(int)BaseManager.userGameData.NowStageLevel];
            setStage();
        }

        /// <summary> 난이도 다운 </summary>
        public void down_level()
        {
            levelKey lvl = BaseManager.userGameData.NowStageLevel;

            if (lvl == 0)            
                lvl = levelKey.hard;            
            else
                lvl--;

            if (BaseManager.userGameData.IsLevelOpen(lvl))
            {
                BaseManager.userGameData.NowStageLevel = lvl;
                show_levelRecord();
            }
        }

        /// <summary> 난이도 업 </summary>
        public void up_level()
        {
            levelKey lvl = BaseManager.userGameData.NowStageLevel;

            if (lvl == levelKey.hard)            
                lvl = levelKey.easy;            
            else
                lvl++;

            if (BaseManager.userGameData.IsLevelOpen(lvl))
            {
                BaseManager.userGameData.NowStageLevel = lvl;
                show_levelRecord();
            }
        }


        #region [ rader ]

//        /// <summary> 레이더 초기화 </summary>
//        void raderInit()
//        {
//            if (BaseManager.userGameData.RemoveAd == false)
//            {
//                //Debug.Log(gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastSave).ToString("yyyy-MM-dd HH:mm:ss"));
//                //Debug.Log(gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastRaderTime).ToString("yyyy-MM-dd HH:mm:ss"));
//                //Debug.Log(BaseManager.instance.PlayTimeMng.ChkRaderTime);
//                //Debug.Log(BaseManager.userGameData.RaderRentalable);
//                //Debug.Log(BaseManager.userGameData.IsSetRader);

//                long delay = (long)(gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastSave) - gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastRaderTime)).TotalSeconds;

//                BaseManager.userGameData.RaderRentalable = (delay >= 900);
                
//                if (BaseManager.userGameData.RaderRentalable == false) // 아직 쿨 안지남
//                {
//                    BaseManager.instance.PlayTimeMng.setRaderRentalCheck(delay); // 남은 시간 계산 맡김
//                }
//            }

//            StartCoroutine(raderUpdate());
//        }

//        /// <summary> 레이더 업데이트 </summary>
//        IEnumerator raderUpdate()
//        {
//            while (true)
//            {
//                raderRefresh();
//                yield return new WaitForSecondsRealtime(1f);
//            }
//        }
        
//        /// <summary> 레이더 누름 </summary>
//        public void getRader()
//        {
//            StartCoroutine(getRaderRoutine());
//        }

//        /// <summary> 레이더 체크 루틴 </summary>
//        IEnumerator getRaderRoutine()
//        {
//            yield return null;

//            if (BaseManager.userGameData.RemoveAd) // 광고제거 구매
//            {
//                BaseManager.userGameData.IsSetRader = !BaseManager.userGameData.IsSetRader;

//                if (BaseManager.userGameData.IsSetRader) // 레이더 가져가
//                {
//                    _raderField.color = Color.white;
//                    _raderTxt.text = "레이더" + System.Environment.NewLine + "챙겼어";
//                }
//                else // 레이더 놓고가
//                {
//                    _raderField.color = Color.gray;
//                    _raderTxt.text = "레이더" + System.Environment.NewLine + "안챙겼어";
//                }
//                _raderlazer.SetActive(BaseManager.userGameData.IsSetRader);
//                _raderTxt.fontSize = 110;
//            }
//            else // 광고 미제거 - 사용 쿨타임 10분
//            {
//                bool result = BaseManager.userGameData.RaderRentalable;

//                if (result) // 쿨타임 지남 = 광고 시청가능
//                {
//                    result = false;
//#if UNITY_EDITOR
//                    BaseManager.userGameData.AdRecord++;
//                    if (BaseManager.userGameData.DayQuestAd == 0)
//                        BaseManager.userGameData.DayQuestAd = 1;

//                    BaseManager.userGameData.IsSetRader = true;

//                    raderRefresh();

//                    result = true;
//#else
//                    AdManager.instance.adReward = () =>
//                    {
//                        BaseManager.userGameData.AdRecord++;
//                        if (BaseManager.userGameData.DayQuestAd == 0)
//                            BaseManager.userGameData.DayQuestAd = 1;

//                        BaseManager.userGameData.IsSetRader = true;

//                        raderRefresh();
                                                
//                        result = true;
//                    };

//                    AdManager.instance.UserChoseToWatchAd();
//#endif
//                    BaseManager.instance.PlayTimeMng.resetRaderTime();
//                    yield return new WaitUntil(() => result == true);

//                    result = false;
//                    NanooManager.instance.getTimeStamp(()=> 
//                    {
//                        BaseManager.userGameData.LastRaderTime = BaseManager.userGameData.LastSave;
//                        result = true;
//                    });
//                    yield return new WaitUntil(() => result == true);

//                    AuthManager.instance.SaveDataServer(true);
//                }
//                else // 아직 안지남
//                {
//                    if (BaseManager.userGameData.IsSetRader)
//                    {
//                        WindowManager.instance.Win_message.showMessage("레이더 이미 챙겼어!");
//                    }
//                    else
//                    {
//                        WindowManager.instance.Win_message.showMessage("아직 가져갈수없어");
//                    }
//                }
//            }
//        }

//        /// <summary> 레이더 새로고침 </summary>
//        void raderRefresh()
//        {
//            if (BaseManager.userGameData.RemoveAd) // 광고제거 구매
//            {
//                if (BaseManager.userGameData.IsSetRader) // 레이더 가져가
//                {
//                    _raderField.color = Color.white;
//                    _raderTxt.text = "레이더" + System.Environment.NewLine + "챙겼어";
//                }
//                else // 레이더 놓고가
//                {
//                    _raderField.color = Color.gray;
//                    _raderTxt.text = "레이더" + System.Environment.NewLine + "안챙겼어";
//                }
//                _raderlazer.SetActive(BaseManager.userGameData.IsSetRader);
//                _raderTxt.fontSize = 110;
//            }
//            else // 광고 미제거 - 사용 쿨타임 10분
//            {
//                if (BaseManager.userGameData.IsSetRader) // 레이더 빌림
//                {
//                    _raderTxt.fontSize = 110;
//                    _raderField.color = Color.white;
//                    _raderlazer.SetActive(true);
//                    _raderTxt.text = "레이더" + System.Environment.NewLine + "챙겼어";
//                }
//                else // 레이더 안빌림
//                {
//                    // Debug.Log("??? : " + BaseManager.userGameData.RaderRentalable);
//                    bool result = BaseManager.userGameData.RaderRentalable; // 빌릴 수 여부

//                    _raderTxt.fontSize = 70;

//                    if (result) // 빌릴수 있음
//                    {
//                        _raderField.color = Color.white;
//                        _raderTxt.text = "광고보고" + System.Environment.NewLine + "레이더" + System.Environment.NewLine + "<size=125>가지러 가기</size>";
//                    }
//                    else // 아직 못빌림
//                    {
//                        int t = 900 - (int)BaseManager.instance.PlayTimeMng.ChkRaderTime;
//                        int m = t / 60;
//                        int s = t % 60;

//                        _raderField.color = Color.gray;
//                        _raderTxt.text = "광고보고" + System.Environment.NewLine + "레이더 가져가기" + System.Environment.NewLine + string.Format("<size=125>{0:00}:{1:00}</size>", m, s);
//                    }
//                    _raderlazer.SetActive(result);
//                }
//            }
//        }

#endregion
    }
}