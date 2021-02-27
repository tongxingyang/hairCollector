using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class postComp : MonoBehaviour, UIInterface
    {
        [SerializeField] Transform _contents;
        [SerializeField] GameObject _postBoxFab;

        LobbyScene _lobby;
        Action<bool> _exclamation;
        List<postBoxScript> _boxies;

        public void Init(LobbyScene lobby, Action<bool> exclamation)
        {
            _lobby = lobby;
            _exclamation = exclamation;
            _boxies = new List<postBoxScript>();

            NanooManager.instance.getPostCount(_exclamation);
            close();
        }

        public void open()
        {
            gameObject.SetActive(true);

            whenOpenRefreshCheck();
        }

        public void whenOpenRefreshCheck()
        {         
            NanooManager.instance.getPostboxList((dictionary) =>
            {
                for (int i = 0; i < _boxies.Count; i++)
                {
                    _boxies[i].clear();
                }

                ArrayList items = (ArrayList)dictionary["item"];
                foreach (Dictionary<string, object> item in items)
                {
                    getPostBox().setBox(item, _lobby);
                }
            });
        }

        void whenCloseCheckPost()
        {
            for (int i = 0; i < _boxies.Count; i++)
            {
                if (_boxies[i].IsUsed)
                {
                    _exclamation?.Invoke(true);
                    return;
                }
            }

            _exclamation?.Invoke(false);
        }



        /// <summary> 소포 가져오기 </summary>
        postBoxScript getPostBox()
        {
            for (int i = 0; i < _boxies.Count; i++)
            {
                if (_boxies[i].IsUsed == false)
                {
                    return _boxies[i];
                }
            }

            postBoxScript box = Instantiate(_postBoxFab).GetComponent<postBoxScript>();
            _boxies.Add(box);
            box.transform.SetParent(_contents);
            return box;
        }

        /// <summary> 수령 요청 </summary>
        public void requestAllPost()
        {
            StartCoroutine(requestAllPostRoutine());
        }

        IEnumerator requestAllPostRoutine()
        {
            ArrayList array = new ArrayList();
            for (int i = 0; i < _boxies.Count; i++)
            {
                if (_boxies[i].IsUsed)
                {
                    array.Add(_boxies[i].PostId);
                }
            }

            if (array.Count == 0)
            {
                WindowManager.instance.Win_message.showMessage("남은 택배가 없어요!");
                yield break;
            }

            bool complete = false;
            NanooManager.instance.PostboxMultiItemUse(array, (dictionary) =>
            {
                ArrayList useItems = (ArrayList)dictionary["item"];
                // nanooPost post;

                int coin = 0, gem = 0, ap = 0;
                //foreach (Dictionary<string, object> item in useItems)
                //{
                //    // post = EnumHelper.StringToEnum<nanooPost>((string)item["item_code"]);

                //    string[] postmsg = item["message"].ToString().Split('/');

                //    coin += int.Parse(postmsg[2]);
                //    gem += int.Parse(postmsg[3]);
                //    ap += int.Parse(postmsg[4]);
                //}

                for (int i = 0; i < _boxies.Count; i++)
                {                    
                    coin += int.Parse(_boxies[i].Postmsg[2]);
                    gem += int.Parse(_boxies[i].Postmsg[3]);
                    ap += int.Parse(_boxies[i].Postmsg[4]);
                }

                if (gem > 0)
                {
                    BaseManager.userGameData.Gem += gem;
                    WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.gem, gem);
                }

                if (coin > 0)
                {
                    BaseManager.userGameData.Coin += coin;
                    WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.coin, coin);
                }

                if (ap > 0)
                {
                    BaseManager.userGameData.Ap += ap;
                    WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.ap, ap);
                }

                for (int i = 0; i < _boxies.Count; i++)
                {
                    _boxies[i].clear();
                }

                complete = true;
            });

            yield return new WaitUntil(() => complete == true);

            AuthManager.instance.SaveDataServer(true);
        }

        public void close()
        {
            //for (int i = 0; i < _boxies.Count; i++)
            //{
            //    _boxies[i].clear();
            //}

            whenCloseCheckPost();
            gameObject.SetActive(false);
        }
    }
}