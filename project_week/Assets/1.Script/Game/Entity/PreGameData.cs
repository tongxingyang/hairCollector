using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class obsData
    {
        public obstacleKeyList _obs;
        public int _h;
        public int _w;
        public int _mount;

        public obsData(obstacleKeyList obs)
        {
            _obs = obs;
            _h      = D_obstacle.GetEntity(_obs.ToString()).f_hsize;
            _w      = D_obstacle.GetEntity(_obs.ToString()).f_wsize;
            _mount  = D_obstacle.GetEntity(_obs.ToString()).f_mount;
        }
    }

    public class PreGameData
    {
        /// <summary> 초기화 </summary>
        public PreGameData()
        {
            setMobData();
            setBossValData();
            setObsValData();
        }

        #region [ mob ]

        /// <summary> 몹생성 한계량 </summary>
        public int[] MobFullRate { get; private set; }

        /// <summary> 몹 데이터 세팅 </summary>
        void setMobData()
        {
            MobFullRate = new int[4];
            for (Mob i = 0; i < Mob.ash; i++)
            {
                for (int j = (int)i; j < (int)Mob.ash; j++)
                {
                    MobFullRate[j] += D_monster.GetEntity(i.ToString()).f_mount;
                }
            }
        }

        #endregion

        #region [ boss ]

        List<Boss> _mapBossList;
        public List<Boss> MapBossList { get => _mapBossList; set => _mapBossList = value; }/// <summary> 보스 데이터 세팅 </summary>
        void setBossValData()
        {
            _mapBossList = new List<Boss>();

            for (Boss i = Boss.boss_owl; i < Boss.max; i++)
            {
                _mapBossList.Add(i);
            }
        }

        #endregion

        #region [ obstacle ]

        /// <summary> 장애물 데이터 </summary>
        public Dictionary<obstacleKeyList, obsData> _obsData;
        /// <summary> 장애물 리스트(등장확률 포함) </summary>
        public List<obstacleKeyList>[] _obsRate; 

        /// <summary> 장애물 데이터 세팅 </summary>
        void setObsValData()
        {
            _obsData = new Dictionary<obstacleKeyList, obsData>();
            _obsRate = new List<obstacleKeyList>[3] { new List<obstacleKeyList>(), new List<obstacleKeyList>(), new List<obstacleKeyList>() };

            for (obstacleKeyList obk = 0; obk < obstacleKeyList.max; obk++)
            {
                obsData data = new obsData(obk);
                _obsData.Add(obk, data);

                for (levelKey i = 0; i < levelKey.max; i++)
                {
                    int n = D_obstacle.GetEntity(obk.ToString()).Get<int>(i.ToString());
                    for (int j = 0; j < n; j++)
                    {
                        _obsRate[(int)i].Add(obk);
                    }
                }
            }

            // 섞기
            obstacleKeyList tmp;
            int rand;
            for (levelKey i = 0; i < levelKey.max; i++)
            {
                for (int j = 0; j < _obsRate[(int)i].Count; j++)
                {
                    rand = Random.Range(0, _obsRate[(int)i].Count);
                    if (j != rand)
                    {
                        tmp = _obsRate[(int)i][j];
                        _obsRate[(int)i][j] = _obsRate[(int)i][rand];
                        _obsRate[(int)i][rand] = tmp;
                    }
                }
            }
        }

        #endregion

    }
}