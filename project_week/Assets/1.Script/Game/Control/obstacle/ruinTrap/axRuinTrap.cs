using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class axRuinTrap : baseRuinTrap
    {
        /// <summary> 망치들 </summary>
        [SerializeField] Transform[] _axis;

        float _outAngle = 1f;
        float _inAngle = -1.5f;
        float _spd = 1f;

        /// <summary> 함정 생성 초기화 - 추가작업 필요시 </summary>
        protected override void whenFixedInit()
        {
            _att = 8f;
        }

        /// <summary> 함정 재사용 초기화 - 추가작업 필요시 </summary>
        protected override void whenRepeatInit()
        {
            Att = _att * _dmgRate * _increase;
            
            //StartCoroutine(rollingAx());
        }

        /// <summary> 망치 컨트롤 </summary>
        protected override IEnumerator trapPlay()
        {
            float time = 0;
            float chkSpd = 0;

            yield return new WaitForSeconds(1f);

            while (true)
            {
                time = Time.deltaTime;

                if (chkSpd < _spd)
                {
                    chkSpd += time;
                }

                _axis[0].Rotate(Vector3.forward, _outAngle * chkSpd);
                _axis[1].Rotate(Vector3.forward, _inAngle * chkSpd);

                yield return new WaitUntil(() => (_gs.Pause == false));
            }
        }

        /// <summary> 충돌 </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                Vector3 knock = (_player.transform.position - transform.position).normalized;
                _player.getDamaged(Att);
                _player.getKnock(knock, 0.1f, 0.2f);
            }
        }
    }
}