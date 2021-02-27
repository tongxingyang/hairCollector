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
        [SerializeField] GameObject[] _postMsgBox;
        [SerializeField] TextMeshProUGUI _postExpire;

        RectTransform _postSize;
        TextMeshProUGUI[] _postMsg;

        LobbyScene _lobby;
        bool _isUsed;
        string _postId;
        int _time;

        enum pmsg { key, msg, coin, gem, ap }
        ObscuredString[] _postmsg;
        public ObscuredString[] Postmsg { get { return _postmsg; } }

        public bool IsUsed { get => _isUsed; }
        public string PostId { get => _postId; }

        private void Awake()
        {
            _postSize = GetComponent<RectTransform>();
            _postMsg = new TextMeshProUGUI[3];
            for (int i = 0; i < 3; i++)
            {
                _postMsg[i] = _postMsgBox[i].GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        /// <summary> 포스트 박스 세팅 </summary>
        public void setBox(Dictionary<string, object> item, LobbyScene lobby)
        {
            _isUsed = true;            

            gameObject.SetActive(true);
            transform.localScale = Vector3.one;
            _lobby = lobby;

            _postId = (string)item["uid"];
            // _postImg.sprite = null;
            nanooPost post = EnumHelper.StringToEnum<nanooPost>((string)item["item_code"]);

            _postmsg = new ObscuredString[5];
            string[] str = item["message"].ToString().Split('/');
            for (int i = 0; i < 5; i++)
            {
                _postmsg[i] = str[i];
            }

            if (post == nanooPost.pack)
            {
                _postHead.text = _postmsg[1];

                string msg = "";
                int size = -1;
                for (int i = 0; i < 3; i++)
                {
                    int mount = int.Parse(_postmsg[2 + i]);
                    _postMsgBox[i].SetActive(mount > 0);

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
                        case nanooPost.ap:
                            msg = $"<color=#38DDB2>{mount}AP</color>를 받았습니다.";
                            break;
                    }

                    _postMsg[i].text = msg;
                    size++;
                }

                _postSize.sizeDelta = new Vector2(_postSize.sizeDelta.x, 205f + 110f * size);
            }
            else
            {
                int mount = int.Parse((string)item["item_count"]);

                _postMsgBox[0].SetActive(true);
                _postMsgBox[1].SetActive(false);
                _postMsgBox[2].SetActive(false);

                switch (post)
                {
                    case nanooPost.coin:
                        _postMsg[0].text = $"<color=#FFC52C>{mount}코인</color>을 받았습니다.";
                        break;
                    case nanooPost.gem:
                        _postMsg[0].text = $"<color=#F758A6>보석 {mount}개</color>를 받았습니다.";
                        break;                    
                    case nanooPost.ap:
                        _postMsg[0].text = $"<color=#38DDB2>{mount}AP</color>를 받았습니다.";
                        break;
                    case nanooPost.skin:
                        string name = DataManager.GetTable<string>(DataTable.skin, ((SkinKeyList)mount).ToString(), SkinValData.skinname.ToString());
                        _postMsg[0].text = $"{name}을 받았습니다.";
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
            while (_isUsed && _time > 0)
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

                string _key = _postmsg[(int)pmsg.key];

                if (post == nanooPost.pack)
                {
                    int coin, gem, ap;
                    if ((coin = int.Parse(_postmsg[(int)pmsg.coin])) > 0)
                    {
                        BaseManager.userGameData.Coin += coin;
                        WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.CoinTxt.position, currency.coin, coin);
                    }

                    if ((gem = int.Parse(_postmsg[(int)pmsg.gem])) > 0)
                    {
                        BaseManager.userGameData.Gem += gem;
                        WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.gem, gem);
                    }

                    if ((ap = int.Parse(_postmsg[(int)pmsg.ap])) > 0)
                    {
                        BaseManager.userGameData.Ap += ap;
                        WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.CoinTxt.position, currency.ap, ap);
                        _lobby.refreshAp();
                    }

                    context = new Context(_key, analyticsWhere.post.ToString()).setProduct(post.ToString(), coin, gem, ap);
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
                        case nanooPost.ap:
                            BaseManager.userGameData.Ap += mount;
                            WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.CoinTxt.position, currency.ap, mount);
                            _lobby.refreshAp();
                            break;
                        case nanooPost.skin:
                            BaseManager.userGameData.HasSkin |= (1 << mount);
                            break;
                    }

                    int[] cur = new int[3] { 0, 0, 0 };
                    cur[(int)post] = mount;

                    context = new Context(_key, analyticsWhere.post.ToString()).setProduct(post.ToString(), cur[0], cur[1], cur[2]);
                }

                AnalyticsManager.instance.Send("getPost", context, null);

                complete = true;
            });

            yield return new WaitUntil(() => complete == true);

            AuthManager.instance.SaveDataServer(true);

            clear();
        }

        /// <summary> 초기화 </summary>
        public void clear()
        {
            _isUsed = false; 
            _postSize.sizeDelta = new Vector2(_postSize.sizeDelta.x, 205f);
            gameObject.SetActive(false);
        }
    }
}