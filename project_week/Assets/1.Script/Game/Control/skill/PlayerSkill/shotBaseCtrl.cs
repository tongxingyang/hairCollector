using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class shotBaseCtrl : poolingObject
    {
        // 투사체 타입
        protected SkillKeyList _skillType;

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

        #endregion
    }
}