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

        readonly string static_RANK_CODE = "snowadventure-RANK-0D39E8A1-E2FF99FE";

        readonly string PRE_RANK_CODE = "snowadventure-RANK-21DE85D1-03980325";
        readonly string RANK_CODE = "snowadventure-RANK-A01270B3-7AFED569";

        public string getRANK_CODE => RANK_CODE;

        protected override void Init()
        {
            Debug.Log("나누 초기화");

            plugin = Plugin.GetInstance();
            plugin.transform.SetParent(transform);
            //AccessEvent();
        }

        public void setUid(string uid)
        {
            //Debug.Log("uid 세팅");

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

        //public void AccessEvent()
        //{
        //    plugin.AccessEvent((state, message, rawData, dictionary) => {
        //        if (state.Equals(Configure.PN_API_STATE_SUCCESS))
        //        {
        //            if (dictionary.ContainsKey("open_id"))
        //            {
        //                //Debug.Log(dictionary["open_id"]);
        //            }

        //            if (dictionary.ContainsKey("server_timestamp"))
        //            {
        //                Debug.Log("시간");
        //                long t = long.Parse((string)dictionary["server_timestamp"])*1000;
        //                Debug.Log(t);
        //                DateTime dt = new DateTime(t);
        //                Debug.Log(dt);
        //                DateTime lastDate = gameValues.epoch.AddMilliseconds(t);
        //                Debug.Log(lastDate);
        //            }

        //            if (dictionary.ContainsKey("postbox_subscription"))
        //            {
        //                foreach (Dictionary<string, object> subscription in (ArrayList)dictionary["postbox_subscription"])
        //                {
        //                    Debug.Log(subscription["product"]);
        //                    Debug.Log(subscription["ttl"]);
        //                }
        //            }

        //            if (dictionary.ContainsKey("invite_rewards"))
        //            {
        //                foreach (Dictionary<string, object> invite in (ArrayList)dictionary["invite_rewards"])
        //                {
        //                    Debug.Log(invite["item_code"]);
        //                    Debug.Log(invite["item_count"]);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("Fail");
        //        }
        //        // Debug.Log(rawData);
        //    });
        //}

        #region [ NANOO RANKING ]         

        /// <summary> 리더보드에서 랭킹 가져오기 </summary>
        public void getRankingTotal(bool isSeason, Action<Dictionary<string, object>> action)
        {
            string str = (isSeason) ? RANK_CODE : static_RANK_CODE;
            plugin.Ranking(str, 28, (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    // Debug.Log("rank : " + rawData);

                    action?.Invoke(dictionary);
                }
                else
                {
                    Debug.Log("getRankingTotal : Fail");
                }
            });
        }

        /// <summary> 시즌 랭킹 등록 </summary>
        public void setSeasonRankingRecord(int boss)
        {
            long record = BaseManager.userGameData.SeasonTimeRecord * 1000 + boss;
            plugin.RankingRecord(RANK_CODE, record, BaseManager.userGameData.getRankData(BaseManager.userGameData.RecordSeasonSkin), (state, message, rawData, dictionary) => {
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

        /// <summary> 전체 랭킹 등록 </summary>
        public void setAllRankingRecord(int boss)
        {
            long record = BaseManager.userGameData.AllTimeRecord * 1000 + boss;
            plugin.RankingRecord(static_RANK_CODE, record, BaseManager.userGameData.getRankData(BaseManager.userGameData.RecordAllSkin), (state, message, rawData, dictionary) => {
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
        public void getRankingPersonal(bool isSeason, Action<Dictionary<string, object>> action)
        {
            string str = (isSeason) ? RANK_CODE : static_RANK_CODE;
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
                    // Debug.Log("PostboxItemSend : " + rawData);
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
                    //Debug.Log(dictionary["item_code"]);
                    //Debug.Log(dictionary["item_count"]);
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
                    //ArrayList useItems = (ArrayList)dictionary["item"];
                    //foreach (Dictionary<string, object> item in useItems)
                    //{
                    //    Debug.Log(item["uid"]);
                    //    Debug.Log(item["item_code"]);
                    //    Debug.Log(item["item_count"]);
                    //}
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
                    //Debug.Log(dictionary["code"]);
                    //Debug.Log(dictionary["item_code"]);
                    //Debug.Log(dictionary["item_count"]);
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