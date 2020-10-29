using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class UserGameData
    {
        public enum defaultStat { hp, att, def, hpgen, cool, exp, coin, speed, max }

        /// <summary> 저장되는 정보 </summary>
        UserEntity _userEntity;

        /// <summary> 랭킹최저점 </summary>
        float _minRank;

        /// <summary> 전투결과 </summary>
        public int[] GameReward { get; set; }

        #region -------------------------[skin value]-------------------------

        /// <summary> 상시적용 스킨 능력치 </summary>
        float[] _addStats;

        /// <summary> 적용할 계절 </summary>
        season? _applySeason = null;

        bool[] _skinBval;
        float[] _skinFval;
        int[] _skinIval;
        snowballType _ballType;

        #region [skin properties]

        public float[] AddStats { get => _addStats; set => _addStats = value; }
        public season? ApplySeason { get => _applySeason; set => _applySeason = value; }
        public bool[] SkinBval { get => _skinBval; }
        public float[] SkinFval { get => _skinFval; }
        public int[] SkinIval { get => _skinIval; }
        public snowballType BallType { get => _ballType; }

        #endregion

        #endregion

        #region [properties]

        public string NickName { get => _userEntity._nickName; set => _userEntity._nickName = value; }
        public int Coin { get => _userEntity._coin; set => _userEntity._coin = value; }
        public int followCoin { get; set; }
        public int Gem { get => _userEntity._gem; set => _userEntity._gem = value; }
        public int followGem { get; set; }
        public int Ap { get => _userEntity._ap; set => _userEntity._ap = value; }
        public SkinKeyList Skin { get => (SkinKeyList)_userEntity._skin; set => _userEntity._skin = (int)value; }

        //  기록 및 퀘스트
        public DateTime LastLogin { get => _userEntity._lastLogin; set => _userEntity._lastLogin = value; }
        // 일일
        public int[] DayQuest { get => _userEntity._dayQuest; set => _userEntity._dayQuest = value; }
        public int QuestSkin { get => _userEntity._questSkin; set => _userEntity._questSkin = value; }
        // 전체
        public int TimeRecord { get => _userEntity._timeRecord; set => _userEntity._timeRecord = value; }
        public int GetTimeReward { get => _userEntity._getTimeReward; set => _userEntity._getTimeReward = value; }
        public int BossRecord { get => _userEntity._bossRecord; set => _userEntity._bossRecord = value; }
        public int GetBossReward { get => _userEntity._getBossReward; set => _userEntity._getBossReward = value; }
        public int ArtifactRecord { get => _userEntity._artifactRecord; set => _userEntity._artifactRecord = value; }
        public int GetArtifactReward { get => _userEntity._getArtifactReward; set => _userEntity._getArtifactReward = value; }
        public int AdRecord { get => _userEntity._adRecord; set => _userEntity._adRecord = value; }
        public int ReinRecord { get => _userEntity._reinRecord; set => _userEntity._reinRecord = value; }

        //  스탯
        public int[] StatusLevel { get => _userEntity._statusLevel; set => _userEntity._statusLevel = value; }
        //  스킨
        public int HasSkin { get => _userEntity._hasSkin; set => _userEntity._hasSkin = value; }

        //  인앱결제
        public bool AddGoods { get => _userEntity._addGoods; set => _userEntity._addGoods = value; }
        public float AddGoodsValue { get => _userEntity._addGoodsValue; set => _userEntity._addGoodsValue = value; }
        public bool RemoveAD { get => _userEntity._removeAD; set => _userEntity._removeAD = value; }
        public bool SkinPack { get => _userEntity._skinPack; set => _userEntity._skinPack = value; }
        public bool StartPack { get => _userEntity._startPack; set => _userEntity._startPack = value; }

        public int o_Hp { get => _userEntity._hp; }
        public float o_Hpgen { get => _userEntity._hpgen; }
        public int o_Def { get => _userEntity._def; }
        public float o_Att { get => _userEntity._att; }
        public float o_Cool { get => _userEntity._cool; }
        public float o_ExpFactor { get => _userEntity._expFactor; }
        public float o_CoinFactor { get => _userEntity._coinFactor; }
        public float SkinEnhance { get => _userEntity._skinEnhance; }


        //  옵션
        public float BgmVol { get => _userEntity._bgmVol; set => _userEntity._bgmVol = value; }
        public float SfxVol { get => _userEntity._sfxVol; set => _userEntity._sfxVol = value; }

        #endregion

        #region [ES3 저장/로드]

        /// <summary> 저장 </summary>
        public void saveUserEntity()
        {
            ES3.Save<UserEntity>("userEntity", _userEntity);
        }

        /// <summary>  </summary>
        public void LoadUserEntity(UserEntity userEntity)
        {
            _userEntity = userEntity;
        }

        #endregion

        #region [초기화]

        public UserGameData()
        {
            _userEntity = new UserEntity();
            _userEntity.applyLevel();

            _skinBval = new bool[(int)skinBvalue.max];
            _skinFval = new float[(int)skinFvalue.max];
            _skinIval = new int[(int)skinIvalue.max];
            _ballType = snowballType.standard;

            applySkin();

            _userEntity.saveData();
        }

        public void flashData()
        {
        }

        #endregion

        #region [강화/스킨/적용]

        public void statusLevelUp(StatusData stat)
        {
            _userEntity.statusLevelUp(stat);
        }

        /// <summary>  </summary>
        public void applySkin()
        {
            SkinKeyList skin = (SkinKeyList)_userEntity._skin;
            string key = skin.ToString();

            string ss = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.season.ToString());

            int j;
            float chk;
            float add = 0;

            _addStats = new float[(int)defaultStat.max];
            for (defaultStat i = defaultStat.hp; i < defaultStat.max; i++)
            {
                j = (int)i;
                chk = DataManager.GetTable<float>(DataTable.skin, key, (SkinValData.d_hp + j).ToString());
                _addStats[(int)i] = chk;

                if (chk == 1)
                    continue;

                if (i < defaultStat.speed)
                {
                    add = _userEntity._statusLevel[(int)StatusData.skin] * DataManager.GetTable<float>(DataTable.skin, key, (SkinValData.ex_hp + j).ToString());
                }

                _addStats[(int)i] += add;
            }

            if (ss.Equals("n") == false)
            {
                _applySeason = EnumHelper.StringToEnum<season>(ss);
            }

            string t_B = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.typeB.ToString());
            if (t_B.Equals("n") == false) 
            {
                skinBvalue sbv = EnumHelper.StringToEnum<skinBvalue>(t_B);
                _skinBval[(int)sbv] = true;
            }

            string t_F = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.typeF.ToString());
            if (t_F.Equals("n") == false)
            {
                skinFvalue sfv = EnumHelper.StringToEnum<skinFvalue>(t_F);
                
                _skinFval[(int)sfv] = DataManager.GetTable<float>(DataTable.skin, key, SkinValData.Fval0.ToString());
                _skinFval[(int)sfv] += _userEntity._statusLevel[(int)StatusData.skin] * DataManager.GetTable<float>(DataTable.skin, key, SkinValData.Fval1.ToString());
            }

            string t_I = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.typeI.ToString());
            if (t_I.Equals("n") == false)
            {
                skinIvalue siv = EnumHelper.StringToEnum<skinIvalue>(t_I);

                _skinIval[(int)siv] = DataManager.GetTable<int>(DataTable.skin, key, SkinValData.Ival.ToString());
            }

            string sb = DataManager.GetTable<string>(DataTable.skin, key, SkinValData.snowball.ToString());
            if (sb.Equals("n") == false)
            {
                _ballType = EnumHelper.StringToEnum<snowballType>(sb);
            }
        }

        /// <summary> 스킨 설명 </summary>
        public string getSkinExplain(SkinKeyList skin)
        {
            string str = "";
            int statVal = 0;
            switch (skin)
            {
                case SkinKeyList.snowman:
                    str = "겨울에 만들어진 눈사람";
                    break;
                case SkinKeyList.fireman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.att] * 100) - 100;
                    str = "여름한정!" + System.Environment.NewLine + $"공격력 {statVal}% 증가";
                    break;
                case SkinKeyList.grassman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.hpgen] * 100) - 100;
                    str = "봄 한정!" + System.Environment.NewLine + $"체력재생량 {statVal}% 증가";
                    break;
                case SkinKeyList.rockman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.hp] * 100) - 100;
                    str = "눈 대신 돌을 던진다." + System.Environment.NewLine + $"체력 {statVal}% 증가";
                    break;
                case SkinKeyList.citrusman:
                    float val = _addStats[(int)defaultStat.coin] * 100 - 100;
                    str = "눈 대신 귤을 던진다." + System.Environment.NewLine + "겨울한정!" + System.Environment.NewLine + string.Format("코인획득량 {0:0.0}% 증가", val);
                    break;
                case SkinKeyList.bulbman:
                    str = "머리의 전구로 밤을 밝힌다.";
                    break;
                case SkinKeyList.wildman:
                    str = $"잃은 체력 1%당 공격력 {_skinFval[(int)skinFvalue.wild]}% 증가";
                    break;
                case SkinKeyList.mineman:
                    str = $"지뢰 개수 +{_skinIval[(int)skinIvalue.mine]}" + System.Environment.NewLine + $"지뢰 공격력 {_skinFval[(int)skinFvalue.mine]}% 증가";
                    break;
                case SkinKeyList.robotman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.def] * 100) - 100;
                    str = "늪지 무효" + System.Environment.NewLine + $"방어력 {statVal}% 증가";
                    break;
                case SkinKeyList.icecreamman:
                    str = "눈덩이가 빙결 적용" + System.Environment.NewLine + $"블리자드, 아이스에이지 발동시 체력 {_skinFval[(int)skinFvalue.iceHeal]}% 회복";
                    break;
                case SkinKeyList.goldenarmorman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.def] * 100) - 100;
                    str = $"30초에 한번씩 {_skinFval[(int)skinFvalue.invincible]}초간 무적" + System.Environment.NewLine + $"방어력 {statVal}% 증가";
                    break;
                case SkinKeyList.angelman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.speed] * 100) - 100;
                    str = $"눈사람이 죽을때 최대체력의 {_skinFval[(int)skinFvalue.rebirth]}%로 부활" + System.Environment.NewLine +"(유적과 버프 상실)" + System.Environment.NewLine + $"이동속도 {statVal}% 증가";
                    break;
                case SkinKeyList.squareman:
                    str = "네모난 눈을 던진다." + System.Environment.NewLine + $"30% 확률로 {_skinFval[(int)skinFvalue.criticDmg]}%의 추가데미지를 준다.";
                    break;
                case SkinKeyList.spiderman:
                    str = "추가로 달린 다리로 더 많은 눈덩이를 던진다." + System.Environment.NewLine + $"눈덩이 공격력 {_skinFval[(int)skinFvalue.snowball]}% 증가";
                    break;
                case SkinKeyList.vampireman:
                    statVal = Convert.ToInt32(_addStats[(int)defaultStat.att] * 100) - 100;
                    str = $"눈덩이로 준 피해의 {_skinFval[(int)skinFvalue.blood]}%만큼 체력을 회복한다." + System.Environment.NewLine + $"공격력 {statVal}% 증가";
                    break;
                case SkinKeyList.heroman:
                    int[] stt = new int[3] { Convert.ToInt32(_addStats[(int)defaultStat.hp] * 100) - 100,
                        Convert.ToInt32(_addStats[(int)defaultStat.att] * 100) - 100, 
                        Convert.ToInt32(_addStats[(int)defaultStat.def] * 100) - 100 };

                    str = "용사의 검을 찾으면 일시적으로 공격력/방어력이 2배가 된다."
                        + System.Environment.NewLine + $"체력 {stt[0]}% 증가/공격력 {stt[1]}% 증가/방어력 {stt[2]}% 증가";
                    break;
                default:
                    break;
            }
            return str;
        }

        #endregion

        /// <summary> float 기록 --> 전부 string으로 변환 </summary>
        public string getLifeTime(float time, bool isTwoLine)
        {
            int year;
            int season;
            int day;
            int m;
            int s;

            year = (int)(time / (24 * 60));
            time -= year * 24 * 60;
            season = (int)(time / (6 * 60));
            time -= season * 6 * 60;
            day = (int)(time / (2 * 60));
            time -= day * 2 * 60;
            m = (time > 60) ? 1 : 0;
            time -= m * 60;
            s = (int)time;

            string str = "";
            if (year > 0)
            {
                str += $"{year}년 ";
            }
            if (season > 0)
            {
                str += $"{season}계절 ";
            }
            if (day > 0)
            {
                switch (day)
                {
                    case 0:
                        str += "첫째날 ";
                        break;
                    case 1:
                        str += "둘째날 ";
                        break;
                    case 2:
                        str += "셋째날 ";
                        break;
                }
            }

            if (isTwoLine)
            {
                str += System.Environment.NewLine;
            }

            if (m > 0)
            {
                str += $"{m}분 ";
            }
            str += $"{s}초";

            return str;
        }

        /// <summary> float 기록 --> 날짜까지만 string으로 변환 </summary>
        public string getTimeRecordToString(float time)
        {
            int year;
            int season;
            int day;

            year = (int)(time / (24 * 60));
            time -= year * 24 * 60;
            season = (int)(time / (6 * 60));
            time -= season * 6 * 60;
            day = (int)(time / (2 * 60));
            time -= day * 2 * 60;

            string str = "";
            if (year > 0)
            {
                str += $"{year}년 ";
            }

            switch (season)
            {
                case 0:
                    str += "봄 ";
                    break;
                case 1:
                    str += "여름 ";
                    break;
                case 2:
                    str += "가을 ";
                    break;
                case 3:
                    str += "겨울 ";
                    break;
            }

            if (day > 0)
            {
                switch (day)
                {
                    case 0:
                        str += "첫째날";
                        break;
                    case 1:
                        str += "둘째날";
                        break;
                    case 2:
                        str += "셋째날";
                        break;
                }
            }

            return str;
        }
    }
}