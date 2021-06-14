using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace week
{
    public class LandObject : IobstacleObject, IPause
    {
        protected PlayerCtrl _player;
        protected enemyManager _enm;
        protected clockManager _clock;
        [SerializeField] Transform _homePos;
        
        List<Boss> _nowBossList;
        bossControl _bosss;

        public override IobstacleObject FixedInit(GameScene gs, obstacleKeyList type)
        {
            _gs = gs;
            _enm = _gs.EnemyMng;
            _player = _gs.Player;
            _clock = _gs.ClockMng;

            getType = type;

            return RepeatInit();
        }

        public override IobstacleObject RepeatInit()
        {
            preInit();

            _nowBossList = new List<Boss>();
            Boss _bs = D_season.GetEntity(_clock.NowSeason.ToString()).f_boss;

            if (_bs == Boss.all)
            {
                for (Boss b = Boss.boss_bear; b < Boss.all; b++)
                {
                    _nowBossList.Add(b);
                }
            }
            else
            {
                _nowBossList.Add(Boss.boss_bear);
                _nowBossList.Add(_bs);
            }

            _bs = _nowBossList[Random.Range(0, _nowBossList.Count)];

            _bosss = _enm.makeBoss(_bs, this, _homePos.position);
            _bosss.PlayObject();

            return this;
        }

        protected override void Destroy()
        {
            _bosss.ForceDestroy();
            preDestroy();
        }

        public virtual void onPause(bool bl)
        {
        }
    }
}