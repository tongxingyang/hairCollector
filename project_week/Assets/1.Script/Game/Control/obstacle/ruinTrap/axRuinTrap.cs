using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class axRuinTrap : baseRuinTrap
    {
        [SerializeField] Transform[] _axis;

        float _outAngle = 1f;
        float _inAngle = -1.5f;
        float _spd = 1f;

        protected override void whenFixedInit()
        {
            _att = 8f;
        }

        protected override void whenRepeatInit()
        {
            Att = _att * Mathf.Pow(1.2f, _clm.Day);
        }

        public override void operate()
        {
            StartCoroutine(rollingAx());
        }

        IEnumerator rollingAx()
        {
            float time = 0;
            float chkSpd = 0;

            yield return new WaitForSeconds(1f);

            while (true)
            {
                time = Time.deltaTime;

                if (_onTrap)
                {
                    if (chkSpd < _spd)
                    {
                        chkSpd += time;
                    }
                }
                else
                {
                    if (chkSpd > 0f)
                    {
                        chkSpd -= time;
                    }
                }

                _axis[0].Rotate(Vector3.forward, _outAngle * chkSpd);
                _axis[1].Rotate(Vector3.forward, _inAngle * chkSpd);

                //for (int i = 0; i < _axis.Length; i++)
                //{
                //    _axis[i].Rotate(Vector3.forward, _angle * chkSpd);     
                //}

                yield return new WaitUntil(() => (_gs.Pause == false && _onTrap));
            }
        }
        
        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (collision.tag.Equals("Player"))
        //    {
        //        Vector3 knock = (_player.transform.position - transform.position).normalized;
        //        _player.getDamaged(Att);
        //        _player.getKnock(knock, 0.1f, 0.2f);
        //    }
        //}
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                Vector3 knock = (_player.transform.position - transform.position).normalized;
                _player.getDamaged(Att);
                _player.getKnock(knock, 0.1f, 0.2f);
            }
        }

        public override void onPause(bool bl)
        {
        }

        public override void Destroy()
        {
        }
    }
}