using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using ES3Types;

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

        protected LandObject _home;
        protected Vector3 _homePos;
        protected readonly float _bossRange = 7.5f;
        protected readonly float _bossSkillCool = 4f;

        protected float MaxHp { get { return _standardStt[(int)snowStt.maxHp]; } set { _standardStt[(int)snowStt.maxHp] = value; } }
        protected float Att { get { return _standardStt[(int)snowStt.att]; } }
        protected float Def { get { return _standardStt[(int)snowStt.def]; } }
        protected float Speed { get { return _standardStt[(int)snowStt.speed]; } }
        protected float Exp { get { return _standardStt[(int)snowStt.exp]; } }

        protected int _skill0 = 1;
        protected int _skill1 = 1;

        Vector3 _direct;
        protected bool _isAppear;
        protected float _appearTime = 2f;

        public bool isFrozen { get; set; }
        public float Slow { get; set; }

        public Boss getType { get { return _boss; } }
        public override float getDamaged(float val)//, bool knockBack = false)
        {
            val = (val - Def > 0) ? val - Def : 0;

            dmgFunc(transform, (int)val, dmgTxtType.standard, false);
            _hp -= val;

            refreshHpbar();

            if (_hp <= 0)
            {
                enemyDie();
            }

            damagedAni();
            return val;
        }

        public void setting(GameScene gs, effManager ef, EnemyProjManager ep, Action<Transform, float, dmgTxtType, bool> dmg, Action<int> kill)
        {
            _player = gs.Player;
            _gs = gs;
            _efMng = ef;
            _enProjMng = ep;

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

            _skill0 = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossValData.skill0.ToString());
            _skill1 = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossValData.skill1.ToString());

            otherWhenFixInit();

            RepeatInit();
        }

        public void RepeatInit() 
        {
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

        public override void enemyDie()
        {
            SoundManager.instance.PlaySFX(SFX.bossdie);
            killFunc((int)Exp);
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
                _player.getDamaged(Att);
            }
            else if (collision.gameObject.tag.Equals("Enemy"))
            {
                collision.gameObject.GetComponent<MobControl>().enemyDieToBoss();
                whenEnemyEnter();
            }
        }        
    }
}