using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.Rendering;
using DG.DemiLib;
using TMPro;

namespace week
{
    public class PlayerCtrl : spineCtrl, IPause
    {
        #region ----------------[GameObject Values]----------------

        [SerializeField] Rigidbody2D _rigid;

        [Header("etc")]
        [SerializeField] Transform _hpbar;
        [SerializeField] GameObject _almightCase;
        [SerializeField] Transform _albar;
        [SerializeField] Camera _main;
        [SerializeField] public BuffParticleManager _bffParticle;
        [SerializeField] TextMeshProUGUI _playerStat;

        GameScene _gs;
        enemyManager _enm;
        playerSkillManager _psm;
        dmgFontManager _dmgFont;
        effManager _efm;
        gameCompass _compass;
        public SkeletonAnimation pSpine { get => _spine; }
        public Camera MainCamera { get => _main; set => _main = value; }

        #endregion        

        #region ----------------[Stat]----------------
        // 기본, 스킬, 퀘, 버프, 기타, 계절
        enum statType { standard, skill, quest, buff, etc, season, max }
        float[][] _status;
        public float MaxHp
        {
            get { return _status[(int)statType.standard][(int)SkillKeyList.HP]; }
            set { _status[(int)statType.standard][(int)SkillKeyList.HP] = value; }
        }
        float _hp;
        public float Hp
        {
            get { return _hp; }
            set { _hp = value; _fluctuationHp?.Invoke(); }
        }
        /// <summary> 체력비율 </summary>
        public float hpRate { get => _hp / MaxHp; }
        /// <summary> 체력 변동시 동작 함수 </summary>
        Action _fluctuationHp;

        /// <summary> 공격력 </summary>
        public float Att { get { return calMul(SkillKeyList.ATT); } }
        /// <summary> 방어력 </summary>
        public float Def { get { return calAdd(SkillKeyList.DEF); } }
        /// <summary> 체력 재생 </summary>
        public float Hpgen { get { return calAdd(SkillKeyList.HPGEN); } }
        float _hpgenTimer;
        /// <summary> 힐량 </summary>
        public float HealMount { get { return calMul(SkillKeyList.HEALMOUNT); } }
        /// <summary> 시전 속도 </summary>
        public float Cool { get { return calMul(SkillKeyList.COOL); } }
        /// <summary> 이동속도 </summary>
        public float Speed { get { return calMul(SkillKeyList.SPEED) * _dashSpeed * _levelSpeed; } }
        /// <summary> 스킬크기 </summary>
        public float Size { get { return calMul(SkillKeyList.SIZE); } }
        /// <summary> 보스 추뎀 </summary>
        public float BossAtt { get { return calMul(SkillKeyList.BOSS); } }
        /// <summary> 회피 </summary>
        public float Dodge { get { return calAdd(SkillKeyList.DODGE); } }
        /// <summary> 경험치 </summary>
        public float Exp { get { return calMul(SkillKeyList.EXP); } }
        public float PlayerMaxExp { get; private set; } = gameValues._startExp;
        float _playerExp = 0f;
        public float ExpRate { get { return _playerExp / PlayerMaxExp; } }
        public bool levelUpable { get { return (_playerExp > PlayerMaxExp); } }
        /// <summary> 코인 </summary>
        public float Coin { get { return calMul(SkillKeyList.COIN); } }

        bool _isUpgrading;

        float _dashTime = 0f;
        float _dashSpeed = 1f;
        float _levelSpeed = 1f;

        public Dictionary<gainableTem, int> Inventory { get; private set; }

        List<BuffEffect> _buffList;
        dotDmg _dotDmg;
        public dotDmg DotDmg { get => _dotDmg; set => _dotDmg = value; }

        public Dictionary<SkillKeyList, ability> _abils;
        public Dictionary<SkillKeyList, skill> _skills;
        public Dictionary<SkillKeyList, ability> Abils { get => _abils; }
        public Dictionary<SkillKeyList, skill> Skills { get => _skills; }

        #endregion 

        #region ----------------[Skin Eff Values]----------------

        snowballType _ballType;

        bool[] _skinBval;
        float[] _skinFval;
        int[] _skinIval;

        bool _adRebirthable;
        public float darkLight 
        {
            get
            {
                if (_skinBval[(int)skinBvalue.light])
                {
                    return _skinFval[(int)skinFvalue.light];
                }
                return 1f;
            } 
        }

        public SkinKeyList Skin { get; private set; }

        #endregion

        /// <summary> 트리별 추가데미지 </summary>
        public int[] skillAddDmg { get; set; }
        public float getAddDmg(skillType type)
        {
            return 0.01f * (100 + skillAddDmg[(int)type]);
        }

        bool _isDie;
        bool _isAlmighty;
        float _almighTime;

        shotCtrl _shotBullet;
        curvedShotCtrl _range;
        skillMarkCtrl _stamp;

        bool _isDmgAction;
        Color _dmgC = new Color(1, 0.4f, 0.4f);
        Color _alC = new Color(0.7f, 1f, 1f);

        Action _gameOver;

        public void Init(GameScene gs, Action gameOver)
        {
            // 클래스 참조
            _gs = gs;
            _enm = _gs.EnemyMng;
            _psm = _gs.SkillMng;
            _dmgFont = _gs.DmgfntMng;
            _efm = _gs.EfMng;
            _gameOver = gameOver;

            // [게임] 체크 변수 초기화
            _adRebirthable = true;
            _isDie = false;
            _isAlmighty = false;
            _levelSpeed = gameValues.SnowmanSpeed * D_level.GetEntity(BaseManager.userGameData.NowStageLevel.ToString()).f_speed;
            skillAddDmg = new int[6];

            // 스킨*스탯
            _spine.skeleton.SetSkin(BaseManager.userGameData.Skin.ToString());
            getOriginStatus();

            Inventory = new Dictionary<gainableTem, int>();
            Inventory.Add(gainableTem.questKey, 0);

            // 버프
            _buffList = new List<BuffEffect>();
            _dotDmg = new dotDmg();

            getSkill(SkillKeyList.SnowBall);
            getSkill(SkillKeyList.Dash);

#if UNITY_EDITOR
            //for (int i = 0; i < 5; i++)
            //{
            //    getTem(gainableTem.questKey);
            //}
#endif
            if (Skin == SkinKeyList.mineman)
            {
                getSkill(SkillKeyList.Mine);
            }
            else if (Skin == SkinKeyList.goldman)
            {
                getSkill(SkillKeyList.Invincible);
            }
            else if (Skin == SkinKeyList.santaman)
            {
                getSkill(SkillKeyList.Present);
            }

            _almightCase.SetActive(false);

            StartCoroutine(skillUpdate());
            StartCoroutine(chk());
        }

        public void getskillaasdfadsf()
        {
            _gs.getInQuest();
        }

        IEnumerator chk()
        {
            while (true)
            {
                showStat();
                yield return new WaitForSeconds(1f);
            }
        }

        void showStat()
        {
            _playerStat.text =          string.Format("HP    : {0:0.0} [{1:0.0}]", Hp, MaxHp)
                + Environment.NewLine + string.Format("ATT   : {0:0.00}", Att)
                + Environment.NewLine + string.Format("DEF   : {0:0.0}", Def)
                + Environment.NewLine + string.Format("HPGEN : {0:0.0} [{1:0.00}][{2:0.0}]", Hpgen * HealMount, Hpgen, HealMount)
                + Environment.NewLine + string.Format("COOL  : {0:0.00}", Cool)
                + Environment.NewLine + string.Format("EXP   : {0:0.00}", Exp)
                + Environment.NewLine + string.Format("COIN  : {0:0.00}", Coin)
                + Environment.NewLine + string.Format("SPEED : {0:0.00}", Speed);
        }

        /// <summary> 최초 능력치 </summary>
        void getOriginStatus()
        {
            _status = new float[(int)statType.max][];
            for (int i = 0; i < (int)statType.max; i++)
            {
                _status[i] = new float[] { 1f, 1f, 0f, 0f, 1f, 1f, 1f, 1f, 1f, 0f, 1f, 1f };  // 방어, 체젠, 회피는 합연산
            }

            Hp = MaxHp = BaseManager.userGameData.o_Hp * BaseManager.userGameData.AddStats[(int)statusKeyList.hp];     // 체력은 계절 상관없이 초기적용

            // 스킨 스탯이 상시적용일때
            if (BaseManager.userGameData.ApplySeason == null)
            {
                _status[(int)statType.standard][(int)SkillKeyList.ATT] = BaseManager.userGameData.o_Att * BaseManager.userGameData.AddStats[(int)statusKeyList.att];    // 공격 곱
                _status[(int)statType.standard][(int)SkillKeyList.DEF] = BaseManager.userGameData.o_Def + BaseManager.userGameData.AddStats[(int)statusKeyList.def];    // 방어 합
                _status[(int)statType.standard][(int)SkillKeyList.HPGEN] = BaseManager.userGameData.o_Hpgen + BaseManager.userGameData.AddStats[(int)statusKeyList.hpgen];  // 체젠 합                
                _status[(int)statType.standard][(int)SkillKeyList.COOL] = (1f - BaseManager.userGameData.o_Cool) * (1f - BaseManager.userGameData.AddStats[(int)statusKeyList.cool]);    // 쿨탐 (1 - 곱)
                _status[(int)statType.standard][(int)SkillKeyList.EXP] = BaseManager.userGameData.o_ExpFactor * BaseManager.userGameData.AddStats[(int)statusKeyList.exp];            // 경치 곱
                _status[(int)statType.standard][(int)SkillKeyList.COIN] = BaseManager.userGameData.o_CoinFactor * BaseManager.userGameData.AddStats[(int)statusKeyList.coin];           // 코인 곱
                _status[(int)statType.standard][(int)SkillKeyList.SPEED] = BaseManager.userGameData.AddStats[(int)UserGameData.defaultStat.speed];           // (스킨+)디폴트 이속
            }
            else // 스킨 스탯이 계절적용일때
            {
                _status[(int)statType.standard][(int)SkillKeyList.ATT] = BaseManager.userGameData.o_Att;
                _status[(int)statType.standard][(int)SkillKeyList.DEF] = BaseManager.userGameData.o_Def;
                _status[(int)statType.standard][(int)SkillKeyList.HPGEN] = BaseManager.userGameData.o_Hpgen;
                _status[(int)statType.standard][(int)SkillKeyList.COOL] = (1f - BaseManager.userGameData.o_Cool);
                _status[(int)statType.standard][(int)SkillKeyList.EXP] = BaseManager.userGameData.o_ExpFactor;
                _status[(int)statType.standard][(int)SkillKeyList.COIN] = BaseManager.userGameData.o_CoinFactor;

                _status[(int)statType.season][(int)SkillKeyList.ATT] = BaseManager.userGameData.AddStats[(int)statusKeyList.att];
                _status[(int)statType.season][(int)SkillKeyList.DEF] = BaseManager.userGameData.AddStats[(int)statusKeyList.def];
                _status[(int)statType.season][(int)SkillKeyList.HPGEN] = BaseManager.userGameData.AddStats[(int)statusKeyList.hpgen];
                _status[(int)statType.season][(int)SkillKeyList.COOL] = (1f - BaseManager.userGameData.AddStats[(int)statusKeyList.cool]);
                _status[(int)statType.season][(int)SkillKeyList.EXP] = BaseManager.userGameData.AddStats[(int)statusKeyList.exp];
                _status[(int)statType.season][(int)SkillKeyList.COIN] = BaseManager.userGameData.AddStats[(int)statusKeyList.coin];
            }

            // 스킨
            Skin = BaseManager.userGameData.Skin;
            // 스킨 볼 타입
            _ballType = BaseManager.userGameData.BallType;

            // 스킨 bool 타입
            string str = D_skin.GetEntity(Skin.ToString()).f_typeB;
            _skinBval = new bool[(int)skinBvalue.max];
            for (skinBvalue i = 0; i < skinBvalue.max; i++)
            {
                _skinBval[(int)i] = (str.Equals(i.ToString())) ? true : false;
            }

            // 스킨 float 타입
            str = D_skin.GetEntity(Skin.ToString()).f_typeF;
            _skinFval = new float[(int)skinFvalue.max];
            for (skinFvalue i = 0; i < skinFvalue.max; i++)
            {
                _skinFval[(int)i] = (str.Equals(i.ToString())) ? (float)BaseManager.userGameData.SkinFval : 1f;
            }

            // 스킨 int 타입
            str = D_skin.GetEntity(Skin.ToString()).f_typeI;
            _skinIval = new int[(int)skinIvalue.max];
            for (skinIvalue i = 0; i < skinIvalue.max; i++)
            {
                _skinIval[(int)i] = (str.Equals(i.ToString())) ? (int)BaseManager.userGameData.SkinIval : 0;
            }

            // =========================================================================================================
            if (_skinBval[(int)skinBvalue.wild])
                _fluctuationHp += chkWild;
            // =========================================================================================================

            _abils = new Dictionary<SkillKeyList, ability>();
            _skills = new Dictionary<SkillKeyList, skill>();
            for (SkillKeyList i = SkillKeyList.HP; i < SkillKeyList.non; i++)
            {
                if (i < SkillKeyList.SnowBall)
                {
                    ability ab = new ability();
                    ab.Init(i);
                    ab.skUp = abilityApply;
                    _abils.Add(i, ab);
                }
                else
                {
                    skill sk = new skill();
                    sk.Init(i);
                    sk.OverChk = (skl) => { _skills[skl].overrideOff(); };
                    _skills.Add(i, sk);
                }
            }
        }

        /// <summary> 모험 스탯강화 적용 </summary>
        void abilityApply(SkillKeyList type)
        {
            if (type < SkillKeyList.SnowBall)
            {
                if (type == SkillKeyList.HP) // 체력일때
                {
                    Debug.Log(_abils[type].val0_increase);
                    _status[(int)statType.standard][(int)type] *= _abils[type].val0_increase;
                    Hp *= _abils[type].val0_increase;
                }
                else
                {
                    string str = D_skill.GetEntity(type.ToString()).f_val0_cal;
                    if (str.Equals("m"))
                    {
                        _status[(int)statType.skill][(int)type] *= _abils[type].val0;
                    }
                    else
                    {
                        _status[(int)statType.skill][(int)type] += _abils[type].val0;
                    }
                }
            }
            else
            {
                Debug.LogError("에바세바 얄리얄리얄랑성 : " + type.ToString());
            }
        }

        #region ----------------[utility]----------------

        /// <summary> 합-계산 </summary>
        float calAdd(SkillKeyList stt)
        {
            float cal = 0f;
            for (int i = 0; i < (int)statType.season; i++)
            {
                cal += _status[i][(int)stt];
            }

            if (applySeason)
                cal += _status[(int)statType.season][(int)stt];

            return cal;
        }

        /// <summary> 곱-계산 </summary>
        float calMul(SkillKeyList stt)
        {
            float cal = 1f;
            for (int i = 0; i < (int)statType.season; i++)
            {
                cal *= _status[i][(int)stt];
            }

            if (applySeason)
                cal *= _status[(int)statType.season][(int)stt];

            return cal;
        }

        bool applySeason
        {
            get => BaseManager.userGameData.ApplySeason == _gs.ClockMng.NowSeason;
        }

        #endregion

        public void setMove(Vector3 pos)
        {
            transform.position += pos * Speed * Time.deltaTime;

            if (Mathf.Abs(pos.x) > Mathf.Abs(pos.y))
            {
                SetAnimation("Side-walk", true, 1f);

                if (pos.x > 0)
                    _spine.transform.localScale = new Vector3(-0.13f, 0.13f);
                else if (pos.x < 0)
                    _spine.transform.localScale = Vector3.one * 0.13f;
                
            }
            else if (Mathf.Abs(pos.x) < Mathf.Abs(pos.y))
            {
                _spine.transform.localScale = Vector3.one * 0.13f;

                if (pos.y > 0)
                    SetAnimation("Back-walk", true, 1f);
                else if (pos.y < 0)
                    SetAnimation("Front-walk", true, 1f);
            }
        }

        #region [ skill ]

        /// <summary> 라이프 사이클 </summary>
        IEnumerator skillUpdate()
        {
            float delTime;
            Vector3 closedMob;
            float mobDist;
            float _dot;

            yield return new WaitUntil(() => _gs.StagePlay == true);

            while (_isDie == false && _gs.GameOver == false)
            {
                delTime = Time.deltaTime;
                closedMob = _gs.mostCloseEnemy(transform);
                mobDist = Vector3.Distance(transform.position, closedMob);

                _gs.InQuestTimeCheck(inQuest_goal_key.time, inQuest_goal_valtype.time, hpRate);

                launchSequence(closedMob, mobDist, delTime); // 런치 스킬

                rangeSequence(closedMob, mobDist, delTime); // 레인지 스킬
                                
                rushSequence(closedMob, mobDist, delTime); // 돌파 스킬               

                shieldSequence(closedMob, mobDist, delTime); // 실드 스킬

                fieldSequence(closedMob, mobDist, delTime); // 필드 스킬

                petSequence(closedMob, mobDist, delTime); // 펫 스킬

                skinSequence(closedMob, mobDist, delTime); // 스킨 스킬

                // getExp(0.14f);

                hpgen(delTime);

                deBuffChk(delTime);
                chkDash();

                _dot = _dotDmg.dotDmging(delTime);
                if (_dot > 0)
                {
                    _dot = MaxHp * _dot * 0.01f;
                    getDamaged(_dot);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        void chkDash()
        {
            _dashTime -= Time.deltaTime;
            if (_dashTime <= 0)
            {
                _dashSpeed = 1f;
            }
        }

        void setDash(float t)
        {
            _dashTime = t;
            _dashSpeed = 1.1f;
        }

        /// <summary> launch 시퀀스 </summary>
        void launchSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_skills[SkillKeyList.SnowBall].chk_shotable(delTime, Cool, mobDist)) // 눈덩이
            {
                int num = 1 + _skinIval[(int)skinIvalue.snowball];
                float angle = (num - 1) * -10f;

                for (int i = 0; i < num; i++)
                {
                    _shotBullet = _psm.getLaunch(SkillKeyList.SnowBall);
                    _shotBullet.transform.position = transform.position;
                    _shotBullet.setTarget(closedMob, angle + (20f * i));

                    _shotBullet.setSnowSprite(_ballType)
                        .repeatInit(SkillKeyList.SnowBall, _skills[SkillKeyList.SnowBall].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.SnowBall].size * Size)
                        .setDmgAction((val) =>
                        {
                            if (_skinBval[(int)skinBvalue.blood])
                            {
                                getHealed(val * _skinFval[(int)skinFvalue.blood]);
                            }
                        })
                        .play();
                }
            }

            if (_skills[SkillKeyList.Icicle].chk_shotable(delTime, Cool, mobDist)) // 얼창
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.Icicle);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.Icicle, _skills[SkillKeyList.Icicle].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.Icicle].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.FrostDrill].chk_shotable(delTime, Cool, mobDist)) // 얼드릴
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.FrostDrill);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.FrostDrill, _skills[SkillKeyList.FrostDrill].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.FrostDrill].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.IceFist].chk_shotable(delTime, Cool, mobDist)) // 얼주먹
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.IceFist);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.IceFist, _skills[SkillKeyList.IceFist].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.IceFist].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.IceKnuckle].chk_shotable(delTime, Cool, mobDist)) // 얼꿀밤 (얼주먹 데미지 + 얼꿀밤 퍼뎀)
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.IceKnuckle);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.IceKnuckle, _skills[SkillKeyList.IceKnuckle].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.IceKnuckle].size * Size)
                    .setPerDamage(_skills[SkillKeyList.IceKnuckle].val1)
                    .play();
            }

            if (_skills[SkillKeyList.HalfIcicle].chk_shotable(delTime, Cool, mobDist)) // 반달고드름
            {
                int num = _skills[SkillKeyList.HalfIcicle].count;
                float degree = 360f / num;
                Vector3 mce = closedMob;

                for (int i = 0; i < num; i++)
                {
                    _shotBullet = _psm.getLaunch(SkillKeyList.HalfIcicle);
                    _shotBullet.transform.position = transform.position;
                    _shotBullet.setTarget(mce, degree * i);
                    _shotBullet.repeatInit(SkillKeyList.HalfIcicle, _skills[SkillKeyList.HalfIcicle].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.HalfIcicle].size * Size)
                    .play();
                }
            }

            if (_skills[SkillKeyList.SnowDart].chk_shotable(delTime, Cool, mobDist)) // 얼음표창
            {
                int num = _skills[SkillKeyList.SnowDart].count;
                float degree = 360f / num;
                Vector3 mce = closedMob;

                for (int i = 0; i < num; i++)
                {
                    _shotBullet = _psm.getLaunch(SkillKeyList.SnowDart);
                    _shotBullet.transform.position = transform.position;
                    _shotBullet.setTarget(mce, degree * i);
                    _shotBullet.repeatInit(SkillKeyList.SnowDart, _skills[SkillKeyList.SnowDart].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.SnowDart].size * Size)
                        .setDmgAction((val) => { _skills[SkillKeyList.SnowDart]._timer += _skills[SkillKeyList.SnowDart].val1; })
                        .play();
                }
            }

            if (_skills[SkillKeyList.Hammer].chk_shotable(delTime, Cool, mobDist)) // 망치
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.Hammer);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);

                _shotBullet.repeatInit(SkillKeyList.Hammer, _skills[SkillKeyList.Hammer].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.Hammer].size * Size)
                    .setSpeed(1.2f)
                    .setHammer(_skills[SkillKeyList.Hammer].Lvl)
                    .play();
            }

            if (_skills[SkillKeyList.GigaDrill].chk_shotable(delTime, Cool, mobDist)) // 기가드릴
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.GigaDrill);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.GigaDrill, _skills[SkillKeyList.GigaDrill].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.GigaDrill].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.Ricoche].chk_shotable(delTime, Cool, mobDist)) // 도탄
            {
                float degree = 72f; // 360 / 5
                Vector3 mce = closedMob;

                for (int i = 0; i < 5; i++)
                {
                    _shotBullet = _psm.getLaunch(SkillKeyList.Ricoche);
                    _shotBullet.transform.position = transform.position;
                    _shotBullet.setTarget(mce, degree * i);
                    _shotBullet.repeatInit(SkillKeyList.Ricoche, _skills[SkillKeyList.Ricoche].val0 * Att * getAddDmg(skillType.launch), _skills[SkillKeyList.Ricoche].size * Size)
                        .setHammer(3)
                        .play();
                }
            }
        }

        /// <summary> range 시퀀스 </summary>
        void rangeSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_skills[SkillKeyList.IceBall].chk_shotable(delTime, Cool, mobDist)) // 얼덩이
            {
                _range = _psm.getCurved();
                _range.transform.position = transform.position;
                _range.setTarget(closedMob);

                _range.repeatInit(SkillKeyList.IceBall, _skills[SkillKeyList.IceBall].val0 * Att * getAddDmg(skillType.range))
                    .setSize(_skills[SkillKeyList.IceBall].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.Hail].chk_shotable(delTime, Cool, mobDist)) // 우박
            {
                StartCoroutine(fallingDown(SkillKeyList.Hail));
            }

            if (_skills[SkillKeyList.Meteor].chk_shotable(delTime, Cool, mobDist)) // 별똥별
            {
                StartCoroutine(fallingDown(SkillKeyList.Meteor));
            }

            if (_skills[SkillKeyList.SinkHole].chk_shotable(delTime, Cool, mobDist)) // 싱크홀
            {
                _range = _psm.getCurved();
                _range.transform.position = transform.position;
                _range.setTarget(closedMob);
                _range.repeatInit(SkillKeyList.SinkHole, _skills[SkillKeyList.SinkHole].val0 * Att * getAddDmg(skillType.range))
                    .setSize(_skills[SkillKeyList.SinkHole].size * Size)
                    .setKeep(_skills[SkillKeyList.SinkHole].keep)
                    .play();
            }

            if (_skills[SkillKeyList.Crevasse].chk_shotable(delTime, Cool, mobDist)) // 크레바스
            {
                _range = _psm.getCurved();
                _range.transform.position = transform.position;
                _range.setTarget(closedMob);
                _range.repeatInit(SkillKeyList.Crevasse, _skills[SkillKeyList.Crevasse].val0 * Att * getAddDmg(skillType.range))
                    .setSize(_skills[SkillKeyList.Crevasse].size * Size)
                    .setKeep(_skills[SkillKeyList.Crevasse].keep)
                    .play();
            }

            if (_skills[SkillKeyList.Circle].chk_shotable(delTime, Cool, mobDist)) // 서클
            {
                _stamp = _psm.getStamp();
                _stamp.transform.position = transform.position;

                _stamp.repeatInit(SkillKeyList.Circle, _skills[SkillKeyList.Circle].val0 * Att * getAddDmg(skillType.range))
                    .setSize(_skills[SkillKeyList.Circle].size * Size)
                    .setMidEff(() => SoundManager.instance.PlaySFX(SFX.magicend))
                    .play();
            }

            if (_skills[SkillKeyList.Poison].chk_shotable(delTime, Cool, mobDist)) // 독병
            {
                _range = _psm.getCurved();
                _range.transform.position = transform.position;
                _range.setTarget(closedMob);
                _range.repeatInit(SkillKeyList.Poison, 0)
                    .setKeep(_skills[SkillKeyList.Poison].keep)
                    .setSize(_skills[SkillKeyList.Poison].size * Size)
                    .setDotRate(_skills[SkillKeyList.Poison].val0)
                    .play();
            }

            if (_skills[SkillKeyList.Lightning].chk_shotable(delTime, Cool, mobDist)) // 번개
            {
                _stamp = _psm.getStamp();
                _stamp.transform.position = closedMob;

                _stamp.repeatInit(SkillKeyList.Lightning, _skills[SkillKeyList.Lightning].val0 * Att * getAddDmg(skillType.range))
                    .setSize(_skills[SkillKeyList.Lightning].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.Vespene].chk_shotable(delTime, Cool, mobDist)) // 베스핀
            {
                _stamp = _psm.getStamp();
                _stamp.transform.position = transform.position;

                _stamp.repeatInit(SkillKeyList.Crevasse, _skills[SkillKeyList.Crevasse].val0 * Att * getAddDmg(skillType.range), _skills[SkillKeyList.Vespene].val0 * Att * getAddDmg(skillType.range))
                    .setSize(_skills[SkillKeyList.Crevasse].size * Size)
                    .setKeep(_skills[SkillKeyList.Crevasse].keep)
                    .play();
                _stamp.setVespene();
            }

            if (_skills[SkillKeyList.Thuncall].chk_shotable(delTime, Cool, mobDist)) // 번개마법진
            {
                _stamp = _psm.getStamp();
                _stamp.transform.position = transform.position;

                _stamp.repeatInit(SkillKeyList.Thuncall, _skills[SkillKeyList.Thuncall].val0 * Att * getAddDmg(skillType.range))
                    .setSize(_skills[SkillKeyList.Thuncall].size * Size)
                    .play();
            }
        }

        /// <summary> rush 시퀀스 </summary>
        void rushSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            _psm.RushMng.setDirection(_gs.pVector);

            if (_skills[SkillKeyList.IceBat].chk_Time(delTime, Cool)) // 얼빠따
            {
                _psm.RushMng.setFixDir();
                _psm.RushMng.setBat(_skills[SkillKeyList.IceBat].val0 * Att * getAddDmg(skillType.rush));
            }

            if (_skills[SkillKeyList.SnowBullet].chk_Time(delTime, Cool)) // 아이스불렛
            {
                StartCoroutine(shotFrontFire(SkillKeyList.SnowBullet));
            }

            if (_skills[SkillKeyList.IceBalt].chk_Time(delTime, Cool)) // 아이스볼트
            {
                int num = _skills[SkillKeyList.IceBalt].count;
                float degree = 90f / num;

                for (int i = 0; i < num; i++)
                {
                    getBalt(SkillKeyList.IceBalt, transform.position + (Vector3)_gs.pVector, -45f + degree * i);
                }
            }

            if (_skills[SkillKeyList.Flurry].chk_Time(delTime, Cool)) // 돌풍
            {
                _psm.RushMng.setFlurry(SkillKeyList.Flurry, _skills[SkillKeyList.Flurry].val0 * Att * getAddDmg(skillType.rush));
            }

            if (_skills[SkillKeyList.ColdStorm].chk_Time(delTime, Cool)) // 얼폭
            {
                _psm.RushMng.setStorm(SkillKeyList.ColdStorm, _skills[SkillKeyList.ColdStorm].val0 * Att * getAddDmg(skillType.rush));
            }

            if (_skills[SkillKeyList.SnowPoint].chk_Time(delTime, Cool)) // 파우더
            {
                StartCoroutine(shotFrontFire(SkillKeyList.SnowPoint));
            }

            if (_skills[SkillKeyList.Recovery].chk_Time(delTime, Cool)) // 아이스볼트- 회수
            {
                int num = _skills[SkillKeyList.Recovery].count;
                float degree = 90f / num;

                for (int i = 0; i < num; i++)
                {
                    getBalt(SkillKeyList.Recovery, transform.position + (Vector3)_gs.pVector, -45f + degree * i);
                }
            }

            if (_skills[SkillKeyList.EyeOfFlurry].chk_Time(delTime, Cool)) // 돌풍의 눈
            {
                _psm.RushMng.setFlurry(SkillKeyList.EyeOfFlurry, _skills[SkillKeyList.EyeOfFlurry].val0 * Att * getAddDmg(skillType.rush));
            }

            if (_skills[SkillKeyList.RotateStorm].chk_Time(delTime, Cool)) // 얼폭
            {
                _psm.RushMng.setStorm(SkillKeyList.RotateStorm, _skills[SkillKeyList.RotateStorm].val0 * Att * getAddDmg(skillType.rush));
            }

            if (_skills[SkillKeyList.LockOn].chk_Time(delTime, Cool)) // 자동사격
            {
                _psm.RushMng.setStorm(SkillKeyList.LockOn, _skills[SkillKeyList.LockOn].val1 * Att * getAddDmg(skillType.rush));
            }
        }

        /// <summary> shield 시퀀스 </summary>
        void shieldSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_psm.ShieldMng.IsUseNormal == false) // 노말 실드 (일반, 거대, 가시)
            {
                if (_skills[SkillKeyList.Shield].chk_Time(delTime, Cool))                   // 일반 방패
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.Shield, _skills[SkillKeyList.Shield].val0 * Att * getAddDmg(skillType.shield))
                        .play(true);
                }

                if (_skills[SkillKeyList.HugeShield].chk_Time(delTime, Cool))               // 거대방패
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.HugeShield, _skills[SkillKeyList.HugeShield].val0 * Att * getAddDmg(skillType.shield))
                        .play(true);
                }

                if (_skills[SkillKeyList.GiantShield].chk_Time(delTime, Cool))              // 거대방패(강화)
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.GiantShield, _skills[SkillKeyList.GiantShield].val0 * Att * getAddDmg(skillType.shield))
                        .play(true);
                }

                if (_skills[SkillKeyList.ThornShield].chk_Time(delTime, Cool))              // 가시방패
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.ThornShield, _skills[SkillKeyList.ThornShield].val0 * Att * getAddDmg(skillType.shield))
                        .play(true);
                }

                if (_skills[SkillKeyList.ReflectShield].chk_Time(delTime, Cool))            // 반사방패
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.ReflectShield, _skills[SkillKeyList.ReflectShield].val0 * Att * getAddDmg(skillType.shield))
                        .play(true);
                }

                // 충전 방패
            }

            if (_psm.ShieldMng.IsUsedHigh == false) // 하이 실드 (무적, 은신)
            {
                if (_skills[SkillKeyList.Invincible].chk_Time(delTime, Cool)) // 무적
                {
                    _psm.ShieldMng.setHighShield(SkillKeyList.Invincible, _skills[SkillKeyList.Invincible].keep)
                        .play(false);
                }

                if (_skills[SkillKeyList.Absorb].chk_Time(delTime, Cool)) // 흡수
                {
                    _psm.ShieldMng.setHighShield(SkillKeyList.Absorb, _skills[SkillKeyList.Absorb].keep)
                        .play(false);
                }

                if (_skills[SkillKeyList.Hide].chk_Time(delTime, Cool)) // 은신
                {
                    _psm.ShieldMng.setHighShield(SkillKeyList.Hide, _skills[SkillKeyList.Hide].keep)
                        .play(false);
                }

                if (_skills[SkillKeyList.Chill].chk_Time(delTime, Cool)) // 한기
                {
                    _psm.ShieldMng.setHighShield(SkillKeyList.Chill, _skills[SkillKeyList.Chill].keep)
                        .play(false);
                }
            }
        }

        /// <summary> field 시퀀스 </summary>
        void fieldSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_skills[SkillKeyList.SnowStorm].chk_Time(delTime, Cool)) // 눈보라
            {
                _psm.FieldMng.getField(SkillKeyList.SnowStorm, _skills[SkillKeyList.SnowStorm].keep, _skills[SkillKeyList.SnowStorm].val0);

                if (_skinBval[(int)skinBvalue.frozen])
                {
                    getHealed(MaxHp * _skinFval[(int)skinFvalue.iceHeal]);
                }
            }

            if (_skills[SkillKeyList.SnowFog].chk_Time(delTime, Cool)) // 눈안개
            {
                _psm.FieldMng.getField(SkillKeyList.SnowFog, _skills[SkillKeyList.SnowFog].keep, _skills[SkillKeyList.SnowFog].val0);

                if (_skinBval[(int)skinBvalue.frozen])
                {
                    getHealed(MaxHp * _skinFval[(int)skinFvalue.iceHeal]);
                }
            }

            if (_skills[SkillKeyList.Aurora].chk_Time(delTime, Cool)) // 오로라
            {
                _psm.FieldMng.getField(SkillKeyList.Aurora, _skills[SkillKeyList.Aurora].keep, _skills[SkillKeyList.Aurora].val0 * getAddDmg(skillType.environment));
                Debug.Log(_skills[SkillKeyList.Aurora].delay);

                if (_skinBval[(int)skinBvalue.frozen])
                {
                    getHealed(MaxHp * _skinFval[(int)skinFvalue.iceHeal]);
                }
            }

            if (_skills[SkillKeyList.Blizzard].chk_Time(delTime, Cool)) // 블리자드
            {
                _psm.FieldMng.getField(SkillKeyList.Blizzard, _skills[SkillKeyList.Blizzard].keep, _skills[SkillKeyList.Blizzard].val1, _skills[SkillKeyList.Blizzard].val0 * Att * getAddDmg(skillType.environment));

                if (_skinBval[(int)skinBvalue.frozen])
                {
                    getHealed(MaxHp * _skinFval[(int)skinFvalue.iceHeal]);
                }
            }

            if (_skills[SkillKeyList.WhiteOut].chk_Time(delTime, Cool)) // 화이트아웃
            {
                _psm.FieldMng.getField(SkillKeyList.WhiteOut, _skills[SkillKeyList.WhiteOut].keep, _skills[SkillKeyList.WhiteOut].val1, _skills[SkillKeyList.WhiteOut].val0 * Att * getAddDmg(skillType.environment));

                if (_skinBval[(int)skinBvalue.frozen])
                {
                    getHealed(MaxHp * _skinFval[(int)skinFvalue.iceHeal]);
                }
            }

            if (_skills[SkillKeyList.SubStorm].chk_Time(delTime, Cool)) // 서브스톰
            {
                _psm.FieldMng.getField(SkillKeyList.SubStorm, _skills[SkillKeyList.SubStorm].keep, _skills[SkillKeyList.SubStorm].val0 * getAddDmg(skillType.environment));

                if (_skinBval[(int)skinBvalue.frozen])
                {
                    getHealed(MaxHp * _skinFval[(int)skinFvalue.iceHeal]);
                }
            }

            if (_skills[SkillKeyList.IceAge].chk_Time(delTime, Cool)) // 아이스에이지
            {
                _psm.FieldMng.getField(SkillKeyList.IceAge, _skills[SkillKeyList.IceAge].keep, _skills[SkillKeyList.IceAge].val0 * Att * getAddDmg(skillType.environment));

                if (_skinBval[(int)skinBvalue.frozen])
                {
                    getHealed(MaxHp * _skinFval[(int)skinFvalue.iceHeal]);
                }
            }
        }

        /// <summary> pet 시퀀스 </summary>
        void petSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_skills[SkillKeyList.Pet].chk_shotable(delTime, Cool, mobDist)) // 쫄따구
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.Pet);
                _shotBullet.transform.position = _psm.PetMng.Pos.position;

                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.Pet, _skills[SkillKeyList.Pet].val0 * Att * getAddDmg(skillType.summon), _skills[SkillKeyList.Pet].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.Pet2].chk_shotable(delTime, Cool, mobDist)) // 쫄따구2
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.Pet);
                _shotBullet.transform.position = _psm.PetMng.Pos.position;

                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.Pet2, _skills[SkillKeyList.Pet2].val0 * Att * getAddDmg(skillType.summon), _skills[SkillKeyList.Pet2].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.BPet].chk_shotable(delTime, Cool, mobDist)) // 쫄따구3
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.Pet);
                _shotBullet.transform.position = _psm.PetMng.Pos.position;

                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.BPet, _skills[SkillKeyList.BPet].val0 * Att * getAddDmg(skillType.summon), _skills[SkillKeyList.BPet].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.IceWall].chk_Time(delTime, Cool)) // 빙벽
            {
                StartCoroutine(getWall(SkillKeyList.IceWall));
            }

            if (_skills[SkillKeyList.IceBerg].chk_Time(delTime, Cool)) // 빙산
            {
                StartCoroutine(getWall(SkillKeyList.IceBerg));
            }

            if (_skills[SkillKeyList.Shard].chk_Time(delTime, Cool)) // 파편 빙벽
            {
                StartCoroutine(getWall(SkillKeyList.Shard));
            }

            if (_skills[SkillKeyList.Mine].chk_Time(delTime, Cool)) // 지뢰
            {
                for (int i = 0; i < 1 + _skills[SkillKeyList.Mine].Lvl + _skinIval[(int)skinIvalue.mine]; i++)
                {
                    _range = _psm.getCurved();
                    _range.transform.position = transform.position;
                    _range.setTarget(transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * _skills[SkillKeyList.Mine].range);

                    _range.repeatInit(SkillKeyList.Mine, _skills[SkillKeyList.Mine].val0 * Att * _skinFval[(int)skinFvalue.mine] * getAddDmg(skillType.summon))
                        .setSize(_skills[SkillKeyList.Mine].size * Size)
                        .setKeep(_skills[SkillKeyList.Mine].keep)
                        .play();
                }
            }
        }


        #region [ 시간 필요 ]

        IEnumerator fallingDown(SkillKeyList sk)
        {
            SFX fx = (sk == SkillKeyList.Hail) ? SFX.hail : SFX.meteor;
            for (int i = 0; i < _skills[sk].count; i++)
            {
                _stamp = _psm.getStamp();
                _stamp.transform.position = transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * _skills[sk].range;

                _stamp.repeatInit(sk, _skills[sk].val0 * Att * getAddDmg(skillType.range))
                    .setSize(_skills[sk].size * Size)
                    .setMidEff(()=> { SoundManager.instance.PlaySFX(fx); })
                    .play();

                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        IEnumerator shotFrontFire(SkillKeyList skilln)
        {
            int num = _skills[skilln].count;
            int num2 = (skilln == SkillKeyList.SnowBullet) ? 1 : 2;

            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    _shotBullet = _psm.getLaunch(SkillKeyList.SnowBullet);
                    _shotBullet.transform.position = _psm.RushMng.openPos[num2 - 1 + j].position;
                    _shotBullet.setTarget(_shotBullet.transform.position + (Vector3)_gs.pVector);

                    _shotBullet.repeatInit(skilln, _skills[skilln].val0 * Att * getAddDmg(skillType.rush), _skills[skilln].size * Size)
                        .setSpeed(2f)
                        .play();
                }

                yield return new WaitForSeconds(0.15f);
                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        public void getBalt(SkillKeyList skilln, Vector3 targetPos, float addAngle = 0)
        {
            _shotBullet = _psm.getLaunch(SkillKeyList.IceBalt);
            _shotBullet.transform.position = transform.position;
            _shotBullet.setTarget(targetPos, addAngle);

            _shotBullet.repeatInit(skilln, _skills[skilln].val0 * Att * getAddDmg(skillType.rush), _skills[skilln].size * Size)
                .setSpeed(1.2f)
                .play();
        }

        IEnumerator getWall(SkillKeyList skilln)
        {
            Vector3 vec = transform.position;
            for (int i = 0; i < _skills[skilln].count; i++)
            {
                _stamp = _psm.getStamp();
                Vector3 pos = _gs.pVector * -1f;
                _stamp.transform.position = vec + (pos * 1f * (i + 1));
                _stamp.repeatInit(skilln, _skills[skilln].val0 * Att * getAddDmg(skillType.summon), _skills[skilln].val1 * Att * getAddDmg(skillType.summon))
                    .play();

                yield return new WaitForSeconds(0.2f);
            }
        }

        #endregion

        #endregion

        #region [ 스킨 관련 함수 ] skin Sequence

        /// <summary> 산타 - 선물 </summary>
        void skinSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_skills[SkillKeyList.Present].chk_Time(delTime, Cool)) // 선물
            {
                float heal = MaxHp * _skinFval[(int)skinFvalue.present];

                for (int i = 0; i < _skinIval[(int)skinIvalue.present]; i++)
                {
                    _range = _psm.getCurved();
                    _range.transform.position = transform.position;
                    _range.setTarget(transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * _skills[SkillKeyList.Present].range);

                    _range.repeatInit(SkillKeyList.Present, heal)
                        .setSize(_skills[SkillKeyList.Present].size * Size)
                        .setKeep(_skills[SkillKeyList.Present].keep)
                        .play();
                }
            }
        }

        /// <summary> 야수스킨 효과 체크 </summary>
        void chkWild()
        {
            _status[(int)statType.etc][(int)SkillKeyList.ATT] = (MaxHp - Hp) / MaxHp * 100f * _skinFval[(int)skinFvalue.wild];
            _status[(int)statType.etc][(int)SkillKeyList.ATT] = (_status[(int)statType.etc][(int)SkillKeyList.ATT] > 0) ? 1f + _status[(int)statType.etc][(int)SkillKeyList.ATT] : 1f;
            Debug.Log("abcc :: "+ _status[(int)statType.etc][(int)SkillKeyList.ATT]);
        }

        /// <summary> 산타스킨 - 선물 상자 먹었을때 </summary>
        public void getAddStat(SkillKeyList key, float val)
        {
            if (key == SkillKeyList.ATT)
            {
                _status[(int)statType.etc][(int)key] *= val;
                _dmgFont.getText(transform, "공업", dmgTxtType.att, true);
            }
            else if (key == SkillKeyList.DEF)
            {
                _status[(int)statType.etc][(int)SkillKeyList.DEF] += val;
                _dmgFont.getText(transform, "방업", dmgTxtType.def, true);
            }
        }

        #endregion

        #region [ 플레이어 함수 ]

        /// <summary> 스킬얻기(스킬타입) </summary>
        public void getSkill(SkillKeyList num, NotiType noti = NotiType.non)
        {
            if (num < SkillKeyList.SnowBall)
            {
                _abils[num].skillUp();
            }
            else if (num < SkillKeyList.non)
            {
                _skills[num].skillUp();

                switch (num)
                {
                    case SkillKeyList.Shield:
                    case SkillKeyList.HugeShield:
                    case SkillKeyList.GiantShield:
                    case SkillKeyList.ThornShield:
                    case SkillKeyList.ReflectShield:
                        _psm.ShieldMng.NsubRate = _skills[num].val1;
                        _psm.ShieldMng.setNormalShield(num, _skills[num].val0 * Att * getAddDmg(skillType.shield))
                        .play(true);
                        break;
                    case SkillKeyList.ChargeShield:
                        _psm.ShieldMng
                            .setLightShield(num, _skills[SkillKeyList.ChargeShield].Lvl == 1)
                            .setLightning((_skills[SkillKeyList.ChargeShield].Lvl == 3) ? 3 : 2)
                            .lightningPlay();
                        break;
                    case SkillKeyList.Absorb:
                    case SkillKeyList.Chill:
                        _psm.ShieldMng.HsubRate = _skills[num].val0;
                        break;
                    case SkillKeyList.Pet:
                    case SkillKeyList.Pet2:
                    case SkillKeyList.BPet:
                        _psm.PetMng.setPet(num);
                        break;
                }
            }

            if (noti != NotiType.non)
            {
                notiData _data = new notiData(noti);
                _data._skill = num;
                _data._rewardKey = inQuest_reward_key.get_skill;
                _gs.InGameInterface.getNotiUI(_data);
            }
        }

        /// <summary> 스킬 여부 확인 </summary>
        public bool isHaveSkill(SkillKeyList skill)
        {
            if (skill < SkillKeyList.SnowBall)
            {
                return (_abils[skill].Lvl > 0);
            }

            return (_skills[skill].Lvl > 0);
        }

        /// <summary> 경험치얻기(경험치) </summary>
        public void getExp(float exp)
        {
            exp *= Exp * gameValues._expRate;
#if UNITY_EDITOR
            exp *= 1;
#endif
            _playerExp += exp;

            if (levelUpable && (_isUpgrading == false))
            {
                _isUpgrading = true;

                // 레벨업 
                StartCoroutine(levelUpCo());
            }

            _gs.InGameInterface.ExpRefresh(ExpRate);
        }

        /// <summary> 스킬획득 포기하고 경험치 전환 </summary>
        public void expFeedback()
        {
            _playerExp -= PlayerMaxExp;
            PlayerMaxExp *= 0.95f;
            _playerExp += PlayerMaxExp / 3f;
            _gs.InGameInterface.ExpRefresh(ExpRate);
        }

        /// <summary> 체력재생 /초 </summary>
        void hpgen(float time)
        {
            _hpgenTimer += time;
            if (_hpgenTimer > 1f)
            {
                _hpgenTimer = 0f;

                Hp += Hpgen * HealMount;
                if (Hp > MaxHp)
                {
                    Hp = MaxHp;
                }

                _hpbar.localScale = new Vector2(Hp / MaxHp, 1f);
            }
        }

        /// <summary> 힐받기(힐) </summary>
        public void getHealed(float heal)
        {
            heal = heal * HealMount;
            Hp += heal;

            if (Hp > MaxHp)
            {
                Hp = MaxHp;
            }
            _dmgFont.getText(transform, Convert.ToInt32(heal).ToString(), dmgTxtType.heal, true);

            _hpbar.localScale = new Vector2(Hp / MaxHp, 1f);
        }

        public void getTem(gainableTem tem)
        {
            notiData data;
            switch (tem)
            {
                case gainableTem.heal:
                    {
                        data = new notiData(NotiType.heal);
                        _gs.InGameInterface.getNotiUI(data);

                        getHealed(MaxHp * gameValues._healpackVal);
                    }
                    break;
                case gainableTem.exp:
                    {
                        data = new notiData(NotiType.exp);
                        _gs.InGameInterface.getNotiUI(data);

                        float val = PlayerMaxExp * 0.03f; // 3퍼
                        val = (val < 5) ? 5 : val;
                        getExp(val);
                    }
                    break;
                case gainableTem.gem:
                    {
                        data = new notiData(NotiType.gem);
                        _gs.InGameInterface.getNotiUI(data);

                        _gs.getGem();
                    }
                    break;
                case gainableTem.sward:
                    {
                        data = new notiData(NotiType.sward);
                        _gs.InGameInterface.getNotiUI(data);

                        setDeBuff(SkillKeyList.ATT, 60f, 2);
                        setDeBuff(SkillKeyList.DEF, 60f, 20);
                    }
                    break;
                case gainableTem.present:
                    {
                        _gs.confirm_skill(gameValues._baseSkill[UnityEngine.Random.Range(0, 6)], NotiType.present);
                    }
                    break;
            }

            SoundManager.instance.PlaySFX(SFX.getTem);
            _gs.setInQuestData(inQuest_goal_key.tem, tem, 1);

            // 인벤토리로
            if (Inventory.ContainsKey(tem) == false)
            {
                Inventory.Add(tem, 0);
            }
            Inventory[tem]++;
        }

        /// <summary> 데미지받기 (공격자/데미지/방무 여부) </summary>
        public void getDamaged(EnemyCtrl enemy, float dmg, bool ignoreDef = false)
        {
            if (_gs.Pause) return;                                  // 정지 시
            if (_isAlmighty) return;                                // 무적   

            if (_psm.ShieldMng.getDamage(ref dmg, enemy)) return;   // 실드처리 및 반사뎀 계산 // 데미지 완벽 방어시
            if (dmg == 0) return;

            if (Dodge > 0 && UnityEngine.Random.Range(0, 100) < Dodge)           // 회피 기동
            {
                _dmgFont.getText(transform, "빗나감", dmgTxtType.standard, true);
                return;
            }

            if (ignoreDef == false)     // 방어력 계산 
            {
                // 방어력 적용 한도 90%
                float r_Def = (Def > 90f) ? 90f : Def;
                dmg *= (100f - r_Def) * 0.01f;
            }

            damagedAni();                                                                               // 데미지 모션      
            _dmgFont.getText(transform, Convert.ToInt32(dmg).ToString(), dmgTxtType.standard, true);    // 데미지 폰트
            _efm.makeEff("playerhit", transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.5f));
            setDash(1f);

            _gs.setInQuestData(inQuest_goal_key.dmg, inQuest_goal_valtype.take, dmg);
#if UNITY_EDITOR
            return;
#endif
            Hp -= dmg;

            if (Hp <= 0) // 뒤짐
            {
                StartCoroutine(playerDie());
            }

            _hpbar.localScale = new Vector2(Hp / MaxHp, 1f);
        }

        /// <summary> 데미지받기(독뎀) (방어 무시 적용된 데미지) </summary>
        public void getDamaged(float dmg)
        {
            if (_gs.Pause) return;              // 정지
            if (_isAlmighty) return;            // 무적

            _psm.ShieldMng.getDamage(ref dmg);  // 실드 계산

            if (dmg == 0) return;               //

            damagedAni();                       // 데미지 모션            

            _dmgFont.getText(transform, Convert.ToInt32(dmg).ToString(), dmgTxtType.poison, true); // 데미지 폰트

            _gs.setInQuestData(inQuest_goal_key.dmg, inQuest_goal_valtype.take, dmg);

#if UNITY_EDITOR
            return;
#endif

            Hp -= dmg;

            if (Hp <= 0) // 뒤짐
            {
                StartCoroutine(playerDie());
            }

            _hpbar.localScale = new Vector2(Hp / MaxHp, 1f);
        }

        public void damagedAni()
        {
            if (_isDmgAction)
            {
                return;
            }

            StartCoroutine(damageAni());
        }

        IEnumerator damageAni()
        {
            _isDmgAction = true;
            _spine.skeleton.SetColor((_isAlmighty) ? _alC : _dmgC);

            yield return new WaitForSeconds(0.1f);

            _spine.skeleton.SetColor(Color.white);

            yield return new WaitForSeconds(0.1f);

            _spine.skeleton.SetColor((_isAlmighty) ? _alC : _dmgC);

            yield return new WaitForSeconds(0.1f);

            _spine.skeleton.SetColor(Color.white);
            _isDmgAction = false;
        }

        /// <summary> 넉백 </summary>
        public void getKnock(Vector3 endP, float power = 0.05f, float duration = 0.1f)
        {
            transform.DOJump(transform.position + endP, power, 1, duration);
        }

        /// <summary> 경험치로 인한 레벨업시 루틴 </summary>
        IEnumerator levelUpCo()
        {
            yield return new WaitUntil(() => _gs.Pause == false);

            while (levelUpable)
            {
                _gs.Uping = true;

                _gs.levelUp();

                yield return new WaitUntil(() => (_gs.Pause == false) && (_gs.Uping == false));
            }

            _isUpgrading = false;
        }
        public void whenLevUpExp()
        {
            if (_playerExp >= PlayerMaxExp)
            {
                _playerExp -= PlayerMaxExp;
                PlayerMaxExp = PlayerMaxExp * gameValues._expIncrease;
                _gs.InGameInterface.ExpRefresh(ExpRate);
            }
        }

        public void getQuestReward(SkillKeyList stat, float val)
        {
            if (stat < SkillKeyList.SnowBall)
            {
                _status[(int)statType.quest][(int)stat] *= val;
            }

            return;
        }

        /// <summary> 스킬 얻은후 무적셋팅 </summary>
        public void setAlmighty()
        {
            if (_isAlmighty == false)
            {
                StartCoroutine(almighty());
            }
            else
            {
                _almighTime = 0;
            }
        }

        // 스킬 얻을때 무적루틴
        IEnumerator almighty()
        {
            _isAlmighty = true;
            _almightCase.SetActive(true);
            _almighTime = 0;

            while (_almighTime < 1f)
            {
                _almighTime += Time.deltaTime;

                _albar.localScale = new Vector2(1 - _almighTime, 1f);
                yield return new WaitForEndOfFrame();
            }

            _isAlmighty = false;
            _almightCase.SetActive(false);
        }

        /// <summary> 플레이어 죽었을때 루틴 </summary>
        IEnumerator playerDie()
        {
            Hp = 0;
            _isDie = true;
            _spine.gameObject.SetActive(false); // 눈사람끄고
            // 이펙트~            
            whenPlayerDie(); // 다멈춤

            //=============================================================

            if (_skinBval[(int)skinBvalue.rebirth] || _adRebirthable) // 부활 가능시
            {
                yield return new WaitForSeconds(0.5f);

                if (_skinBval[(int)skinBvalue.rebirth]) // 스킨 부활 가능
                {
                    _skinBval[(int)skinBvalue.rebirth] = false;

                    yield return new WaitForSeconds(1.5f);

                    Hp = MaxHp * _skinFval[(int)skinFvalue.rebirth];
                    if (Hp > MaxHp)
                    {
                        MaxHp = Hp;
                    }
                }
                else if (_adRebirthable) // 광고 부활 가능
                {
                    _adRebirthable = false;

                    if (_gs.RecordTime < 120) // 1일미만 부활 불가능
                    {
                        _gameOver();
                        yield break;
                    }

                    yield return new WaitForSeconds(1f);

                    bool _chk = false;
                    if (BaseManager.userGameData.RemoveAd == false) // 광고제거 구매 여부
                    {
                        // 광고제거 미구매) 광고 부활) 창 오픈
                        _gs.openAdRebirthPanel(() =>
                        {
                            _chk = true;
                        }, _gameOver);

                        yield return new WaitUntil(() => _chk == true); // 광고 후 진행
                    }
                    _gs.RebirthQst++;

                    // 광고제거) 즉시 부활

                    if (BaseManager.userGameData.DayQuest[(int)Quest.day_revive] == 0)
                        BaseManager.userGameData.DayQuest[(int)Quest.day_revive] = 1;

                    Hp = MaxHp;
                }

                // 부활 완료 -- 부활 이펙트 대기 후 진행
                _efm.getRebirth(transform.position, () =>
                {
                    _spine.gameObject.SetActive(true);
                }, () =>
                {
                    _hpbar.localScale = new Vector2(Hp / MaxHp, 1f);
                    _gs.whenResume(true);
                    _isDie = false;
                    StartCoroutine(skillUpdate());
                });
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                _gameOver();
            }

            yield return null;
        }

        /// <summary> 플레이어가 '딱' 죽는순간 </summary>
        public void whenPlayerDie()
        {
            // 총알파괴
            _psm.onClear();

            // 버프삭제
            _status[(int)statType.buff] = new float[] { 1f, 1f, 0f, 0f, 1f, 1f, 1f, 1f, 1f, 0f, 1f, 1f };  // 방어, 체젠, 회피는 합연산
            // 스킬 쿨타임 초기화
            for (SkillKeyList eq = SkillKeyList.SnowBall; eq < SkillKeyList.Poison; eq++)
            {
                _skills[eq]._timer = 0;
            }

            _gs.whenPause();
        }

#endregion

#region [ 버프 관련 함수 ]

        /// <summary> 버프 세팅 </summary>
        /// <param name="bff"> 버프 종류 </param>
        /// <param name="term"> 버프 기간 </param>
        /// <param name="val"> 버프 값 </param>
        /// <param name="isterm"> 버프 타입 </param>
        public BuffEffect setDeBuff(SkillKeyList bff, float term, float val, BuffEffect.buffTermType isterm = BuffEffect.buffTermType.term)
        {
            // 감속+로봇스킨 이면 무시
            if ((bff == SkillKeyList.SPEED && Skin == SkinKeyList.robotman) && (val < 1f))
            {
                return null;
            }

            BuffEffect DBuff = new BuffEffect(bff, term, val, isterm);

            if (bff == SkillKeyList.DEF || bff == SkillKeyList.HPGEN)
            {
                _status[(int)statType.buff][(int)bff] += DBuff.Val;
            }
            else
            {
                _status[(int)statType.buff][(int)bff] *= DBuff.Val;
            }

            _buffList.Add(DBuff);

            return DBuff;
        }

        /// <summary> 버프 시간 체크 </summary>
        void deBuffChk(float delTime)
        {
            for (int i = 0; i < _buffList.Count; i++)
            {
                _buffList[i].Term -= delTime;

                if (_buffList[i].TermOver)
                {
                    SkillKeyList bff = _buffList[i].Bff;
                    _buffList[i].whenOver();

                    _buffList.RemoveAt(i);

                    reCalBuff(bff);
                    i--;
                }
            }
        }

        /// <summary> 삭제된 타입 버프 일괄계산 </summary>
        void reCalBuff(SkillKeyList bff)
        {
            _status[(int)statType.buff][(int)bff] = (bff == SkillKeyList.HPGEN) ? 0f : 1f;

            for (int i = 0; i < _buffList.Count; i++)
            {
                if (_buffList[i].Bff == bff)
                {
                    _status[(int)statType.buff][(int)bff] *= _buffList[i].Val;
                }
            }
        }
        
        /// <summary> 네임드 버프 습득 </summary>
        public void getNamedBuff(Boss _boss)
        {
            BuffEffect bEff = null;
            BuffEffect.buffNamed bfn = (BuffEffect.buffNamed)_boss;
            SkillKeyList ebf = SkillKeyList.ATT;

            switch (_boss)
            {
                case Boss.boss_butterfly:
                    bEff = setNamedDeBuff(ebf, 30f, 1.2f);
                    break;
                case Boss.boss_flower:
                    ebf = SkillKeyList.DEF;
                    bEff = setNamedDeBuff(ebf, 30f, 1.1f);
                    break;
                case Boss.boss_scarecrow:
                    ebf = SkillKeyList.HPGEN;
                    bEff = setNamedDeBuff(ebf, 30f, 1.3f);
                    break;
                case Boss.boss_owl:
                    ebf = SkillKeyList.COOL;
                    bEff = setNamedDeBuff(ebf, 30f, 0.85f);
                    break;
                case Boss.boss_bear:
                    ebf = SkillKeyList.COIN;
                    bEff = setNamedDeBuff(ebf, 30f, 1.25f);
                    break;
                default:
                    Debug.LogError("보스 : " + gameObject.name + "// _boss : " + _boss.ToString());
                    break;
            }

            bool exist = false;
            for (int i = 0; i < _buffList.Count; i++)
            {
                if (_buffList[i].Name == bfn)
                {
                    _buffList[i].Term += 30f;
                    exist = true;
                    break;
                }
            }

            if (exist == false)
            {
                _bffParticle.getBuffParticle(bfn);
                bEff.setNamed(bfn, () => { _bffParticle.Buffoff(bfn); });

                _status[(int)statType.buff][(int)ebf] *= bEff.Val;

                _buffList.Add(bEff);
            }

            BuffEffect setNamedDeBuff(SkillKeyList bff, float term, float val, BuffEffect.buffTermType isterm = BuffEffect.buffTermType.term)
            {
                if (bff == SkillKeyList.HPGEN)
                {
                    val = (Hpgen * (val - 1) > 5f) ? val : 5f;
                }

                BuffEffect DBuff = new BuffEffect(bff, term, val, isterm);

                return DBuff;
            }
        }

        /// <summary> 버프 수동 삭제 </summary>
        public void manualRemoveDeBuff(BuffEffect bff)
        {
            if (bff != null)
            {
                SkillKeyList ebff = bff.Bff;

                _buffList.Remove(bff);

                reCalBuff(ebff);
            }
        }

#endregion
        
#region [ 플레이어 외적으로 ]

        public void cameraShake()
        {
            if (BaseManager._innerData.OnShake)
            {
                _main.DOShakePosition(0.5f, 0.5f, 50);
            }
        }

        public void onPause(bool bl)
        {
            spinePause(bl);

            _psm.onPause(bl);
        }

#endregion
    }
}