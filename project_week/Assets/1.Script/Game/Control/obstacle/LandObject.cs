using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace week
{
    public class LandObject : poolingObject
    {
        [SerializeField] mapObstacle _type;
        [SerializeField] GameObject _pocket;
        public mapObstacle getType { get => _type; }

        protected GameScene _gs;
        protected PlayerCtrl _player;
        protected enemyManager _enm;
        protected clockManager _clock;

        Transform _homePos;

        #region Boss

        [ShowIf("chk_boss")]
        [SerializeField] Transform[] bosPos;
        bool chk_boss { get { return _type < mapObstacle.map_0; } }

        bossControl[] bosss;

        #endregion

        #region Ruin

        [ShowIf("chk_ruin")]
        [SerializeField] LandItem present;
        [ShowIf("chk_ruin")]
        [SerializeField] baseRuinTrap _trap;
        bool chk_ruin { get { return _type >= mapObstacle.ruin0; } }

        #endregion

        #region normal

        [ShowIf("chk_normal")]
        [SerializeField] LandItem[] _normalTem;
        [ShowIf("chk_normal")]
        [SerializeField] seasonlyBase[] _objs;
        [ShowIf("chk_normal")]
        [SerializeField] environmentObject[] _envs;
        bool chk_normal { get { return chk_boss == false && chk_ruin == false; } }
       

        #endregion

        public void FixInit(GameScene gs, tileBase tile, season ss)
        {
            preInit();

            _gs = gs;
            _enm = _gs.EnemyMng;
            _player = _gs.Player;
            _clock = _gs.ClockMng;

            if (chk_boss)
            {
            }
            else if (chk_ruin)
            {
                _trap.fixedInit(_gs);
            }
            else
            {
                foreach (seasonlyBase sb in _objs)
                {
                    sb.FixedInit();
                }
                if (_envs != null)
                {
                    foreach (environmentObject ev in _envs)
                    {
                        ev.Init(_gs);
                    }
                }
            }

            otherSetInit();
            RepeatInit(tile, ss);
        }

        public void RepeatInit(tileBase tile, season ss)
        {
            preInit();

            _homePos = tile.transform;
            tile.reclaim += Destroy;

            if (chk_boss)
            {
                bossInit();
            }
            else if (chk_ruin)
            {
                ruinInit();
                _trap.repeatInit();
            }
            else
            {
                normalInit(ss);
            }

            StartCoroutine(chkInUseActive());
        }

        protected virtual void otherSetInit() { }

        IEnumerator chkInUseActive()
        {
            bool chk;
            bool prev;
            while (_isUse)
            {
                chk = (Mathf.Abs(_player.transform.position.x - transform.position.x) < 17.5f) && (Mathf.Abs(_player.transform.position.y - transform.position.y) < 17.5f);

                prev = _pocket.activeSelf;
                _pocket.SetActive(chk);

                if (prev == false)
                {
                    if (chk_ruin && chk)
                    {
                        _trap.operate();
                    }
                }

                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        #region bossLand

        void bossInit()
        {
            present = null;
            _objs = null;
            _normalTem = null;

            bosss = new bossControl[bosPos.Length];

            // Boss _bs = (Boss)Random.Range(0, (int)Boss.max);

            Boss _bs = (Random.Range(0, 5) == 0) ? Boss.boss_bear : (Boss)((int)_clock.Season);

            for (int i = 0; i < bosPos.Length; i++)
            {
                bosss[i] = _enm.makeBoss(_bs, this, bosPos[i].position);
                bosss[i].PlayObject();
            }
        }

        #endregion

        #region ruinLand

        void ruinInit()
        {
            bosPos = null;
            _objs = null;
            _normalTem = null;

            present.presentRespone();
            present.Init(_gs, ()=> { _trap.OnTrap = false; });

            StartCoroutine(chkPlayer());
        }

        IEnumerator chkPlayer()
        {
            yield return new WaitForSeconds(1f);
            while (true)
            {
                _trap.OnTrap = (Vector3.Distance(_player.transform.position, transform.position) < 8f);

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        #endregion

        #region normalLand

        void normalInit(season ss)
        {
            bosPos = null;
            present = null;
            foreach (LandItem tem in _normalTem)
            {
                tem.Init(_gs, ()=> { });
            }
            foreach (seasonlyBase sb in _objs)
            {
                sb.setSeason(ss);
            }
        }

        #endregion

        public override void Destroy()
        {
            preDestroy();
            otherInDestroy();
        }

        protected virtual void otherInDestroy()
        {
            if (chk_boss)
            {
                for (int i = 0; i < bosPos.Length; i++)
                {
                    bosss[i].chkDestroy(this);
                }
            }
            else if (chk_ruin)
            {
            }
            else
            {
            }
            
        }

        [ContextMenu("SetNormalField")]
        void checkNormalObj()
        {
            _objs = GetComponentsInChildren<seasonlyBase>();
            _normalTem = GetComponentsInChildren<LandItem>();
            _envs = GetComponentsInChildren<environmentObject>();
        }

        public override void onPause(bool bl)
        {
        }
    }
}