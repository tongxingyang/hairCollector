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
        static Dictionary<SkillKeyList, GameObject> _shotFabs;
        static Dictionary<EnShot, GameObject> _enProjFabs;
        public static Dictionary<Mob, GameObject> MobFabs { get => _mobFabs; set => _mobFabs = value; }
        public static Dictionary<Boss, GameObject> BobFabs { get => _bobFabs; set => _bobFabs = value; }
        public static Dictionary<mapObstacle, GameObject> ObstacleFabs { get => _obstacleFabs; set => _obstacleFabs = value; }
        public static Dictionary<SkillKeyList, GameObject> ShotFabs { get => _shotFabs; set => _shotFabs = value; }
        public static Dictionary<EnShot, GameObject> EnProjFabs { get => _enProjFabs; set => _enProjFabs = value; }

        static Dictionary<StatusData, Sprite> _statusicon;
        static Dictionary<SkillKeyList, Sprite> _skillicon;
        static Dictionary<SkinKeyList, Sprite> _skinSprite;
        public static Dictionary<StatusData, Sprite> Statusicon { get => _statusicon; set => _statusicon = value; }
        public static Dictionary<SkillKeyList, Sprite> Skillicon { get => _skillicon; set => _skillicon = value; }
        public static Dictionary<SkinKeyList, Sprite> SkinSprite { get => _skinSprite; set => _skinSprite = value; }

        public static bool LoadBGdata()
        {
            datalist = new BGMetaEntity[(int)DataTable.max];

            datalist[(int)DataTable.skill] = BGRepo.I["skill"];
            datalist[(int)DataTable.monster] = BGRepo.I["monster"];
            datalist[(int)DataTable.status] = BGRepo.I["status"];
            datalist[(int)DataTable.boss] = BGRepo.I["boss"];
            datalist[(int)DataTable.skin] = BGRepo.I["skin"];
            datalist[(int)DataTable.enproj] = BGRepo.I["enproj"];
            datalist[(int)DataTable.quest] = BGRepo.I["quest"];
            datalist[(int)DataTable.product] = BGRepo.I["product"];

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
            for (Mob i = (Mob)0; i < Mob.max; i++)
            {
                GameObject go = Resources.Load("prefabs/enemy/mop/" + i.ToString()) as GameObject;
                _mobFabs.Add(i, go);
            }

            _bobFabs = new Dictionary<Boss, GameObject>();
            for (Boss i = (Boss)0; i < Boss.max; i++)
            {
                GameObject go = Resources.Load("prefabs/enemy/boss/" + i.ToString()) as GameObject;
                _bobFabs.Add(i, go);
            }

            _obstacleFabs = new Dictionary<mapObstacle, GameObject>();

            for (mapObstacle i = (mapObstacle)0; i < mapObstacle.max; i++)
            {
                GameObject go = Resources.Load("prefabs/obstacle/" + $"{i.ToString()}") as GameObject;
                _obstacleFabs.Add(i, go);
            }

            _shotFabs = new Dictionary<SkillKeyList, GameObject>();
            for (SkillKeyList i = SkillKeyList.snowball; i < SkillKeyList.max; i++)
            {
                if (i == SkillKeyList.icetornado || i == SkillKeyList.iceshield || i == SkillKeyList.iceage || i == SkillKeyList.blizzard)
                    continue;

                GameObject go = Resources.Load("prefabs/skill/playerSkill/" + i.ToString()) as GameObject;
                _shotFabs.Add(i, go);
            }

            _enProjFabs = new Dictionary<EnShot, GameObject>();
            for (EnShot i = (EnShot)0; i < EnShot.max; i++)
            {
                GameObject go = Resources.Load("prefabs/skill/enemySkill/" + i.ToString()) as GameObject;
                _enProjFabs.Add(i, go);
            }

            _statusicon = new Dictionary<StatusData, Sprite>();
            Sprite[] sts = Resources.LoadAll<Sprite>("sprite/statIcons");
            string name;
            for (int i = 0; i < sts.Length; i++)
            {
                name = sts[i].name;

                for (StatusData sd = StatusData.hp; sd < StatusData.max; sd++)
                {
                    if (name.Equals(sd.ToString()))
                    {
                        _statusicon.Add(sd, sts[i]);
                    }
                }
            }

            _skillicon = new Dictionary<SkillKeyList, Sprite>();
            Sprite[] sps = Resources.LoadAll<Sprite>("sprite/skills");

            for (int i = 0; i < sps.Length; i++)
            {
                name = sps[i].name;

                for (SkillKeyList sk= SkillKeyList.hp; sk < SkillKeyList.present; sk++)
                {
                    if (name.Equals(sk.ToString()))
                    {
                        _skillicon.Add(sk, sps[i]);
                    }
                }
            }

            _skinSprite = new Dictionary<SkinKeyList, Sprite>();
            Sprite[] sks = Resources.LoadAll<Sprite>("sprite/snowmans");

            for (int i = 0; i < sks.Length; i++)
            {
                name = sks[i].name;

                for (SkinKeyList sk = SkinKeyList.snowman; sk < SkinKeyList.max; sk++)
                {
                    if (name.Equals(sk.ToString()))
                    {
                        _skinSprite.Add(sk, sks[i]);
                    }
                }
            }
        }
    }
}