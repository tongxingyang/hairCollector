using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class BaseProjControl : poolingObject, IPause
    {
        [SerializeField] SkillKeyList _skillType;
        public SkillKeyList getSkillType { get => _skillType; protected set => _skillType = value; }
        public int skillLvl { get; set; }

        protected float _dmg;
        protected float _speed = 4f;
        protected float _keep;
        protected effManager _efm;
        protected PlayerCtrl _player;
        protected GameScene _gs;

        public void fixedInit(GameScene gs, effManager efm)
        {
            _gs = gs;
            _player = gs.Player;
            _efm = efm;

            whenFixedInit();
        }
        protected abstract void whenFixedInit();
        public virtual void setTarget(Vector3 target, float addAngle = 0f, bool rand = false)
        {
            Vector3 _direct = target - transform.position;

            float angle = Mathf.Atan2(_direct.x, _direct.y) * Mathf.Rad2Deg;
            float add = (rand) ? Random.Range(-addAngle, addAngle) : addAngle;
            transform.rotation = Quaternion.AngleAxis(angle + add, Vector3.back);
        }
        public virtual void repeatInit(float dmg, float size = 1f, float speed = 1f, float keep = 1f) { }

        protected override void Destroy()
        {
            preDestroy();

            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        public virtual void onPause(bool bl)
        {
        }
    }
}