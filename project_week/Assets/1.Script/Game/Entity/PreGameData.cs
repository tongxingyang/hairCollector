using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class PreGameData
    {
        public class obsData
        {
            public obstacleList _ob;
            public int _x;
            public int _y;
            public int _val;

            public obsData(obstacleList o, int x, int y, int v)
            {
                _ob = o;
                _x = x;
                _y = y;
                _val = v;
            }
        }

        List<Boss> _mapBossList;
        public List<Boss> MapBossList { get => _mapBossList; set => _mapBossList = value; }

        List<obsData> _obstacleData;
        List<int> _obVal;

        public PreGameData()
        {
            setBossValData();
        }

        void setBossValData()
        {
            _mapBossList = new List<Boss>();

            for (Boss i = Boss.boss_owl; i < Boss.max; i++)
            {
                _mapBossList.Add(i);
            }
        }

        //public obstacleType getBoss()
        //{
        //    int cnt = MapBossList.Count;
        //    cnt = Random.Range(0, cnt);
        //    string str = MapBossList[cnt].ToString() + "_zone";

        //    return EnumHelper.StringToEnum<obstacleType>(str);
        //}

        //public obsData getObs()
        //{
        //    int cnt = Random.Range(0, _obVal);

        //    foreach (obsData od in _obstacleData)
        //    {
        //        if (od._val > cnt)
        //        {
        //            return od;
        //        }
        //    }

        //    int lt = _obstacleData.Count;
        //    Debug.LogError("레인지 벗어남 : " + cnt + " > " + _obstacleData[lt - 1]._val);
        //    return null;
        //}
    }
}