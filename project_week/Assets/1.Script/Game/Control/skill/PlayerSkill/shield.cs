using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class shield : MonoBehaviour
    {
        GameScene _gs;
        dmgFontManager _dfm;

        bool _isUse;
        float _hp = 0;
        public bool IsUse { get => _isUse; set => _isUse = value; }

        Action _dest;

        private void Awake()
        {
            IsUse = false;
            gameObject.SetActive(false);
        }

        public void FixedInit(GameScene gs)
        {
            _gs = gs;
            _dfm = gs.DmgfntMng;
        }

        public void repeatInit(float hp, Action dest)
        {
            _hp = hp;
            _dest = dest;

            IsUse = true;
            gameObject.SetActive(true);
        }

        public void getDamage(float dmg)
        {
            _hp -= dmg;

            if (_hp <= 0)
            {
                _dest();
                Destroy();
            }

            _dfm.getText(transform, dmg, dmgTxtType.shield, true);
        }

        private void Destroy()
        {
            IsUse = false;
            gameObject.SetActive(false);
        }
    }
}