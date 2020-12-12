using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class adRebirthPopup : MonoBehaviour, UIInterface
    {
        [SerializeField] Image _GemBtn;
        [SerializeField] Image _bar;

        Action _watch;
        Action _timeover;
        float time;

        void Awake()
        {
            close();
        }

        public void open()
        {
            _bar.fillAmount = 1f;
            _GemBtn.raycastTarget = (BaseManager.userGameData.Gem >= 10);
            _GemBtn.color = (BaseManager.userGameData.Gem >= 10) ? Color.white : Color.gray;

            gameObject.SetActive(true);

            StartCoroutine(timeChk());
        }

        public void close()
        {
            gameObject.SetActive(false);
        }        

        public void setAction(Action watch, Action timeover)
        {
            _watch = watch;
            _timeover = timeover;
        }

        public void watchingAd()
        {
#if UNITY_EDITOR
            _watch();
            close();

#elif UNITY_ANDROID
            if (AuthManager.instance.networkCheck() == false)
            {                
                return;
            }

            AdManager.instance.adReward = () =>
            {
                _watch();
                BaseManager.userGameData.AdRecord++;
                if (BaseManager.userGameData.DayQuestAd == 0)
                    BaseManager.userGameData.DayQuestAd = 1;

                close();
            };
            AdManager.instance.UserChoseToWatchAd();
#endif
            time = 2.5f;
        }

        public void payGem()
        {
            BaseManager.userGameData.Gem -= 10;
            _watch();
            close();
        }

        IEnumerator timeChk()
        {
            time = 0f;
            while (time < 3f)
            {
                time += Time.deltaTime;
                _bar.fillAmount = 1f - (time / 3);

                yield return new WaitForEndOfFrame();
            }

            cancel();
        }

        public void cancel()
        {
            // _timeover();
            close();
        }
    }
}