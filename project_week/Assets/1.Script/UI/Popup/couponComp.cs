using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class couponComp : MonoBehaviour, UIInterface
    {
        [SerializeField] InputField _input;
        LobbyScene _lobby;
        Action _whenClose;
        public void Init(LobbyScene lobby,Action whenClose)
        {
            _lobby = lobby;
            _whenClose = whenClose;
            close();
        }

        public void open()
        {            
            _input.text = "";
            gameObject.SetActive(true);
        }

        public void close()
        {
            gameObject.SetActive(false);
            _whenClose?.Invoke();
        }

        public void useCoupon()
        {
            if (string.IsNullOrWhiteSpace(_input.text))
                return;

            StartCoroutine(useCouponRoutine());
        }

        IEnumerator useCouponRoutine()
        {
            yield return null;

            Context context = null;
            productKeyList presentSize = productKeyList.m_gem;

            bool complete = false;
            NanooManager.instance.Coupon(_input.text, (dictionary) =>
            {
                nanooPost present = EnumHelper.StringToEnum<nanooPost>((string)dictionary["item_code"]);
                int mount = int.Parse((string)dictionary["item_count"]);

                switch (present)
                {
                    case nanooPost.coin:
                        presentSize = productKeyList.m_coin;
                        BaseManager.userGameData.Coin += mount;
                        break;
                    case nanooPost.gem:
                        presentSize = productKeyList.m_gem;
                        BaseManager.userGameData.Gem += mount;
                        break;
                }

                WindowManager.instance.Win_purchase.setCoupon(D_product.GetEntity(presentSize.ToString()).f_image, $"보석{mount}개 선물")
                .setImgSize(false);

                WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.gem, mount);
                
                string key = AnalyticsManager.instance.getKey();

                context = new Context(key, analyticsWhere.store.ToString())
                   .setProduct(D_product.GetEntity(presentSize.ToString()).f_name, 0, 0);
                

                complete = true;
            });
            
            yield return new WaitUntil(() => complete == true);

            _lobby.refresh_Cost();
            AuthManager.instance.SaveDataServer(true);
            AnalyticsManager.instance.Send("coupon_" + presentSize.ToString(), context, null);
            _input.text = "";
        }
    }
}