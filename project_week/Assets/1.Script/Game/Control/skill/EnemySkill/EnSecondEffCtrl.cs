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
                case EnShot.flower_mine:
                    _keep = 12f;
                    StartCoroutine(timechk());
                    break;
                default:
                    break;
            }
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
            chkCollision(_gs.Player);
        }

        void chkCollision(PlayerCtrl player)
        {
            switch (getSkillType)
            {
                case EnShot.flower_mine:
                    player.getDamaged(_dmg);
                    _efm.makeEff(effAni.mine, transform.position);
                    Destroy();
                    break;
                default:
                    Debug.LogError("잘못된 입력 : " + getSkillType.ToString() + "/" + gameObject.name);
                    break;
            }
        }
    }
}