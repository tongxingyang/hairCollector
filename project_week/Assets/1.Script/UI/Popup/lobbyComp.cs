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
        [Header("rader")]
        [SerializeField] GameObject _adImg;
        [SerializeField] Image _raderField;
        [SerializeField] GameObject _raderlazer;
        [SerializeField] TextMeshProUGUI _raderTxt;

        public void Init()
        {
            _adImg.SetActive(!BaseManager.userGameData.RemoveAd);
            
            raderInit();
        }

        #region [ rader ]

        /// <summary> 레이더 초기화 </summary>
        void raderInit()
        {
            if (BaseManager.userGameData.RemoveAd == false)
            {
                //Debug.Log(gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastSave).ToString("yyyy-MM-dd HH:mm:ss"));
                //Debug.Log(gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastRaderTime).ToString("yyyy-MM-dd HH:mm:ss"));
                //Debug.Log(BaseManager.instance.PlayTimeMng.ChkRaderTime);
                //Debug.Log(BaseManager.userGameData.RaderRentalable);
                //Debug.Log(BaseManager.userGameData.IsSetRader);

                long delay = (long)(gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastSave) - gameValues.epoch.AddMilliseconds(BaseManager.userGameData.LastRaderTime)).TotalSeconds;

                BaseManager.userGameData.RaderRentalable = (delay >= 900);
                
                if (BaseManager.userGameData.RaderRentalable == false) // 아직 쿨 안지남
                {
                    BaseManager.instance.PlayTimeMng.setRaderRentalCheck(delay); // 남은 시간 계산 맡김
                }
            }

            StartCoroutine(raderUpdate());
        }

        /// <summary> 레이더 업데이트 </summary>
        IEnumerator raderUpdate()
        {
            while (true)
            {
                raderRefresh();
                yield return new WaitForSecondsRealtime(1f);
            }
        }
        
        /// <summary> 레이더 누름 </summary>
        public void getRader()
        {
            StartCoroutine(getRaderRoutine());
        }

        /// <summary> 레이더 체크 루틴 </summary>
        IEnumerator getRaderRoutine()
        {
            yield return null;

            if (BaseManager.userGameData.RemoveAd) // 광고제거 구매
            {
                BaseManager.userGameData.IsSetRader = !BaseManager.userGameData.IsSetRader;

                if (BaseManager.userGameData.IsSetRader) // 레이더 가져가
                {
                    _raderField.color = Color.white;
                    _raderTxt.text = "레이더" + System.Environment.NewLine + "챙겼어";
                }
                else // 레이더 놓고가
                {
                    _raderField.color = Color.gray;
                    _raderTxt.text = "레이더" + System.Environment.NewLine + "안챙겼어";
                }
                _raderlazer.SetActive(BaseManager.userGameData.IsSetRader);
                _raderTxt.fontSize = 110;
            }
            else // 광고 미제거 - 사용 쿨타임 10분
            {
                bool result = BaseManager.userGameData.RaderRentalable;

                if (result) // 쿨타임 지남 = 광고 시청가능
                {
                    result = false;
#if UNITY_EDITOR
                    BaseManager.userGameData.AdRecord++;
                    if (BaseManager.userGameData.DayQuestAd == 0)
                        BaseManager.userGameData.DayQuestAd = 1;

                    BaseManager.userGameData.IsSetRader = true;

                    raderRefresh();

                    result = true;
#else
                    AdManager.instance.adReward = () =>
                    {
                        BaseManager.userGameData.AdRecord++;
                        if (BaseManager.userGameData.DayQuestAd == 0)
                            BaseManager.userGameData.DayQuestAd = 1;

                        BaseManager.userGameData.IsSetRader = true;

                        raderRefresh();
                                                
                        result = true;
                    };

                    AdManager.instance.UserChoseToWatchAd();
#endif
                    BaseManager.instance.PlayTimeMng.resetRaderTime();
                    yield return new WaitUntil(() => result == true);

                    result = false;
                    NanooManager.instance.getTimeStamp(()=> 
                    {
                        BaseManager.userGameData.LastRaderTime = BaseManager.userGameData.LastSave;
                        result = true;
                    });
                    yield return new WaitUntil(() => result == true);

                    AuthManager.instance.SaveDataServer(true);
                }
                else // 아직 안지남
                {
                    if (BaseManager.userGameData.IsSetRader)
                    {
                        WindowManager.instance.Win_message.showMessage("레이더 이미 챙겼어!");
                    }
                    else
                    {
                        WindowManager.instance.Win_message.showMessage("아직 가져갈수없어");
                    }
                }
            }
        }

        /// <summary> 레이더 새로고침 </summary>
        void raderRefresh()
        {
            if (BaseManager.userGameData.RemoveAd) // 광고제거 구매
            {
                if (BaseManager.userGameData.IsSetRader) // 레이더 가져가
                {
                    _raderField.color = Color.white;
                    _raderTxt.text = "레이더" + System.Environment.NewLine + "챙겼어";
                }
                else // 레이더 놓고가
                {
                    _raderField.color = Color.gray;
                    _raderTxt.text = "레이더" + System.Environment.NewLine + "안챙겼어";
                }
                _raderlazer.SetActive(BaseManager.userGameData.IsSetRader);
                _raderTxt.fontSize = 110;
            }
            else // 광고 미제거 - 사용 쿨타임 10분
            {
                if (BaseManager.userGameData.IsSetRader) // 레이더 빌림
                {
                    _raderTxt.fontSize = 110;
                    _raderField.color = Color.white;
                    _raderlazer.SetActive(true);
                    _raderTxt.text = "레이더" + System.Environment.NewLine + "챙겼어";
                }
                else // 레이더 안빌림
                {
                    // Debug.Log("??? : " + BaseManager.userGameData.RaderRentalable);
                    bool result = BaseManager.userGameData.RaderRentalable; // 빌릴 수 여부

                    _raderTxt.fontSize = 70;

                    if (result) // 빌릴수 있음
                    {
                        _raderField.color = Color.white;
                        _raderTxt.text = "광고보고" + System.Environment.NewLine + "레이더" + System.Environment.NewLine + "<size=125>가지러 가기</size>";
                    }
                    else // 아직 못빌림
                    {
                        int t = 900 - (int)BaseManager.instance.PlayTimeMng.ChkRaderTime;
                        int m = t / 60;
                        int s = t % 60;

                        _raderField.color = Color.gray;
                        _raderTxt.text = "광고보고" + System.Environment.NewLine + "레이더 가져가기" + System.Environment.NewLine + string.Format("<size=125>{0:00}:{1:00}</size>", m, s);
                    }
                    _raderlazer.SetActive(result);
                }
            }
        }

#endregion
    }
}