using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.Rendering;
using DG.DemiLib;

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
        [SerializeField] BuffParticleManager _bffParticle;

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

        float _playerExp = 0;
        float _playerMaxExp = gameValues._startExp;
        bool _isUpgrading;
        public float ExpRate { get { return _playerExp / _playerMaxExp; } }
        public bool levelUpable { get { return (_playerExp > _playerMaxExp); } }

        float[] _standardStt;
        float[] _skillStt;
        float[] _buffStt;
        float[] _seasonStt;
        float[] _addedStt;

        /// <summary> 
        /// 현재 - 계절,버프등 일시적으로 증가하거나 감소하는 효과는 없음
        /// max체력은 확정 증가만 있음 - (계절,버프 적용시 추가작업 필요)
        /// </summary>
        public float MaxHp
        {
            get { return _standardStt[(int)snowStt.maxHp]; }
            set { _standardStt[(int)snowStt.maxHp] = value; }
        }
        float _hp;
        float Hp
        {
            get { return _hp; }
            set { _hp = value; chkWild(); }
        }

        public float Att
        {
            get
            {
                float calAtt = _standardStt[(int)snowStt.att] * _skillStt[(int)SkillKeyList.ATT] * _buffStt[(int)eBuff.att];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calAtt * _seasonStt[(int)snowStt.att] + _addedStt[(int)snowStt.att];
                }
                else if (_hasWild)
                {
                    return calAtt * _wildAtt + _addedStt[(int)snowStt.att];
                }

                return calAtt + _addedStt[(int)snowStt.att];
            }
        }

        public float Def
        {
            get
            {
                float calDef = _standardStt[(int)snowStt.def] * _skillStt[(int)SkillKeyList.DEF] * _buffStt[(int)eBuff.def];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calDef * _seasonStt[(int)snowStt.def] * _addedStt[(int)snowStt.def];
                }

                return calDef * _addedStt[(int)snowStt.def];
            }
        }

        float _genTime;
        float HpgenTimer = 0;
        float Hpgen
        {
            get
            {
                float calHpgen = _standardStt[(int)snowStt.hpgen] + _skillStt[(int)SkillKeyList.GEN] + _buffStt[(int)eBuff.hpgen];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calHpgen + _seasonStt[(int)snowStt.hpgen];
                }

                float getHp = (MaxHp * calHpgen) + BaseManager.userGameData.o_Hpgen;
                return getHp;
            }
        }


        float Cool
        {
            get
            {
                float calCool = _standardStt[(int)snowStt.cool] * _skillStt[(int)SkillKeyList.COOL] * _buffStt[(int)eBuff.cool];
                //Debug.Log(_standardStt[(int)snowStt.cool] + "*" + _skillStt[(int)snowStt.cool] + "*" + _buffStt[(int)eBuff.cool]);
                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calCool * _seasonStt[(int)snowStt.cool];
                }

                return calCool;
            }
        }


        float Exp
        {
            get
            {
                float calExp = _standardStt[(int)snowStt.exp] * _skillStt[(int)SkillKeyList.EXP] * _buffStt[(int)eBuff.exp];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calExp * _seasonStt[(int)snowStt.exp];
                }

                return calExp;
            }
        }

        float Size
        {
            get
            {
                float calSize = _standardStt[(int)snowStt.size] * _skillStt[(int)SkillKeyList.SIZE] * _buffStt[(int)eBuff.size];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calSize * _seasonStt[(int)snowStt.size];
                }

                return calSize;
            }
        }

        float HealMount
        {
            get
            {
                float calHealMount = _standardStt[(int)snowStt.heal] * _skillStt[(int)SkillKeyList.HEAL] * _buffStt[(int)eBuff.heal];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calHealMount * _seasonStt[(int)snowStt.heal];
                }

                return calHealMount;
            }
        }

        float Speed
        {
            get
            {
                float calSpeed = gameValues._defaultSpeed * _standardStt[(int)snowStt.speed] * _skillStt[(int)SkillKeyList.SPD] * _buffStt[(int)eBuff.speed];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calSpeed * _seasonStt[(int)snowStt.speed];
                }

                return calSpeed;
            }
        }

        public float Coin
        {
            get
            {
                float calCoin = _standardStt[(int)snowStt.coin] * _buffStt[(int)eBuff.coin];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calCoin * _seasonStt[(int)snowStt.coin];
                }

                return calCoin;
            }
        }

        List<BuffEffect> _buffList;
        dotDmg _dotDmg;
        public dotDmg DotDmg { get => _dotDmg; set => _dotDmg = value; }

        #endregion 

        #region ----------------[Skill Values]----------------

        Dictionary<SkillKeyList, ability> _abils;
        public Dictionary<SkillKeyList, ability> Abils { get => _abils; }
        Dictionary<SkillKeyList, skill> _skills;
        public Dictionary<SkillKeyList, skill> Skills { get => _skills; }
        //Dictionary<SkillKeyList, skill> _equips;
        //public Dictionary<SkillKeyList, skill> Equips { get => _equips; }
        //public SkillKeyList[] selectEquips { get; set; }
        //int _equipSlotCnt = 2;
        //public bool isGetSlot { get { return _equipSlotCnt == 3; } }

        #endregion

        #region ----------------[Skin Eff Values]----------------

        snowballType _ballType;
        bool _hasWild;
        float _wildMount;
        float _wildAtt;

        bool _rebirth_skin;
        bool _rebirth_bonus;
        bool _invSlow;
        bool _isHero;
        bool _hasPresent;
        float _presentHeal;

        bool _isInvinc;
        float _invincibleCool = 30f;
        float _chkInvincTime;

        bool _hasFrozen;
        bool _hasCritic;
        float _iceHealMount;
        float _bloodMount;
        float _criDmg;
        float _snowballDmg;
        public bool HasFrozen { get => _hasFrozen; }
        public bool HasCritic { get => _hasCritic; }
        public float BloodMount { get => _bloodMount; }
        public float CriDmg { get => _criDmg; }
        public float SnowballDmg { get => _snowballDmg; }
        public bool IsHero { get => _isHero; }
        public bool IsInvinc
        {
            get { return _isInvinc; }
            set
            {
                // _invincield.SetActive(value);
                _isInvinc = value;
            }
        }
        public bool RebirthBonus { get => _rebirth_bonus; set => _rebirth_bonus = value; }

        #endregion

        bool _isDie;
        bool _isAlmighty;
        float _almighTime;
        float _dustTime;

        shotCtrl _shotBullet;
        curvedShotCtrl _range;
        skillMarkCtrl _stamp;

        SsnowballCtrl _sbc;
        BaseProjControl _pjt;
        SsuddenObstacleCtrl _soc;
        SsuddenEnergeCtrl _sec;
        hailSkill _hail;

        bool _isDmgAction;
        Color _dmgC = new Color(1, 0.4f, 0.4f);
        Color _alC = new Color(0.7f, 1f, 1f);

        public Action _gameOver { private get; set; }

        public void Init(GameScene gs)
        {
            // 클래스 참조
            _gs = gs;
            _enm = _gs.EnemyMng;
            _psm = _gs.SkillMng;
            _dmgFont = _gs.DmgfntMng;
            _efm = _gs.EfMng;
            // _compass = _gs.Compass;

            // [게임] 체크 변수 초기화
            _isDie = false;
            _isAlmighty = false;
            _rebirth_bonus = true;

            // 능력치
            _abils = new Dictionary<SkillKeyList, ability>();
            _skills = new Dictionary<SkillKeyList, skill>();
            //_equips = new Dictionary<SkillKeyList, skill>();
            //selectEquips = new SkillKeyList[3] { SkillKeyList.max, SkillKeyList.max, SkillKeyList.max };

            getOriginStatus();

            for (SkillKeyList i = SkillKeyList.HP; i < SkillKeyList.max; i++)
            {
                if (i < SkillKeyList.Snowball)
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
                    sk.OverChk = (skl)=> { _skills[skl].overrideOff(); };
                    _skills.Add(i, sk);
                }
            }

            // _shield.FixedInit();

            _buffList = new List<BuffEffect>();
            _dotDmg = new dotDmg();

            // _pet.Init(_gs, () => { _skills[SkillKeyList.Snowball].att *= 2; });
                        
            getSkill(SkillKeyList.Snowball);            

#if UNITY_EDITOR

            // getNamedBuff(Boss.boss_scarecrow);
#endif
            if (BaseManager.userGameData.SkinBval[(int)skinBvalue.mine])
            {
                getSkill(SkillKeyList.Mine);
                // selectEquips[0] = SkillKeyList.mine;
            }
            if (_hasPresent)
            {
                getSkill(SkillKeyList.Present);
            }

            _almightCase.SetActive(false);

            StartCoroutine(skillUpdate());
            // StartCoroutine(chk());
        }

        public void getskillaasdfadsf()
        {
            // Debug.Log(_skills[SkillKeyList.Ricoche].att + " * " + Att + " = " +_skills[SkillKeyList.Ricoche].att * Att);

            //Debug.Log(_skills[SkillKeyList.Shard].val0 * Att);
            //getSkill(SkillKeyList.Shard);
            //Debug.Log(_skills[SkillKeyList.Shard].val0 * Att);

            //SkillKeyList prev = SkillKeyList.Pet;
            //SkillKeyList mid = SkillKeyList.Pet2;
            //SkillKeyList next = SkillKeyList.BPet;

            //if (Skills[mid].Lvl > 2)
            //{
            //    Debug.Log(_skills[next].val0 * Att);
            //    getSkill(next);
            //    Debug.Log(_skills[next].val0 * Att);
            //}
            //else if (Skills[prev].Lvl > 2)
            //{
            //    Debug.Log(_skills[mid].val0 * Att);
            //    getSkill(mid);
            //    Debug.Log(_skills[mid].val0 * Att);
            //}
            //else 
            //{
            //    Debug.Log(_skills[prev].val0 * Att);
            //    getSkill(prev);
            //    Debug.Log(_skills[prev].val0 * Att);
            //}
        }

        IEnumerator chk()
        {
            while (true)
            {
                Debug.Log("lvl : " + _gs._lvl + $" - 경치 ({_playerExp}/{_playerMaxExp}");
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary> 최초의 능력치 </summary>
        void getOriginStatus()
        {
            // hpgen[3]은 합연산이라 초기값 0
            _standardStt = new float[] { 1f, 1f, 1f, 0f, 1f, 1f, 1f, 1f, 1f, 1f }; // snowStt (10개)
            _addedStt = new float[] { 0f, 0f, 1f }; //, 0f, 0f, 0f, 0f, 0f, 0f, 0f }; // snowStt (10개)
            _skillStt = new float[] { 1f, 1f, 1f, 0f, 1f, 1f, 1f, 1f, 1f, 1f }; // SkillKeyList (10개)
            _seasonStt = new float[] { 1f, 1f, 1f, 0f, 1f, 1f, 1f, 1f, 1f, 1f }; // snowStt (10개)
            // hpgen[2]은 합연산이라 초기값 0
            _buffStt = new float[] { 1f, 1f, 1f, 0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // ebuff (11개)

            // _standardStt : (def, cool)은 감소형태로 (1 - value) 계산
            if (BaseManager.userGameData.ApplySeason == null)
            {
                Hp = _standardStt[(int)snowStt.maxHp] = BaseManager.userGameData.o_Hp * BaseManager.userGameData.AddStats[0];                 // 체력 곱
                _standardStt[(int)snowStt.att] = BaseManager.userGameData.o_Att * BaseManager.userGameData.AddStats[1];                // 공격 곱
                _standardStt[(int)snowStt.def] = (1f - BaseManager.userGameData.o_Def) * (1f - BaseManager.userGameData.AddStats[2]);  // 방어 (1 - 곱)
                _standardStt[(int)snowStt.hpgen] = BaseManager.userGameData.AddStats[3];              // 체젠 합
                _standardStt[(int)snowStt.cool] = (1f - BaseManager.userGameData.o_Cool) * (1f - BaseManager.userGameData.AddStats[4]); // 쿨탐 (1 - 곱)
                _standardStt[(int)snowStt.exp] = BaseManager.userGameData.o_ExpFactor * BaseManager.userGameData.AddStats[5];          // 경치 곱
            }
            else
            {
                Hp = _standardStt[(int)snowStt.maxHp] = BaseManager.userGameData.o_Hp;
                _standardStt[(int)snowStt.att] = BaseManager.userGameData.o_Att;
                _standardStt[(int)snowStt.hpgen] = 0f;
                _standardStt[(int)snowStt.exp] = BaseManager.userGameData.o_ExpFactor;
                _standardStt[(int)snowStt.def] = (1f - BaseManager.userGameData.o_Def);
                _standardStt[(int)snowStt.cool] = (1f - BaseManager.userGameData.o_Cool);

                _seasonStt[(int)snowStt.att] = BaseManager.userGameData.AddStats[1];
                _seasonStt[(int)snowStt.hpgen] = BaseManager.userGameData.AddStats[3];
                _seasonStt[(int)snowStt.exp] = BaseManager.userGameData.AddStats[5];
                _seasonStt[(int)snowStt.def] = (1f - BaseManager.userGameData.AddStats[2]);
                _seasonStt[(int)snowStt.cool] = (1f - BaseManager.userGameData.AddStats[4]);
                _seasonStt[(int)snowStt.coin] = BaseManager.userGameData.AddStats[(int)snowStt.coin];
            }

            _spine.skeleton.SetSkin(BaseManager.userGameData.Skin.ToString());
            
            _ballType = BaseManager.userGameData.BallType;

            _hasWild = BaseManager.userGameData.SkinBval[(int)skinBvalue.wild];
            _wildMount = BaseManager.userGameData.SkinFval[(int)skinFvalue.wild] * 0.01f;
            _invSlow = BaseManager.userGameData.SkinBval[(int)skinBvalue.invSlow];
            _isHero = BaseManager.userGameData.SkinBval[(int)skinBvalue.hero];
            _hasPresent = BaseManager.userGameData.SkinBval[(int)skinBvalue.present];
            _presentHeal = BaseManager.userGameData.SkinFval[(int)skinFvalue.present] * 0.01f;
            _wildAtt = 1f;
            _rebirth_skin = BaseManager.userGameData.SkinBval[(int)skinBvalue.rebirth];
            _hasFrozen = BaseManager.userGameData.SkinBval[(int)skinBvalue.frozen];
            _hasCritic = BaseManager.userGameData.SkinBval[(int)skinBvalue.critical];
            _bloodMount = BaseManager.userGameData.SkinFval[(int)skinFvalue.blood] * 0.01f;
            _criDmg = BaseManager.userGameData.SkinFval[(int)skinFvalue.criticDmg] * 0.01f;
            _snowballDmg = BaseManager.userGameData.SkinFval[(int)skinFvalue.snowball] * 0.01f;
            _iceHealMount = BaseManager.userGameData.SkinFval[(int)skinFvalue.iceHeal] * 0.01f;
        }

        /// <summary> 모험 스탯강화 적용 </summary>
        void abilityApply(SkillKeyList type)
        {
            if (type < SkillKeyList.Snowball)
            {
                switch (type)
                {
                    case SkillKeyList.HP: // 체력 곱
                        _standardStt[(int)snowStt.maxHp] *= (_abils[type].val0 * 0.01f);
                        Hp *= (_abils[type].val0 * 0.01f);
                        break;
                    case SkillKeyList.ATT:          // 공격 곱
                    case SkillKeyList.HEAL:         // 힐증가량 곱
                    case SkillKeyList.SIZE:         // 크기 곱
                    case SkillKeyList.SPD:          // 속도 곱
                    case SkillKeyList.EXP:          // 경치 곱
                    case SkillKeyList.COIN:         // 코인 곱
                    case SkillKeyList.COOL:         // 쿨탐 합
                        _skillStt[(int)type] *= _abils[type].val0;
                        break;
                    case SkillKeyList.DEF:          // 방어
                    case SkillKeyList.GEN:          // 체젠 합 (값/100)
                        _skillStt[(int)type] += _abils[type].val0;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.LogError("에바세바 얄리얄리얄랑성 : " + type.ToString());
            }
        }

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
            else
            {
                _spine.transform.localScale = Vector3.one * 0.13f;

                if (pos.y > 0)
                    SetAnimation("Back-walk", true, 1f);
                else if (pos.y < 0)
                    SetAnimation("Front-walk", true, 1f);
            }

            if (Vector3.Distance(Vector3.zero, pos) > 0.4f)
            {
                _dustTime += Time.deltaTime;
                if (_dustTime > 0.15f)
                {
                    _dustTime = 0f;
                    _efm.makeDust(transform.position);
                }
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

            while (_isDie == false && _gs.GameOver == false)
            {
                delTime = Time.deltaTime;
                closedMob = _gs.mostCloseEnemy(transform);
                mobDist = Vector3.Distance(transform.position, closedMob);

                launchSequence(closedMob, mobDist, delTime); // 런치 스킬

                rangeSequence(closedMob, mobDist, delTime); // 레인지 스킬
                                
                rushSequence(closedMob, mobDist, delTime); // 돌파 스킬               

                shieldSequence(closedMob, mobDist, delTime); // 실드 스킬

                fieldSequence(closedMob, mobDist, delTime); // 필드 스킬

                petSequence(closedMob, mobDist, delTime); // 펫 스킬

                //skinSequence(closedMob, mobDist, delTime); 스킨 스킬

                // _compass.comPassMove();

                getExp(0.04f);

                hpgen(delTime);

                deBuffChk(delTime);

                _dot = _dotDmg.dotDmging(delTime);
                if (_dot > 0)
                {
                    _dot = MaxHp * _dot * 0.01f;
                    getDamaged(_dot);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        /// <summary> launch 시퀀스 </summary>
        void launchSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_skills[SkillKeyList.Snowball].chk_shotable(delTime, Cool, mobDist)) // 눈덩이
            {
                int num = 1 + BaseManager.userGameData.SkinIval[(int)skinIvalue.snowball];
                float angle = (num - 1) * -10f;

                for (int i = 0; i < num; i++)
                {
                    _shotBullet = _psm.getLaunch(SkillKeyList.Snowball);
                    _shotBullet.transform.position = transform.position;
                    _shotBullet.setTarget(closedMob, angle + (20f * i));

                    _shotBullet.repeatInit(SkillKeyList.Snowball, _skills[SkillKeyList.Snowball].val0 * Att, _skills[SkillKeyList.Snowball].size * Size)
                        .setSnowSprite(_ballType)
                        .play();
                }
            }

            if (_skills[SkillKeyList.IcicleSpear].chk_shotable(delTime, Cool, mobDist)) // 얼창
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.IcicleSpear);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.IcicleSpear, _skills[SkillKeyList.IcicleSpear].val0 * Att, _skills[SkillKeyList.IcicleSpear].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.FrostDrill].chk_shotable(delTime, Cool, mobDist)) // 얼드릴
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.FrostDrill);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.FrostDrill, _skills[SkillKeyList.FrostDrill].val0 * Att, _skills[SkillKeyList.FrostDrill].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.IceFist].chk_shotable(delTime, Cool, mobDist)) // 얼주먹
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.IceFist);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.IceFist, _skills[SkillKeyList.IceFist].val0 * Att, _skills[SkillKeyList.IceFist].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.IceKnuckle].chk_shotable(delTime, Cool, mobDist)) // 얼꿀밤 (얼주먹 데미지 + 얼꿀밤 퍼뎀)
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.IceKnuckle);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.IceKnuckle, _skills[SkillKeyList.IceKnuckle].val0 * Att, _skills[SkillKeyList.IceKnuckle].size * Size)
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
                    _shotBullet.repeatInit(SkillKeyList.HalfIcicle, _skills[SkillKeyList.HalfIcicle].val0 * Att, _skills[SkillKeyList.HalfIcicle].size * Size)
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
                    _shotBullet.repeatInit(SkillKeyList.SnowDart, _skills[SkillKeyList.SnowDart].val0 * Att, _skills[SkillKeyList.SnowDart].size * Size)
                        .setDmgAction((val) => { _skills[SkillKeyList.SnowDart]._timer += 0.13f; })
                        .play();
                }
            }

            if (_skills[SkillKeyList.Hammer].chk_shotable(delTime, Cool, mobDist)) // 망치
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.Hammer);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);

                _shotBullet.repeatInit(SkillKeyList.Hammer, _skills[SkillKeyList.Hammer].val0 * Att, _skills[SkillKeyList.Hammer].size * Size)
                    .setSpeed(1.2f)
                    .setHammer(_skills[SkillKeyList.Hammer].Lvl)
                    .play();
            }

            if (_skills[SkillKeyList.GigaDrill].chk_shotable(delTime, Cool, mobDist)) // 기가드릴
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.GigaDrill);
                _shotBullet.transform.position = transform.position;
                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.GigaDrill, _skills[SkillKeyList.GigaDrill].val0 * Att, _skills[SkillKeyList.GigaDrill].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.Ricoche].chk_shotable(delTime, Cool, mobDist)) // 도탄
            {
                // int num = _skills[SkillKeyList.Ricoche].count; == 5
                float degree = 72f; // 360 / 5
                Vector3 mce = closedMob;

                for (int i = 0; i < 5; i++)
                {
                    _shotBullet = _psm.getLaunch(SkillKeyList.Ricoche);
                    _shotBullet.transform.position = transform.position;
                    _shotBullet.setTarget(mce, degree * i);
                    _shotBullet.repeatInit(SkillKeyList.Ricoche, _skills[SkillKeyList.Ricoche].val0 * Att, _skills[SkillKeyList.Ricoche].size * Size)
                        .setHammer(3)
                        .play();
                }
            }
        }

        /// <summary> range 시퀀스 </summary>
        void rangeSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_skills[SkillKeyList.Iceball].chk_shotable(delTime, Cool, mobDist)) // 얼덩이
            {
                _range = _psm.getCurved();
                _range.transform.position = transform.position;
                _range.setTarget(closedMob);

                _range.repeatInit(SkillKeyList.Iceball, _skills[SkillKeyList.Iceball].val0 * Att)
                    .setSize(_skills[SkillKeyList.Iceball].size * Size)
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

            if (_skills[SkillKeyList.SnowBomb].chk_shotable(delTime, Cool, mobDist)) // 눈폭탄
            {
                _range = _psm.getCurved();
                _range.transform.position = transform.position;
                _range.setTarget(closedMob);
                _range.repeatInit(SkillKeyList.SnowBomb, _skills[SkillKeyList.SnowBomb].val0 * Att)
                    .setSize(_skills[SkillKeyList.SnowBomb].size * Size)
                    .setKeep(_skills[SkillKeyList.SnowBomb].keep)
                    .play();
            }

            if (_skills[SkillKeyList.SnowMissile].chk_shotable(delTime, Cool, mobDist)) // 눈미사일
            {
                _range = _psm.getCurved();
                _range.transform.position = transform.position;
                _range.setTarget(closedMob);
                _range.repeatInit(SkillKeyList.SnowMissile, _skills[SkillKeyList.SnowMissile].val0 * Att)
                    .setSize(_skills[SkillKeyList.SnowMissile].size * Size)
                    .setKeep(_skills[SkillKeyList.SnowMissile].keep)
                    .play();
            }

            if (_skills[SkillKeyList.Circle].chk_shotable(delTime, Cool, mobDist)) // 서클
            {
                _stamp = _psm.getStamp();
                _stamp.transform.position = transform.position;

                _stamp.repeatInit(SkillKeyList.Circle, _skills[SkillKeyList.Circle].val0 * Att)
                    .setSize(_skills[SkillKeyList.Circle].size * Size)
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

                _stamp.repeatInit(SkillKeyList.Lightning, _skills[SkillKeyList.Lightning].val0 * Att)
                    .setSize(_skills[SkillKeyList.Lightning].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.PoisonBomb].chk_shotable(delTime, Cool, mobDist)) // 독미사일
            {
                _range = _psm.getCurved();
                _range.transform.position = transform.position;
                _range.setTarget(closedMob);
                _range.repeatInit(SkillKeyList.PoisonBomb, _skills[SkillKeyList.PoisonBomb].val0 * Att)
                    .setSize(_skills[SkillKeyList.PoisonBomb].size * Size)
                    .setKeep(_skills[SkillKeyList.PoisonBomb].keep)
                    .setDotRate(_skills[SkillKeyList.PoisonBomb].val1)
                    .play();
            }

            if (_skills[SkillKeyList.Thuncall].chk_shotable(delTime, Cool, mobDist)) // 번개마법진
            {
                _stamp = _psm.getStamp();
                _stamp.transform.position = transform.position;

                _stamp.repeatInit(SkillKeyList.Thuncall, _skills[SkillKeyList.Thuncall].val0 * Att)
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
                _psm.RushMng.setBat(_skills[SkillKeyList.IceBat].val0 * Att);
            }

            if (_skills[SkillKeyList.OpenRoader].chk_Time(delTime, Cool)) // Sherpa 셰르파
            {
                StartCoroutine(shotOpenRoader(SkillKeyList.OpenRoader));
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
                _psm.RushMng.setFlurry(SkillKeyList.Flurry, _skills[SkillKeyList.Flurry].val0 * Att);
            }

            if (_skills[SkillKeyList.ColdStorm].chk_Time(delTime, Cool)) // 얼폭
            {
                _psm.RushMng.setStorm(SkillKeyList.ColdStorm, _skills[SkillKeyList.ColdStorm].val0 * Att);
            }

            if (_skills[SkillKeyList.IcePowder].chk_Time(delTime, Cool)) // 파우더
            {
                StartCoroutine(shotOpenRoader(SkillKeyList.IcePowder));
            }

            if (_skills[SkillKeyList.Recovery].chk_Time(delTime, Cool)) // 아이스볼트- 회수
            {
                int num = _skills[SkillKeyList.IceBalt].count;
                float degree = 90f / num;

                for (int i = 0; i < num; i++)
                {
                    getBalt(SkillKeyList.Recovery, transform.position + (Vector3)_gs.pVector, -45f + degree * i);
                }
            }

            if (_skills[SkillKeyList.EyeOfFlurry].chk_Time(delTime, Cool)) // 돌풍의 눈
            {
                _psm.RushMng.setFlurry(SkillKeyList.EyeOfFlurry, _skills[SkillKeyList.EyeOfFlurry].val0 * Att);
            }

            if (_skills[SkillKeyList.RotateStorm].chk_Time(delTime, Cool)) // 얼폭
            {
                _psm.RushMng.setStorm(SkillKeyList.RotateStorm, _skills[SkillKeyList.RotateStorm].val0 * Att);
            }

            if (_skills[SkillKeyList.LockOn].chk_Time(delTime, Cool)) // 자동사격
            {
                _psm.RushMng.setStorm(SkillKeyList.LockOn, _skills[SkillKeyList.LockOn].val1 * Att);
            }
        }

        /// <summary> shield 시퀀스 </summary>
        void shieldSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_psm.ShieldMng.IsUseNormal == false) // 노말 실드 (일반, 거대, 가시)
            {
                if (_skills[SkillKeyList.Shield].chk_Time(delTime, Cool))                   // 일반 방패
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.Shield, _skills[SkillKeyList.Shield].val0 * Att)
                        .play(true);
                }

                if (_skills[SkillKeyList.HugeShield].chk_Time(delTime, Cool))               // 거대방패
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.HugeShield, _skills[SkillKeyList.HugeShield].val0 * Att)
                        .play(true);
                }

                if (_skills[SkillKeyList.GiantShield].chk_Time(delTime, Cool))              // 거대방패(강화)
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.GiantShield, _skills[SkillKeyList.HugeShield].val0 * Att)
                        .play(true);
                }

                if (_skills[SkillKeyList.ThornShield].chk_Time(delTime, Cool))              // 가시방패
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.ThornShield, _skills[SkillKeyList.ThornShield].val0 * Att)
                        .setSubRate(_skills[SkillKeyList.ThornShield].val1)
                        .play(true);
                }

                if (_skills[SkillKeyList.ReflectShield].chk_Time(delTime, Cool))            // 반사방패
                {
                    _psm.ShieldMng.setNormalShield(SkillKeyList.ReflectShield, _skills[SkillKeyList.ReflectShield].val0 * Att)
                        .setSubRate(_skills[SkillKeyList.ReflectShield].val1)
                        .play(true);
                }

                // 번개방패

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
                        .setSubRate(_skills[SkillKeyList.Absorb].val0)
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
                        .setSubRate(_skills[SkillKeyList.Chill].val0)
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

                if (_iceHealMount > 0)
                {
                    getHealed(MaxHp * _iceHealMount);
                }
            }

            if (_skills[SkillKeyList.SnowFog].chk_Time(delTime, Cool)) // 눈안개
            {
                _psm.FieldMng.getField(SkillKeyList.SnowFog, _skills[SkillKeyList.SnowFog].keep, _skills[SkillKeyList.SnowFog].val0);

                if (_iceHealMount > 0)
                {
                    getHealed(MaxHp * _iceHealMount);
                }
            }

            if (_skills[SkillKeyList.Aurora].chk_Time(delTime, Cool)) // 오로라
            {
                _psm.FieldMng.getField(SkillKeyList.Aurora, _skills[SkillKeyList.Aurora].keep, _skills[SkillKeyList.Aurora].val0);

                if (_iceHealMount > 0)
                {
                    getHealed(MaxHp * _iceHealMount);
                }
            }

            if (_skills[SkillKeyList.Blizzard].chk_Time(delTime, Cool)) // 블리자드
            {
                _psm.FieldMng.getField(SkillKeyList.Blizzard, _skills[SkillKeyList.Blizzard].keep, _skills[SkillKeyList.Blizzard].val1, _skills[SkillKeyList.Blizzard].val0 * Att);

                if (_iceHealMount > 0)
                {
                    getHealed(MaxHp * _iceHealMount);
                }
            }

            if (_skills[SkillKeyList.WhiteOut].chk_Time(delTime, Cool)) // 화이트아웃
            {
                _psm.FieldMng.getField(SkillKeyList.WhiteOut, _skills[SkillKeyList.WhiteOut].keep, _skills[SkillKeyList.WhiteOut].val1, _skills[SkillKeyList.WhiteOut].val0 * Att);

                if (_iceHealMount > 0)
                {
                    getHealed(MaxHp * _iceHealMount);
                }
            }

            if (_skills[SkillKeyList.SubStorm].chk_Time(delTime, Cool)) // 서브스톰
            {
                _psm.FieldMng.getField(SkillKeyList.SubStorm, _skills[SkillKeyList.SubStorm].keep, _skills[SkillKeyList.SubStorm].val0);

                if (_iceHealMount > 0)
                {
                    getHealed(MaxHp * _iceHealMount);
                }
            }

            if (_skills[SkillKeyList.IceAge].chk_Time(delTime, Cool)) // 아이스에이지
            {
                _psm.FieldMng.getField(SkillKeyList.IceAge, _skills[SkillKeyList.IceAge].keep, _skills[SkillKeyList.IceAge].val0 * Att);

                if (_iceHealMount > 0)
                {
                    getHealed(MaxHp * _iceHealMount);
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
                _shotBullet.repeatInit(SkillKeyList.Pet, _skills[SkillKeyList.Pet].val0 * Att, _skills[SkillKeyList.Pet].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.Pet2].chk_shotable(delTime, Cool, mobDist)) // 쫄따구2
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.Pet);
                _shotBullet.transform.position = _psm.PetMng.Pos.position;

                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.Pet2, _skills[SkillKeyList.Pet2].val0 * Att, _skills[SkillKeyList.Pet2].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.BPet].chk_shotable(delTime, Cool, mobDist)) // 쫄따구3
            {
                _shotBullet = _psm.getLaunch(SkillKeyList.Pet);
                _shotBullet.transform.position = _psm.PetMng.Pos.position;

                _shotBullet.setTarget(closedMob);
                _shotBullet.repeatInit(SkillKeyList.BPet, _skills[SkillKeyList.BPet].val0 * Att, _skills[SkillKeyList.BPet].size * Size)
                    .play();
            }

            if (_skills[SkillKeyList.IceWall].chk_Time(delTime, Cool)) // 빙벽
            {
                StartCoroutine(getWall(SkillKeyList.IceWall));
            }

            if (_skills[SkillKeyList.Iceberg].chk_Time(delTime, Cool)) // 빙산
            {
                StartCoroutine(getWall(SkillKeyList.Iceberg));
            }

            if (_skills[SkillKeyList.Shard].chk_Time(delTime, Cool)) // 파편 빙벽
            {
                StartCoroutine(getWall(SkillKeyList.Shard));
            }

            if (_skills[SkillKeyList.Mine].chk_Time(delTime, Cool)) // 지뢰
            {
                for (int i = 0; i < 1 + _skills[SkillKeyList.Mine].Lvl + BaseManager.userGameData.SkinIval[(int)skinIvalue.mine]; i++)
                {
                    _range = _psm.getCurved();
                    _range.transform.position = transform.position;
                    _range.setTarget(transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * _skills[SkillKeyList.Mine].range);

                    _range.repeatInit(SkillKeyList.Mine, _skills[SkillKeyList.Mine].val0 * Att)
                        .setSize(_skills[SkillKeyList.Mine].size * Size)
                        .setKeep(_skills[SkillKeyList.Mine].keep)
                        .play();
                }
            }
        }

        void skillSequence(Vector3 closedMob,float mobDist, float delTime)
        {
            //if (_skills[SkillKeyList.hail].chk_Time(delTime, Cool)) // 우박
            //{
            //    StartCoroutine(fallingHail());
            //}

            //if (_skills[SkillKeyList.thunder].chk_rangeshotable(delTime, Cool, mobDist)) // 번개
            //{
            //    _sec = (SsuddenEnergeCtrl)_psm.getSudden(SkillKeyList.thunder);

            //    _sec.transform.position = _gs.randomCloseEnemy(transform, _skills[SkillKeyList.thunder].range); // 근거리 아무적으로 교체
            //    _sec.Init(_skills[SkillKeyList.thunder]);
            //}

            //if (_skills[SkillKeyList.blackhole].chk_shotable(delTime, Cool, mobDist)) // 소용돌이
            //{
            //    _pjt = _psm.getPrej(SkillKeyList.blackhole);
            //    _pjt.transform.position = transform.position;
            //    _pjt.setTarget(closedMob);
            //    _pjt.repeatInit(_skills[SkillKeyList.blackhole].att * Att, _skills[SkillKeyList.blackhole].size * Size, 1f, _skills[SkillKeyList.blackhole].keep);
            //}
        }

        #region [ 시간 필요 ]
        IEnumerator fallingDown(SkillKeyList sk)
        {
            for (int i = 0; i < _skills[sk].count; i++)
            {
                _stamp = _psm.getStamp();
                _stamp.transform.position = transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * _skills[sk].range;

                _stamp.repeatInit(sk, _skills[sk].val0 * Att)
                    .setSize(_skills[sk].size * Size)
                    .play();

                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        IEnumerator shotOpenRoader(SkillKeyList skilln)
        {
            int num = _skills[skilln].count;

            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    _shotBullet = _psm.getLaunch(SkillKeyList.OpenRoader);
                    _shotBullet.transform.position = _psm.RushMng.openPos[j].position;
                    _shotBullet.setTarget(_shotBullet.transform.position + (Vector3)_gs.pVector);

                    _shotBullet.repeatInit(skilln, _skills[skilln].val0 * Att, _skills[skilln].size * Size)
                        .setSpeed(2f)
                        .play();
                }

                yield return new WaitForSeconds(0.2f);
                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        public void getBalt(SkillKeyList skilln, Vector3 targetPos, float addAngle = 0)
        {
            _shotBullet = _psm.getLaunch(SkillKeyList.IceBalt);
            _shotBullet.transform.position = transform.position;
            _shotBullet.setTarget(targetPos, addAngle);

            _shotBullet.repeatInit(skilln, _skills[skilln].val0 * Att, _skills[skilln].size * Size)
                .setSpeed(1.2f)
                .play();
        }

        IEnumerator getWall(SkillKeyList skilln)
        {
            Debug.Log($"개수 : {_skills[skilln].count}");
            for (int i = 0; i < _skills[skilln].count; i++)
            {
                _stamp = _psm.getStamp();
                Vector3 pos = _gs.pVector * -1.1f;
                _stamp.transform.position = transform.position + (pos * 1f * (i + 1));
                _stamp.repeatInit(skilln, _skills[skilln].val0 * Att)
                    .play();

                yield return new WaitForSeconds(0.2f);
            }
        }

        #endregion

        #endregion

        #region 전설 장비

        //void equipSequence(Vector3 closedMob, float mobDist, float delTime)
        //{
        //    if (_equips[SkillKeyList.poison].chk_shotable(delTime, Cool, mobDist)) // 독병
        //    {
        //        _pjt = _psm.getPrej(SkillKeyList.poison);
        //        _pjt.transform.position = transform.position;
        //        _pjt.setTarget(closedMob);
        //        _pjt.repeatInit(_equips[SkillKeyList.poison].att, _equips[SkillKeyList.poison].size * Size, 1f, _equips[SkillKeyList.poison].keep);
        //    }

        //    if (_equips[SkillKeyList.hammer].chk_shotable(delTime, Cool, mobDist)) // 망치
        //    {
        //        _pjt = _psm.getPrej(SkillKeyList.hammer);
        //        _pjt.transform.position = transform.position;
        //        _pjt.setTarget(closedMob);
        //        _pjt.skillLvl = _equips[SkillKeyList.hammer].Lvl;
        //        _pjt.repeatInit(_equips[SkillKeyList.hammer].att * Att, _equips[SkillKeyList.hammer].size * Size, 1.2f);
        //    }

        //    if (_equips[SkillKeyList.thunder].chk_rangeshotable(delTime, Cool, mobDist)) // 번개
        //    {
        //        _sec = (SsuddenEnergeCtrl)_psm.getSudden(SkillKeyList.thunder);

        //        _sec.transform.position = _gs.randomCloseEnemy(transform, _equips[SkillKeyList.thunder].range); // 근거리 아무적으로 교체
        //        _sec.Init(_equips[SkillKeyList.thunder]);
        //    }

        //    if (_equips[SkillKeyList.mine].chk_Time(delTime, Cool)) // 지뢰
        //    {
        //        for (int i = 0; i < 1 + _equips[SkillKeyList.mine].Lvl + BaseManager.userGameData.SkinIval[(int)skinIvalue.mine]; i++)
        //        {
        //            _pjt = _psm.getPrej(SkillKeyList.mine);
        //            _pjt.transform.position = transform.position;
        //            _pjt.setTarget(transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 1.5f);
        //            _pjt.repeatInit(_equips[SkillKeyList.mine].att * Att, _equips[SkillKeyList.mine].size * Size, 1f, _equips[SkillKeyList.mine].keep);
        //        }
        //    }

        //    if (_equips[SkillKeyList.blackhole].chk_shotable(delTime, Cool, mobDist)) // 소용돌이
        //    {
        //        _pjt = _psm.getPrej(SkillKeyList.blackhole);
        //        _pjt.transform.position = transform.position;
        //        _pjt.setTarget(closedMob);
        //        _pjt.repeatInit(_equips[SkillKeyList.blackhole].att * Att, _equips[SkillKeyList.blackhole].size * Size, 1f, _equips[SkillKeyList.blackhole].keep);
        //    }

        //    if (_equips[SkillKeyList.pet].chk_shotable(delTime, Cool, mobDist)) // 쫄따구
        //    {
        //        _pjt = _psm.getPrej(SkillKeyList.pet);
        //        _pjt.transform.position = _pet.Pos.position;

        //        _pjt.setTarget(closedMob);
        //        _pjt.repeatInit(_equips[SkillKeyList.pet].att * Att, _equips[SkillKeyList.pet].size);
        //    }
        //}

        #endregion

        #region

        void skinSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (BaseManager.userGameData.SkinBval[(int)skinBvalue.invincible])
            {
                _chkInvincTime += delTime;
                if (_chkInvincTime > _invincibleCool) // 무적
                {
                    _chkInvincTime = 0;
                    StartCoroutine(invincible());
                }
            }

            if (_skills[SkillKeyList.Present].chk_Time(delTime, Cool)) // 선물
            {
                Debug.Log("asdf");
                float heal = MaxHp * _presentHeal;

                for (int i = 0; i < BaseManager.userGameData.SkinIval[(int)skinIvalue.present]; i++)
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

        IEnumerator invincible()
        {
            IsInvinc = true;
            // 무적 그림 켜기
            float time = 0;
            while (time < BaseManager.userGameData.SkinFval[(int)skinFvalue.invincible])
            {
                time += Time.deltaTime;
                yield return new WaitUntil(() => _gs.Pause == false);
            }

            IsInvinc = false; 
            _chkInvincTime = 0;
        }

        /// <summary> 야수스킨 효과 체크 </summary>
        void chkWild()
        {
            if (_hasWild)
            {
                _wildAtt = (MaxHp - Hp) * MaxHp * 0.01f * _wildMount;
                _wildAtt = (_wildAtt > 0) ? 1f + _wildAtt : 1f;
            }
        }

        #endregion

        #region 

        /// <summary> 체력재생 /초 </summary>
        void hpgen(float time)
        {
            _genTime += time;
            if (_genTime > 1f)
            {
                _genTime = 0f;

                Hp += Hpgen * HealMount;
                if (Hp > MaxHp)
                {
                    Hp = MaxHp;
                }

                _hpbar.localScale = new Vector2(Hp / MaxHp, 1f);
            }
        }

        void deBuffChk(float delTime)
        {
            for (int i = 0; i < _buffList.Count; i++)
            {
                _buffList[i].Term -= delTime;

                if (_buffList[i].TermOver)
                {
                    eBuff bff = _buffList[i].Bff;
                    _buffList[i].whenOver();

                    _buffList.RemoveAt(i);

                    reCalBuff(bff);
                    i--;
                }
            }
        }

        /// <summary> 삭제된 타입 버프 일괄계산 </summary>
        void reCalBuff(eBuff bff)
        {
            // _buffStt = new float[] { 1f, 1f, 1f, 0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // ebuff (11개)
            _buffStt[(int)bff] = (bff == eBuff.hpgen) ? 0f : 1f;
            // _buffStt[(int)bff] = 1f;

            for (int i = 0; i < _buffList.Count; i++)
            {
                if (_buffList[i].Bff == bff)
                {
                    _buffStt[(int)bff] *= _buffList[i].Val;
                }
            }
        }

        #endregion

        /// <summary> 스킬얻음 </summary>
        public void getSkill(SkillKeyList num)
        {
            if (num < SkillKeyList.Snowball)
            {
                _abils[num].skillUp();
            }
            else if (num < SkillKeyList.max)
            {
                _skills[num].skillUp();

                switch (num)
                {
                    case SkillKeyList.LightningShield:
                    case SkillKeyList.ChargeShield:
                        _psm.ShieldMng.setLightShield(num, _skills[SkillKeyList.LightningShield].Lvl == 1)
                        .setLightning((_skills[num].Lvl == 3) ? 3 : 2)
                        .lightningPlay();
                        break;
                    case SkillKeyList.Pet:
                    case SkillKeyList.Pet2:
                    case SkillKeyList.BPet:
                        _psm.PetMng.setPet(num);
                        break;
                }
            }
        }

        public void getExp(float exp)
        {
            _playerExp += exp * Exp;

            if (levelUpable && (_isUpgrading == false))
            {
                _isUpgrading = true;
                // 레벨업 
                StartCoroutine(levelUpCo());
            }

            _gs.ExpRefresh(ExpRate);
        }

        IEnumerator levelUpCo()
        {
            yield return new WaitUntil(() => _gs.Pause == false);

            while (levelUpable) 
            {
                _gs.Uping = true;

                _playerExp -= _playerMaxExp;
                _playerMaxExp = _playerMaxExp * gameValues._expIncrease;

                _gs.levelUp();

                yield return new WaitUntil(() => (_gs.Pause == false) && (_gs.Uping == false) );
            }

            _isUpgrading = false;
        }

        public void getAddStat(float att, float def)
        {
            _addedStt[(int)snowStt.att] += att;
            _addedStt[(int)snowStt.def] -= def;

            if (att > 0)
            {
                _dmgFont.getText(transform, "공업", dmgTxtType.att, true);
            }
            if (def > 0)
            {
                _dmgFont.getText(transform, "방업", dmgTxtType.def, true);
            }
        }

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

        /// <summary> 몹 데미지 (공격자/데미지/방무 여부) </summary>
        public void getDamaged(EnemyCtrl enemy, float dmg, bool ignoreDef = false)
        {
            if (_gs.Pause) return;                                  // 정지 시
            if (_isAlmighty) return;                                // 무적   
            
            if (_psm.ShieldMng.getDamage(ref dmg, enemy)) return;   // 실드처리 및 반사뎀 계산 // 데미지 완벽 방어시
            
            if (dmg == 0) return;

            damagedAni();                                           // 데미지 모션         

            _dmgFont.getText(transform, Convert.ToInt32(dmg).ToString(), dmgTxtType.standard, true);    // 데미지 폰트

            if (ignoreDef == false)     // 방무 아닐때만 이펙트 (몹이 때릴땐 방무없음 (나중에 생길지도 ? ))
            {
                dmg *= (100 - Def) * 0.01f;
            }
            _efm.makeEff(effAni.playerhit, transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.5f));

            return;

            Hp -= dmg;

            if (Hp <= 0) // 뒤짐
            {
                StartCoroutine(playerDie());
            }

            _hpbar.localScale = new Vector2(Hp / MaxHp, 1f);
        }

        /// <summary> 자기 데미지(독뎀) (방어 무시 적용된 데미지) </summary>
        public void getDamaged(float dmg)
        {
            if (_gs.Pause) return;              // 정지
            if (_isAlmighty) return;            // 무적

            _psm.ShieldMng.getDamage(ref dmg);  // 실드 계산

            if (dmg == 0) return;               //

            damagedAni();                       // 데미지 모션            

            _dmgFont.getText(transform, Convert.ToInt32(dmg).ToString(), dmgTxtType.poison, true); // 데미지 폰트

            Hp -= dmg;

            if (Hp <= 0) // 뒤짐
            {
                StartCoroutine(playerDie());
            }

            _hpbar.localScale = new Vector2(Hp / MaxHp, 1f);
        }

        IEnumerator playerDie()
        {
            Hp = 0;
            _isDie = true;
            _spine.gameObject.SetActive(false); // 눈사람끄고
            // 이펙트~            
            _gs.preGameOver(); // 다멈춤

            //=============================================================

            if (_rebirth_skin || _rebirth_bonus)
            {
                yield return new WaitForSeconds(0.5f);

                if (_rebirth_skin) // 스킨 부활
                {
                    yield return new WaitForSeconds(1.5f);

                    _rebirth_skin = false;

                    Hp = MaxHp * BaseManager.userGameData.SkinFval[(int)skinFvalue.rebirth] * 0.01f;
                    if (Hp > MaxHp)
                    {
                        MaxHp = Hp;
                    }
                }
                else if (_rebirth_bonus) // 광고 부활 가능 여부
                {
                    _rebirth_bonus = false;

                    int val = Convert.ToInt32(BaseManager.userGameData.SeasonTimeRecord * 0.2f);
                    int limit = (val > 300f) ? 300 : (val < 120) ? 120 : val;
                    if (_gs.ClockMng.RecordTime < limit)
                    {
                        _gameOver();
                        yield break;
                    }

                    bool _chk = false;

                    yield return new WaitForSeconds(1f);

                    if (BaseManager.userGameData.RemoveAd == false) // 광고제거 구매 여부
                    {
                        // 광고제거 미구매) 광고 부활) 창 오픈
                        _gs.openAdRebirthPanel(() =>
                        {
                            _chk = true;
                        }, _gameOver);

                        yield return new WaitUntil(() => _chk == true); // 광고 후 진행
                    }
                    else // 광고제거) 즉시 부활
                    {
                        BaseManager.userGameData.AdRecord++;
                        if (BaseManager.userGameData.DayQuestAd == 0)
                            BaseManager.userGameData.DayQuestAd = 1;
                    }

                    Hp = MaxHp;        
                }

                // 부활 완료 -- 부활 이펙트 대기 후 진행
                _efm.getRebirth(transform.position, () =>
                {
                    _spine.gameObject.SetActive(true);
                },()=> 
                {
                    _hpbar.localScale = new Vector2(Hp / MaxHp, 1f);
                    setAlmighty(); // 무적 주고 같이 시작
                    _gs.whenResume();
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

        // 무적
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

        /// <summary> 버프 세팅 </summary>
        /// <param name="bff"> 버프 종류 </param>
        /// <param name="term"> 버프 기간 </param>
        /// <param name="val"> 버프 값 </param>
        /// <param name="isterm"> 버프 타입 </param>
        public BuffEffect setDeBuff(eBuff bff, float term, float val, BuffEffect.buffTermType isterm = BuffEffect.buffTermType.term)
        {
            // 감속 무시 && 감속
            if (bff == eBuff.speed && _invSlow && (val < 1f))
            {
                return null;
            }

            BuffEffect DBuff = new BuffEffect(bff, term, val, isterm);

            _buffStt[(int)bff] *= DBuff.Val;

            _buffList.Add(DBuff);

            return DBuff;
        }

        public BuffEffect setNamedDeBuff(eBuff bff, float term, float val, BuffEffect.buffTermType isterm = BuffEffect.buffTermType.term)
        {
            if (bff == eBuff.hpgen)
            {
                val = (Hpgen * (val - 1) > 5f) ? val : 5f;
            }

            BuffEffect DBuff = new BuffEffect(bff, term, val, isterm);

            return DBuff;
        }

        public void getNamedBuff(Boss _boss)
        {
            BuffEffect bEff = null;            
            BuffEffect.buffNamed bfn = (BuffEffect.buffNamed)_boss;
            eBuff ebf = eBuff.att;

            switch (_boss)
            {
                case Boss.boss_butterfly:
                    bEff = setNamedDeBuff(ebf, 30f, 1.2f);
                    break;
                case Boss.boss_flower:
                    ebf = eBuff.def;
                    bEff = setNamedDeBuff(ebf, 30f, 1.1f);
                    break;
                case Boss.boss_scarecrow:
                    ebf = eBuff.hpgen;
                    bEff = setNamedDeBuff(ebf, 30f, 1.3f);
                    break;
                case Boss.boss_owl:
                    ebf = eBuff.cool;
                    bEff = setNamedDeBuff(ebf, 30f, 0.85f);
                    break;
                case Boss.boss_bear:
                    ebf = eBuff.coin;
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

                _buffStt[(int)ebf] *= bEff.Val;

                _buffList.Add(bEff);
            }
        }

        /// <summary> 버프 수동 삭제 </summary>
        public void manualRemoveDeBuff(BuffEffect bff)
        {
            if (bff != null)
            {
                eBuff ebff = bff.Bff;

                _buffList.Remove(bff);

                reCalBuff(ebff);
            }
        }

        public void getKnock(Vector3 endP, float power = 0.05f, float duration = 0.1f)
        {
            transform.DOJump(transform.position + endP, power, 1, duration);
        }

        public void cameraShake()
        {
            _main.DOShakePosition(0.5f, 0.5f, 50);
        }

        public void onPause(bool bl)
        {
            spinePause(bl);

            _psm.onPause(bl);
        }

        public void whenPlayerDie()
        {
            // 총알파괴
            _psm.onClear();

            // 쿨타임 초기화
            for (SkillKeyList eq = SkillKeyList.Snowball; eq < SkillKeyList.Poison; eq++)
            {
                _skills[eq]._timer = 0;
            }

            // 스킨 스킬쿨 초기화
            _chkInvincTime = 0;

            IsInvinc = false;

            // 디버프 제거

            // 무적 제거

        }
    }
}