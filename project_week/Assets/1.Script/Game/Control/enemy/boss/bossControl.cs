using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace week
{
    public abstract class bossControl : EnemyCtrl
    {
        [Header("type")]
        [SerializeField] Boss _boss = Boss.boss_owl;

        [SerializeField] protected GameObject _hpCase;
        [SerializeField] protected Transform _hpbar;

        protected Vector3[] _shotDir = new Vector3[4] { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
        protected enum dir { back, right, front, left }
        protected enum stat { Idle, back, Patrol, Trace, skill0, skill1, die }
        protected stat _stats;
        protected dir _dir;

        protected Action<float> killFunc;
        protected LandObject _home;
        protected Vector3 _homePos;
        protected readonly float _bossRange = 7.5f;
        protected readonly float _bossSkillCool = 4f;

        protected float MaxHp { get { return _finalStt[(int)snowStt.maxHp]; } }
        protected float Att { get { return _finalStt[(int)snowStt.att]; } }
        protected float Def { get { return _finalStt[(int)snowStt.def]; } }
        protected float Speed { get { return _standardStt[(int)snowStt.speed]; } }
        protected float Exp { get { return _standardStt[(int)snowStt.exp]; } }

        protected float _bossCoin = 1;

        float _skill0 = 1;
        float _skill1 = 1;
        protected float Skill0 { get; set; }
        protected float Skill1 { get; set; }

        Vector3 _direct;
        protected bool _isAppear;
        protected float _appearTime = 2f;

        public bool isFrozen { get; set; }
        public float Slow { get; set; }

        public Boss getType { get { return _boss; } }


        public override float getDamaged(float val, bool ignoreDef = false)
        {
            if (ignoreDef == false)
            {
                val = (val - Def > 0) ? val - Def : 0;
            }

            dmgFunc(transform, Convert.ToInt32(val).ToString(), dmgTxtType.standard, false);
            _hp -= val;

            refreshHpbar();

            if (_hp <= 0)
            {
                enemyDie();
            }

            damagedAni();
            return val;
        }

        public void setting(GameScene gs, Action<Transform, string, dmgTxtType, bool> dmg, Action<float> kill)
        {
            _player = gs.Player;
            _gs = gs;
            _efMng = gs.EfMng;
            _enProjMng = gs.EnProjMng;
            _clMng = gs.ClockMng;

            dmgFunc = dmg;
            killFunc = kill;
        }

        public void FixInit()
        {
            _standardStt = new float[(int)snowStt.max];
            _standardStt[(int)snowStt.maxHp] = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossValData.hp.ToString());
            _standardStt[(int)snowStt.att] = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossValData.att.ToString());
            _standardStt[(int)snowStt.def]    = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossValData.def.ToString());
            _standardStt[(int)snowStt.speed]  = DataManager.GetTable<float>(DataTable.boss, _boss.ToString(), BossValData.speed.ToString()) * gameValues._defaultSpeed;
            _standardStt[(int)snowStt.exp] = 100f;

            _bossCoin = DataManager.GetTable<float>(DataTable.boss, _boss.ToString(), BossValData.coin.ToString());

            _skill0 = DataManager.GetTable<float>(DataTable.boss, _boss.ToString(), BossValData.skill0.ToString());
            _skill1 = DataManager.GetTable<float>(DataTable.boss, _boss.ToString(), BossValData.skill1.ToString());

            _dotDmg = new dotDmg();

            otherWhenFixInit();

            RepeatInit();
        }

        public void RepeatInit() 
        {
            _dotDmg.reset();

            _finalStt = new float[(int)snowStt.max];

            _finalStt[(int)snowStt.maxHp] = _standardStt[(int)snowStt.maxHp] * Mathf.Pow(1.2f, _clMng.Day);
            _finalStt[(int)snowStt.att] = _standardStt[(int)snowStt.att] * Mathf.Pow(1.2f, _clMng.Day);
            _finalStt[(int)snowStt.def] = _standardStt[(int)snowStt.def] * Mathf.Pow(1.1f, _clMng.Day);

            Skill0 = _skill0 * _finalStt[(int)snowStt.att];
            Skill1 = _skill1 * _finalStt[(int)snowStt.att];


            _hp = MaxHp;
            _isDie = false;
            IsUse = true;
            _isAppear = false;
            isFrozen = false;
            Slow = 1f;

            refreshHpbar();
            _isDmgAction = false;
            gameObject.SetActive(true);

            otherWhenRepeatInit();
        }

        public override void setBuff(eBuff bff, float val)
        {
            Debug.LogError("보스는 디버프 걸리지 않음");
        }

        protected void refreshHpbar()
        {
            _hpbar.localScale = new Vector2(_hp / MaxHp, 1f);
        }

        public void PlayObject()
        {
            StartCoroutine(lifeCycle());
        }

        public void setHome(LandObject home, Vector3 homePos)
        {
            _home = home;
            _homePos = homePos;
        }

        /// <summary> 보스 독뎀 1% </summary>
        protected void chkDotDmg()
        {
            _dot = _dotDmg.dotDmging(deltime);
            if (_dot > 0)
            {
                _dot = MaxHp * _dot * 0.01f;
                getDamaged(_dot, true);
            }
        }

        public override void enemyDie()
        {
            SoundManager.instance.PlaySFX(SFX.bossdie);
            killFunc(_bossCoin);

            _player.getNamedBuff(_boss);            

            _efMng.makeEff(effAni.bossExplosion, transform.position);
            Destroy();
        }

        public void chkDestroy(LandObject land)
        {
            if (land == _home)
            {
                Destroy();
            }
        }

        public override void Destroy()
        {
            _isDie = true;
            preDestroy();
        }

        public override void setFrozen(float term) { }

        protected virtual IEnumerator lifeCycle()
        {
            yield return null;
        }
        protected abstract void switchStat(stat st);

        public abstract void whenEnemyEnter();
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(this, Att);
            }
            else if (collision.gameObject.tag.Equals("Enemy"))
            {
                collision.gameObject.GetComponent<MobControl>().enemyDieToBoss();
                whenEnemyEnter();
            }
        }
    }
}