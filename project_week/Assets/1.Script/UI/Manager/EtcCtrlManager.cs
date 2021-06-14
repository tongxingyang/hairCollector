using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace week
{
    public class EtcCtrlManager : MonoBehaviour
    {
        [Button]
        public void getAllSkin()
        {
#if UNITY_EDITOR

            for (SkinKeyList i = 0; i < SkinKeyList.max; i++)
            {
                if (D_skin.GetEntity(i.ToString()).f_enable)
                {
                    BaseManager.userGameData.HasSkin |= (1 << (int)i);
                }
            }
#endif
        }
        [Button]
        public void loseAllSkin()
        {
#if UNITY_EDITOR
            BaseManager.userGameData.HasSkin = 1;
#endif
        }

        [Button]
        public void getAllproduct()
        {
#if UNITY_EDITOR
            BaseManager.userGameData.RemoveAd = true;
            BaseManager.userGameData.StartPack = true;
            BaseManager.userGameData.VampPack = false;
            BaseManager.userGameData.HeroPack = false;
#endif
        }

        [Button]
        public void loseAllproduct()
        {
#if UNITY_EDITOR
            BaseManager.userGameData.resetPaymentChkList();
            BaseManager.userGameData.VampPack = false;
            BaseManager.userGameData.HeroPack = false;
#endif
        }

        [Button]
        public void resetMyRecord()
        {
#if UNITY_EDITOR
            for (int i = 0; i < BaseManager.userGameData.SeasonTimeRecord.Length; i++) 
            {
                BaseManager.userGameData.SeasonTimeRecord[i] = 0;
                BaseManager._innerData.RecommendDay[i] = 1;
            }

            BaseManager._innerData.showRecommend = false;
#endif
        }

        [Button]
        public void getStat()
        {
#if UNITY_EDITOR
            BaseManager.userGameData.StatusLevel[(int)statusKeyList.hp] = 90;
            BaseManager.userGameData.StatusLevel[(int)statusKeyList.att] = 85;
            BaseManager.userGameData.StatusLevel[(int)statusKeyList.def] = 70;
            BaseManager.userGameData.StatusLevel[(int)statusKeyList.hpgen] = 50;
            BaseManager.userGameData.StatusLevel[(int)statusKeyList.cool] = 30;
            BaseManager.userGameData.StatusLevel[(int)statusKeyList.coin] = 15;
            BaseManager.userGameData.StatusLevel[(int)statusKeyList.exp] = 5;
#endif
        }

        [Button]
        public void loseStat()
        {
#if UNITY_EDITOR
            for (statusKeyList i = 0; i < statusKeyList.skin; i++)
            {
                BaseManager.userGameData.StatusLevel[(int)i] = 0;
            }
#endif
        }

        [Button]
        public void setRecommend()
        {
#if UNITY_EDITOR
            BaseManager._innerData.showRecommend = true;
#endif
        }

        [Button]
        public void openLevel()
        {
#if UNITY_EDITOR
            BaseManager.userGameData.setLevelOpen(levelKey.easy);
            BaseManager.userGameData.setLevelOpen(levelKey.normal);
#endif
        }

        [Button]
        public void closeLevel()
        {
#if UNITY_EDITOR
            BaseManager.userGameData.Property._isLevelOpen = 1;
#endif
        }
    }
}