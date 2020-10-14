using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class storeComp : MonoBehaviour, UIInterface
    {
        Action _costRefresh;

        public void getSmallCoin()
        {
            Debug.Log("골드 조금 겟또다제");
            BaseManager.userGameData.Coin += 10000;
            _costRefresh();
        }

        public void getMiddleCoin()
        {
            Debug.Log("골드 중간 겟또다제");
            BaseManager.userGameData.Coin += 50000;
            _costRefresh();
        }

        public void getLargeCoin()
        {
            Debug.Log("골드 많이 겟또다제");
            BaseManager.userGameData.Coin += 200000;
            _costRefresh();
        }

        public void getSmallGem()
        {
            Debug.Log("보석 조금 겟또다제");
            BaseManager.userGameData.Gem += 10;
            _costRefresh();
        }

        public void getMiddleGem()
        {
            Debug.Log("보석 중간 겟또다제");
            BaseManager.userGameData.Gem += 50;
            _costRefresh();
        }

        public void getLargeGem()
        {
            Debug.Log("보석 많이 겟또다제");
            BaseManager.userGameData.Gem += 200;
            _costRefresh();
        }

        public void getDoubleCoin()
        {
            Debug.Log("코인 2배 겟또다제");
            BaseManager.userGameData.DoubleCoin = 2;
            BaseManager.userGameData.RemoveAD = true;
        }
        public void open()
        {
            gameObject.SetActive(true);
        }

        public void close()
        {
            gameObject.SetActive(false);
        }

        /// <summary> 코인 새로고침 받아오기 </summary>
        public void costRefresh(Action act)
        {
            _costRefresh = null;
            _costRefresh = act;
        }
    }
}