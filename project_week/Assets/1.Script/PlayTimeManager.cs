using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class PlayTimeManager : MonoBehaviour
    {
        int _timeStack;
        public int TimeStack 
        {
            get
            {
                int t = _timeStack;
                _timeStack = 0;
                return t;
            }
        }

        long _nowTime;
        bool _isSetUserData = false;

        // ====== [   store   ] ======
        DateTime _eve = new DateTime(2020, 12, 31, 23, 59, 59);
        TimeSpan _leftTime;
        public TimeSpan LeftTime { get => _leftTime; }
        // ====== [   adgem   ] ======
        DateTime _nextAdGem;
        TimeSpan adgem_leftTime;
        public TimeSpan AdGem_LeftTime { get => adgem_leftTime; }
        // ====== [   rader   ] ======
        bool _chkRader;
        long _chkRaderTime;
        public long ChkRaderTime { get => _chkRaderTime; }


        public void Init()
        {
            StartCoroutine(chkTime());
        }

        IEnumerator chkTime()
        {
            yield return new WaitUntil(() => BaseManager.userGameData != null);

            while (true)
            {
                BaseManager.userGameData.TimeCheck++;
                _timeStack++;

                _nowTime += 1000;

                if (_isSetUserData)
                {
                    adgem_leftTime = _nextAdGem - gameValues.epoch.AddMilliseconds(_nowTime);
                }

                // 광고제거 안샀고 && 레이더 시간 체크 필요 && 레이더 아직 대여불가
                //if (BaseManager.userGameData.RemoveAd == false && _chkRader && BaseManager.userGameData.RaderRentalable == false) 
                //{
                //    _chkRaderTime++;
                //    if (_chkRaderTime > 900)
                //    {
                //        BaseManager.userGameData.RaderRentalable = true;
                //    }
                //}

                
                _leftTime = _eve - gameValues.epoch.AddMilliseconds(_nowTime);

                if (BaseManager.userGameData.TimeCheck > 300f) // 5분
                {
                    BaseManager.userGameData.TimeCheck = 0;

                    AuthManager.instance.checkNextDay();
                }

                yield return new WaitForSecondsRealtime(1f);
            }

            yield return null;
        }


        public void setAdGem()
        {
            _nextAdGem = gameValues.epoch.AddMilliseconds(BaseManager.userGameData.NextAdGemTime);

            _isSetUserData = true;
        }

        public void adGemRefresh()
        {
            BaseManager.userGameData.NextAdGemTime = BaseManager.userGameData.LastSave + Convert.ToInt64(new TimeSpan(0, 30, 0).TotalMilliseconds);
            setAdGem();
        }

        #region [ Time Check List ]

        /// <summary> 레이더 체크 세팅 </summary>
        public void setRaderRentalCheck(long time)
        {
            _chkRader = true;

            _chkRaderTime = time;
        }

        public void resetRaderTime()
        {
            _chkRaderTime = 0;
            BaseManager.userGameData.RaderRentalable = false;
        }

        public void setStoreCheck(long time)
        {
            _nowTime = time;
        }

        #endregion
    }
}