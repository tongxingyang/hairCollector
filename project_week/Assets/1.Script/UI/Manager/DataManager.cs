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

        public static bool LoadBGdata()
        {
            datalist = new BGMetaEntity[(int)DataTable.max];

            datalist[(int)DataTable.skill] = BGRepo.I["skill"];

            return true;
        }

        public static T GetTable<T>(DataTable data, string key, string row)
        {
            T t = datalist[(int)data][key].Get<T>(datalist[(int)data].GetField(row));
            return t;
        }
    }
}