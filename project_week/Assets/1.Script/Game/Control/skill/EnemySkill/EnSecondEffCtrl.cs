using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class EnSecondEffCtrl : EnCurvedEffBase
    {
        List<EnemyCtrl> _enemies;
        public override void prevInit()
        {
            switch (getSkillType)
            {
                case EnShot.banana:
                    _keep = 2f;
                    StartCoroutine(timechk());
                    break;
                case EnShot.scarecrow_shot:
                    _efm.makeEff("crowfire", transform.position);
                    _keep = 2f;
                    StartCoroutine(secondEff());
                    break;
                case EnShot.flower_mine:
                    _keep = 12f;
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
            if (collision.gameObject.tag.Equals("Player"))
            {
                chkCollision(_gs.Player);
            }
        }

        void chkCollision(PlayerCtrl player)
        {
            switch (getSkillType)
            {
                case EnShot.banana:
                    if (_dmg == 0)
                        _gs.DmgfntMng.getText(transform, "빗나감", dmgTxtType.standard, true);
                    else
                        player.getDamaged(_dmg); ;
                    break;
                case EnShot.scarecrow_shot:
                    player.getDamaged(_dmg);
                    _efm.makeEff("lava", player.transform.position);
                    break;
                case EnShot.flower_mine:
                    player.getDamaged(_dmg);
                    _efm.makeEff("Mine", transform.position);
                    Destroy();
                    break;
                default:
                    Debug.LogError("잘못된 입력 : " + getSkillType.ToString() + "/" + gameObject.name);
                    break;
            }
        }
    }
}