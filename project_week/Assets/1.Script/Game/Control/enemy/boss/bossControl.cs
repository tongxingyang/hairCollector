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


        protected Vector3[] _shotDir = new Vector3[4] { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
        protected enum dir { back, right, front, left }
        protected enum stat { Idle, back, Patrol, Trace, skill0, skill1, die }
        protected stat _stats;
        protected dir _dir;

        protected LandObject _home;
        protected Vector3 _homePos;

        protected int _skill0 = 1;
        protected int _skill1 = 1;

        Vector3 _direct;
        protected bool _isAppear;
        protected float _appearTime = 2f;

        public bool isFrozen { get; set; }
        public float Slow { get; set; }

        public float getDamage { get => _att; }

        public Boss getType { get { return _boss; } }

        public void setting(GameScene gs, effManager ef, EnemyProjManager ep, Action<Transform, int, dmgTxtType, bool> dmg, Action<int> kill)
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
            _hp = _maxhp = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossData.hp.ToString());
            _att = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossData.att.ToString());
            _def = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossData.def.ToString());
            _speed = DataManager.GetTable<float>(DataTable.boss, _boss.ToString(), BossData.speed.ToString()) * gameValues._defaultSpeed;

            _skill0 = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossData.skill0.ToString());
            _skill1 = DataManager.GetTable<int>(DataTable.boss, _boss.ToString(), BossData.skill1.ToString());

            otherWhenFixInit();

            RepeatInit();
        }

        public void RepeatInit() 
        {
            _isDie = false;
            IsUse = true;
            _isAppear = false;
            isFrozen = false;
            Slow = 1f;

            _spine.skeleton.SetColor(Color.white);
            _hpbar.localScale = new Vector2(_hp / _maxhp, 1f);
            _isDmgAction = false;
            gameObject.SetActive(true);

            otherWhenRepeatInit();
        }

        public void PlayObject(Vector3 pos)
        {
            transform.position = pos;
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
            killFunc((int)_exp);
            _efMng.makeEff(effAni.bossExplosion, transform.position);
            Destroy();
        }

        public override void Destroy()
        {
            _isDie = true;
            preDestroy();
        }

        public override void setFrozen(float term) { }

        // Update is called once per frame
        protected virtual IEnumerator lifeCycle()
        {
            yield return null;
        }
        protected abstract void switchStat(stat st);

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(_att);
            }
            else if (collision.gameObject.tag.Equals("Enemy"))
            {
                collision.gameObject.GetComponent<MobControl>().enemyDieToBoss();                
            }
            else if (collision.gameObject.tag.Equals("Finish"))
            {
                Destroy();
            }
        }

        //void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (collision.gameObject.tag.Equals("Player"))
        //    {
        //        _player.getDamaged(_att);
        //    }
        //    else if (collision.gameObject.tag.Equals("Player"))
        //    {
        //        _player.getDamaged(_att);
        //    }
        //    else if (collision.tag.Equals("Finish"))
        //    {
        //        Destroy();
        //    }
        //}
    }
}