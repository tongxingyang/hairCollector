using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

namespace week
{
    public class AnalyticsManager : TSingleton<AnalyticsManager>
    {
        string user_id;

        protected override void Init()
        {

        }
        #region [ Analystics ]

        public void AnalyticsLogin()
        {
            FirebaseAnalytics.SetUserId(user_id);
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        }

        // 공통 전송 부분 + 로그 전송
        //public void Send(string eventName, Context context, List<Parameter> parameters)
        //{
        //    var temp = new List<Parameter>();
        //    temp.Add(new Parameter("dateTime", long.Parse(DateTime.Now.ToString("yyMMddHHmmss"))));
        //    temp.Add(new Parameter("user_id", user_id));
        //    temp.Add(new Parameter("nickname", main.accountDataCtrl.nickname));
        //    temp.Add(new Parameter("account_level", main.accountDataCtrl.accountLevel));
        //    temp.Add(new Parameter("play_minute", main.timeDataCtrl.playMinutes));

        //    if (context != null)
        //    {
        //        temp.Add(new Parameter("context_id", context.id));
        //        temp.Add(new Parameter("where", context.where.ToString()));

        //        if (string.IsNullOrWhiteSpace(context.source) == false)
        //            temp.Add(new Parameter("source", context.source));

        //        if (string.IsNullOrWhiteSpace(context.source2) == false)
        //            temp.Add(new Parameter("source2", context.source2));

        //        if (context.stat_factor > 0f)
        //            temp.Add(new Parameter("stat_factor", context.stat_factor));
        //    }

        //    var list = new List<Parameter>();
        //    list.AddRange(temp);

        //    if (parameters != null)
        //        list.AddRange(parameters);

        //    FirebaseAnalytics.LogEvent(eventName, list.ToArray());
        //}

        #endregion


        //// 공통 전송 부분 + 로그 전송
        //public void Send(string eventName, Context context, List<Parameter> parameters)
        //{
        //    var temp = new List<Parameter>();
        //    temp.Add(new Parameter("dateTime", long.Parse(DateTime.Now.ToString("yyMMddHHmmss"))));
        //    temp.Add(new Parameter("user_id", user_id));
        //    temp.Add(new Parameter("nickname", main.accountDataCtrl.nickname));
        //    temp.Add(new Parameter("account_level", main.accountDataCtrl.accountLevel));
        //    temp.Add(new Parameter("play_minute", main.timeDataCtrl.playMinutes));

        //    if (context != null)
        //    {
        //        temp.Add(new Parameter("context_id", context.id));
        //        temp.Add(new Parameter("where", context.where.ToString()));

        //        if (string.IsNullOrWhiteSpace(context.source) == false)
        //            temp.Add(new Parameter("source", context.source));

        //        if (string.IsNullOrWhiteSpace(context.source2) == false)
        //            temp.Add(new Parameter("source2", context.source2));

        //        if (context.stat_factor > 0f)
        //            temp.Add(new Parameter("stat_factor", context.stat_factor));
        //    }

        //    var list = new List<Parameter>();
        //    list.AddRange(temp);

        //    if (parameters != null)
        //        list.AddRange(parameters);

        //    FirebaseAnalytics.LogEvent(eventName, list.ToArray());
        //}

        //public void CashShopProcessPurchase(Context context, string productId, string receipt)
        //{
        //    var temp = new List<Parameter>();
        //    temp.Add(new Parameter("product_id", productId));
        //    temp.Add(new Parameter("receipt", receipt));

        //    Send("cash_shop_process_purchase", context, temp);
        //}
    }
}