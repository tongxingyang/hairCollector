using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayNANOO;
using NaughtyAttributes;

namespace week
{
    public class NanooManager : TSingleton<NanooManager>
    {
        Plugin plugin;

        // readonly string static_RANK_CODE = "snowadventure-RANK-0D39E8A1-E2FF99FE";
        readonly string[] static_RANK_CODE = {  "snowadventure-RANK-A7261CBA-47783D3B",
                                                        "snowadventure-RANK-E47977E5-3A7BF95B",
                                                        "snowadventure-RANK-7B930EE4-22AB1BB7"};

        readonly string PRE_RANK_CODE = "snowadventure-RANK-0D39E8A1-E2FF99FE";

        public string getRANK_CODE(levelKey lvl) => static_RANK_CODE[(int)lvl];

        protected override void Init()
        {
            Debug.Log("나누 초기화");

            plugin = Plugin.GetInstance();
            plugin.transform.SetParent(transform);
            //AccessEvent();
        }

        public void setUid(string uid)
        {
            plugin.SetUUID(uid);
            plugin.SetNickname(BaseManager.userGameData.NickName);
            plugin.SetLanguage(Configure.PN_LANG_KO);
        }

        public void getTimeStamp(Action getTimeAction)
        {
            plugin.AccessEvent((state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    if (dictionary.ContainsKey("server_timestamp"))
                    {
                        long t = long.Parse((string)dictionary["server_timestamp"]) * 1000;
                        BaseManager.userGameData.LastSave = t;
                        BaseManager.instance.PlayTimeMng.setStoreCheck(BaseManager.userGameData.LastSave);
                        
                        getTimeAction?.Invoke();
                        // DateTime lastDate = gameValues.epoch.AddMilliseconds(t);
                    }
                }
                else
                {                    
                    Debug.Log("Fail");
                }
            });
        }

        public void getPostCount(Action<bool> getCountAction)
        {
            plugin.AccessEvent((state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    if (dictionary.ContainsKey("postbox_count"))
                    {
                        int cnt = int.Parse((string)dictionary["postbox_count"]);
                        getCountAction?.Invoke(cnt > 0);
                    }
                }
                else
                {                    
                    Debug.Log("Fail");
                }
            });
        }

        #region [ NANOO RANKING ]         

        /// <summary> 리더보드에서 랭킹 가져오기 </summary>
        public void getRankingTotal(levelKey lvl, Action<Dictionary<string, object>> action)
        {
            string str = static_RANK_CODE[(int)lvl];
            plugin.Ranking(str, 28, (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    action?.Invoke(dictionary);
                }
                else
                {
                    Debug.Log("getRankingTotal : Fail");
                }
            });
        }

        /// <summary> 시즌 랭킹 등록 </summary>
        public void setSeasonRankingRecord(levelKey lvl)
        {
            long record = BaseManager.userGameData.TimeRecord(lvl) * 1000 
                + BaseManager.userGameData.RecordBoss(lvl);
            plugin.RankingRecord(static_RANK_CODE[(int)lvl], record, BaseManager.userGameData.getRankData(BaseManager.userGameData.RecordSkin(lvl)), (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    Debug.Log("Success");
                }
                else
                {
                    Debug.Log("setRankingRecord : Fail");
                }
            });
        }

        /// <summary> 리더보드에서 퍼스널랭킹 가져오기 </summary>
        public void getRankingPersonal(levelKey lvl, Action<Dictionary<string, object>> action)
        {
            string str = static_RANK_CODE[(int)lvl];
            plugin.RankingPersonal(str, (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    action?.Invoke(dictionary);
                }
                else
                {
                    Debug.Log("getRankingPersonal : Fail");
                }
            });
        }

        /// <summary> 리더보드에서 이전 (시즌)퍼스널랭킹 가져오기 </summary>
        public void getPreSeasonRankingPersonal(Action<Dictionary<string, object>> action)
        {
            plugin.RankingPersonal(PRE_RANK_CODE, (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    action?.Invoke(dictionary);
                }
                else
                {
                    Debug.Log("getRankingPersonal : Fail");
                }
            });
        }

        #endregion

        #region [ Nanoo postbox ] 

        [Button]
        public void boxlist()
        {
            getPostboxList((Dictionary<string, object> dictionary) =>
            {
                ArrayList items = (ArrayList)dictionary["item"];
                foreach (Dictionary<string, object> item in items)
                {
                    Debug.Log(item["uid"]);
                    Debug.Log(item["message"]);
                    Debug.Log(item["item_code"]);
                    Debug.Log(item["item_count"]);
                    Debug.Log(item["expire_sec"]);
                }
            });
        }

        /// <summary> 우편함 받아오기 </summary>
        public void getPostboxList(Action<Dictionary<string, object>> setList)
        {
            plugin.PostboxItem((state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    Debug.Log("getPostboxList : " + rawData);

                    setList?.Invoke(dictionary);
                }
                else
                {
                    Debug.Log("getPostboxList : Fail");
                }
            });
        }

        /// <summary> 우편함 보내기 </summary>
        public void PostboxItemSend(nanooPost post, int amount, string postMessage)
        {
            plugin.PostboxItemSend(post.ToString(), amount, 7, postMessage, (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    Debug.Log("PostboxItemSend : Success");
                }
                else
                {
                    Debug.Log("PostboxItemSend : Fail");
                }
            });
        }

        /// <summary> 우편함 사용하기 </summary>
        public void PostboxItemUse(string postId, Action<Dictionary<string, object>> getItem)
        {
            plugin.PostboxItemUse(postId, (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    getItem?.Invoke(dictionary);
                }
                else
                {
                    Debug.Log("PostboxItemUse : Fail");
                }
            });
        }

        /// <summary> 우편함 한번에 다 사용하기? </summary>
        public void PostboxMultiItemUse(ArrayList items, Action<Dictionary<string, object>> getItem)
        {
            plugin.PostboxMultiItemUse(items, (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    getItem?.Invoke(dictionary);
                }
                else
                {
                    Debug.Log("Fail");
                }
            });
        }

        /// <summary> 우편함 비우기 </summary>
        public void PostboxClear()
        {
            plugin.PostboxClear((state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    Debug.Log("Success");
                }
                else
                {
                    Debug.Log("Fail");
                }
            });
        }

        #endregion

        #region [ Nanoo coupon ]

        public void Coupon(string couponkey, Action<Dictionary<string, object>> getPresent)
        {
            plugin.Coupon(couponkey, (state, message, rawData, dictionary) =>
            {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    getPresent?.Invoke(dictionary);
                }
                else
                {
                    Debug.Log("Fail");
                }
            });
        }

        #endregion
    }
}