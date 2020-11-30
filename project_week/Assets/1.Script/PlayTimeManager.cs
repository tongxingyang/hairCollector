using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class PlayTimeManager : MonoBehaviour
    {
        float _timeStack;
        public int TimeStack 
        {
            get
            {
                int t = Convert.ToInt32(_timeStack);
                _timeStack = 0;
                return t;
            }
        }

        public void Init()
        {
            StartCoroutine(chkTime());
        }

        IEnumerator chkTime()
        {
            yield return new WaitUntil(() => BaseManager.userGameData != null);

            while (true)
            {
                BaseManager.userGameData.TimeCheck += Time.deltaTime;
                _timeStack += Time.deltaTime;

                if (BaseManager.userGameData.TimeCheck > 300f) // 5분
                {
                    BaseManager.userGameData.TimeCheck = 0;

                    AuthManager.instance.checkNextDay();
                }

                yield return null;
            }

            yield return null;
        }
    }
}