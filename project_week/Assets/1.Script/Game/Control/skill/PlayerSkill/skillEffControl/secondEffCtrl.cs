using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;

namespace week
{
    public class secondEffCtrl : bombEffBase
    {
        //bool _targetEnemy = true;
        //List<EnemyCtrl> _enemies;
        //public override void prevInit()
        //{
        //    _targetEnemy = true;
        //    switch (getSkillType)
        //    {
        //        case SkillKeyList.poison:
        //        case SkillKeyList.snowbomb:                    
        //            StartCoroutine(secondEff());
        //            break;
        //        case SkillKeyList.blackhole:
        //            _enemies = new List<EnemyCtrl>();
        //            StartCoroutine(tickEff());
        //            break;
        //        case SkillKeyList.mine:
        //            StartCoroutine(timechk());
        //            break;
        //        case SkillKeyList.present:
        //            _targetEnemy = false;
        //            StartCoroutine(timechk());
        //            break;
        //        default:
        //            break;
        //    }                  
        //}

        //IEnumerator secondEff()
        //{
        //    Ani.SetTrigger("start");

        //    float time = 0;
        //    while (time < _keep)
        //    {
        //        time += Time.deltaTime;
        //        yield return new WaitUntil(() => _gs.Pause == false);
        //    }

        //    Ani.SetTrigger("end");
        //    time = 0;
        //    while (time < 1f)
        //    {
        //        time += Time.deltaTime;
        //        yield return new WaitUntil(() => _gs.Pause == false);
        //    }

        //    Destroy();
        //}

        //IEnumerator tickEff()
        //{
        //    Ani.SetTrigger("start");

        //    float time = 0;
        //    float tick = 0;
        //    EnemyCtrl ec;
        //    while (time < _keep)
        //    {
        //        time += Time.deltaTime;
        //        tick += Time.deltaTime;

        //        if (tick > 0.3f)
        //        {
        //            tick = 0;
        //            for (int i = 0; i < _enemies.Count; i++)
        //            {
        //                ec = _enemies[i];
        //                ec.getKnock((transform.position - ec.transform.position).normalized, 0.1f, 0.25f);

        //                ec.getDamaged(_dmg);
        //                if (ec.IsUse == false)
        //                {
        //                    _enemies.Remove(ec);
        //                    i--;
        //                }
        //            }
        //        }

        //        yield return new WaitUntil(() => _gs.Pause == false);
        //    }

        //    Ani.SetTrigger("end");
        //    time = 0;
        //    while (time < 1f)
        //    {
        //        time += Time.deltaTime;
        //        yield return new WaitUntil(() => _gs.Pause == false);
        //    }

        //    Destroy();
        //}

        //IEnumerator timechk()
        //{
        //    float time = 0;
        //    while (time < _keep)
        //    {
        //        time += Time.deltaTime;
        //        yield return new WaitUntil(() => _gs.Pause == false);
        //    }

        //    Destroy();
        //}

        //void Destroy()
        //{
        //    gameObject.SetActive(false);
        //    _del();
        //}

        //EnemyCtrl ec;
        //void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (_targetEnemy)
        //    {
        //        if (collision.gameObject.tag.Equals("Enemy") || collision.gameObject.tag.Equals("Boss"))
        //        {
        //            ec = collision.gameObject.GetComponentInParent<EnemyCtrl>();
        //            if (ec == null)
        //            {
        //                Debug.LogError(collision.name);
        //                return;
        //            }

        //            chkCollision(ec);
        //        }
        //    }
        //    else
        //    {
        //        if (collision.gameObject.tag.Equals("Player"))
        //        {
        //            int num = Random.Range(0, 10);

        //            switch (num)
        //            {
        //                case 0:
        //                    _gs.Player.getAddStat(1, 0);
        //                    break;
        //                case 9:
        //                    _gs.Player.getAddStat(0, 0.001f);
        //                    break;
        //                default:
        //                    _gs.Player.getHealed(_dmg);
        //                    break;
        //            }

        //            Destroy();
        //        }
        //    }
        //}

        //void chkCollision(EnemyCtrl ec)
        //{
        //    switch (getSkillType)
        //    {
        //        case SkillKeyList.snowbomb:
        //            ec.getDamaged(_dmg);
        //            break;
        //        case SkillKeyList.poison:
        //            ec.DotDmg.setDotDmg(_dmg, _keep);
        //            break;
        //        case SkillKeyList.mine:
        //            ec.getDamaged(_dmg);
        //            _efm.makeEff(effAni.mine, transform.position);
        //            Destroy();
        //            break;
        //        case SkillKeyList.blackhole:
        //            _enemies.Add(ec);
        //            break;
        //        default:
        //            Debug.LogError("잘못된 입력 : " + getSkillType.ToString() + "/" + gameObject.name);
        //            break;
        //    }
        //}
    }
}