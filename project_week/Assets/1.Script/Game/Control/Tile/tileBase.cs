using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace week
{
    public class tileBase : MonoBehaviour
    {
        [SerializeField] SpriteRenderer[] _renders;

        GameScene _gs;
        ObstacleManager _obm;
        MapManager _mpm;

        public Vector2Int _co;
        bool[][] _mapTable;
        int _rate = 0;
        List<IobstacleObject> _allocateObs;

        public Action reclaim;
        List<obstacleKeyList> _nowObsList;
        public void FixedInit(GameScene gs)
        {
            _gs = gs;
            _obm = _gs.ObtMng;
            _mpm = _gs.MapMng;

            _renders[0].gameObject.SetActive(false);
            _renders[1].gameObject.SetActive(true);

            _renders[1].sprite = _mpm.SeasonMaps[(int)_gs.ClockMng.NowSeason];

            _mapTable = new bool[20][];
            for (int i = 0; i < 20; i++)
                _mapTable[i] = new bool[20];

            _allocateObs = new List<IobstacleObject>();
            _nowObsList = BaseManager.PreGameData._obsRate[(int)BaseManager.userGameData.NowStageLevel];
        }

        public void setTransform(Vector3 vec, bool first = false)
        {
            transform.position = vec;

            reclaim?.Invoke();
            reclaim = null;

            makeOb(first);
        }

        public void changeSeasonMap(season ss)
        {
            StartCoroutine(changeSeason(ss));
        }

        IEnumerator changeSeason(season ss)
        {
            _renders[0].gameObject.SetActive(true);
            _renders[1].gameObject.SetActive(true);

            _renders[0].sprite = _renders[1].sprite;
            _renders[1].sprite = _mpm.SeasonMaps[(int)ss];

            Color prev = Color.white;
            Color next = Color.white;
            next.a = 0;

            _renders[0].color = prev;
            _renders[1].color = next;

            while (next.a < 1f)
            {
                prev.a -= Time.deltaTime;
                next.a += Time.deltaTime;

                _renders[0].color = prev;
                _renders[1].color = next;

                yield return new WaitForEndOfFrame();
            }

            _renders[0].gameObject.SetActive(false);
        }

        /// <summary> 장애물 생성 </summary>
        public void makeOb(bool first = false)
        {
            reset_allocateObs();

            int ran = (first) ? 100 : UnityEngine.Random.Range(0, 201);
            _rate = 0;

            if (first)
            {
                set_normalField(first);
            }
            else if (ran < 4) // 4 -> 2퍼 - 보스
            {
                set_bossField();
            }
            else if(ran > 196) // 196 -> 2퍼 - 함정
            {
                set_trapField();
            }
            else // 일반
            {
                set_normalField(first);
            }
        }

        /// <summary> 장애물 리셋 </summary>
        void reset_allocateObs()
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    _mapTable[i][j] = false;
                }
            }

            for (int i = 0; i < _allocateObs.Count; i++)
            {
                _allocateObs[i].forceDestroy();
            }
            _allocateObs.Clear();
        }

        /// <summary> 보스필드 장애물 </summary>
        void set_bossField()
        {
            obstacleKeyList obs = obstacleKeyList.bosszone;

            int w = BaseManager.PreGameData._obsData[obs]._w;
            int h = BaseManager.PreGameData._obsData[obs]._h;

            IobstacleObject ioo = _obm.getEachObstacle(obs, get_Coordinate(new Vector2Int(0, 0)));
            _allocateObs.Add(ioo);
        }

        /// <summary> 함정필드 장애물 </summary>
        void set_trapField()
        {
            Vector2Int t;
            obstacleKeyList obs;
            int ran, w, h;

            // 함정
            ran = UnityEngine.Random.Range(0, 3);
            {
                obs = obstacleKeyList.ruin_thunder + ran;
                w = BaseManager.PreGameData._obsData[obs]._w;
                h = BaseManager.PreGameData._obsData[obs]._h;

                IobstacleObject ioo = _obm.getEachObstacle(obs, get_Coordinate(new Vector2Int(4, 4)));
                _allocateObs.Add(ioo);

                _rate += BaseManager.PreGameData._obsData[obs]._mount;

                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        _mapTable[4 + i][4 + j] = true;
                    }
                }
            }

            // 추가 장애물
            while (_rate < 400 * 0.4f)
            {
                ran = UnityEngine.Random.Range(0, _nowObsList.Count);
                obs = _nowObsList[ran];
                w = BaseManager.PreGameData._obsData[obs]._w;
                h = BaseManager.PreGameData._obsData[obs]._h;

                t = new Vector2Int(UnityEngine.Random.Range(0, 20 - w), UnityEngine.Random.Range(0, 20 - h));

                bool result = true;
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (_mapTable[t.x + i][t.y + j])
                        {
                            result = false;
                        }
                    }
                }

                if (result)
                {
                    IobstacleObject ioo = _obm.getEachObstacle(obs, get_Coordinate(t));
                    _allocateObs.Add(ioo);

                    _rate += BaseManager.PreGameData._obsData[obs]._mount;

                    for (int i = 0; i < w; i++)
                    {
                        for (int j = 0; j < h; j++)
                        {
                            _mapTable[t.x + i][t.y + j] = true;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        /// <summary> 일반필드 장애물 </summary>
        void set_normalField(bool first = false)
        {
            Vector2Int t;
            obstacleKeyList obs;
            int ran, w, h;

            if (first)
            {
                for (int i = 9; i < 11; i++)
                {
                    for (int j = 9; j < 11; j++)
                    {
                        _mapTable[i][j] = true;
                    }
                }
            }

            // 퀘스트 집 (함수계산 특정위치에 생성)
            ran = _co.y - (2 * _co.x) + 5;
            if (ran % 10 == 0)
                setSimpleBuild(obstacleKeyList.npc_house);

            // 늪 (10% 확률로 생성)
            ran = UnityEngine.Random.Range(0, 20);
            if (ran <= 1)            
                setSimpleBuild(obstacleKeyList.swamp0 + ran);            

            // 힐팩
            setSimpleBuild(obstacleKeyList.healpack);            

            while (_rate < 400 * 0.15f)
            {
                ran = UnityEngine.Random.Range(0, _nowObsList.Count);
                obs = _nowObsList[ran];
                w = BaseManager.PreGameData._obsData[obs]._w;
                h = BaseManager.PreGameData._obsData[obs]._h;

                t = new Vector2Int(UnityEngine.Random.Range(0, 20 - w), UnityEngine.Random.Range(0, 20 - h));
                //Debug.Log(t);

                bool result = true;
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (_mapTable[t.x + i][t.y + j])
                        {
                            result = false;
                        }
                    }
                }

                if (result)
                {
                    IobstacleObject ioo = _obm.getEachObstacle(obs, get_Coordinate(t));
                    _allocateObs.Add(ioo);

                    _rate += BaseManager.PreGameData._obsData[obs]._mount;

                    for (int i = 0; i < w; i++)
                    {
                        for (int j = 0; j < h; j++)
                        {
                            _mapTable[t.x + i][t.y + j] = true;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        /// <summary> 좌표에 따른 실제 위치가져오기 </summary>
        Vector3 get_Coordinate(Vector2Int co)
        {
            return transform.position + new Vector3(-10 + co.x, 10 - co.y);
        }

        /// <summary> 간편 장애물 1개? 설치 </summary>
        void setSimpleBuild(obstacleKeyList obs)
        {
            int w = BaseManager.PreGameData._obsData[obs]._w;
            int h = BaseManager.PreGameData._obsData[obs]._h;

            Vector2Int t;
            bool result = true;
            do
            {
                t = new Vector2Int(UnityEngine.Random.Range(0, 20 - w), UnityEngine.Random.Range(0, 20 - h));

                result = true;
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (_mapTable[t.x + i][t.y + j])
                        {
                            result = false;

                            i = w;
                            break;
                        }
                    }
                }
            } while (result == false);

            IobstacleObject ioo = _obm.getEachObstacle(obs, get_Coordinate(t));
            _allocateObs.Add(ioo);

            _rate += BaseManager.PreGameData._obsData[obs]._mount;

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    _mapTable[t.x + i][t.y + j] = true;
                }
            }
        }

        bool bll = false;
        [Button]
        public void onOff()
        {
            for (int i = 0; i < _allocateObs.Count; i++)
            {
                _allocateObs[i].gameObject.SetActive(!bll);
            }
            bll = !bll;
        }
    }
}