using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using System;

namespace week
{
    public class AdManager : TSingleton<AdManager>
    {
        private string rewardID = "ca-app-pub-6349048174225682/9266473905";
        // private string rewardTestID = "ca-app-pub-3940256099942544/5224354917";

        private RewardedAd rewardedAd;

        public Action adReward;
        private bool rewarded = false;

        public void adStart()
        {
            CreateAndLoadRewardedAd();
        }

        //private void Update()
        //{
        //    if (rewarded)
        //    {
        //        Debug.Log("리워드 획득");
        //        rewarded = false;
        //    }
        //}

        /// <summary> 유저가 광고를 선택했을때 </summary>
        public void UserChoseToWatchAd()
        {
            if (rewardedAd.IsLoaded())
            {
                rewardedAd.Show();
            }
        }

        public void CreateAndLoadRewardedAd()
        {
            rewardedAd = new RewardedAd(rewardID);

            rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            AdRequest request = new AdRequest.Builder().Build();
            rewardedAd.LoadAd(request); // 광고 로드
        }

        public void HandleUserEarnedReward(object sender, Reward e)
        {
            rewarded = true; 
            adReward?.Invoke();
            Debug.Log("리워드 획득");
        }

        public void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            CreateAndLoadRewardedAd();
        }
    }
}