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

            notOpenRefreshCheck();
            close();
        }

        public void open()
        {
            gameObject.SetActive(true);

            notOpenRefreshCheck();
        }

        public void notOpenRefreshCheck()
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
                    getPostBox().setBox(item, _lobby, gameObject.activeSelf);
                }

                refreshCheckPost();
            });
        }

        void refreshCheckPost()
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
            ArrayList array = new ArrayList();
            for(int i = 0; i < _boxies.Count;i++)
            {
                if (_boxies[i].IsUsed)
                {
                    array.Add(_boxies[i].PostId);
                }
            }

            if (array.Count == 0)
            {
                WindowManager.instance.Win_message.showMessage("남은 택배가 없어요!");
                return;
            }

            NanooManager.instance.PostboxMultiItemUse(array, (dictionary) =>
            {
                ArrayList useItems = (ArrayList)dictionary["item"];
                nanooPost post;
                int mount;

                int gem = 0, coin = 0, ap = 0;
                foreach (Dictionary<string, object> item in useItems)
                {
                    
                    post = EnumHelper.StringToEnum<nanooPost>((string)item["item_code"]);
                    mount = int.Parse((string)item["item_count"]);

                    switch (post)
                    {
                        case nanooPost.gem:
                            gem += mount;
                            break;
                        case nanooPost.coin:
                            coin += mount;
                            break;
                        case nanooPost.ap:
                            ap += mount;
                            break;
                        case nanooPost.skin:
                            bool result = (BaseManager.userGameData.HasSkin & (1 << mount)) > 0;
                            if (result == false)
                            {
                                BaseManager.userGameData.HasSkin |= (1 << mount);
                            }
                            break;
                    }
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
            });
        }

        public void close()
        {
            //for (int i = 0; i < _boxies.Count; i++)
            //{
            //    _boxies[i].clear();
            //}

            refreshCheckPost();
            gameObject.SetActive(false);
        }
    }
}