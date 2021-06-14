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
        [SerializeField] GameObject _empty;

        LobbyScene _lobby;
        Action<bool> _exclamation;
        List<postBoxScript> _usedBoxies; // 사용중인 택배
        Queue<postBoxScript> _offBoxies; // 풀링 택배

        /// <summary> 초기화 </summary>
        public void Init(LobbyScene lobby, Action<bool> exclamation)
        {
            _lobby = lobby;
            _exclamation = exclamation;
            _usedBoxies = new List<postBoxScript>();
            _offBoxies = new Queue<postBoxScript>();

            NanooManager.instance.getPostCount((bl)=> { 
                _exclamation(bl); 
                _empty.gameObject.SetActive(!bl); 
            });

            close();
        }

        /// <summary> 열기 </summary>
        public void open()
        {
            _empty.gameObject.SetActive(false);
            gameObject.SetActive(true);

            whenOpenRefreshCheck();
        }

        /// <summary> 닫기 </summary>
        public void close()
        {
            whenCloseCheckPost();

            gameObject.SetActive(false);
        }

        /// <summary> (오픈시) 내 택배 체크 및 불러오기 </summary>
        void whenOpenRefreshCheck()
        {
            NanooManager.instance.getPostboxList((dictionary) =>
            {
                for (int i = 0; i < _usedBoxies.Count;)
                {                    
                    postBoxScript pbs = _usedBoxies[0];
                    pbs.clear();
                    _usedBoxies.Remove(pbs);
                    _offBoxies.Enqueue(pbs);
                    pbs.gameObject.SetActive(false);                    
                }

                ArrayList items = (ArrayList)dictionary["item"];

                if (items.Count > 0)
                {
                    foreach (Dictionary<string, object> item in items)
                    {
                        getPostBox().setBox(item, _lobby, remainPostCheck);
                    }
                    _empty.SetActive(false);
                }
                else
                {
                    _empty.SetActive(true);
                }
            });
        }

        /// <summary> (닫을때) 남은 택배 여부 체크 </summary>
        void whenCloseCheckPost()
        {
            _exclamation?.Invoke(_usedBoxies.Count > 0);
        }

        void remainPostCheck(postBoxScript pbs = null)
        {
            if (pbs != null)
            {
                _usedBoxies.Remove(pbs);
                _offBoxies.Enqueue(pbs);
            }
                        
            if (_usedBoxies.Count == 0)
            {
                _empty.SetActive(true);
            }
        }

        /// <summary> 택배 풀링 가져오기 </summary>
        postBoxScript getPostBox()
        {
            if (_offBoxies.Count > 0)
            {
                postBoxScript pbs = _offBoxies.Dequeue();
                _usedBoxies.Add(pbs);
                return pbs;
            }

            postBoxScript box = Instantiate(_postBoxFab).GetComponent<postBoxScript>();
            _usedBoxies.Add(box);
            box.transform.SetParent(_contents);
            return box;
        }

        /// <summary> 모든 택배 수령 요청 </summary>
        public void requestAllPost()
        {
            StartCoroutine(requestAllPostRoutine());
        }

        IEnumerator requestAllPostRoutine()
        {
            ArrayList array = new ArrayList();
            for (int i = 0; i < _usedBoxies.Count; i++)
            {
                array.Add(_usedBoxies[i].PostId);
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

                int coin = 0, gem = 0;//, ap = 0;
                //foreach (Dictionary<string, object> item in useItems)
                //{
                //    // post = EnumHelper.StringToEnum<nanooPost>((string)item["item_code"]);

                //    string[] postmsg = item["message"].ToString().Split('/');

                //    coin += int.Parse(postmsg[2]);
                //    gem += int.Parse(postmsg[3]);
                //    ap += int.Parse(postmsg[4]);
                //}

                for (int i = 0; i < _usedBoxies.Count; i++)
                {                    
                    coin += int.Parse(_usedBoxies[i].PostMsgStr[2]);
                    gem += int.Parse(_usedBoxies[i].PostMsgStr[3]);
                    //ap += int.Parse(_usedBoxies[i].Postmsg[4]);
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

                //if (ap > 0)
                //{
                //    BaseManager.userGameData.Ap += ap;
                //    WindowManager.instance.Win_coinGenerator.getWealth2Point(transform.position, _lobby.GemTxt.position, currency.ap, ap);
                //}

                for (int i = 0; i < _usedBoxies.Count; )
                {
                    postBoxScript pbs = _usedBoxies[0];
                    pbs.clear();
                    _usedBoxies.Remove(pbs);
                    _offBoxies.Enqueue(pbs);
                }

                remainPostCheck();
                complete = true;
            });

            yield return new WaitUntil(() => complete == true);

            AuthManager.instance.SaveDataServer(true);
        }
    }
}