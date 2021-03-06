using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class EnSkillControl : EnSkillBase
    {
        /// <summary> 투사체 타입 </summary>
        [SerializeField] EnShot _shotType;
        public EnShot getType { get { return _shotType; } }

        [SerializeField] protected bool _targeting;
        [SerializeField] protected bool _lookRotate;

        /// <summary> 매니저들 </summary>
        protected GameScene _gs;
        protected PlayerCtrl _player;
        protected effManager _efm;

        /// <summary> 애니메이션 </summary>
        Animator _ani;
        protected Animator Ani
        {
            get 
            {
                if (_ani == null)
                {
                    _ani = GetComponent<Animator>();
                }

                return _ani;
            }
        }

        #region 초기화 세팅

        /// <summary> 최초 초기화 </summary>
        public void Init(GameScene gs, effManager efm)
        {
            _gs = gs;
            _player = _gs.Player;
            _efm = efm;

            _speed = gameValues._defaultProjSpeed * D_enproj.GetEntity(_shotType.ToString()).f_speed;

            whenInit();

            recycleInit();
        }

        /// <summary> 최초 초기화때 추가 </summary>
        protected abstract void whenInit();

        /// <summary> 재사용 초기화 </summary>
        public void recycleInit()
        {
            preInit();

            whenRecycleInit();
        }

        protected abstract void whenRecycleInit();

        public void setDamage(float dmg)
        {
            _dmg = dmg;
        }

        protected abstract void setTarget(Vector3 target, float addAngle = 0f);

        #endregion

        public abstract void operation(Vector3 target, float addAngle = 0f);
        public abstract void operation(float addAngle = 0f);
    }
}