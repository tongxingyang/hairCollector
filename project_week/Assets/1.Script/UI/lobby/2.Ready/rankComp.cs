using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        [Space]        
        [SerializeField] TextMeshProUGUI _rankTitle;
        [SerializeField] rankBox _myRankBox;
        [SerializeField] GameObject _rankPanel;
        [SerializeField] Button[] _taps;
        [Header("prefab")]
        [SerializeField] Transform _boxParent;
        [SerializeField] GameObject _rankBoxFab;
        [Header("ranker")]
        [SerializeField] TextMeshProUGUI[] _rankerName;
        [SerializeField] Image[] _rankerSkin;

        [Header("medal")]
        [SerializeField] Sprite[] _medals;
        public Sprite getMedalSprite(int i) { return _medals[i]; }
        List<rankBox> _boxes;

        levelKey _lvl;
        int _roadCompleteMyRank = 0, _roadCompleteRankData = 0;
        
        List<rankData>[] _rankDataList;
        rankData[] _myRankData;

        Coroutine setAni;

        /// <summary> 초기화 </summary>
        public void Init()
        {
            _boxes = new List<rankBox>();
            _rankDataList = new List<rankData>[(int)levelKey.max];
            _myRankData = new rankData[(int)levelKey.max];

            _taps[0].onClick.AddListener(() => changeLevel(levelKey.easy));
            _taps[1].onClick.AddListener(() => changeLevel(levelKey.normal));
            _taps[2].onClick.AddListener(() => changeLevel(levelKey.hard));

            StartCoroutine(setRankBoxes());
        }

        /// <summary> 열기 </summary>
        public void open()
        {
            _lvl = BaseManager.userGameData.NowStageLevel;

            _rankPanel.SetActive(true);

            StartCoroutine(setting());
        }

        /// <summary> 닫기 </summary>
        public void close()
        {
            _rankPanel.SetActive(false);
        }

        /// <summary>  </summary>
        IEnumerator setRankBoxes()
        {
            if(AuthManager.instance.networkCheck() == false)
            {
                WindowManager.instance.Win_message.showMessage("네트워크가 없어눈~");
                yield break;
            }

            for (levelKey lvl = levelKey.easy; lvl < levelKey.max; lvl++)
            {
                getMyRankings(lvl);
            }

            yield return new WaitUntil(() => _roadCompleteMyRank == 3);

            for (levelKey lvl = levelKey.easy; lvl < levelKey.max; lvl++)
            {
                getRankLists(lvl);
            }

            yield return new WaitUntil(() => _roadCompleteRankData == 3);

            _rankTitle.text = "2.0 랭-King";
            setAni = StartCoroutine(setting());
        }

        /// <summary> 랭킹박스 (실질적 UI부분) 세팅 (애니메이션 효과 - 코루틴사용) </summary>
        IEnumerator setting()
        {
            yield return new WaitUntil(() => (_roadCompleteMyRank == 3) && (_roadCompleteRankData == 3));

            if (_myRankData[(int)_lvl] == null)
                _myRankBox.blink();
            else
                _myRankBox.setRankBox(_lvl, _myRankData[(int)_lvl].rank, _myRankData[(int)_lvl].nick, 
                    _myRankData[(int)_lvl].score, _myRankData[(int)_lvl].subdata, getMedalSprite);

            for (int i = 0; i < _boxes.Count; i++)
            {
                _boxes[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _rankDataList[(int)_lvl].Count; i++)
            {
                if (_boxes.Count <= i)
                {
                    rankBox box = Instantiate(_rankBoxFab).GetComponent<rankBox>();
                    box.transform.SetParent(_boxParent);
                    box.transform.localScale = Vector3.one;
                    box.transform.localPosition = Vector3.zero;
                    _boxes.Add(box);
                }

                _boxes[i].setRankBox(_lvl, _rankDataList[(int)_lvl][i].rank, _rankDataList[(int)_lvl][i].nick, 
                    _rankDataList[(int)_lvl][i].score, _rankDataList[(int)_lvl][i].subdata, getMedalSprite);
                _boxes[i].gameObject.SetActive(true);

                yield return new WaitForSeconds(0.04f);
            }

            for (int i = 0; i < 3; i++)
            {
                if (_rankDataList[(int)_lvl].Count > i)
                {
                    _rankerName[i].text = _rankDataList[(int)_lvl][i].nick;
                    _rankerSkin[i].sprite = DataManager.SkinSprite[(SkinKeyList)_rankDataList[(int)_lvl][i].subdata._skin];
                    _rankerSkin[i].color = Color.white;
                }
                else
                {
                    _rankerName[i].text = "공석";
                    _rankerSkin[i].sprite = DataManager.SkinSprite[SkinKeyList.snowman];
                    _rankerSkin[i].color = Color.gray;
                }
            }
        }

        /// <summary> 난이도별 랭킹리스트 변경 </summary>
        void changeRankType()
        {
            _rankTitle.text = D_level.GetEntity(_lvl.ToString()).f_trans;

            if (setAni != null)
            {
                StopCoroutine(setAni);
            }

            setAni = StartCoroutine(setting());
        }

        /// <summary> 다음 레벨 랭크데이터 </summary>
        public void changeLevel(levelKey lvl)
        {
            _lvl = lvl;

            for (levelKey i = 0; i < levelKey.max; i++)
            {
                if (i == lvl)
                    _taps[(int)i].transform.localScale = Vector3.one * 1.1f;
                else
                    _taps[(int)i].transform.localScale = Vector3.one * 0.85f;
            }

            switch (_lvl)
            {
                case levelKey.easy:
                case levelKey.hard:
                    _taps[(int)levelKey.normal].transform.SetAsLastSibling();
                    break;
            }
            _taps[(int)_lvl].transform.SetAsLastSibling();

            changeRankType();
        }

        #region [ get data ] ==================================================


        /// <summary> 내 랭킹 가져오기 </summary>
        void getMyRankings(levelKey lvl)
        {
            // 시즌
            NanooManager.instance.getRankingPersonal(lvl, (dictionary) =>
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
                    _myRankData[(int)lvl] = null;
                }
                else
                {
                    rankSubData data = JsonUtility.FromJson<rankSubData>((string)dictionary["data"]);

                    _myRankData[(int)lvl] = new rankData(rank, BaseManager.userGameData.NickName, int.Parse((string)dictionary["score"]), data);
                }
                _roadCompleteMyRank++;
            });
        }

        /// <summary> 상위 랭킹 가져오기 </summary>
        void getRankLists(levelKey lvl)
        {
            // 시즌
            NanooManager.instance.getRankingTotal(lvl, (dictionary) => 
            {
                ArrayList list = (ArrayList)dictionary["list"];
                int i = 0;
                                
                _rankDataList[(int)lvl] = new List<rankData>();

                foreach (Dictionary<string, object> rank in list)
                {
                    rankSubData data = JsonUtility.FromJson<rankSubData>((string)rank["data"]);

                    rankData rdata = new rankData(i + 1, rank["nickname"].ToString(), int.Parse((string)rank["score"]), data);
                    _rankDataList[(int)lvl].Add(rdata);

                    i++; 
                }
                _roadCompleteRankData++;
                //_completeRoadSD = true;
            });
        }

        #endregion     
    }
}