using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class SsuddenAppearCtrl : poolingObject, IPause
    {
        [SerializeField] protected Animator _ani;

        [SerializeField]  protected SkillKeyList _skillType;
        public SkillKeyList getSkillType { get => _skillType; }
        protected GameScene _gs;

        public void setting(GameScene gs)
        {
            _gs = gs;
        }

        public void select()
        {
            _isUse = true;
        }

        public abstract void Init(skillBase state);


        public override void onPause(bool pause)
        {
            _ani.speed = (pause) ? 0 : 1f;
        }

        public override void Destroy()
        {
            _isUse = false;
            gameObject.SetActive(false);
        }
    }
}