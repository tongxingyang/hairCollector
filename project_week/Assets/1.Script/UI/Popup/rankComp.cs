using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class rankComp : MonoBehaviour, UIInterface
    {
        [SerializeField] GameObject _rankBoxFab;
        [SerializeField] rankBox _myRankBox;
        [SerializeField] Transform _boxParent;

        List<rankBox> _boxes;

        bool _refable;
        float _refTime;
        readonly float _cool = 60f;

        public void Init()
        {
            _boxes = new List<rankBox>();
            _refTime = _cool + 1f;
            _refable = true;

            refreshRank();
        }

        public void refreshRank()
        {
            settingRankBoxes();
            settingMyRanking();
        }

        /// <summary> 랭킹 로드 </summary>
        public void setRankBoxes()
        {
            if (AuthManager.instance.networkCheck() == false)
            {
                WindowManager.instance.Win_message.showMessage("네트워크가 없어눈~");
                return;
            }

            //if (_refTime > _cool)
            //{
            //    _refTime -= _cool;
            //    settingRankBoxes();
            //}
            //else
            //{
            //    WindowManager.instance.Win_message.showMessage($"잠시만요~ (쿨타임 : {Convert.ToInt32(_cool - _refTime)}초)");
            //}
        }

        /// <summary> 랭킹 리스트 세팅 </summary>
        void settingRankBoxes()
        {
            // yield return StartCoroutine(AuthManager.instance.loadRankDataFromFB());
            NanooManager.instance.getRankingTotal((dictionary) => 
            {
                ArrayList list = (ArrayList)dictionary["list"];
                int i = 0;

                foreach (Dictionary<string, object> rank in list)
                {
                    if (_boxes.Count <= i)
                    {
                        rankBox box = Instantiate(_rankBoxFab).GetComponent<rankBox>();
                        box.transform.SetParent(_boxParent);
                        box.transform.localScale = Vector3.one;
                        _boxes.Add(box);
                    }

                    rankData data = JsonUtility.FromJson<rankData>((string)rank["data"]);

                    _boxes[i].setRankBox(i+1, rank["nickname"].ToString(), int.Parse((string)rank["score"]), data);
                    i++; 
                }
            });
        }

        void settingMyRanking()
        {
            NanooManager.instance.getRankingPersonal((dictionary) => 
            {
                if (dictionary == null)
                {
                    Debug.LogError("랭킹데이터 없음");
                    return;
                }

                int rank = int.Parse((string)dictionary["ranking"]);
                int totalPlayer = int.Parse((string)dictionary["total_player"]);

                if (rank == -1)
                {
                    Debug.Log("랭킹 미등록");
                    _myRankBox.blink();
                }
                else
                {
                    rankData data = JsonUtility.FromJson<rankData>((string)dictionary["data"]);
                    _myRankBox.setRankBox(rank, BaseManager.userGameData.NickName, int.Parse((string)dictionary["score"]), data);
                }
            });
        }

        ///// <summary> 내 랭킹 어필 </summary>
        //public void uploadRequest()
        //{
        //    if (AuthManager.instance.networkCheck() == false)
        //    {
        //        WindowManager.instance.Win_message.showMessage("네트워크가 없어용~");
        //        return;
        //    }

        //    if (BaseManager.userGameData._minRank < BaseManager.userGameData.TimeRecord)
        //    {
        //        if (_reqTime > _cool)
        //        {
        //            _reqTime -= _cool;
        //            // AuthManager.instance.saveRankDataFromFB();
        //            WindowManager.instance.Win_message.showMessage("접수되었습니다." + System.Environment.NewLine + "반영까지 쬐금만 기달려주세요");
        //        }
        //        else
        //        {
        //            WindowManager.instance.Win_message.showMessage($"잠시만요~ (쿨타임 : {Convert.ToInt32(_cool - _refTime)}초)");
        //        }
        //    }
        //    else
        //    {
        //        WindowManager.instance.Win_message.showMessage("랭킹 반영안된거 같을때 눌러줭");
        //    }
        //}

        //void Update()
        //{
        //    if (_refable == false)
        //    {
        //        if (_refTime <= _cool)
        //        {
        //            _refTime += Time.deltaTime;
        //        }
        //        else
        //        {
        //            _refable = true;
        //        }
        //    }
        //}

        public void open()
        {
            gameObject.SetActive(true);

            //if (_refTime > _cool)
            //{
            //    _refTime -= _cool;
            //    refreshRank();
            //}

            refreshRank();
        }

        public void close()
        {
            gameObject.SetActive(false);
        }
    }
}