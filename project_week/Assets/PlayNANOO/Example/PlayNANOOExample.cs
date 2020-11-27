using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PlayNANOO;
using PlayNANOO.SimpleJSON;

public class PlayNANOOExample : MonoBehaviour
{
    Plugin plugin;
    void Start()
    {
        plugin = Plugin.GetInstance();

        plugin.SetUUID("PLAYNANOO:TEST:000001");
        plugin.SetNickname("TESTER00001");
        plugin.SetLanguage(Configure.PN_LANG_KO);

        //AccessEvent();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Open Forum Banner
    /// </summary>
    public void OpenBanner()
    {
        plugin.OpenBanner();
    }

    /// <summary>
    /// Open Forum
    /// </summary>
    public void OpenForum()
    {
        plugin.OpenForum();
    }

    /// <summary>
    /// Open HelpDesk
    /// </summary>
    public void OpenHelpDesk()
    {
        plugin.SetHelpDeskOptional("OptionTest1", "ValueTest1");
        plugin.SetHelpDeskOptional("OptionTest2", "ValueTest2");
        plugin.OpenHelpDesk();
    }

    /// <summary>
    /// Data Query in Forum
    /// </summary>
    public void ForumThread()
    {
        plugin.ForumThread(Configure.PN_FORUM_THREAD, 10, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                ArrayList list = (ArrayList)dictionary["list"];
                foreach (Dictionary<string, object> thread in list)
                {
                    Debug.Log(thread["seq"]);
                    Debug.Log(thread["title"]);
                    Debug.Log(thread["summary"]);
                    Debug.Log(thread["attach_file"]);
                    Debug.Log(thread["url"]);
                    Debug.Log(thread["post_date"]);

                    foreach (string attach in (ArrayList)thread["attach_file"])
                    {
                        Debug.Log(attach);
                    }
                }
            }
            else
            {
                Debug.Log("실패");
            }
        });
    }

    /// <summary>
    /// Server Time
    /// </summary>
    public void ServerTime()
    {
        plugin.ServerTime((state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["timezone"]);
                Debug.Log(dictionary["timestamp"]);
                Debug.Log(dictionary["ISO_8601_date"]);
                Debug.Log(dictionary["date"]);
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// AccessEvent
    /// </summary>
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
        });
    }

    /// <summary>
    /// Coupon
    /// </summary>
    public void Coupon()
    {
        plugin.Coupon("COUPON_CODE", (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["code"]);
                Debug.Log(dictionary["item_code"]);
                Debug.Log(dictionary["item_count"]);
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    
    /// <summary>
    /// Data Save in Cloud Data
    /// </summary>
    public void StorageSave()
    {
        plugin.StorageSave("TEST_KEY_001", "TEST_VALUE_001", false, (state, message, rawData, dictionary) => {
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

    /// <summary>
    /// Data Load in Cloud Data
    /// </summary>
    public void StorageLoad()
    {
        plugin.StorageLoad("TEST_KEY_001", (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["value"]);
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    

    /// <summary>
    /// Season Query in LeaderBoard
    /// </summary>
    public void RankingSeason()
    {
        plugin.RankingSeasonInfo("RANKING_CODE", (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["season"]);
                Debug.Log(dictionary["expire_sec"]);
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// Android Verification
    /// </summary>
    public void IapReceiptionAndroid()
    {
        plugin.ReceiptVerificationAOS("PRODUCT_ID", "RECEIPT", "SIGNATURE", "CURRENCY", 100, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["package"]);
                Debug.Log(dictionary["product_id"]);
                Debug.Log(dictionary["order_id"]);
                Debug.Log("Issue Item");
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// iOS Verification
    /// </summary>
    public void IapReceiptioniOS()
    {
        plugin.ReceiptVerificationIOS("PRODUCT_ID", "RECEIPT", "CURRENCY", 100, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["package"]);
                Debug.Log(dictionary["product_id"]);
                Debug.Log(dictionary["order_id"]);
                Debug.Log("Issue Item");
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// ONE store(KR) Verification
    /// </summary>
    public void IapReceiptionOneStoreKR()
    {
        plugin.ReceiptVerificationOneStoreKR("PRODUCT_ID", "PURCHASE_ID", "RECEIPT", "CURRENCY", 100, true, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["package"]);
                Debug.Log(dictionary["product_id"]);
                Debug.Log(dictionary["order_id"]);
                Debug.Log("Issue Item");
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// Invite
    /// </summary>
    /// <param name="inviteCode">Invite code.</param>
    public void Invite(string inviteCode)
    {
        plugin.Invite(inviteCode, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                string url = dictionary["url"].ToString();

                plugin.OpenShare("Please Enter Invite Message", url);
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// Cache Exists
    /// </summary>
    public void CacheExists()
    {
        string cacheKey = "TEST001";
        plugin.CacheExists(cacheKey, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["value"].ToString());
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// Cache Get
    /// </summary>
    public void CacheGet()
    {
        string cacheKey = "TEST001";
        plugin.CacheGet(cacheKey, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["value"].ToString());
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// Cache Multi Get
    /// </summary>
    public void CacheMultiGet()
    {
        ArrayList cacheKeys = new ArrayList();
        cacheKeys.Add("TEST001");
        cacheKeys.Add("TEST002");
        cacheKeys.Add("TEST003");

        plugin.CacheMultiGet(cacheKeys, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                foreach (Dictionary<string, object> value in (ArrayList)dictionary["values"])
                {
                    Debug.Log(value["key"]);
                    Debug.Log(value["value"]);
                }
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// Cache Set
    /// </summary>
    public void CacheSet()
    {
        string cacheKey = "TEST001";
        string cacheValue = "TESTValue";
        int cacheTTL = 3600;
        plugin.CacheSet(cacheKey, cacheValue, cacheTTL, (state, message, rawData, dictionary) => {
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

    /// <summary>
    /// Cache Incrby
    /// </summary>
    public void CacheIncrby()
    {
        string cacheKey = "TEST002";
        int cacheValue = 100;
        int cacheTTL = 3600;
        plugin.CacheIncrby(cacheKey, cacheValue, cacheTTL, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["value"].ToString());
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// Cache Decrby 
    /// </summary>
    public void CacheDecrby()
    {
        string cacheKey = "TEST003";
        int cacheValue = 100;
        int cacheTTL = 3600;
        plugin.CacheDecrby(cacheKey, cacheValue, cacheTTL, (state, message, rawData, dictionary) => {
            if (state.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log(dictionary["value"].ToString());
            }
            else
            {
                Debug.Log("Fail");
            }
        });
    }

    /// <summary>
    /// Cache Delete 
    /// </summary>
    public void CacheDelete()
    {
        string cacheKey = "TEST003";
        plugin.CacheDel(cacheKey, (state, message, rawData, dictionary) => {
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

    public void OnApplicationFocus(bool focus)
    {
        if (plugin != null && focus)
        {
            AccessEvent();
        }
        Debug.Log("Focus");
    }
}
