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
        [SerializeField] Rigidbody2D _rigid;
        
        [Space(20)]
        [Header("Skill")]
        [SerializeField] tornado _tornado = null;
        [SerializeField] shield _shield = null;
        [SerializeField] SpetCtrl _pet = null;
        [SerializeField] Animator _iceage = null;
        [Header("etc")]
        [SerializeField] Transform _hpbar;
        [SerializeField] GameObject _almightCase;
        [SerializeField] Transform _albar;
        [SerializeField] Camera _main;

        GameScene _gs; 
        playerSkillManager _psm;
        dmgFontManager _dmgFont;
        effManager _efm;

        Dictionary<getSkillList, ability> _abils;
        public Dictionary<getSkillList, ability> Abils { get => _abils; }
        Dictionary<getSkillList, skill> _skills;
        public Dictionary<getSkillList, skill> Skills { get => _skills; }
        Dictionary<getSkillList, skill> _equips;
        public Dictionary<getSkillList, skill> Equips { get => _equips; }
        public getSkillList[] selectEquips { get; set; }
        int _equipSlotCnt = 2;
        public bool isGetSlot { get { return _equipSlotCnt == 3; } }
        float _hpgenTimer = 0;

        #region [Stat]

        float _playerExp = 0;
        float _playerMaxExp = 20f * 3f;
        public float ExpRate { get { return _playerExp / _playerMaxExp; } }
        float _hp;

        float _maxhp;
        float _hpValue = 1f;
        float MaxHp { get { return _maxhp * _hpValue; } }

        float _att;
        float _attValue = 1f;
        float Att { get { return _att * _attValue; } }

        float _def;
        float _defValue;
        float Def { get { return _def + _defValue; } }

        float _genTime;
        float _hpgen;
        float _hpgenValue;
        float Hpgen { get { return _hpgen + _hpgenValue; } }

        float _cool = 1f;
        float _coolValue = 1f;
        float Cool { get { return _cool * _coolValue; } }

        float _exp;
        float _expValue = 1f;
        float Exp { get { return _exp * _expValue; } }

        float _sizeValue;
        float _healMountValue;

        float _speed = gameValues._defaultSpeed * 0.8f;
        float _environmentSpeed = 1f;
        float _speedValue = 1f;
        float Speed 
        {
            get
            {
                float Dbff = 1;
                for (int i = 0; i < _buffList.Count; i++)
                {
                    if (_buffList[i]._eDB == eDeBuff.slow)
                    {
                        Dbff *= _buffList[i]._val;
                    }
                }
                return _speed * _speedValue * Dbff * _environmentSpeed;
            }
        }

        List<BuffEffect> _buffList;

        #endregion

        bool _isDie;
        bool _isAlmighty;
        float _dustTime;
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
        public float EnvironmentSpeed { set => _environmentSpeed = value; }

        // Start is called before the first frame update
        public void Init(GameScene gs)
        {
            _gs = gs;
            _psm = _gs.SkillMng;
            _dmgFont = _gs.DmgfntMng;
            _efm = _gs.EfMng;

            _isDie = false;
            _isAlmighty = false;

            _abils = new Dictionary<getSkillList, ability>();
            _skills = new Dictionary<getSkillList, skill>();
            _equips = new Dictionary<getSkillList, skill>();
            selectEquips = new getSkillList[3] { getSkillList.max, getSkillList.max, getSkillList.max };

            _hp = _maxhp = BaseManager.userEntity.Hp * 100;
            _att = BaseManager.userEntity.AttFactor;
            _def = BaseManager.userEntity.Def;
            _hpgen = BaseManager.userEntity.Hpgen;
            _cool = BaseManager.userEntity.Cool;
            _exp = BaseManager.userEntity.ExpFactor;

            for (getSkillList i = getSkillList.hp; i < getSkillList.max; i++)
            {
                if (i < getSkillList.snowball)
                {
                    ability ab = new ability();
                    ab.Init((int)i);
                    ab.skUp = abilityApply;
                    _abils.Add(i, ab);
                }
                else if(i < getSkillList.poison)
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

            _pet.Init(_gs, () => { _skills[getSkillList.snowball].att *= 2; });

            getSkill(getSkillList.snowball);
            //getSkill(getSkillList.snowbomb);
            //getSkill(getSkillList.poison);
            //getSkill(getSkillList.blackhole);
            //selectEquips[0] = getSkillList.poison;
            //selectEquips[1] = getSkillList.blackhole;

            StartCoroutine(skillUpdate());
        }

        void abilityApply(getSkillList type)
        {
            switch (type)
            {
                case getSkillList.hp:
                    _hpValue *= _abils[type].val;
                    break;
                case getSkillList.att:
                    _attValue *= _abils[type].val;
                    break;
                case getSkillList.def:
                    _defValue += _abils[type].val;
                    break;
                case getSkillList.hpgen:
                    _hpgen += _abils[type].val;
                    break;
                case getSkillList.cool:
                    _coolValue *= _abils[type].val; 
                    break;
                case getSkillList.exp:
                    _expValue *= _abils[type].val;
                    break;
                case getSkillList.size:
                    _sizeValue *= _abils[type].val;
                    break;
                case getSkillList.healmount:
                    _healMountValue *= _abils[type].val;
                    break;
                case getSkillList.spd:
                    _speedValue *= _abils[type].val;
                    break;
                default:
                    break;
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

        IEnumerator skillUpdate()
        {
            float delTime;
            Vector3 closedMob;
            float mobDist;

            while (_isDie == false && _gs.GameOver == false)
            {
                delTime = Time.deltaTime;
                closedMob = _gs.mostCloseEnemy(transform);
                mobDist = Vector3.Distance(transform.position, closedMob);

                skillSequence(closedMob, mobDist, delTime);

                equipSequence(closedMob, mobDist, delTime);

                hpgen(delTime);

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        void skillSequence(Vector3 closedMob,float mobDist, float delTime)
        {
            if (_skills[getSkillList.snowball].chk_shotable(delTime, Cool, mobDist)) // 눈덩이
            {
                // int num = (_skills[(int)skills.snowball].lvl > 4) ? 4 : _skills[(int)skills.snowball].lvl;

                _pjt = _psm.getPrej(getSkillList.snowball);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_skills[getSkillList.snowball].att * Att, _skills[getSkillList.snowball].size);
            }

            if (_skills[getSkillList.icefist].chk_shotable(delTime, Cool, mobDist)) // 얼음주먹
            {
                _pjt = _psm.getPrej(getSkillList.icefist);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_skills[getSkillList.icefist].att * Att, _skills[getSkillList.icefist].size);
            }


            if (_skills[getSkillList.icicle].chk_shotable(delTime, Cool, mobDist)) // 고드름
            {
                _pjt = _psm.getPrej(getSkillList.icicle);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_skills[getSkillList.icicle].att * Att, _skills[getSkillList.icicle].size);
            }

            if (_skills[getSkillList.halficicle].chk_shotable(delTime, Cool, mobDist)) // 반달고드름
            {
                int num = _skills[getSkillList.halficicle].Lvl;
                float degree = 360f / num;
                Vector3 mce = closedMob;

                for (int i = 0; i < num; i++)
                {
                    _pjt = _psm.getPrej(getSkillList.halficicle);
                    _pjt.transform.position = transform.position;
                    _pjt.setTarget(mce, degree * i);
                    _pjt.repeatInit(_skills[getSkillList.halficicle].att * Att, _skills[getSkillList.halficicle].size);
                }
            }

            if (_skills[getSkillList.hail].chk_Time(delTime, Cool)) // 우박
            {
                StartCoroutine(fallingHail());
            }

            if (_skills[getSkillList.icewall].chk_Time(delTime, Cool)) // 빙벽
            {
                _soc = (SsuddenObstacleCtrl)_psm.getSudden(getSkillList.icewall);
                Vector3 pos = _gs.pVector * -1;
                _soc.transform.position = transform.position + (pos * 1.2f);
                _soc.Init(_skills[getSkillList.icicle]);
            }

            if (_skills[getSkillList.iceage].chk_Time(delTime, Cool)) // 아이스에이지
            {
                StartCoroutine(iceage());
            }

            if (_skills[getSkillList.blizzard].chk_Time(delTime, Cool)) // 블리자드
            {
                StartCoroutine(blizzarding());
            }

            if (_skills[getSkillList.snowbomb].chk_shotable(delTime, Cool, mobDist)) // 눈폭탄
            {
                _pjt = _psm.getPrej(getSkillList.snowbomb);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_skills[getSkillList.snowbomb].att * Att, _skills[getSkillList.snowbomb].size);
            }

            if (_skills[getSkillList.iceshield].chk_Time(delTime, Cool) && _shield.IsUse == false) // 실드
            {
                _shield.repeatInit(_skills[getSkillList.iceshield].att * Att, () => { _skills[getSkillList.iceshield]._timer = 0; });
            }
        }

        IEnumerator fallingHail()
        {
            Vector2 initPos;
            Vector2 targetPos;
            for (int i = 0; i < _skills[getSkillList.hail].count; i++)
            {
                _hail = _psm.getHail();

                targetPos = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-3f, 3f));
                initPos = targetPos + Vector2.one * 6;

                _hail.transform.position = initPos;
                _hail.Init(targetPos, _skills[getSkillList.halficicle].att * Att);

                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(()=>_gs.Pause == false);
            }
        }

        IEnumerator iceage()
        {
            _iceage.gameObject.SetActive(true);
            _iceage.SetTrigger("iceage");

            EnemyFrozen(_skills[getSkillList.iceage].keep);
            EnemyDamage(_skills[getSkillList.iceage].att * Att);

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

                    EnemyDamage(_skills[getSkillList.blizzard].att * Att);
                }
                yield return new WaitUntil(() => _gs.Pause == false);
            }            

            _skills[getSkillList.blizzard]._timer = 0;
            _gs.enemySlow(false, 3f, 0.8f);
            Blizzard(false);
        }

        #endregion

        #region 전설 장비

        void equipSequence(Vector3 closedMob, float mobDist, float delTime)
        {
            if (_equips[getSkillList.poison].chk_shotable(delTime, Cool, mobDist)) // 독병
            {
                _pjt = _psm.getPrej(getSkillList.poison);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_equips[getSkillList.poison].att * Att, _equips[getSkillList.poison].size, 1f, _equips[getSkillList.poison].keep);
            }

            if (_equips[getSkillList.hammer].chk_shotable(delTime, Cool, mobDist)) // 망치
            {
                _pjt = _psm.getPrej(getSkillList.hammer);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.skillLvl = _equips[getSkillList.hammer].Lvl;
                _pjt.repeatInit(_equips[getSkillList.hammer].att * Att, _equips[getSkillList.hammer].size, 1.2f);
            }

            if (_equips[getSkillList.thunder].chk_rangeshotable(delTime, Cool, mobDist)) // 번개
            {
                _sec = (SsuddenEnergeCtrl)_psm.getSudden(getSkillList.thunder);

                _sec.transform.position = _gs.randomCloseEnemy(transform, _equips[getSkillList.thunder].range); // 근거리 아무적으로 교체
                _sec.Init(_equips[getSkillList.thunder]);
            }

            if (_equips[getSkillList.mine].chk_Time(delTime, Cool)) // 지뢰
            {
                for (int i = 0; i < 1 + _equips[getSkillList.mine].Lvl; i++)
                {
                    _pjt = _psm.getPrej(getSkillList.mine);
                    _pjt.transform.position = transform.position;
                    _pjt.setTarget(transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 1.5f);
                    _pjt.repeatInit(_equips[getSkillList.mine].att * Att, _equips[getSkillList.mine].size, 1f, _equips[getSkillList.mine].keep);
                }
            }

            if (_equips[getSkillList.blackhole].chk_shotable(delTime, Cool, mobDist)) // 소용돌이
            {
                _pjt = _psm.getPrej(getSkillList.blackhole);
                _pjt.transform.position = transform.position;
                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_equips[getSkillList.blackhole].att * Att, _equips[getSkillList.blackhole].size, 1f, _equips[getSkillList.blackhole].keep);
            }

            if (_equips[getSkillList.pet].chk_shotable(delTime, Cool, mobDist)) // 쫄따구
            {
                _pjt = _psm.getPrej(getSkillList.pet);
                _pjt.transform.position = _pet.Pos.position;

                _pjt.setTarget(closedMob);
                _pjt.repeatInit(_equips[getSkillList.pet].att * Att, _equips[getSkillList.pet].size);
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
            }
        }

        void deBuffChk(float delTime)
        {
            for (int i = 0; i < _buffList.Count; i++)
            {
                _buffList[i].Term -= delTime;

                if (_buffList[i]._eDB == eDeBuff.slow)
                {
                    if (_buffList[i].TermOver)
                    {
                        _buffList.RemoveAt(i);
                        i--;
                    }
                }
                else if (_buffList[i]._eDB == eDeBuff.dotDem)
                {
                    if (_buffList[i].chkOne(delTime))
                    {
                        getDamaged(_buffList[i]._val);
                    }

                    if (_buffList[i].TermOver)
                    {
                        _buffList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        #endregion

        /// <summary> 스킬얻음 </summary>
        public void getSkill(getSkillList num)
        {
            if (num < getSkillList.snowball)
            {
                _abils[num].skillUp();
            }
            else if (num < getSkillList.poison)
            {
                _skills[num].skillUp();

                if (_skills[num].active)
                {
                    if (num == getSkillList.icetornado)
                    {
                        _tornado.gameObject.SetActive(true);
                        _tornado.Init(_skills[num].att, _skills[num].delay, _skills[num].size);
                    }
                    else if (num == getSkillList.iceshield && _shield.IsUse == false)
                    {
                        _shield.repeatInit(_skills[num].att, () => { _skills[num]._timer = 0; });
                    }
                }
            }
            else
            {
                _equips[num].skillUp();

                if (num == getSkillList.pet)
                {
                    _pet.gameObject.SetActive(true);
                    _pet.appear(_equips[getSkillList.pet].Lvl);
                }
                else if (num == getSkillList.slot)
                {
                    _equipSlotCnt = 3;
                }
            }
        }

        public void setEquip(int num, getSkillList skill)
        {
            if (skill != getSkillList.slot)
            {
                selectEquips[num] = skill;
            }

            getSkill(skill);
        }

        public void chkSkillObj()
        {
            if (_equips[getSkillList.pet].active == false)
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
            _dmgFont.getText(transform, (int)heal, dmgTxtType.heal, true);

            _hpbar.localScale = new Vector2(_hp / _maxhp, 1f);
        }

        public void getDamaged(float dmg)
        {
            if (_shield.IsUse)
            {
                _shield.getDamage(dmg);
                return;
            }

            damagedAni();

            if (_isAlmighty)
            {
                return;
            }

            if (true || dmg > Def)
            {
                dmg -= Def;
                _dmgFont.getText(transform, (int)dmg, dmgTxtType.standard, true);

                _hp -= dmg;

                if (_hp <= 0)
                {
                    _hp = 0;                    
                    _isDie = true; 
                    _gameOver();
                }

                _hpbar.localScale = new Vector2(_hp / _maxhp, 1f);
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

        public void setDeBuff(eDeBuff e, bool l, float t, float v)
        {
            BuffEffect DBuff = new BuffEffect(e,l,t,v);
            _buffList.Add(DBuff);
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