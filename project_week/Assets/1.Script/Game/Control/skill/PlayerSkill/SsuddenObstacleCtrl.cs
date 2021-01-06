using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class SsuddenObstacleCtrl : SsuddenAppearCtrl, IPause
    {
        float _hp;

        public override void Init(skillBase status)
        {
            preInit();
            _skillType = SkillKeyList.IceWall;

            skill _skill = (skill)status;

            _hp = _skill.att;
            transform.localScale = Vector3.one * _skill.size;

            _ani.SetTrigger("icewall");

            StartCoroutine(wallTime());
        }

        // Update is called once per frame
        IEnumerator wallTime()
        {
            float time = 0;
            while (time < 3f)
            {
                time += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            Destroy();
        }

        void getDamaged(float dmg)
        {
            _hp -= dmg;
            if (_hp < 0)
            {
                Destroy();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {         
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                EnemyCtrl ec = collision.gameObject.GetComponent<EnemyCtrl>();
                getDamaged(ec.getDamage);
                ec.enemyDie();
            }
        }
    }
}