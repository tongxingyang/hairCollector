using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;
using System.IO;
using System;

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
        static Dictionary<Mob, GameObject> _mobFabs;
        static Dictionary<Boss, GameObject> _bobFabs;
        static Dictionary<mapObstacle, GameObject> _obstacleFabs;
        static Dictionary<getSkillList, GameObject> _shotFabs;
        static Dictionary<EnShot, GameObject> _enProjFabs;
        public static Dictionary<Mob, GameObject> MobFabs { get => _mobFabs; set => _mobFabs = value; }
        public static Dictionary<Boss, GameObject> BobFabs { get => _bobFabs; set => _bobFabs = value; }
        public static Dictionary<mapObstacle, GameObject> ObstacleFabs { get => _obstacleFabs; set => _obstacleFabs = value; }
        public static Dictionary<getSkillList, GameObject> ShotFabs { get => _shotFabs; set => _shotFabs = value; }
        public static Dictionary<EnShot, GameObject> EnProjFabs { get => _enProjFabs; set => _enProjFabs = value; }

        static Dictionary<getSkillList, Sprite> _skillicon;
        public static Dictionary<getSkillList, Sprite> Skillicon { get => _skillicon; set => _skillicon = value; }

        public static bool LoadBGdata()
        {
            datalist = new BGMetaEntity[(int)DataTable.max];

            datalist[(int)DataTable.skill] = BGRepo.I["skill"];
            datalist[(int)DataTable.monster] = BGRepo.I["monster"];
            datalist[(int)DataTable.status] = BGRepo.I["status"];
            datalist[(int)DataTable.boss] = BGRepo.I["boss"];
            datalist[(int)DataTable.stage] = BGRepo.I["stage"];
            datalist[(int)DataTable.enproj] = BGRepo.I["enproj"];

            return true;
        }

        public static T GetTable<T>(DataTable data, string key, string row)
        {
            T t = datalist[(int)data][key].Get<T>(datalist[(int)data].GetField(row));
            return t;
        }

        public static void loadPrefabs()
        {
            _mobFabs = new Dictionary<Mob, GameObject>();
            for (Mob i = Mob.mob_fire; i < Mob.max; i++)
            {
                GameObject go = Resources.Load("prefabs/enemy/mop/" + i.ToString()) as GameObject;
                _mobFabs.Add(i, go);
            }

            _bobFabs = new Dictionary<Boss, GameObject>();
            for (Boss i = Boss.boss_owl; i < Boss.max; i++)
            {
                GameObject go = Resources.Load("prefabs/enemy/boss/" + i.ToString()) as GameObject;
                _bobFabs.Add(i, go);
            }

            _obstacleFabs = new Dictionary<mapObstacle, GameObject>();

            for (mapObstacle i = mapObstacle.bosszone_0; i < mapObstacle.max; i++)
            {
                GameObject go = Resources.Load("prefabs/obstacle/" + $"{i.ToString()}") as GameObject;
                _obstacleFabs.Add(i, go);
            }

            _shotFabs = new Dictionary<getSkillList, GameObject>();
            for (getSkillList i = getSkillList.snowball; i < getSkillList.max; i++)
            {
                if (i == getSkillList.icetornado || i == getSkillList.iceshield || i == getSkillList.iceage || i == getSkillList.blizzard)
                    continue;

                GameObject go = Resources.Load("prefabs/skill/playerSkill/" + i.ToString()) as GameObject;
                _shotFabs.Add(i, go);
            }

            _enProjFabs = new Dictionary<EnShot, GameObject>();
            for (EnShot i = EnShot.fireball; i < EnShot.max; i++)
            {
                GameObject go = Resources.Load("prefabs/skill/enemySkill/" + i.ToString()) as GameObject;
                _enProjFabs.Add(i, go);
            }

            _skillicon = new Dictionary<getSkillList, Sprite>();
            Sprite[] sps = Resources.LoadAll<Sprite>("sprite/skills");
            string name;
            for (int i = 0; i < sps.Length; i++)
            {
                name = sps[i].name;

                for (getSkillList sk= getSkillList.hp; sk < getSkillList.max; sk++)
                {
                    if (name.Equals(sk.ToString()))
                    {
                        _skillicon.Add(sk, sps[i]);
                    }
                }
            }
        }
    }
}