using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeStage.AntiCheat.ObscuredTypes;

namespace week
{
    public class postBoxScript : MonoBehaviour
    {        
        [SerializeField] Image _postImg;
        [SerializeField] TextMeshProUGUI _postHead;
        [SerializeField] GameObject[] _innerPostBox;
        TextMeshProUGUI[] _innerPostMsg;
        [SerializeField] TextMeshProUGUI _postExpire; // 남은 시간

        RectTransform _postSize; // 위아래 사이즈

        LobbyScene _lobby;
        Action<postBoxScript> _whenRequestPost;
        string _postId;
        int _time;

        enum pmsg { key, msg, coin, gem, max }
        public ObscuredString[] PostMsgStr { get; private set; }

        public string PostId { get => _postId; }

        /// <summary> 초기화 </summary>
        private void Awake()
        {
            _postSize = GetComponent<RectTransform>();
            _innerPostMsg = new TextMeshProUGUI[_innerPostBox.Length];
            for (int i = 0; i < _innerPostBox.Length; i++)
            {
                _innerPostMsg[i] = _innerPostBox[i].GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        /// <summary> 포스트 박스 세팅 </summary>
        public void setBox(Dictionary<string, object> item, LobbyScene lobby, Action<postBoxScript> whenRequestPost)
        {
            gameObject.SetActive(true);

            _lobby = lobby;
            _whenRequestPost = whenRequestPost;

            transform.localScale = Vector3.one;

            // 포스트

            _postId         = (string)item["uid"];
            nanooPost post  = EnumHelper.StringToEnum<nanooPost>((string)item["item_code"]);

            //Debug.Log($"택배 내용 : {_postId}, {post.ToString()},");

            string[] str    = item["message"].ToString().Split('/');
            PostMsgStr      = new ObscuredString[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                //Debug.Log($"item{i} : " + str[i]);
                PostMsgStr[i] = str[i];
            }

            if (post == nanooPost.pack)
            {
                _postHead.text = PostMsgStr[1];

                string msg = "";
                int size = -1;

                for (int i = 0; i < 2; i++) // _innerPostBox.Length == 2
                {
                    int mount = int.Parse(PostMsgStr[2 + i]); // 0 번호, 1 상품, 2 코, 3 젬
                    _innerPostBox[i].SetActive(mount > 0);

                    if (mount == 0)
                        continue;

                    switch ((nanooPost)i)
                    {
                        case nanooPost.coin:
                            msg = $"<color=#FFC52C>{mount}코인</color>을 받았습니다.";
                            break;
                        case nanooPost.gem:
                            msg = $"<color=#F758A6>보석 {mount}개</color>를 받았습니다.";
                            break;
                    }

                    _innerPostMsg[i].text = msg;

                    size++;
                }

                _postSize.sizeDelta = new Vector2(_postSize.sizeDelta.x, 205f + 105f * size);
            }
            else
            {
                int mount = int.Parse((string)item["item_count"]);

                _innerPostBox[0].SetActive(true);
                _innerPostBox[1].SetActive(false);

                switch (post)
                {
                    case nanooPost.coin:
                        _innerPostMsg[0].text = $"<color=#FFC52C>{mount}코인</color>을 받았습니다.";
                        break;
                    case nanooPost.gem:
                        _innerPostMsg[0].text = $"<color=#F758A6>보석 {mount}개</color>를 받았습니다.";
                        break; 
                    case nanooPost.skin:
                        string name = D_skin.GetEntity(((SkinKeyList)mount).ToString()).f_skinname;
                        _innerPostMsg[0].text = $"{name}을 받았습니다.";
                        break;
                }
            }

            _time = int.Parse((string)item["expire_sec"]);

            calTime();

            StartCoroutine(chkTime());
        }

        /// <summary> 포스트 박스 세팅 </summary>
        IEnumerator chkTime()
        {
            while (_time > 0)
            {
                _time -= 1;
                calTime();

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary> 포스트 박스 시간 => string </summary>
        public void calTime()
        {
            int day = _time / 86400;
            if (day > 0)
            {
                _postExpire.text = $"{day}일";
                return;
            }

            day = _time / 3600;
            if (day > 0)
            {
                _postExpire.text = $"{day}시간";
                return;
            }

            day = _time / 60;
            if (day > 0)
            {
                _postExpire.text = $"{day}분";
                return;
            }

            _postExpire.text = $"{day}초";
        }

        /// <summary> 수령 요청 </summary>
        public void requestPost()
        {
            StartCoroutine(getPostRoutine());
        }

        IEnumerator getPostRoutine()
        {
            bool complete = false;
            NanooManager.instance.PostboxItemUse(_postId, (dictionary) =>
            {
                nanooPost post = EnumHelper.StringToEnum<nanooPost>((string)dictionary["item_code"]);
                Context context = null;

                string _key = PostMsgStr[(int)pmsg.key];

                if (post == nanooPost.pack)
                {
                    int coin, gem;
                    if ((coin = int.Parse(PostMsgStr[(int)pmsg.coin])) > 0)
                    {
                        BaseManager.userGameData.Coin += coin;
                        WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.CoinTxt.position, currency.coin, coin);
                    }

                    if ((gem = int.Parse(PostMsgStr[(int)pmsg.gem])) > 0)
                    {
                        BaseManager.userGameData.Gem += gem;
                        WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.gem, gem);
                    }

                    context = new Context(_key, analyticsWhere.post.ToString()).setProduct(post.ToString(), coin, gem);
                }
                else
                {
                    int mount = int.Parse((string)dictionary["item_count"]);

                    switch (post)
                    {
                        case nanooPost.coin:
                            BaseManager.userGameData.Coin += mount;
                            WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.CoinTxt.position, currency.coin, mount);
                            break;
                        case nanooPost.gem:
                            BaseManager.userGameData.Gem += mount;
                            WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.gem, mount);
                            break; 
                        case nanooPost.skin:
                            BaseManager.userGameData.HasSkin |= (1 << mount);
                            break;
                    }

                    int[] cur = new int[2] { 0, 0 };
                    cur[(int)post] = mount;

                    context = new Context(_key, analyticsWhere.post.ToString()).setProduct(post.ToString(), cur[0], cur[1]);
                }

                AnalyticsManager.instance.Send("getPost", context, null);

                complete = true;
            });

            yield return new WaitUntil(() => complete == true);

            AuthManager.instance.SaveDataServer(true);
            _whenRequestPost?.Invoke(this);

            clear();
        }

        /// <summary> 초기화 </summary>
        public void clear()
        {
            _postSize.sizeDelta = new Vector2(_postSize.sizeDelta.x, 205f);
            gameObject.SetActive(false);
        }
    }
}