using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class SsuddenEnergeCtrl : SsuddenAppearCtrl
    {
        float _att;

        public override void Init(skillBase state)
        {
            skill _skill = (skill)state;
            _att = _skill.att;

            _ani.SetTrigger("start");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                EnemyCtrl ec = collision.GetComponentInParent<EnemyCtrl>();
                if (ec == null)
                {
                    Debug.LogError("없는 놈 : " + collision.name);
                    return;
                }

                _gs.EfMng.makeEff(effAni.electric, transform.position);
                ec.enemyDie();
            }
        }
    }
}