using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class adRebirthPopup : MonoBehaviour, UIInterface
    {
        Action _watch;
        Action _skip;

        void Awake()
        {
            close();
        }

        public void open()
        {
            gameObject.SetActive(true);
        }

        public void close()
        {
            gameObject.SetActive(false);
        }        

        public void setAction(Action watch, Action skip)
        {
            _watch = watch;
            _skip = skip;
        }

        public void watchingAd()
        {
#if UNITY_EDITOR
            _watch();
            close();
#elif UNITY_ANDROID
            AdManager.instance.adReward = () =>
            {
                _watch();
                close();
            };
            AdManager.instance.UserChoseToWatchAd();
#endif
        }

        public void cancel()
        {
            _skip();
            close();
        }
    }
}