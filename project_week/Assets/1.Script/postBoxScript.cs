using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace week
{
    public class postBoxScript : MonoBehaviour
    {
        [SerializeField] Image _postImg;
        [SerializeField] TextMeshProUGUI _postHead;
        [SerializeField] TextMeshProUGUI _postMsg;
        [SerializeField] TextMeshProUGUI _postExpire;

        LobbyScene _lobby;
        bool _isUsed;
        string _postId;
        int _time;

        public bool IsUsed { get => _isUsed; }
        public string PostId { get => _postId; }

        /// <summary> 포스트 박스 세팅 </summary>
        public void setBox(Dictionary<string, object> item, LobbyScene lobby)
        {
            gameObject.SetActive(true);
            _lobby = lobby;
            _isUsed = true;

            _postId = (string)item["uid"];
            _postImg.sprite = null;
            _postHead.text = (string)item["message"];
            nanooPost post = EnumHelper.StringToEnum<nanooPost>((string)item["item_code"]);
            int mount = int.Parse((string)item["item_count"]);

            string msg = "";
            switch (post)
            {
                case nanooPost.gem:
                    msg = $"보석 {mount}개를 받았습니다.";
                    break;
                case nanooPost.coin:
                    msg = $"{mount}코인을 받았습니다.";
                    break;
                case nanooPost.ap:
                    msg = $"{mount}AP를 받았습니다.";
                    break;
                case nanooPost.skin:
                    string name = DataManager.GetTable<string>(DataTable.skin, ((SkinKeyList)mount).ToString(), SkinValData.skinname.ToString());
                    msg = $"{name}을 받았습니다.";
                    break;
            }
            _postMsg.text = msg;

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
            NanooManager.instance.PostboxItemUse(_postId, (dictionary) =>
            {
                nanooPost post = EnumHelper.StringToEnum<nanooPost>((string)dictionary["item_code"]);
                int mount = int.Parse((string)dictionary["item_count"]);

                switch (post)
                {
                    case nanooPost.gem:
                        BaseManager.userGameData.Gem += mount;
                        WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.gem, mount);
                        break;
                    case nanooPost.coin:
                        BaseManager.userGameData.Coin += mount;
                        WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.coin, mount);
                        break;
                    case nanooPost.ap:
                        BaseManager.userGameData.Ap += mount;
                        WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.ap, mount);
                        break;
                    case nanooPost.skin:
                        BaseManager.userGameData.HasSkin |= (1 << mount);
                        break;
                }

                clear();
            });
        }

        /// <summary> 초기화 </summary>
        public void clear()
        {
            _isUsed = false;
            gameObject.SetActive(false);
        }
    }
}