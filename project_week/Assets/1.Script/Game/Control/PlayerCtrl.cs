using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.Rendering;
using ES3Types;
using DG.DemiLib;

namespace week
{
    public class PlayerCtrl : spineCtrl, IPause
    {
        #region ----------------[GameObject Values]----------------

        [SerializeField] Rigidbody2D _rigid;
        
        [Header("Skill")]
        [SerializeField] tornado _tornado = null;
        [SerializeField] shield _shield = null;
        [SerializeField] SpetCtrl _pet = null;
        [SerializeField] Animator _iceage = null;
        [SerializeField] GameObject _invincield = null;
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

        #endregion        

        #region ----------------[Stat]----------------

        float _playerExp = 0;
        float _playerMaxExp = 20f * 3f;
        public float ExpRate { get { return _playerExp / _playerMaxExp; } }
        

        float[] _standardStt;
        float[] _skillStt;
        float[] _buffStt;
        float[] _seasonStt;

        float _hp;
        float MaxHp 
        { 
            get
            { 
                return _standardStt[(int)snowStt.maxHp]; 
            }
            set 
            {
                _standardStt[(int)snowStt.maxHp] = value;
            }
        }
        float Att 
        {
            get
            {
                float calAtt = _standardStt[(int)snowStt.att] * _skillStt[(int)snowStt.att] * _buffStt[(int)eBuff.att];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calAtt * _seasonStt[(int)snowStt.att];
                }
                else if (_hasWild)
                {
                    return calAtt * _wildAtt;
                }

                return calAtt;
            }
        }

        float Def
        {
            get
            {
                float calDef = _standardStt[(int)snowStt.def] * _skillStt[(int)snowStt.def] * _buffStt[(int)eBuff.def];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calDef * _seasonStt[(int)snowStt.def];
                }

                return calDef;
            }
        }

        float _genTime;
        float _hpgenTimer = 0;
        float Hpgen
        {
            get
            {
                float calHpgen = _standardStt[(int)snowStt.hpgen] * _skillStt[(int)snowStt.hpgen] * _buffStt[(int)eBuff.hpgen];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calHpgen * _seasonStt[(int)snowStt.hpgen];
                }

                return calHpgen;
            }
        }

        
        float Cool
        {
            get
            {
                float calCool = _standardStt[(int)snowStt.cool] * _skillStt[(int)snowStt.cool] * _buffStt[(int)eBuff.cool];

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
                float calExp = _standardStt[(int)snowStt.exp] * _skillStt[(int)snowStt.exp] * _buffStt[(int)eBuff.exp];

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
                float calSize = _standardStt[(int)snowStt.size] * _skillStt[(int)snowStt.size] * _buffStt[(int)eBuff.size];

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
                float calHealMount = _standardStt[(int)snowStt.heal] * _skillStt[(int)snowStt.heal] * _buffStt[(int)eBuff.heal];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calHealMount * _seasonStt[(int)snowStt.heal];
                }

                return calHealMount;
            }
        }

        float _speed = gameValues._defaultSpeed * 0.8f;
        float Speed 
        {
            get
            {
                float calSpeed = gameValues._defaultSpeed * _standardStt[(int)snowStt.speed] * _skillStt[(int)snowStt.speed] * _buffStt[(int)eBuff.speed];

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
                float calCoin = 1f * _buffStt[(int)eBuff.coin];
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
        Dictionary<SkillKeyList, skill> _equips;
        public Dictionary<SkillKeyList, skill> Equips { get => _equips; }
        public SkillKeyList[] selectEquips { get; set; }
        int _equipSlotCnt = 2;
        public bool isGetSlot { get { return _equipSlotCnt == 3; } }

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
                _invincield.SetActive(value);
                _isInvinc = value; 
            }
        }
        public bool RebirthBonus { get => _rebirth_bonus; set => _rebirth_bonus = value; }

        #endregion

        bool _isDie;
        bool _isAlmighty;
        float _dustTime;
        SsnowballCtrl _sbc;
        BaseProjControl _pjt;
        SsuddenObstacleCtrl _soc;
        SsuddenEnergeCtrl _sec;
        hailSkill _hail;

        bool _isDmgAction;
        Color _dmgC = new Color(1, 0.4f, 0.4f);
        Color _alC = new Color(0.7f, 1f, 1f);

        public Action _gameOver { private get; set; }
        public Action<float> EnemyDamage { private get; set; }
        public Action<bool> Blizzard { private get; set; }
        public Action<float> EnemyFrozen { private get; set; }

        public void Init(GameScene gs)
        {
            _gs = gs;
            _enm = _gs.EnemyMng;
            _psm = _gs.SkillMng;
            _dmgFont = _gs.DmgfntMng;
            _efm = _gs.EfMng;
            _compass = _gs.Compass;

            _isDie = false;
            _isAlmighty = false;
            _rebirth_bonus = true;

            _abils = new Dictionary<SkillKeyList, ability>();
            _skills = new Dictionary<SkillKeyList, skill>();
            _equips = new Dictionary<SkillKeyList, skill>();
            selectEquips = new SkillKeyList[3] { SkillKeyList.max, SkillKeyList.max, SkillKeyList.max };

            getOriginStatus();

            for (SkillKeyList i = SkillKeyList.hp; i < SkillKeyList.max; i++)
            {
                if (i < SkillKeyList.snowball)
                {
                    ability ab = new ability();
                    ab.Init((int)i);
                    ab.skUp = abilityApply;
                    _abils.Add(i, ab);
                }
                else if(i < SkillKeyList.poison)
                {
                    skill sk = new skill();
                    sk.Init((int)i);
                    _skills.Add(i, sk);
                }
                else
                {
                    skill eq = new skill();
                    eq.Init((int)i);
                    _equips.Add(i, eq);
                }
            }

            _shield.FixedInit(_gs);

            _buffList = new List<BuffEffect>();
            _dotDmg = new dotDmg();

            _pet.Init(_gs, () => { _skills[SkillKeyList.snowball].att *= 2; });
            
            getSkill(SkillKeyList.snowball);

            if (BaseManager.userGameData.SkinBval[(int)skinBvalue.mine])
            {
                getSkill(SkillKeyList.mine);
                selectEquips[0] = SkillKeyList.mine;
            }

            _almightCase.SetActive(false);

            StartCoroutine(skillUpdate());
            StartCoroutine(chk());
        }

        IEnumerator chk()
        {
            while (true)
            {
                Debug.Log(Att + "/" + Def + "/" + Hpgen + "/" + Cool + "/" + Coin);
                yield return new WaitForSeconds(1f);
            }
        }

        void getOriginStatus()
        {
            int len = (int)snowStt.max;

            _standardStt    = new float[len];
            _skillStt       = new float[len];
            _seasonStt      = new float[len];                        
            for (int i = 0; i < len; i++)
            {
                _standardStt[i] = 1f;
                _skillStt[i] = 1f;
                _seasonStt[i] = 1f;
            }

            len = (int)eBuff.max;
            _buffStt = new float[len];
            for (int i = 0; i < len; i++)
            {
                _buffStt[i] = 1f;
            }

            if (BaseManager.userGameData.ApplySeason == null)
            {
                _hp = _standardStt[(int)snowStt.maxHp] = BaseManager.userGameData.o_Hp * BaseManager.userGameData.AddStats[0]   * 100;
                _standardStt[(int)snowStt.att] = BaseManager.userGameData.o_Att * BaseManager.userGameData.AddStats[1]          * 100;
                _standardStt[(int)snowStt.def] = BaseManager.userGameData.o_Def * BaseManager.userGameData.AddStats[2]          + 50;
                _standardStt[(int)snowStt.hpgen] = BaseManager.userGameData.o_Hpgen * BaseManager.userGameData.AddStats[3]      + 50;
                _standardStt[(int)snowStt.cool] = BaseManager.userGameData.o_Cool * BaseManager.userGameData.AddStats[4];
                _standardStt[(int)snowStt.exp] = BaseManager.userGameData.o_ExpFactor * BaseManager.userGameData.AddStats[5];
            }
            else
            {
                _hp = _standardStt[(int)snowStt.maxHp] = BaseManager.userGameData.o_Hp;
                _standardStt[(int)snowStt.att]      = BaseManager.userGameData.o_Att;
                _standardStt[(int)snowStt.def]      = BaseManager.userGameData.o_Def;
                _standardStt[(int)snowStt.hpgen]    = BaseManager.userGameData.o_Hpgen;
                _standardStt[(int)snowStt.cool]     = BaseManager.userGameData.o_Cool;
                _standardStt[(int)snowStt.exp]      = BaseManager.userGameData.o_ExpFactor;

                _seasonStt[(int)snowStt.att]      = BaseManager.userGameData.AddStats[1];
                _seasonStt[(int)snowStt.def]      = BaseManager.userGameData.AddStats[2];
                _seasonStt[(int)snowStt.hpgen]    = BaseManager.userGameData.AddStats[3];
                _seasonStt[(int)snowStt.cool]     = BaseManager.userGameData.AddStats[4];
                _seasonStt[(int)snowStt.exp]      = BaseManager.userGameData.AddStats[5];
            }

            _spine.skeleton.SetSkin(BaseManager.userGameData.Skin.ToString());

            _ballType = BaseManager.userGameData.BallType;

            _hasWild = BaseManager.userGameData.SkinBval[(int)skinBvalue.wild];
            _wildMount = BaseManager.userGameData.SkinFval[(int)skinFvalue.wild] * 0.01f;
            _invSlow = BaseManager.userGameData.SkinBval[(int)skinBvalue.invSlow];
            _isHero = BaseManager.userGameData.SkinBval[(int)skinBvalue.hero];
            _wildAtt = 1f;
            _rebirth_skin = BaseManager.userGameData.SkinBval[(int)skinBvalue.rebirth];
            _hasFrozen = BaseManager.userGameData.SkinBval[(int)skinBvalue.frozen];
            _hasCritic = BaseManager.userGameData.SkinBval[(int)skinBvalue.critical];
            _bloodMount = BaseManager.userGameData.SkinFval[(int)skinFvalue.blood] * 0.01f;
            _criDmg = BaseManager.userGameData.SkinFval[(int)skinFvalue.criticDmg] * 0.01f;
            _snowballDmg = BaseManager.userGameData.SkinFval[(int)skinFvalue.snowball] * 0.01f;
            _iceHealMount = BaseManager.userGameData.SkinFval[(int)skinFvalue.iceHeal] * 0.01f;
        }

        void abilityApply(SkillKeyList type)
        {
            if (type < SkillKeyList.snowball)
            {
                if (type == SkillKeyList.hp)
                {
                    _standardStt[(int)snowStt.maxHp] *= _abils[type].val;
                }
                else
                {
                    _skillStt[(int)type] *= _abils[type].val;
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

                skillSequence(closedMob, mobDist, delTime);

                equipSequence(closedMob, mobDist, delTime);

                skinSequence(closedMob, mobDist, delTime);

                _compass.comPassMove();

                hpgen(delTime);

                deBuffChk(delTime);

                _dot = _dotDmg.dotDmging(delTime);
                if (_dot > 0)
                {
                    _dot = MaxHp * _dot * 0.01f;
                    getDamaged(_dot, true);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        void skillSequence(Vector3 closedMob,float mobDist, float delTime)
        {
            if (_skills[SkillKeyList.snowball].chk_shotable(delTime, Cool, mobDist)) // 눈덩이
            {
                int num = 1 + BaseManager.userGameData.SkinIval[(int)skinIvalue.snowball];
                float angle = (num - 1) * -10f;

                for (int i = 0; i < num; i++)
                {
                    _sbc = (SsnowballCtrl)_psm.getPrej(SkillKeyList.snowball);
                    _sbc.transform.position = transform.position;
                    _sbc.setTarget(closedMob, angle + (20f * i));
                    _sbc.setSprite(_ballType);
                    _sbc.repeatInit(_skills[SkillKeyList.snowball].att * Att, _skills[SkillKeyList.snowball].size);
                }
            }

            if (_skills[SkillKeyList.icefist].chk_shotable(delTime, Cool, mobDist)) // 얼음주먹
            {
                _pjt = _psm.getPrej(SkillKeyList.icefist);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_skills[SkillKeyList.icefist].att * Att, _skills[SkillKeyList.icefist].size);
            }

            if (_skills[SkillKeyList.icicle].chk_shotable(delTime, Cool, mobDist)) // 고드름
            {
                _pjt = _psm.getPrej(SkillKeyList.icicle);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_skills[SkillKeyList.icicle].att * Att, _skills[SkillKeyList.icicle].size);
            }

            if (_skills[SkillKeyList.halficicle].chk_shotable(delTime, Cool, mobDist)) // 반달고드름
            {
                int num = _skills[SkillKeyList.halficicle].Lvl;
                float degree = 360f / num;
                Vector3 mce = closedMob;

                for (int i = 0; i < num; i++)
                {
                    _pjt = _psm.getPrej(SkillKeyList.halficicle);
                    _pjt.transform.position = transform.position;
                    _pjt.setTarget(mce, degree * i);
                    _pjt.repeatInit(_skills[SkillKeyList.halficicle].att * Att, _skills[SkillKeyList.halficicle].size);
                }
            }

            if (_skills[SkillKeyList.hail].chk_Time(delTime, Cool)) // 우박
            {
                StartCoroutine(fallingHail());
            }

            if (_skills[SkillKeyList.icewall].chk_Time(delTime, Cool)) // 빙벽
            {
                _soc = (SsuddenObstacleCtrl)_psm.getSudden(SkillKeyList.icewall);
                Vector3 pos = _gs.pVector * -1;
                _soc.transform.position = transform.position + (pos * 1.2f);
                _soc.Init(_skills[SkillKeyList.icicle]);
            }

            if (_skills[SkillKeyList.iceage].chk_Time(delTime, Cool)) // 아이스에이지
            {
                StartCoroutine(iceage());
                if (_iceHealMount > 0)
                {
                    getHealed(MaxHp * _iceHealMount);
                }
            }

            if (_skills[SkillKeyList.blizzard].chk_Time(delTime, Cool)) // 블리자드
            {
                StartCoroutine(blizzarding());
                if (_iceHealMount > 0)
                {
                    getHealed(MaxHp * _iceHealMount);
                }
            }

            if (_skills[SkillKeyList.snowbomb].chk_shotable(delTime, Cool, mobDist)) // 눈폭탄
            {
                _pjt = _psm.getPrej(SkillKeyList.snowbomb);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_skills[SkillKeyList.snowbomb].att * Att, _skills[SkillKeyList.snowbomb].size);
            }

            if (_skills[SkillKeyList.iceshield].chk_Time(delTime, Cool) && _shield.IsUse == false) // 실드
            {
                _shield.repeatInit(_skills[SkillKeyList.iceshield].att * Att, () => { _skills[SkillKeyList.iceshield]._timer = 0; });
            }
        }

        IEnumerator fallingHail()
        {
            Vector2 initPos;
            Vector2 targetPos;
            for (int i = 0; i < _skills[SkillKeyList.hail].count; i++)
            {
                _hail = _psm.getHail();

                targetPos = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-3f, 3f));
                initPos = targetPos + Vector2.one * 6;

                _hail.transform.position = initPos;
                _hail.Init(targetPos, _skills[SkillKeyList.halficicle].att * Att);

                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(()=>_gs.Pause == false);
            }
        }

        IEnumerator iceage()
        {
            _iceage.gameObject.SetActive(true);
            _iceage.SetTrigger("iceage");

            EnemyFrozen(_skills[SkillKeyList.iceage].keep);
            EnemyDamage(_skills[SkillKeyList.iceage].att * Att);

            yield return new WaitForSeconds(3f);

            _iceage.gameObject.SetActive(false);
        }

        IEnumerator blizzarding()
        {
            Blizzard(true);

            float cnt = 0;
            float time = 0;
            while (cnt < 4)
            {
                time += Time.deltaTime;
                if (time > 1f)
                {
                    time = 0;
                    cnt++;

                    EnemyDamage(_skills[SkillKeyList.blizzard].att * Att);
                }
                yield return new WaitUntil(() => _gs.Pause == false);
            }            

            _skills[SkillKeyList.blizzard]._timer = 0;
            _enm.enemySlow(3f, 0.8f);            
            Blizzard(false);
        }

        #endregion

        #region 전설 장비

        void equipSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_equips[SkillKeyList.poison].chk_shotable(delTime, Cool, mobDist)) // 독병
            {
                _pjt = _psm.getPrej(SkillKeyList.poison);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_equips[SkillKeyList.poison].att, _equips[SkillKeyList.poison].size, 1f, _equips[SkillKeyList.poison].keep);
            }

            if (_equips[SkillKeyList.hammer].chk_shotable(delTime, Cool, mobDist)) // 망치
            {
                _pjt = _psm.getPrej(SkillKeyList.hammer);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.skillLvl = _equips[SkillKeyList.hammer].Lvl;
                _pjt.repeatInit(_equips[SkillKeyList.hammer].att * Att, _equips[SkillKeyList.hammer].size, 1.2f);
            }

            if (_equips[SkillKeyList.thunder].chk_rangeshotable(delTime, Cool, mobDist)) // 번개
            {
                _sec = (SsuddenEnergeCtrl)_psm.getSudden(SkillKeyList.thunder);

                _sec.transform.position = _gs.randomCloseEnemy(transform, _equips[SkillKeyList.thunder].range); // 근거리 아무적으로 교체
                _sec.Init(_equips[SkillKeyList.thunder]);
            }

            if (_equips[SkillKeyList.mine].chk_Time(delTime, Cool)) // 지뢰
            {
                for (int i = 0; i < 1 + _equips[SkillKeyList.mine].Lvl + BaseManager.userGameData.SkinIval[(int)skinIvalue.mine]; i++)
                {
                    _pjt = _psm.getPrej(SkillKeyList.mine);
                    _pjt.transform.position = transform.position;
                    _pjt.setTarget(transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 1.5f);
                    _pjt.repeatInit(_equips[SkillKeyList.mine].att * Att, _equips[SkillKeyList.mine].size, 1f, _equips[SkillKeyList.mine].keep);
                }
            }

            if (_equips[SkillKeyList.blackhole].chk_shotable(delTime, Cool, mobDist)) // 소용돌이
            {
                _pjt = _psm.getPrej(SkillKeyList.blackhole);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_equips[SkillKeyList.blackhole].att * Att, _equips[SkillKeyList.blackhole].size, 1f, _equips[SkillKeyList.blackhole].keep);
            }

            if (_equips[SkillKeyList.pet].chk_shotable(delTime, Cool, mobDist)) // 쫄따구
            {
                _pjt = _psm.getPrej(SkillKeyList.pet);
                _pjt.transform.position = _pet.Pos.position;

                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_equips[SkillKeyList.pet].att * Att, _equips[SkillKeyList.pet].size);
            }
        }

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

        void chkWild()
        {
            if (_hasWild)
            {
                _wildAtt = (MaxHp - _hp) * MaxHp * 0.01f * _wildMount;
                _wildAtt = (_wildAtt > 0) ? 1f + _wildAtt : 1f;
            }
        }

        #endregion

        #region 

        void hpgen(float time)
        {
            _genTime += time;
            if (_genTime > 1f)
            {
                _hp += MaxHp * Hpgen;
                if (_hp > MaxHp)
                {
                    _hp = MaxHp;
                }

                chkWild();

                _hpbar.localScale = new Vector2(_hp / MaxHp, 1f);
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
            _buffStt[(int)bff] = 1f;

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
            if (num < SkillKeyList.snowball)
            {
                _abils[num].skillUp();
            }
            else if (num < SkillKeyList.poison)
            {
                _skills[num].skillUp();

                if (_skills[num].active)
                {
                    if (num == SkillKeyList.icetornado)
                    {
                        _tornado.gameObject.SetActive(true);
                        _tornado.Init(_skills[num].att, _skills[num].delay, _skills[num].size);
                    }
                    else if (num == SkillKeyList.iceshield && _shield.IsUse == false)
                    {
                        _shield.repeatInit(_skills[num].att, () => { _skills[num]._timer = 0; });
                    }
                }
            }
            else
            {
                _equips[num].skillUp();

                if (num == SkillKeyList.pet)
                {
                    _pet.gameObject.SetActive(true);
                    _pet.appear(_equips[SkillKeyList.pet].Lvl);
                }
                else if (num == SkillKeyList.slot)
                {
                    _equipSlotCnt = 3;
                }
            }
        }

        public void setEquip(int num, SkillKeyList skill)
        {
            if (skill != SkillKeyList.slot)
            {
                selectEquips[num] = skill;
            }

            getSkill(skill);
        }

        public void chkSkillObj()
        {
            if (_equips[SkillKeyList.pet].active == false)
            {
                _pet.gameObject.SetActive(false);
            }
        }

        public void getExp(int exp)
        {
            _playerExp += exp * Exp;

            if (_playerExp > _playerMaxExp)
            {
                // 레벨업 
                _gs.levelUp();

                _playerExp -= _playerMaxExp;
                _playerMaxExp = _playerMaxExp * 1.3f;
            }
        }

        public void getHealed(float heal)
        {
            _hp += heal;
            if (_hp > MaxHp)
            {
                _hp = MaxHp;
            }
            _dmgFont.getText(transform, heal, dmgTxtType.heal, true);

            chkWild();
            _hpbar.localScale = new Vector2(_hp / MaxHp, 1f);
        }

        public void getDamaged(float dmg, bool ignoreDef = false)
        {
            if (_shield.IsUse) // 실드
            {
                _shield.getDamage(dmg);
                return;
            }

            if (_isAlmighty || IsInvinc) // 무적
            {
                return;
            }

            damagedAni(); // 데미지 모션

            if (dmg > Def || ignoreDef) // 데미지>방어력 or 방어력무시
            {
                if (ignoreDef == false)
                {
                    dmg -= Def;
                }

                _dmgFont.getText(transform, dmg, dmgTxtType.standard, true); // 데미지 폰트

                if (ignoreDef == false) // 방무 아닐때만 이펙트
                {
                    _efm.makeEff(effAni.playerhit, transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.5f));
                }

                _hp -= dmg;

                if (_hp <= 0) // 뒤짐
                {
                    StartCoroutine(playerDie());
                }

                chkWild();

                _hpbar.localScale = new Vector2(_hp / MaxHp, 1f);
            }
            else
            {
                return;
            }
        }

        IEnumerator playerDie()
        {
            _hp = 0;
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

                    _hp = MaxHp * BaseManager.userGameData.SkinFval[(int)skinFvalue.rebirth] * 0.01f;
                    if (_hp > MaxHp)
                    {
                        MaxHp = _hp;
                    }
                }
                else if (_rebirth_bonus) // 광고 부활템
                {
                    _rebirth_bonus = false;
                    bool _chk = false;

                    yield return new WaitForSeconds(1f);

                    if (BaseManager.userGameData.RemoveAD == false)
                    { 
                        // 창 오픈 - 광고제거 사면 바로 부활
                        _gs.openAdRebirthPanel(() =>
                        {
                            _chk = true;
                        }, _gameOver);

                        yield return new WaitUntil(() => _chk == true); // 광고 후 진행
                    }

                    _hp = MaxHp;        
                }

                _efm.getRebirth(transform.position, () =>
                {
                    _spine.gameObject.SetActive(true);
                },()=> 
                {
                    _hpbar.localScale = new Vector2(_hp / MaxHp, 1f);
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
            StartCoroutine(almighty());
        }

        // 무적
        IEnumerator almighty()
        {
            _isAlmighty = true;
            _almightCase.SetActive(true);
            float time = 0;
            float val = 0;

            while (time < 1f)
            {
                time += Time.deltaTime;
                val = 1 - time;
                _albar.localScale = new Vector2(val, 1f);
                yield return new WaitForEndOfFrame();
            }

            _isAlmighty = false;
            _almightCase.SetActive(false);
        }

        public void playerSlow()
        {
            StartCoroutine(Slow());
        }

        IEnumerator Slow()
        {
            _speed = gameValues._defaultSpeed / 2;

            yield return new WaitForSeconds(3f);

            _speed = gameValues._defaultSpeed;
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
            _pet.onPause(bl);
        }

        public void whenPlayerDie()
        {
            // 총알파괴
            _psm.onClear();

            // 유물제거
            for (SkillKeyList eq = SkillKeyList.poison; eq < SkillKeyList.max; eq++)
            {
                _equips[eq].clear();
            }

            // 쿨타임 초기화
            for (SkillKeyList eq = SkillKeyList.snowball; eq < SkillKeyList.poison; eq++)
            {
                _skills[eq]._timer = 0;
            }

            // 스킨 스킬쿨 초기화
            _chkInvincTime = 0;

            _iceage.gameObject.SetActive(false);
            Blizzard(false);
            IsInvinc = false;
            _shield.Destroy();

            // 디버프 제거

            // 무적 제거

        }
    }
}