using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class shieldCollider : MonoBehaviour
    {
        SkillKeyList _skill;

        GameScene _gs;
        ShieldCtrl _shield = null;
        SpriteRenderer _render;

        public SkillKeyList Skill { get => _skill; set => _skill = value; }
        public SpriteRenderer Render { get => _render; set => _render = value; }

        public void Init(ShieldCtrl shield, GameScene gs, SkillKeyList sk)
        {
            if (_shield == null)
            {
                _gs = gs;
                _shield = shield;
                _render = GetComponent<SpriteRenderer>();
            }

            _skill = sk;
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.tag.Equals("Enemy") || coll.gameObject.tag.Equals("Boss"))
            {

                IDamage id = coll.GetComponentInParent<IDamage>();
                _shield.onTriggerEnemy(id, _skill);
            }
        }
    }
}