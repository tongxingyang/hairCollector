using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class shotBaseCtrl : poolingObject, IPause
    {
        // 투사체 타입
        protected SkillKeyList _skillType;
        protected ShotList _bulletType;

        #region [ references ]

        protected GameScene _gs;
        protected PlayerCtrl _player;
        protected effManager _efm;

        #endregion

        #region [ launch status ]

        protected float _dmg;
        protected float _speed = 4f;
        protected float _keep;
        protected float _size;

        protected attackData _aData = new attackData();

        #endregion

        protected void getEff()
        {
            string str = D_skill.GetEntity(_skillType.ToString()).f_eff;
            _efm.makeEff(str, transform.position);
        }

        public virtual void onPause(bool bl) { }
    }
}