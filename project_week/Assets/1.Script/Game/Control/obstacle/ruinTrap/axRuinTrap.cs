using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class axRuinTrap : baseRuinTrap
    {
        [SerializeField] Transform[] _axis;

        float _angle = 4f;
        float _spd = 1.5f;

        protected override void whenFixedInit()
        {
        }
        protected override void whenRepeatInit()
        {
        }

        public override void operate()
        {
            _spd = 1f;
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

                for (int i = 0; i < _axis.Length; i++)
                {
                    _axis[i].Rotate(Vector3.forward, _angle * chkSpd);     
                }

                yield return new WaitUntil(() => (_gs.Pause == false && _onTrap));
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