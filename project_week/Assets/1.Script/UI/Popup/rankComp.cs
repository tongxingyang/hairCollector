using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace week
{
    class rankData
    {
        public int rank;
        public string nick;
        public int score;
        public rankSubData subdata;
        public rankData(int rank, string nick, int score, rankSubData subdata)
        {
            this.rank = rank;
            this.nick = nick;
            this.score = score;
            this.subdata = subdata;
        }
    }

    public class rankComp : MonoBehaviour, UIInterface
    {
        [SerializeField] TextMeshProUGUI _rankTitle;
        [SerializeField] GameObject _rankBoxFab;
        [SerializeField] rankBox _myRankBox;
        [SerializeField] Transform _boxParent;
        [SerializeField] GameObject[] _arrows;

        bool SSarrow { set { _arrows[0].SetActive(value); _arrows[1].SetActive(!value); } }

        List<rankBox> _boxes;

        bool _isSeason;
        bool _completeRoadMySS, _completeRoadSD, _completeRoadMyAL, _completeRoadAD;
        List<rankData> _ssData;
        List<rankData> _allData;

        rankData _mySeason, _myAll;
        Coroutine setAni;

        bool _refable;
        float _refTime;
        readonly float _cool = 60f;

        public void Init()
        {
            _isSeason = true;

            _boxes = new List<rankBox>();

            _completeRoadSD = _completeRoadAD = false;
            _completeRoadMySS = _completeRoadMyAL = false;

            _refTime = _cool + 1f;
            _refable = true;

            getRankData();
        }

        IEnumerator setRankBoxes()
        {
            if(AuthManager.instance.networkCheck() == false)
            {
                WindowManager.instance.Win_message.showMessage("네트워크가 없어눈~");
                yield break;
            }

            getRankData();
            yield return new WaitUntil(() => _completeRoadMySS && _completeRoadSD && _completeRoadMyAL && _completeRoadAD);

            //if (_isSeason)
            //{
            setAni = StartCoroutine(setting(_mySeason, _ssData, true));
            _rankTitle.text = "시즌 랭-King";
            SSarrow = true;
            //}
            //else
            //{
            //    setAni = StartCoroutine(setting(_myAll, _allData, false));
            //}
        }

        /// <summary> 랭킹박스랑 실질적 UI부분에서 세팅 </summary>
        IEnumerator setting(rankData rd, List<rankData> rdList, bool isSeason)
        {
            if (rd == null)
                _myRankBox.blink().setBoxType(isSeason);
            else
                _myRankBox.setRankBox(rd.rank, rd.nick, rd.score, rd.subdata).setBoxType(isSeason);

            for (int i = 0; i < _boxes.Count; i++)
            {
                _boxes[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < rdList.Count; i++)
            {
                if (_boxes.Count <= i)
                {
                    rankBox box = Instantiate(_rankBoxFab).GetComponent<rankBox>();
                    box.transform.SetParent(_boxParent);
                    box.transform.localScale = Vector3.one;
                    _boxes.Add(box);
                }

                _boxes[i].setRankBox(rdList[i].rank, rdList[i].nick, rdList[i].score, rdList[i].subdata).setBoxType(isSeason);
                _boxes[i].gameObject.SetActive(true);

                yield return new WaitForSeconds(0.1f);
            }
        }

        public void changeRankType()
        {
            _isSeason = !_isSeason;

            if (_isSeason)
            {
                if (setAni != null)
                {
                    StopCoroutine(setAni);
                }

                setAni = StartCoroutine(setting(_mySeason, _ssData, true));
                _rankTitle.text = "시즌 랭-King";
                SSarrow = true;
            }
            else
            {
                if (setAni != null)
                {
                    StopCoroutine(setAni);
                }

                setAni = StartCoroutine(setting(_myAll, _allData, false));
                _rankTitle.text = "전체 랭-King";
                SSarrow = false;
            }
        }

        public void open()
        {
            gameObject.SetActive(true);

            StartCoroutine(setRankBoxes());
        }

        public void close()
        {
            gameObject.SetActive(false);
        }

        #region [ get data ] ==================================================

        public void getRankData()
        {
            getRankLists();
            getMyRankings();
        }

        /// <summary> 랭킹 리스트 세팅 </summary>
        void getRankLists()
        {
            // 시즌
            NanooManager.instance.getRankingTotal(true, (dictionary) => 
            {
                ArrayList list = (ArrayList)dictionary["list"];
                int i = 0;

                if (_ssData != null)
                {
                    _ssData.Clear();
                }
                _ssData = new List<rankData>();

                foreach (Dictionary<string, object> rank in list)
                {
                    rankSubData data = JsonUtility.FromJson<rankSubData>((string)rank["data"]);

                    rankData rdata = new rankData(i + 1, rank["nickname"].ToString(), int.Parse((string)rank["score"]), data);
                    _ssData.Add(rdata);

                    i++; 
                }
                _completeRoadSD = true;
            });

            // 전체
            NanooManager.instance.getRankingTotal(false, (dictionary) =>
            {
                ArrayList list = (ArrayList)dictionary["list"];
                int i = 0;

                if (_allData != null)
                {
                    _allData.Clear();
                }
                _allData = new List<rankData>();

                foreach (Dictionary<string, object> rank in list)
                {

                    rankSubData data = JsonUtility.FromJson<rankSubData>((string)rank["data"]);

                    rankData rdata = new rankData(i + 1, rank["nickname"].ToString(), int.Parse((string)rank["score"]), data);
                    _allData.Add(rdata);

                    i++;
                }
                _completeRoadAD = true;
            });
        }

        void getMyRankings()
        {
            // 시즌
            NanooManager.instance.getRankingPersonal(true, (dictionary) => 
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
                    _mySeason = null;
                }
                else
                {
                    rankSubData data = JsonUtility.FromJson<rankSubData>((string)dictionary["data"]);

                    _mySeason = new rankData(rank, BaseManager.userGameData.NickName, int.Parse((string)dictionary["score"]), data);
                }
                _completeRoadMySS = true;
            });

            // 전체
            NanooManager.instance.getRankingPersonal(false, (dictionary) =>
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
                    _myAll = null;
                }
                else
                {
                    rankSubData data = JsonUtility.FromJson<rankSubData>((string)dictionary["data"]);

                    _myAll = new rankData(rank, BaseManager.userGameData.NickName, int.Parse((string)dictionary["score"]), data);
                }
                _completeRoadMyAL = true;
            });
        }

        #endregion     
    }
}