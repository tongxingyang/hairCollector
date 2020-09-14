using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class baseRuinTrap : poolingObject
    {
        protected GameScene _gs;
        protected EnemyProjManager _epm;

        protected PlayerCtrl _player;

        protected bool _onTrap;
        public bool OnTrap { set => _onTrap = value; }

        public abstract void operate();

        public void fixedInit(GameScene gs)
        {
            _gs = gs;
            _player = _gs.Player;
            _epm = _gs.EnProjMng;

            preInit();
            whenFixedInit();

            repeatInit();
        }
        protected abstract void whenFixedInit();

        public void repeatInit()
        {
            _onTrap = false;

            whenRepeatInit();
        }
        protected abstract void whenRepeatInit();
    }
}