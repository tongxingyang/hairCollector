using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;

namespace week
{
    public static class DataManager
    {
        static BGMetaEntity[] datalist;

        public static bool BGLoaded
        {
            get
            {
                return BGRepo.DefaultRepoLoaded;
            }
        }

        public static int bgCount
        {
            get { return BGRepo.I["hair"].CountEntities; }
        }

        public static bool LoadBGdata()
        {
            datalist = new BGMetaEntity[(int)DataTable.max];

            datalist[(int)DataTable.hair] = BGRepo.I["hair"];
            datalist[(int)DataTable.eyebrow] = BGRepo.I["eyebrow"];
            datalist[(int)DataTable.beard] = BGRepo.I["beard"];
            datalist[(int)DataTable.cloth] = BGRepo.I["cloth"];

            return true;
        }

        public static T GetTable<T>(DataTable data, string key, string row)
        {
            T t = datalist[(int)data][key].Get<T>(datalist[(int)data].GetField(row));
            return t;
        }
    }
}