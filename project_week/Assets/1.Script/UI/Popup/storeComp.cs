using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class storeComp : MonoBehaviour
    {
        Action coinRefresh;
        public Action CoinRefresh { set => coinRefresh = value; }

        public void getSmallCoin()
        {
            Debug.Log("골드 조금 겟또다제");
            BaseManager.userEntity.Coin += 10000;
            coinRefresh();
        }

        public void getMiddleCoin()
        {
            Debug.Log("골드 중간 겟또다제");
            BaseManager.userEntity.Coin += 50000;
            coinRefresh();
        }

        public void getLargeCoin()
        {
            Debug.Log("골드 많이 겟또다제");
            BaseManager.userEntity.Coin += 200000;
            coinRefresh();
        }

        public void getSmallGem()
        {
            Debug.Log("보석 조금 겟또다제");
            BaseManager.userEntity.Gem += 10;
            coinRefresh();
        }

        public void getMiddleGem()
        {
            Debug.Log("보석 중간 겟또다제");
            BaseManager.userEntity.Gem += 50;
            coinRefresh();
        }

        public void getLargeGem()
        {
            Debug.Log("보석 많이 겟또다제");
            BaseManager.userEntity.Gem += 200;
            coinRefresh();
        }

        public void getDoubleCoin()
        {
            Debug.Log("코인 2배 겟또다제");
            BaseManager.userEntity.DoubleCoin = 2;
            BaseManager.userEntity.RemoveAD = true;
        }
    }
}