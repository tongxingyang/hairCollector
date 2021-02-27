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
        static GameObject _shotFabs;
        static GameObject _curvedFabs;
        static GameObject _stampFabs;
        static GameObject _rushFabs;
        static GameObject _shieldFabs;
        static GameObject _fieldFabs;
        static GameObject _petFabs;
        static Dictionary<EnShot, GameObject> _enProjFabs;
        static Dictionary<ShotList, Sprite> _shotImgs;
        static Dictionary<ShotList, Sprite> _curveImgs;
        static Dictionary<snowballType, Sprite> _snowballImg;
        public static Dictionary<Mob, GameObject> MobFabs { get => _mobFabs; set => _mobFabs = value; }
        public static Dictionary<Boss, GameObject> BobFabs { get => _bobFabs; set => _bobFabs = value; }
        public static Dictionary<mapObstacle, GameObject> ObstacleFabs { get => _obstacleFabs; set => _obstacleFabs = value; }
        public static GameObject ShotFabs { get => _shotFabs; set => _shotFabs = value; }
        public static GameObject CurvedFabs { get => _curvedFabs; set => _curvedFabs = value; }
        public static GameObject StampFabs { get => _stampFabs; set => _stampFabs = value; }
        public static GameObject RushFabs { get => _rushFabs; set => _rushFabs = value; }
        public static GameObject ShieldFabs { get => _shieldFabs; set => _shieldFabs = value; }
        public static GameObject FieldFabs { get => _fieldFabs; set => _fieldFabs = value; }
        public static GameObject PetFabs { get => _petFabs; set => _petFabs = value; }

        public static Dictionary<EnShot, GameObject> EnProjFabs { get => _enProjFabs; set => _enProjFabs = value; }

        static Dictionary<statusKeyList, Sprite> _statusicon;
        static Dictionary<SkillKeyList, Sprite> _skillicon;
        static Dictionary<SkinKeyList, Sprite> _skinSprite;
        public static Dictionary<statusKeyList, Sprite> Statusicon { get => _statusicon; set => _statusicon = value; }
        public static Dictionary<SkillKeyList, Sprite> Skillicon { get => _skillicon; set => _skillicon = value; }
        public static Dictionary<SkinKeyList, Sprite> SkinSprite { get => _skinSprite; set => _skinSprite = value; }
        public static Dictionary<ShotList, Sprite> ShotImgs { get => _shotImgs; }
        public static Dictionary<ShotList, Sprite> CurveImgs { get => _curveImgs; }
        public static Dictionary<snowballType, Sprite> SnowballImg { get => _snowballImg; }
        

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
            //Debug.Log(key + "," + row);
            T t = datalist[(int)data][key].Get<T>(datalist[(int)data].GetField(row));
            return t;
        }

        public static void loadPrefabs()
        {
            _mobFabs = new Dictionary<Mob, GameObject>();
            for (Mob i = (Mob)0; i < Mob.max; i++)
            {
                GameObject go = Resources.Load("prefabs/enemy/mop/mob_" + i.ToString()) as GameObject;
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

            _shotFabs = Resources.Load("prefabs/skill/playerSkill/shotFab") as GameObject;
            _curvedFabs = Resources.Load("prefabs/skill/playerSkill/curvedFab") as GameObject;
            _stampFabs = Resources.Load("prefabs/skill/playerSkill/stampFab") as GameObject;
            _rushFabs = Resources.Load("prefabs/skill/playerSkill/rushFab") as GameObject;
            _shieldFabs = Resources.Load("prefabs/skill/playerSkill/shieldFab") as GameObject;
            _fieldFabs = Resources.Load("prefabs/skill/playerSkill/fieldFab") as GameObject;
            _petFabs = Resources.Load("prefabs/skill/playerSkill/PetFab") as GameObject;

            _enProjFabs = new Dictionary<EnShot, GameObject>();
            for (EnShot i = (EnShot)0; i < EnShot.max; i++)
            {
                GameObject go = Resources.Load("prefabs/skill/enemySkill/" + i.ToString()) as GameObject;
                _enProjFabs.Add(i, go);
            }

            _statusicon = new Dictionary<statusKeyList, Sprite>();
            Sprite[] sts = Resources.LoadAll<Sprite>("sprite/statIcons");
            string name;
            for (int i = 0; i < sts.Length; i++)
            {
                name = sts[i].name;

                for (statusKeyList sd = statusKeyList.hp; sd < statusKeyList.max; sd++)
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

                for (SkillKeyList sk= SkillKeyList.HP; sk < SkillKeyList.Present; sk++)
                {
                    if (name.Equals(sk.ToString()))
                    {
                        _skillicon.Add(sk, sps[i]);
                    }
                }
            }

            _shotImgs = new Dictionary<ShotList, Sprite>();
            _snowballImg = new Dictionary<snowballType, Sprite>();
            Sprite[] shots = Resources.LoadAll<Sprite>("sprite/skillShot");

            for (int i = 0; i < shots.Length; i++)
            {
                name = shots[i].name;

                for (ShotList sk = ShotList.Snowball; sk < ShotList.None; sk++)
                {
                    if (name.Equals(sk.ToString()))
                    {
                        _shotImgs.Add(sk, shots[i]);
                        break;
                    }
                }

                for (snowballType ball = snowballType.Citrusbaall; ball < snowballType.standard; ball++)
                {
                    if (name.Equals(ball.ToString()))
                    {
                        _snowballImg.Add(ball, shots[i]);
                        break;
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