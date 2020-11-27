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
        string RANK_CODE = "snowadventure-RANK-9A9E5FB8-93912F59";


        protected override void Init()
        {
            Debug.Log("나누 초기화");

            plugin = Plugin.GetInstance();
            plugin.transform.SetParent(transform);
            //AccessEvent();
        }

        public void setUid(string uid)
        {
            Debug.Log("uid 세팅");

            plugin.SetUUID(uid);
            plugin.SetNickname(BaseManager.userGameData.NickName);
            plugin.SetLanguage(Configure.PN_LANG_KO);
        }

        public void AccessEvent()
        {
            plugin.AccessEvent((state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    if (dictionary.ContainsKey("open_id"))
                    {
                        Debug.Log(dictionary["open_id"]);
                    }

                    if (dictionary.ContainsKey("server_timestamp"))
                    {
                        Debug.Log(dictionary["server_timestamp"]);
                    }

                    if (dictionary.ContainsKey("postbox_subscription"))
                    {
                        foreach (Dictionary<string, object> subscription in (ArrayList)dictionary["postbox_subscription"])
                        {
                            Debug.Log(subscription["product"]);
                            Debug.Log(subscription["ttl"]);
                        }
                    }

                    if (dictionary.ContainsKey("invite_rewards"))
                    {
                        foreach (Dictionary<string, object> invite in (ArrayList)dictionary["invite_rewards"])
                        {
                            Debug.Log(invite["item_code"]);
                            Debug.Log(invite["item_count"]);
                        }
                    }
                }
                else
                {
                    Debug.Log("Fail");
                }
                Debug.Log(rawData);
            });
        }

        #region [ NANOO RANKING ] 

        /// <summary> 리더보드에서 랭킹 가져오기 </summary>
        public void getRankingTotal(Action<Dictionary<string,object>> action)
        {
            plugin.Ranking(RANK_CODE, 28, (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    Debug.Log("rank : " + rawData);

                    action?.Invoke(dictionary);
                }
                else
                {
                    Debug.Log("getRankingTotal : Fail");
                }
            });
        }

        /// <summary> 랭킹 등록 </summary>
        public void setRankingRecord()
        {
            long record = BaseManager.userGameData.TimeRecord * 1000 + BaseManager.userGameData.BossRecord;
            plugin.RankingRecord(RANK_CODE, record, BaseManager.userGameData.getRankData(), (state, message, rawData, dictionary) => {
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

        /// <summary> Personal Query in LeaderBoard </summary>
        public void getRankingPersonal()
        {
            plugin.RankingPersonal(RANK_CODE, (state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    Debug.Log(dictionary["ranking"]);
                    Debug.Log(dictionary["data"]);
                    Debug.Log(dictionary["total_player"]);
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

        [Button]
        public void send()
        {
            PostboxItemSend(nanooPost.gem, 10);
        }

        /// <summary> 우편함 받아오기 </summary>
        public void getPostboxList(Action<Dictionary<string,object>> setList)
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
        public void PostboxItemSend(nanooPost post, int amount)
        {
            plugin.PostboxItemSend(post.ToString(), amount, 7, "메세지",(state, message, rawData, dictionary) => {
                if (state.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    Debug.Log("PostboxItemSend : Success");
                    Debug.Log("PostboxItemSend : " + rawData);
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
    }
}