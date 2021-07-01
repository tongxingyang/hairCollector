using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;
using System.IO;
using System;

namespace week
{
    #region

    public class shotbulletData
    {
        public float ColSize { get; private set; }
        public float TailSize { get; private set; }
        public float Speed { get; private set; }
        public string Ani { get; private set; }
        public Sprite Img { get; private set; }

        public shotbulletData(float colSize, float tailSize, float speed, string ani, Sprite sp)
        {
            ColSize = colSize;
            TailSize = tailSize;
            Speed = speed;
            Ani = ani;
            Img = sp;
        }
    }

    #endregion
    public static class DataManager
    {
        static BGMetaEntity[] datalist;

        static Dictionary<Mob, GameObject> _mobFabs;
        static Dictionary<Boss, GameObject> _bobFabs;
        static Dictionary<obstacleKeyList, GameObject> _eachObsFabs;
        static GameObject _shotFabs;
        static GameObject _curvedFabs;
        static GameObject _stampFabs;
        static GameObject _rushFabs;
        static GameObject _fieldFabs;
        static GameObject _petFabs;

        public static TextAsset AgreeTxt { get; private set; }

        static Dictionary<EnShot, GameObject> _enProjFabs;
        static Dictionary<SkillKeyList, Sprite> _shieldImgs;
        public static Dictionary<lobbyPanel, GameObject> PanelFabs { get; private set; }
        public static Dictionary<Mob, GameObject> MobFabs { get => _mobFabs; set => _mobFabs = value; }
        public static Dictionary<Boss, GameObject> BobFabs { get => _bobFabs; set => _bobFabs = value; }
        public static Dictionary<obstacleKeyList, GameObject> EachObsFabs { get => _eachObsFabs; set => _eachObsFabs = value; }
        public static GameObject ShotFabs { get => _shotFabs; set => _shotFabs = value; }
        public static GameObject CurvedFabs { get => _curvedFabs; set => _curvedFabs = value; }
        public static GameObject StampFabs { get => _stampFabs; set => _stampFabs = value; }
        public static GameObject RushFabs { get => _rushFabs; set => _rushFabs = value; }
        public static GameObject ShieldFabs { get; private set; }
        public static GameObject FieldFabs { get => _fieldFabs; set => _fieldFabs = value; }
        public static GameObject PetFabs { get => _petFabs; set => _petFabs = value; }

        public static Dictionary<EnShot, GameObject> EnProjFabs { get => _enProjFabs; set => _enProjFabs = value; }

        public static Dictionary<statusKeyList, Sprite> Statusicon { get; set; }
        public static Dictionary<SkillKeyList, Sprite> Skillicon { get; private set; }
        public static Dictionary<SkinKeyList, Sprite> SkinSprite { get; private set; }
        public static Dictionary<ShotList, shotbulletData> ShotBulletData { get; private set; }
        public static Dictionary<ShotList, Sprite> CurveImgs { get; private set; }
        public static Dictionary<snowballType, Sprite> SnowballImg { get; private set; }
        public static Dictionary<SkillKeyList, Sprite> ShieldImgs { get => _shieldImgs; }

        // 인게임전용
        public static Dictionary<NotiType, Sprite> NotiSprite { get; private set; }
        public static Dictionary<InQuestKeyList, Sprite> QuestSprite { get; private set; }

        // 사운드
        public static Dictionary<BGM, AudioClip> Bgm { get; private set; }
        public static Dictionary<SFX, AudioClip> Sfx { get; private set; }

        public static bool LoadBGdata()
        {
            //datalist = new BGMetaEntity[(int)DataTable.max];

            //datalist[(int)DataTable.skill] = BGRepo.I["skill"];
            //datalist[(int)DataTable.monster] = BGRepo.I["monster"];
            //datalist[(int)DataTable.status] = BGRepo.I["status"];
            //datalist[(int)DataTable.boss] = BGRepo.I["boss"];
            //datalist[(int)DataTable.skin] = BGRepo.I["skin"];
            //datalist[(int)DataTable.enproj] = BGRepo.I["enproj"];
            //datalist[(int)DataTable.quest] = BGRepo.I["quest"];
            //datalist[(int)DataTable.product] = BGRepo.I["product"];

            //datalist[(int)DataTable.obstacle] = BGRepo.I["obstacle"];
            //datalist[(int)DataTable.inquest] = BGRepo.I["inquest"];
            //datalist[(int)DataTable.level] = BGRepo.I["level"];
            //datalist[(int)DataTable.bulletData] = BGRepo.I["bulletData"];

            return true;
        }

        static T GetTable<T>(DataTable data, string key, string row)
        {
            // Debug.Log(key + "," + row);
            T t = datalist[(int)data][key].Get<T>(datalist[(int)data].GetField(row));
            return t;
        }

        public static void loadPrefabs()
        {
            AgreeTxt = Resources.Load("agree") as TextAsset;

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

            _eachObsFabs = new Dictionary<obstacleKeyList, GameObject>();

            for (obstacleKeyList i = (obstacleKeyList)0; i < obstacleKeyList.max; i++)
            {
                GameObject go = Resources.Load("prefabs/obstacle/obs/" + $"{i.ToString()}") as GameObject;
                _eachObsFabs.Add(i, go);
            }

            _shotFabs = Resources.Load("prefabs/skill/playerSkill/shotFab") as GameObject;
            _curvedFabs = Resources.Load("prefabs/skill/playerSkill/curvedFab") as GameObject;
            _stampFabs = Resources.Load("prefabs/skill/playerSkill/stampFab") as GameObject;
            _rushFabs = Resources.Load("prefabs/skill/playerSkill/rushFab") as GameObject;
            ShieldFabs = Resources.Load("prefabs/skill/playerSkill/shieldFab") as GameObject;
            _fieldFabs = Resources.Load("prefabs/skill/playerSkill/fieldFab") as GameObject;
            _petFabs = Resources.Load("prefabs/skill/playerSkill/PetFab") as GameObject;

            _enProjFabs = new Dictionary<EnShot, GameObject>();
            for (EnShot i = (EnShot)0; i < EnShot.max; i++)
            {
                GameObject go = Resources.Load("prefabs/skill/enemySkill/" + i.ToString()) as GameObject;
                _enProjFabs.Add(i, go);
            }

            Statusicon = new Dictionary<statusKeyList, Sprite>();
            Sprite[] sts = Resources.LoadAll<Sprite>("sprite/statIcons");
            string name;
            for (int i = 0; i < sts.Length; i++)
            {
                name = sts[i].name;

                for (statusKeyList sd = statusKeyList.hp; sd < statusKeyList.max; sd++)
                {
                    if (name.Equals(sd.ToString()))
                    {
                        Statusicon.Add(sd, sts[i]);
                    }
                }
            }

            Skillicon = new Dictionary<SkillKeyList, Sprite>();
            Sprite[] sps = Resources.LoadAll<Sprite>("sprite/skills");

            for (int i = 0; i < sps.Length; i++)
            {
                name = sps[i].name;
                if (name[0] != 'i')
                    continue;

                for (SkillKeyList sk= SkillKeyList.HP; sk < SkillKeyList.non; sk++)
                {
                    if (name.Equals("i_" + sk.ToString()))
                    {
                        Skillicon.Add(sk, sps[i]);
                    }
                }
            }

            ShotBulletData = new Dictionary<ShotList, shotbulletData>();
            CurveImgs = new Dictionary<ShotList, Sprite>();
            SnowballImg = new Dictionary<snowballType, Sprite>();
            Sprite[] shots = Resources.LoadAll<Sprite>("sprite/skillShot");

            for (int i = 0; i < shots.Length; i++)
            {
                name = shots[i].name;

                for (ShotList sk = ShotList.SnowBall; sk < ShotList.IceBall; sk++)
                {
                    if (name.Equals(sk.ToString()))
                    {
                        shotbulletData sbd = new shotbulletData(
                            D_bulletData.GetEntity(sk.ToString()).f_colSize,
                            D_bulletData.GetEntity(sk.ToString()).f_tailSize,
                            D_bulletData.GetEntity(sk.ToString()).f_speed,
                            D_bulletData.GetEntity(sk.ToString()).f_ani,
                            shots[i]
                            );

                        ShotBulletData.Add(sk, sbd);
                        break;
                    }
                }

                for (ShotList sk = ShotList.IceBall; sk < ShotList.None; sk++)
                {
                    if (name.Equals(sk.ToString()))
                    {
                        CurveImgs.Add(sk, shots[i]);
                        break;
                    }
                }

                for (snowballType ball = snowballType.CitrusBall; ball < snowballType.standard; ball++)
                {
                    if (name.Equals(ball.ToString()))
                    {
                        SnowballImg.Add(ball, shots[i]);
                        break;
                    }
                }
                
            }

            SkinSprite = new Dictionary<SkinKeyList, Sprite>();
            Sprite[] sks = Resources.LoadAll<Sprite>("sprite/snowmans");

            for (int i = 0; i < sks.Length; i++)
            {
                name = sks[i].name;

                for (SkinKeyList sk = SkinKeyList.snowman; sk < SkinKeyList.max; sk++)
                {
                    if (name.Equals(sk.ToString()))
                    {
                        SkinSprite.Add(sk, sks[i]);
                    }
                }
            }

            PanelFabs = new Dictionary<lobbyPanel, GameObject>();
            for (lobbyPanel lp = (lobbyPanel)0; lp < lobbyPanel.max; lp++)
            {
                GameObject go = Resources.Load("prefabs/UI/Lobby/" + lp.ToString()) as GameObject;
                PanelFabs.Add(lp, go);
            }

            // 사운드
            Bgm = new Dictionary<BGM, AudioClip>();
            for (BGM i = 0; i < BGM.max; i++)
            {
                AudioClip ac = Resources.Load<AudioClip>("sound/BGM/" + i.ToString());
                Bgm.Add(i, ac);
            }

            Sfx = new Dictionary<SFX, AudioClip>();
            for (SFX i = 0; i < SFX.max; i++)
            {
                AudioClip ac = Resources.Load<AudioClip>("sound/SFX/" + i.ToString());
                Sfx.Add(i, ac);
            }

            // 인게임용 데이터 가져오기
            {
                NotiSprite = new Dictionary<NotiType, Sprite>();
                for (NotiType i = NotiType.boss; i < NotiType.non; i++)
                {
                    Sprite sp = Resources.Load<Sprite>("sprite/inGame/" + i.ToString());
                    NotiSprite.Add(i, sp);
                }

                QuestSprite = new Dictionary<InQuestKeyList, Sprite>();
                Sprite[] qst = Resources.LoadAll<Sprite>("sprite/inGame/quest");
                for (int i = 0; i < qst.Length; i++)
                {
                    QuestSprite.Add((InQuestKeyList)i, qst[i]);
                }

                _shieldImgs = new Dictionary<SkillKeyList, Sprite>();
                Sprite[] sd = Resources.LoadAll<Sprite>("sprite/shield");
                for (int i = 0; i < sd.Length; i++)
                {
                    SkillKeyList skl = EnumHelper.StringToEnum<SkillKeyList>(sd[i].name);
                    _shieldImgs.Add(skl, sd[i]);
                }
            }
        }
    }
}