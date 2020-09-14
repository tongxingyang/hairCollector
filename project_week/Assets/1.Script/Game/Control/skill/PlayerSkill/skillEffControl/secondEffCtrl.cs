using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;

namespace week
{
    public class secondEffCtrl : bombEffBase
    {
        List<EnemyCtrl> _enemies;
        public override void prevInit()
        {
            switch (getSkillType)
            {
                case getSkillList.poison:
                case getSkillList.snowbomb:                    
                    StartCoroutine(secondEff());
                    break;
                case getSkillList.blackhole:
                    _enemies = new List<EnemyCtrl>();
                    StartCoroutine(tickEff());
                    break;
                case getSkillList.mine:
                    StartCoroutine(timechk());
                    break;
                default:
                    break;
            }                  
        }

        IEnumerator secondEff()
        {
            Ani.SetTrigger("start");

            float time = 0;
            while (time < _keep)
            {
                time += Time.deltaTime;
                yield return new WaitUntil(() => _gs.Pause == false);
            }

            Ani.SetTrigger("end");
            time = 0;
            while (time < 1f)
            {
                time += Time.deltaTime;
                yield return new WaitUntil(() => _gs.Pause == false);
            }

            Destroy();
        }

        IEnumerator tickEff()
        {
            Ani.SetTrigger("start");

            float time = 0;
            float tick = 0;
            EnemyCtrl ec;
            while (time < _keep)
            {
                time += Time.deltaTime;
                tick += Time.deltaTime;

                if (tick > 0.3f)
                {
                    tick = 0;
                    for (int i = 0; i < _enemies.Count; i++)
                    {
                        ec = _enemies[i];
                        ec.getKnock((transform.position - ec.transform.position).normalized, 0.1f, 0.25f);

                        if (ec.getDamaged(_dmg))
                        {
                            _enemies.Remove(ec);
                            i--;
                        }
                    }
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            Ani.SetTrigger("end");
            time = 0;
            while (time < 1f)
            {
                time += Time.deltaTime;
                yield return new WaitUntil(() => _gs.Pause == false);
            }

            Destroy();
        }

        IEnumerator timechk()
        {
            float time = 0;
            while (time < _keep)
            {
                time += Time.deltaTime;
                yield return new WaitUntil(() => _gs.Pause == false);
            }

            Destroy();
        }

        void Destroy()
        {
            gameObject.SetActive(false);
            _del();
        }

        EnemyCtrl ec;
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy") || collision.gameObject.tag.Equals("Boss")) 
            {
                ec = collision.gameObject.GetComponentInParent<EnemyCtrl>();
                if(ec == null)
                {
                    Debug.LogError(collision.name);
                    return;    
                }

                chkCollision(ec);
            }
        }

        void chkCollision(EnemyCtrl ec)
        {
            switch (getSkillType)
            {
                case getSkillList.snowbomb:
                    ec.getDamaged(_dmg);
                    break;
                case getSkillList.poison:
                    ec.setBuff(eDeBuff.dotDem, false, _keep, _dmg);
                    break;
                case getSkillList.mine:
                    ec.getDamaged(_dmg);
                    _efm.makeEff(effAni.mine, transform.position);
                    Destroy();
                    break;
                case getSkillList.blackhole:
                    _enemies.Add(ec);
                    break;
                default:
                    Debug.LogError("잘못된 입력 : " + getSkillType.ToString() + "/" + gameObject.name);
                    break;
            }
        }
    }
}