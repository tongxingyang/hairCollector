using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;

namespace week
{
    [Serializable]
    public class DeviceData
    {
        [SerializeField] public float BgmVol;
        [SerializeField] public float SfxVol;

        /// <summary> 화면 흔들림 on/off </summary>
        [SerializeField] public bool OnShake;

        /// <summary> 난이도별 날짜 넘어가는 신기록 갱신시 리뷰창 소환 </summary>
        [SerializeField] public int[] RecommendDay;   
        
        /// <summary> 리뷰창 소환 여부 </summary>
        public bool showRecommend { get; set; } = false;    //->게임실행시 초기화

        public DeviceData()
        {
            BgmVol = SfxVol = 1f;
            RecommendDay = new int[3] { 1, 1, 1 };
            OnShake = true;
        }
    }
    
    public class UserGameData
    {
        public enum defaultStat { hp, att, def, hpgen, cool, exp, coin, speed, max }
        
        /// <summary> 저장되는 정보 </summary>
        UserEntity _userEntity;
        
        /// <summary> 가장 최근 랭킹 최신화 시간 </summary>
        public DateTime _rankRefreshTime { get; set; }

        /// <summary> 전투결과 </summary>
        public ObscuredInt[] GameReward { get; set; }

        /// <summary> 이전 랭킹 </summary>
        public int preRank { get; set; } = -1;

        bool autoRecivePost;

        /// <summary> 타임스탬프 간격 </summary>
        public float TimeCheck { get; set; }

        #region -------------------------[skin value]-------------------------

        /// <summary> 적용할 계절 </summary>
        season? _applySeason = null;


        #region [skin properties]

        /// <summary> 상시적용 스킨 능력치 </summary>
        public ObscuredFloat[] AddStats { get; private set; }
        public season? ApplySeason { get => _applySeason; set => _applySeason = value; }
        public ObscuredFloat SkinFval { get; private set; }
        public ObscuredInt SkinIval { get; private set; }
        public snowballType BallType { get; private set; }

        #endregion

        #endregion

        public UserEntity.property Property { get => _userEntity._property; set => _userEntity._property = value; }
        public UserEntity.status Status { get => _userEntity._status; set => _userEntity._status = value; }
        public UserEntity.quest Quest { get => _userEntity._quest; set => _userEntity._quest = value; }
        public UserEntity.payment Payment { get => _userEntity._payment; set => _userEntity._payment = value; }
        public UserEntity.gameUtility Util { get => _userEntity._util; set => _userEntity._util = value; }

        #region [properties]

        // 기본 정보 ==============================================================
        // - 닉 / 재화 
        public string NickName { get => _userEntity._property._nickName; set => _userEntity._property._nickName = value; }
        public ObscuredInt Coin { get => _userEntity._property._currency[(int)currency.coin]; set => _userEntity._property._currency[(int)currency.coin] = value; }
        public ObscuredInt Gem { get => _userEntity._property._currency[(int)currency.gem]; set => _userEntity._property._currency[(int)currency.gem] = value; }
        public ObscuredInt Ap { get => _userEntity._property._currency[(int)currency.ap]; set => _userEntity._property._currency[(int)currency.ap] = value; }

        public ObscuredInt followCoin { get; set; }
        public ObscuredInt followGem { get; set; }              

        // - 스킨
        public ObscuredInt HasSkin { get => _userEntity._property._hasSkin; set => _userEntity._property._hasSkin = value; }
        public ObscuredInt[] SkinLevel { get => _userEntity._property._skinLevel; set => _userEntity._property._skinLevel = value; }
        public SkinKeyList Skin { get => (SkinKeyList)((int)_userEntity._property._skin); set => _userEntity._property._skin = (int)value; }

        // - 난이도
        public levelKey NowStageLevel { get => (levelKey)Convert.ToInt32(_userEntity._property._nowStageLevel); set => _userEntity._property._nowStageLevel = (int)value; }
        public bool IsLevelOpen(levelKey lvl) { return (_userEntity._property._isLevelOpen & (1 << (int)lvl)) > 0; }
        /// <summary> 어느 난이도를 클리어했는지 입력 -> 클리어한 난이도+1된 난이도 해금 </summary>
        public void setLevelOpen(levelKey nowLvl) 
        { 
            int lvldata = _userEntity._property._isLevelOpen;
            lvldata|= (1 << (int)nowLvl + 1); 
            _userEntity._property._isLevelOpen = lvldata;
        }
        
        // - 템(구 레이더)

        // 플레이어 통계 데이터 ==============================================================
        public ObscuredInt WholeAccessTime { get => _userEntity._statistics._wholeAccessTime; set => _userEntity._statistics._wholeAccessTime = value; }
        public ObscuredInt PlayCount { get => _userEntity._statistics._playCount; set => _userEntity._statistics._playCount = value; }
        public ObscuredInt StoreUseCount { get => _userEntity._statistics._storeUseCount; set => _userEntity._statistics._storeUseCount = value; }

        // 기록 ==============================================================
        // public string NowSeasonRankKey() { return AuthManager. _userEntity._record._levelRecord[NowStageLevel]._nowSeasonRankKey; }
        public UserEntity.levelRecord[] LevelsRecord   { get => _userEntity._record._levelRecord; set => _userEntity._record._levelRecord = value; }

        //public int[] SeasonRecordSkin   { get => _userEntity._record._season_RecordSkin; set => _userEntity._record._season_RecordSkin = value; }
        //public int[] SeasonRecordLevel  { get => _userEntity._record._season_RecordLevel; set => _userEntity._record._season_RecordLevel = value; }
        //public int[] SeasonRecordBoss   { get => _userEntity._record._season_RecordBoss; set => _userEntity._record._season_RecordBoss = value; }

        public int TimeRecord(levelKey lvl) { return _userEntity._record._levelRecord[(int)lvl]._season_TimeRecord; }
        public int RecordSkin(levelKey lvl) { return _userEntity._record._levelRecord[(int)lvl]._season_RecordSkin; }
        public int RecordLevel(levelKey lvl) { return _userEntity._record._levelRecord[(int)lvl]._season_RecordLevel; }
        public int RecordBoss(levelKey lvl) { return _userEntity._record._levelRecord[(int)lvl]._season_RecordBoss; }

        public ObscuredInt RequestRecord { get => _userEntity._record._requestRecord; set => _userEntity._record._requestRecord = value; }
        public ObscuredInt ReinRecord { get => _userEntity._record._reinRecord; set => _userEntity._record._reinRecord = value; }
        public ObscuredInt WholeTimeRecord { get => _userEntity._record._wholeTimeRecord; set => _userEntity._record._wholeTimeRecord = value; }

        // 퀘스트 ==============================================================
        // - 일일
        /// <summary> (일일퀘스트) 1~6 </summary>
        public ObscuredInt[] DayQuest { get => _userEntity._quest._questChk; set => _userEntity._quest._questChk = value; }
        /// <summary> (일일퀘스트) 오늘의 스킨 </summary>
        public int QuestSkin { get => _userEntity._quest._questSkin; set => _userEntity._quest._questSkin = value; }
        /// <summary> (일일퀘스트) 오늘의 스킬s 정보 </summary>
        public SkillKeyList QuestSkill(int i) { return (SkillKeyList)Convert.ToInt32(_userEntity._quest._questSkill[i]); }
        // - 반복
        public ObscuredInt QuestRein { get => _userEntity._quest._questRein; set => _userEntity._quest._questRein = value; }
        public ObscuredInt QuestRequest { get => _userEntity._quest._questRequest; set => _userEntity._quest._questRequest = value; }
        // - 난이도별
        /// <summary> 생존일 </summary>
        public ObscuredInt[] LvlTimeReward { get => _userEntity._quest._lvlTimeReward; set => _userEntity._quest._lvlTimeReward = value; }
        /// <summary> 보스킬 </summary>
        public ObscuredInt[] LvlBossReward { get => _userEntity._quest._lvlBossReward; set => _userEntity._quest._lvlBossReward = value; }

        // 스탯 ==============================================================
        public ObscuredInt[] StatusLevel { get => _userEntity._status._statusLevel; set => _userEntity._status._statusLevel = value; }

        public ObscuredFloat o_Hp           { get; set; }
        public ObscuredFloat o_Att          { get; set; }
        public ObscuredFloat o_Hpgen        { get; set; }
        public ObscuredFloat o_Def          { get; set; }
        public ObscuredFloat o_Cool         { get; set; }
        public ObscuredFloat o_ExpFactor    { get; set; }
        public ObscuredFloat o_CoinFactor   { get; set; }
        // public ObscuredFloat SkinEnhance    { get; set; }

        // 인앱결제 ==============================================================
        public int amcl { get => _userEntity._payment._mulCoinList; }
        public void AddMulCoinList(mulCoinChkList index) 
        { 
            _userEntity._payment._mulCoinList |= (1 << (int)index); 
        }
        public bool chkMulCoinList(mulCoinChkList index)
        {
            return (_userEntity._payment._mulCoinList & (1 << (int)index)) > 0;
        }
        public ObscuredInt PaymentChkList { get => _userEntity._payment._chkList; set => _userEntity._payment._chkList = value; }
        public bool RemoveAd { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.removeAD)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.removeAD) : 0; }        
        public bool StartPack { get => (_userEntity._payment._chkList & (1 << (int)paymentChkList.startPack)) > 0;
            set => _userEntity._payment._chkList |= (value) ? (1 << (int)paymentChkList.startPack) : 0; }

        public bool VampPack { get => _userEntity._payment._vampPack; set => _userEntity._payment._vampPack = value; }
        public bool HeroPack { get => _userEntity._payment._heroPack; set => _userEntity._payment._heroPack = value; }

        public int LeftFreeGem { get => _userEntity._payment._leftFreeGem; set => _userEntity._payment._leftFreeGem = value; }

        // 유틸 ==============================================================
        public ObscuredLong LastSave { get => _userEntity._util._lastSave;
            set
            {
                if (AuthManager.instance.networkCheck()) 
                { _userEntity._util._lastSave = value; };
            }
        }
        public ObscuredInt PublishDate { get => _userEntity._util._publishDate; set => _userEntity._util._publishDate = value; }
        public ObscuredInt UtilChkList { get => _userEntity._util._chkList; set => _userEntity._util._chkList = value; }
        public bool FreeNichkChange { get => (_userEntity._util._chkList & (1 << (int)utilityChkList.freeNickChange)) > 0;
                                        set => _userEntity._util._chkList |= (value) ? (1 << (int)utilityChkList.freeNickChange) : 0; }
        public bool Change_SecondStatus { get => (_userEntity._util._chkList & (1 << (int)utilityChkList.change_SecondStatus)) > 0;
                                        set => _userEntity._util._chkList |= (value) ? (1 << (int)utilityChkList.change_SecondStatus) : 0; }
        public bool Success_Recommend { get => (_userEntity._util._chkList & (1 << (int)utilityChkList.success_Recommend)) > 0;
            set => _userEntity._util._chkList |= (value) ? (1 << (int)utilityChkList.success_Recommend) : 0; }

        /// <summary> 이용약관 동의 </summary>
        public bool Agreement { get => _userEntity._util._agreement; set => _userEntity._util._agreement = value; }
        /// <summary> 210622일자 버그로 인한 랭킹 초기화 체크 </summary>
        public bool BugLogChk { get => _userEntity._util._bugLogChk; set => _userEntity._util._bugLogChk = value; }

        #endregion

        #region [ES3 저장/로드]

        /// <summary> 데이터 기기 저장(오프라인) </summary>
        public void saveOffLineData()
        {            
            ES3.Save(gameValues._offlineKey, _userEntity.saveData());
        }

        /// <summary> 데이터 기기 로드(오프라인) </summary>
        public void loadOffLineData()
        {
            ObscuredString _offlineDataJson = ES3.Load<string>(gameValues._offlineKey);
            UserEntity _offlineData = JsonConvert.DeserializeObject<UserEntity>(_offlineDataJson, new ObscuredValueConverter());

            _userEntity = _offlineData;
        }

        public void setUserEntity(UserEntity entity)
        {
            //_userEntity = new UserEntity(entity);
            _userEntity = entity;
            applySkin();
        }

        #endregion

        #region [초기화]

        public UserGameData()
        {
            _userEntity = new UserEntity();
            applyLevel();

            applySkin();

            setDayData();
        }

        public UserGameData setTest()
        {
            _userEntity._property._nickName = "Test";
            FreeNichkChange = true;
            _userEntity._property._hasSkin = 131071;

            return this;
        }

        public void flashData()
        {
        }

        /// <summary> obscured 키 Randomize </summary>
        public IEnumerator RandomizeKey_Coroutine()
        {
            var oneSecondWait = new WaitForSecondsRealtime(1f);
            while(true)
            {
                yield return oneSecondWait;

                // 재화 gBrtJ0L0MzMadc5SrEWT3qE2y3B2
                for (int i = 0; i < 3; i++)
                {
                    _userEntity._property._currency[i].RandomizeCryptoKey();
                }
                Coin.RandomizeCryptoKey();
                Gem.RandomizeCryptoKey();
                Ap.RandomizeCryptoKey();

                // 강화창
                for (int i = 0; i < (int)statusKeyList.max; i++)
                {
                    _userEntity._status._statusLevel[i].RandomizeCryptoKey();
                    StatusLevel[i].RandomizeCryptoKey();
                }                
            }
        }

        /// <summary> 스탯 레벨 -> 스탯에 적용 </summary>
        public void applyLevel()
        {
            o_Hp            = D_status.GetEntity(statusKeyList.hp.ToString()).f_origin      + getAddit(statusKeyList.hp);
            o_Att           = D_status.GetEntity(statusKeyList.att.ToString()).f_origin     + getAddit(statusKeyList.att);
            o_Def           = D_status.GetEntity(statusKeyList.def.ToString()).f_origin     + getAddit(statusKeyList.def);
            o_Hpgen         = D_status.GetEntity(statusKeyList.hpgen.ToString()).f_origin   + getAddit(statusKeyList.hpgen);
            o_Cool          = D_status.GetEntity(statusKeyList.cool.ToString()).f_origin    + getAddit(statusKeyList.cool);
            o_ExpFactor     = D_status.GetEntity(statusKeyList.exp.ToString()).f_origin     + getAddit(statusKeyList.exp);
            o_CoinFactor    = D_status.GetEntity(statusKeyList.coin.ToString()).f_origin    + getAddit(statusKeyList.coin);
            // SkinEnhance     = StatusLevel[(int)statusKeyList.skin];
        }

        public float getAddit(statusKeyList type, int add = 0)
        {
            if (type == statusKeyList.skin)
                return StatusLevel[(int)statusKeyList.skin] + add;

            int lvl = StatusLevel[(int)type] + add;
            float val = 0;
            int bronze = D_status.GetEntity(type.ToString()).f_bronze;
            int silver = D_status.GetEntity(type.ToString()).f_silver- D_status.GetEntity(type.ToString()).f_bronze;
            int gold = D_status.GetEntity(type.ToString()).f_gold - D_status.GetEntity(type.ToString()).f_silver;
            int plat = D_status.GetEntity(type.ToString()).f_platinum - D_status.GetEntity(type.ToString()).f_gold;


            if (lvl < bronze)
            {
                val += D_status.GetEntity(type.ToString()).f_bronzeAddit * lvl;
            }
            else
            {
                val += D_status.GetEntity(type.ToString()).f_bronzeAddit * (bronze - 1) + D_status.GetEntity(type.ToString()).f_bronzeReward;
                lvl -= bronze;

                if (lvl == 0) return val;

                if (lvl < silver)
                {
                    val += D_status.GetEntity(type.ToString()).f_silverAddit * lvl;
                }
                else
                {
                    val += D_status.GetEntity(type.ToString()).f_silverAddit * (silver - 1) + D_status.GetEntity(type.ToString()).f_silverReward;
                    lvl -= silver;

                    if (lvl == 0) return val;

                    if (lvl < gold)
                    {
                        val += D_status.GetEntity(type.ToString()).f_goldAddit * lvl;
                    }
                    else
                    {
                        val += D_status.GetEntity(type.ToString()).f_goldAddit * gold + D_status.GetEntity(type.ToString()).f_goldReward;
                        lvl -= gold;

                        if (lvl == 0) return val;

                        if (lvl < plat)
                        {
                            val += D_status.GetEntity(type.ToString()).f_platinumAddit * lvl;
                        }
                    }
                }
            }

            return val;
        }

        #endregion

        #region [강화/스킨/적용]

        /// <summary> 스탯 레벨 업 </summary>
        public void statusLevelUp(statusKeyList stat)
        {
            StatusLevel[(int)stat]++;

            switch (stat)
            {
                case statusKeyList.hp:
                    o_Hp = D_status.GetEntity($"{statusKeyList.hp}").f_origin + (getAddit(statusKeyList.hp) * StatusLevel[(int)statusKeyList.hp]);
                    break;
                case statusKeyList.att:
                    o_Att = D_status.GetEntity($"{statusKeyList.att}").f_origin + (getAddit(statusKeyList.att) * StatusLevel[(int)statusKeyList.att]);
                    break;
                case statusKeyList.def:
                    o_Def = D_status.GetEntity($"{statusKeyList.def}").f_origin + (getAddit(statusKeyList.def) * StatusLevel[(int)statusKeyList.def]);
                    break;
                case statusKeyList.hpgen:
                    o_Hpgen = D_status.GetEntity($"{statusKeyList.hpgen}").f_origin+ (getAddit(statusKeyList.hpgen) * StatusLevel[(int)statusKeyList.hpgen]);
                    Debug.Log("2 : " + o_Hpgen);
                    break;
                case statusKeyList.cool:
                    o_Cool = D_status.GetEntity($"{statusKeyList.cool}").f_origin + (getAddit(statusKeyList.cool) * StatusLevel[(int)statusKeyList.cool]);
                    break;
                case statusKeyList.exp:
                    o_ExpFactor = D_status.GetEntity($"{statusKeyList.exp}").f_origin + (getAddit(statusKeyList.exp) * StatusLevel[(int)statusKeyList.exp]);
                    break;
                case statusKeyList.coin:
                    o_CoinFactor = D_status.GetEntity($"{statusKeyList.coin}").f_origin + (getAddit(statusKeyList.coin) * StatusLevel[(int)statusKeyList.coin]);
                    break;
                case statusKeyList.skin:
                    //SkinEnhance = StatusLevel[(int)statusKeyList.skin];
                    break;
            }
        }
        
        /// <summary> 스킨과 스킨 능력치 적용 </summary>
        public void applySkin()
        {
            SkinKeyList skin = (SkinKeyList)((int)_userEntity._property._skin);
            string key = skin.ToString();

            string ss = D_skin.GetEntity(key.ToString()).f_season;

            ObscuredInt j;

            BallType = snowballType.standard;

            AddStats = new ObscuredFloat[(int)defaultStat.max];
            for (defaultStat i = defaultStat.hp; i < defaultStat.max; i++)
            {
                j = (int)i;

                float m = (i == defaultStat.def || i == defaultStat.hpgen) ? 1f : 0.01f;
                AddStats[(int)i] = D_skin.GetEntity(key.ToString()).Get<float>((SkinValData.d_hp + j).ToString()) * m;

                if (i < defaultStat.speed)
                {
                    AddStats[(int)i] += D_skin.GetEntity(key.ToString()).Get<float>((SkinValData.ex_hp + j).ToString()) * m * SkinLevel[(int)skin];
                }
            }

            if (ss.Equals("n") == false)
            {
                _applySeason = EnumHelper.StringToEnum<season>(ss);
            }

            string t_F = D_skin.GetEntity(key.ToString()).f_typeF;
            if (t_F.Equals("n") == false)
            {
                SkinFval = D_skin.GetEntity(key.ToString()).f_Fval0 + SkinLevel[(int)skin] * D_skin.GetEntity(key.ToString()).f_Fval1;
            }

            string t_I = D_skin.GetEntity(key.ToString()).f_typeI;
            if (t_I.Equals("n") == false)
            {
                SkinIval = D_skin.GetEntity(key.ToString()).f_Ival;
            }

            string sb = D_skin.GetEntity(key.ToString()).f_snowball;
            if (sb.Equals("n") == false)
            {
                BallType = EnumHelper.StringToEnum<snowballType>(sb);
            }
        }

        /// <summary> 스킨구매전 데이터 제공용 </summary>
        //public void getSkinInfo(SkinKeyList skin, ref float[] fval, ref int[] ival, ref float[] stats)
        public void getSkinInfo(SkinKeyList skin, ref float[] stats)
        {
            string key = skin.ToString();
            int j;
            Debug.Log(D_skin.GetEntity(key).f_d_speed);
            for (defaultStat i = defaultStat.hp; i < defaultStat.max; i++)
            {
                j = (int)i;

                stats[(int)i] = D_skin.GetEntity(key).Get<float>((SkinValData.d_hp + j).ToString()) * 0.01f;

                if (i < defaultStat.speed)
                {
                    stats[(int)i] += D_skin.GetEntity(key).Get<float>((SkinValData.ex_hp + j).ToString()) * 0.01f; ;
                }
            }

            //string t_F = D_skin.GetEntity(key.ToString()).f_typeF;
            //if (t_F.Equals("n") == false)
            //{
            //    skinFvalue sfv = EnumHelper.StringToEnum<skinFvalue>(t_F);

            //    fval[(int)sfv] = D_skin.GetEntity(key.ToString()).f_Fval0;
            //    fval[(int)sfv] += _userEntity._status._statusLevel[(int)statusKeyList.skin] * D_skin.GetEntity(key.ToString()).f_Fval1;
            //}

            //string t_I = D_skin.GetEntity(key.ToString()).f_typeI;

            //if (t_I.Equals("n") == false)
            //{
            //    skinIvalue siv = EnumHelper.StringToEnum<skinIvalue>(t_I);

            //    ival[(int)siv] = D_skin.GetEntity(key.ToString()).f_Ival;
            //    Debug.Log(ival[(int)siv]);
            //}
        }

        /// <summary> 스킨 설명 </summary>
        public string getSkinExplain(SkinKeyList skin, bool ApplyData = true)
        {
            string key = skin.ToString();

            float[] stats = new float[(int)defaultStat.max];
            float fval = D_skin.GetEntity(skin.ToString()).f_Fval0 + SkinLevel[(int)skin] * D_skin.GetEntity(skin.ToString()).f_Fval1;
            int ival = D_skin.GetEntity(skin.ToString()).f_Ival;

            int j;
            for (defaultStat i = defaultStat.hp; i < defaultStat.max; i++)
            {
                j = (int)i;

                //float m = (i == UserGameData.defaultStat.def || i == UserGameData.defaultStat.hpgen) ? 1f : 0.01f;
                stats[(int)i] = D_skin.GetEntity(key).Get<float>((SkinValData.d_hp + j).ToString());

                if (i < defaultStat.speed)
                {
                    stats[(int)i] += D_skin.GetEntity(key).Get<float>((SkinValData.ex_hp + j).ToString()) * SkinLevel[(int)skin];
                }
            }

            string str = "";

            switch (skin)
            {
                case SkinKeyList.snowman:
                    str = "겨울에 만들어진 눈사람";
                    break;
                case SkinKeyList.fireman:
                    str = "여름한정!" + System.Environment.NewLine + string.Format("공격력 {0:0}% 증가", stats[(int)defaultStat.att] - 100);
                    break;
                case SkinKeyList.grassman:
                    str = "봄 한정!" + System.Environment.NewLine + string.Format("체력재생량 {0:0}% 증가", stats[(int)defaultStat.hpgen] - 100);
                    break;
                case SkinKeyList.rockman:
                    str = "눈 대신 돌을 던진다." + System.Environment.NewLine + string.Format("체력 {0:0}% 증가", stats[(int)defaultStat.hp] - 100);
                    break;
                case SkinKeyList.citrusman:
                    // 미정
                    str = "눈 대신 귤을 던진다.";
                    break;
                case SkinKeyList.bulbman:
                    str = "머리의 전구로 밤을 밝힌다.";
                    break;
                case SkinKeyList.presentman:
                    str = "눈 대신 선물 받아라!" + System.Environment.NewLine + "겨울한정!" + System.Environment.NewLine + string.Format("코인획득량 {0:0.0}% 증가", stats[(int)defaultStat.coin] - 100);
                    break;
                case SkinKeyList.wildman:
                    str = String.Format("잃은 체력 1%당 공격력 {0:0.0}% 증가", fval);
                    break;
                case SkinKeyList.mineman:
                    str = $"지뢰 개수 +{ival}" + System.Environment.NewLine + $"지뢰 공격력 {fval * 100f}% 증가";
                    break;
                case SkinKeyList.robotman:
                    str = "늪지 무효" + System.Environment.NewLine + string.Format("추가 방어 {0:0}", stats[(int)defaultStat.def]);
                    break;
                case SkinKeyList.icecreamman:
                    str = "눈덩이가 빙결 적용" + System.Environment.NewLine + $"블리자드, 아이스에이지 발동시 체력 {fval}% 회복";
                    break;
                case SkinKeyList.goldman:
                    str = $"스킬 무적 즉시습득" + System.Environment.NewLine + string.Format("방어력 {0:0}% 증가", stats[(int)defaultStat.def]);
                    break;
                case SkinKeyList.angelman:
                    str = $"눈사람이 죽을때" + System.Environment.NewLine + $"최대체력의 {fval * 100f}%로 부활" + System.Environment.NewLine + string.Format("이동속도 {0:0}% 증가", stats[(int)defaultStat.speed] - 100);
                    break;
                case SkinKeyList.squareman:
                    str = "네모난 눈을 던진다." + System.Environment.NewLine + $"30% 확률로 {fval}%의 추가데미지를 준다.";
                    break;
                case SkinKeyList.spiderman:
                    str = "추가로 달린 다리로 더 많은 눈덩이를 던진다." + System.Environment.NewLine + $"눈덩이 공격력 {fval}% 증가";
                    break;
                case SkinKeyList.vampireman:
                    str = string.Format("공격력 {0:0}% 증가", stats[(int)defaultStat.att] - 100) + System.Environment.NewLine + $"눈덩이로 준 피해의 {fval * 100f}%만큼 체력 회복."  ;
                    break;
                case SkinKeyList.heroman:
                    ObscuredInt[] stt = new ObscuredInt[3] { Convert.ToInt32(stats[(int)defaultStat.hp]) - 100,
                        Convert.ToInt32(stats[(int)defaultStat.att]) - 100, 
                        Convert.ToInt32(stats[(int)defaultStat.def])};

                    str = "맵에서 용사의 검을 찾으면" + System.Environment.NewLine + "일시적으로 공격/방어 대폭 강화."
                        + System.Environment.NewLine + $"체력 {stt[0]}% 증가 / 공격력 {stt[1]}% 증가" + System.Environment.NewLine + $"/방어력 {stt[2]} 증가";
                    break;
                case SkinKeyList.santaman:
                    str = $"넌 선물 1개! 난 선물 {ival}개!" + System.Environment.NewLine
                        + $"선물 먹으면 체력 {fval * 100f}% 회복" + System.Environment.NewLine + "낮은 확률로 공격력 or 방어력 증가";
                    break;
                case SkinKeyList.dragonman:
                    // 미정
                    break;
                default:
                    break;
            }
            return str;
        }

        #endregion

        #region [ 기타 설정 함수 ]

        /// <summary> (하루경과) 일일퀘스트 재설정 - [플레이시 한번 체크] </summary>
        public void setNextDay(int today)
        {
            //int today = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

            //if (today > PublishDate) // 다음날 - 새 퀘스트
            {
                PublishDate = today;

                Quest.setNextDay();

                LeftFreeGem = 3;
            }
        }

        /// <summary> 신기록 </summary>
        public void setNewSeasonRecord(int newRecord, int snowLvl, int boss)
        {
            LevelsRecord[(int)NowStageLevel].newRecord ( newRecord, _userEntity._property._skin, snowLvl, boss);
        }

        /// <summary> 새 시즌 개시시 초기화 </summary>
        public void whenRecordNewSeason()
        {
            for (int i = 0; i < (int)levelKey.max; i++)
            {
                _userEntity._record.initSeasonRecord();
            }
        }

        #endregion

        #region []

        Dictionary<levelKey, List<KeyValuePair<season, int>>> seasonStringData;
        public enum sson { season0, season1, season2, season3, max }
        string[] _dayName = new string[] { "첫째날 ", "둘째날 ", "셋째날 ", "넷째날 ", "다섯째날 ", "여섯째날 ",
                                            "일곱째날 ", "여덟째날 ", "아홉째날 ", "열째날 ", "열한째날 ", "열둘째날 " };

        /// <summary> 해당 레벨에서의 season별 day 데이터 </summary>
        void setDayData()
        {
            seasonStringData = new Dictionary<levelKey, List<KeyValuePair<season, int>>>();
            for (levelKey lvk = 0; lvk < levelKey.max; lvk++)
            {
                //season start = D_level.GetEntity(lvk.ToString()).f_startSeason;
                
                List<KeyValuePair<season, int>> seasonData = new List<KeyValuePair<season, int>>();

                for (sson ssn = 0; ssn < sson.max; ssn++)
                {
                    string[] str = D_level.GetEntity(NowStageLevel.ToString()).Get<string>(ssn.ToString()).Split(',');

                    season key = EnumHelper.StringToEnum<season>(str[0]);
                    int val = int.Parse(str[1]);

                    int num = seasonData.Count;
                    if (num > 0 && seasonData[num - 1].Key == key)
                    {
                        val += seasonData[num - 1].Value;
                        seasonData.RemoveAt(num);
                    }                    
                    
                    seasonData.Add(new KeyValuePair<season, int>(key, val));
                }

                //while (true)
                //{
                //    start++; 
                //    if (start == season.max)
                //    {
                //        start = season.spring;
                //    }

                //    int val = D_season.GetEntity(start.ToString()).Get<int>(lvk.ToString());

                //    if (seasonData.ContainsKey(start))
                //        break;
                //    else if (val == 0)
                //        continue;

                //    seasonData.Add(start, val);
                //}

                seasonStringData.Add(lvk, seasonData);
            }
        }

        /// <summary> ObscuredFloat 기록 --> 전부 string으로 변환 </summary>
        public string getLifeTime(levelKey lvl, float time, bool onlyDay = false, bool isTwoLine = false)
        {
            int _year = (int)(time / (12 * 120));
            time -= _year * 12 * 120;

            season _season = season.max;
            int _day = (int)(time / 120);
            time -= _day * 120;

            foreach (KeyValuePair<season, int> kv in seasonStringData[lvl])
            {
                _season = kv.Key;
                if (_day >= kv.Value)
                {
                    _day -= kv.Value;
                }
                else
                    break;
            }

            int _m;
            int _s;

            _m = (time > 60) ? 1 : 0;
            time -= _m * 60;

            _s = (int)time;

            // string으로 변환 ======================================================
            string str = "";
            if (_year > 0)
            {
                str += $"{_year}년 ";
            }

            str += D_season.GetEntity(_season.ToString()).f_sName + " ";
            
            str += _dayName[_day];

            if (onlyDay)
                return str;

            // ========== 두줄 =================
            if (isTwoLine)            
                str += System.Environment.NewLine;
            

            if (_m > 0)
            {
                str += $"{_m}분 ";
            }
            str += $"{_s}초";

            return str;
        }

        /// <summary> 일퀘에서 day -> string </summary>
        public string dayToTimeRecord(levelKey lvl, int day)
        {
            int _year = (int)(day / 12);
            day -= _year * 12;

            season _season = season.max;
            int _day = day;

            foreach (KeyValuePair<season, int> kv in seasonStringData[lvl])
            {
                _season = kv.Key;
                if (_day >= kv.Value)
                {
                    _day -= kv.Value;
                }
                else
                    break;
            }

            // string으로 변환 ======================================================
            string str = "";
            if (_year > 0)
            {
                str += $"{_year}년 ";
            }

            str += D_season.GetEntity(_season.ToString()).f_sName + " ";

            str += _dayName[_day];

            return str;
        }

        #endregion

        /// <summary> 데이터 저장 </summary>
        public string getUserData()
        {
            return _userEntity.saveData();
        }

        public Dictionary<string, object> getSeasonRankData(string uid, levelKey lvl)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["_uid"] = uid;
            data["_nick"] = NickName;
            data["_time"] = LevelsRecord[(int)lvl]._season_TimeRecord;
            data["_skin"] = LevelsRecord[(int)lvl]._season_RecordSkin;
            data["_level"] = LevelsRecord[(int)lvl]._season_RecordLevel;
            data["_boss"] = LevelsRecord[(int)lvl]._season_RecordBoss;            
            data["_version"] = 11;

            return data;
        }

        /// <summary>  </summary>
        public string getRankData(int skin)
        {
            rankSubData data = new rankSubData(skin);

            return JsonUtility.ToJson(data);
        }

        // 삭제
        public void resetPaymentChkList()
        {
            _userEntity._payment._chkList = 0;    
        }
    }
}