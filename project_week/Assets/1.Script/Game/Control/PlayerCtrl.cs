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

        GameScene _gs;
        enemyManager _enm;
        playerSkillManager _psm;
        dmgFontManager _dmgFont;
        effManager _efm;

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
                float calAtt = _standardStt[(int)snowStt.att] * _skillStt[(int)snowStt.att] * _buffStt[(int)snowStt.att];

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
                float calDef = _standardStt[(int)snowStt.def] * _skillStt[(int)snowStt.def] * _buffStt[(int)snowStt.def];

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
                float calHpgen = _standardStt[(int)snowStt.def] * _skillStt[(int)snowStt.def] * _buffStt[(int)snowStt.def];

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
                float calCool = _standardStt[(int)snowStt.cool] * _skillStt[(int)snowStt.cool] * _buffStt[(int)snowStt.cool];

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
                float calExp = _standardStt[(int)snowStt.exp] * _skillStt[(int)snowStt.exp] * _buffStt[(int)snowStt.exp];

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
                float calSize = _standardStt[(int)snowStt.size] * _skillStt[(int)snowStt.size] * _buffStt[(int)snowStt.size];

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
                float calHealMount = _standardStt[(int)snowStt.heal] * _skillStt[(int)snowStt.heal] * _buffStt[(int)snowStt.heal];

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
                float calSpeed = gameValues._defaultSpeed * _standardStt[(int)snowStt.speed] * _skillStt[(int)snowStt.speed] * _buffStt[(int)snowStt.speed];

                if (BaseManager.userGameData.ApplySeason == _gs.ClockMng.Season)
                {
                    return calSpeed * _seasonStt[(int)snowStt.speed];
                }

                return calSpeed;
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

        bool _rebirth;
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

            _isDie = false;
            _isAlmighty = false;

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
            getSkill(SkillKeyList.blizzard);

            if (BaseManager.userGameData.SkinBval[(int)skinBvalue.mine])
            {
                getSkill(SkillKeyList.mine);
                selectEquips[0] = SkillKeyList.mine;
            }

            StartCoroutine(skillUpdate());
            //StartCoroutine(chk());
        }

        IEnumerator chk()
        {
            while (true)
            {
                Debug.Log(Att);
                yield return new WaitForSeconds(1f);
            }
        }

        void getOriginStatus()
        {
            int len = (int)snowStt.max;

            _standardStt    = new float[len];
            _skillStt       = new float[len];
            _buffStt        = new float[len];
            _seasonStt      = new float[len];
            for (int i = 0; i < len; i++)
            {
                _standardStt[i] = 1f;
                _skillStt[i] = 1f;
                _buffStt[i] = 1f;
                _seasonStt[i] = 1f;
            }

            if (BaseManager.userGameData.ApplySeason == null)
            {
                _hp = _standardStt[(int)snowStt.maxHp] = BaseManager.userGameData.o_Hp * BaseManager.userGameData.AddStats[0] * 100;
                _standardStt[(int)snowStt.att] = BaseManager.userGameData.o_Att * BaseManager.userGameData.AddStats[1];
                _standardStt[(int)snowStt.def] = BaseManager.userGameData.o_Def * BaseManager.userGameData.AddStats[2];
                _standardStt[(int)snowStt.hpgen] = BaseManager.userGameData.o_Hpgen * BaseManager.userGameData.AddStats[3];
                _standardStt[(int)snowStt.cool] = BaseManager.userGameData.o_Cool * BaseManager.userGameData.AddStats[4];
                _standardStt[(int)snowStt.exp] = BaseManager.userGameData.o_ExpFactor * BaseManager.userGameData.AddStats[5];
            }
            else
            {
                _hp = _standardStt[(int)snowStt.maxHp] = BaseManager.userGameData.o_Hp * 10;
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
            _rebirth = BaseManager.userGameData.SkinBval[(int)skinBvalue.rebirth];
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

                hpgen(delTime);

                deBuffChk(delTime);
                _dot = _dotDmg.dotDmging(delTime);
                if (_dot > 0)
                {
                    getDamaged(_dot);
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
                _pjt.repeatInit(_equips[SkillKeyList.poison].att * Att, _equips[SkillKeyList.poison].size, 1f, _equips[SkillKeyList.poison].keep);
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
                    snowStt stt = _buffList[i].Stt;

                    _buffList.RemoveAt(i);

                    reCalBuff(stt);
                    i--;
                }
            }
        }

        /// <summary> 삭제된 타입 버프 일괄계산 </summary>
        void reCalBuff(snowStt stt)
        {
            _buffStt[(int)stt] = 1f;

            for (int i = 0; i < _buffList.Count; i++)
            {
                if (_buffList[i].Stt == stt)
                {
                    _buffStt[(int)stt] *= _buffList[i].Val;
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

        public void getDamaged(float dmg)
        {
            if (_shield.IsUse)
            {
                _shield.getDamage(dmg);
                return;
            }

            damagedAni();

            if (_isAlmighty || IsInvinc)
            {
                return;
            }

            if (true || dmg > Def)
            {
                dmg -= Def;
                _dmgFont.getText(transform, dmg, dmgTxtType.standard, true);

                _hp -= dmg;

                if (_hp <= 0)
                {
                    _hp = 0;                    
                    _isDie = true;

                    if (_rebirth)
                    {
                        _rebirth = false;
                        // 이펙
                        Debug.Log("부활");
                        _hp = MaxHp * BaseManager.userGameData.SkinFval[(int)skinFvalue.rebirth] * 0.01f;
                        if (_hp > MaxHp)
                        {
                            MaxHp = _hp;
                        }
                    }
                    else
                    {
                        _gameOver();
                    }
                }

                chkWild();

                _hpbar.localScale = new Vector2(_hp / MaxHp, 1f);
            }
            else
            {
                return;
            }
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

        /// <summary> </summary>
        public BuffEffect setDeBuff(snowStt stt, float term, float val, BuffEffect.buffTermType isterm = BuffEffect.buffTermType.term)
        {
            if (stt == snowStt.speed && _invSlow)
            { 
                return null; 
            }

            BuffEffect DBuff = new BuffEffect(stt, term, val, isterm);

            _buffStt[(int)stt] *= DBuff.Val;

            _buffList.Add(DBuff);

            return DBuff;
        }

        /// <summary> 버프 수동 삭제 </summary>
        public void manualRemoveDeBuff(BuffEffect bff)
        {
            if (bff != null)
            {
                snowStt stt = bff.Stt;

                _buffList.Remove(bff);

                reCalBuff(stt);
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
    }
}