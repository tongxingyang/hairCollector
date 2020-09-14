using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace week
{
    public class spikeRuinTrap : baseRuinTrap
    {
        class pair
        {
            public int originX;
            public int originY;

            public int _x;
            public int _y;

            public pair(int x, int y)
            {
                originX = x;
                originY = y;

                _x = -6;
                _y = -6;
            }

            public void set(int x, int y)
            {
                _x = x;
                _y = y;
            }


            public static bool operator ==(pair p0, pair p1)
            {
                return (p0._x == p1._x && p0._y == p1._y);
            }

            public static bool operator !=(pair p0, pair p1)
            {
                return (p0._x != p1._x || p0._y != p1._y);
            }
        }

        [SerializeField] Animator[] _spikes;

        pair[] _pos;
        readonly int _trapCnt = 16;

        protected override void whenFixedInit()
        {
            _pos = new pair[_trapCnt];

            _pos[0] = new pair(-1, 2);
            _pos[1] = new pair(0, 2);
            _pos[2] = new pair(1, 2);
            _pos[3] = new pair(2, 2);
            _pos[4] = new pair(2, 1);
            _pos[5] = new pair(2, 0);
            _pos[6] = new pair(2, -1);
            _pos[7] = new pair(2, -2);
            _pos[8] = new pair(1, -2);
            _pos[9] = new pair(0, -2);
            _pos[10] = new pair(-1, -2);
            _pos[11] = new pair(-2, -1);
            _pos[12] = new pair(-2, 0);
            _pos[13] = new pair(-2, 1);
            _pos[14] = new pair(-2, 1);
            _pos[15] = new pair(-2, 2);
        }

        protected override void whenRepeatInit()
        {
            for (int i = 0; i < _trapCnt; i++)
            {
                _spikes[i].transform.position = transform.position + new Vector3(_pos[i].originX * 0.6f, _pos[i].originY * 0.6f);
                _spikes[i].SetTrigger("idle");
            }
        }

        public override void Destroy()
        {

        }

        public override void operate()
        {
            StartCoroutine(spikeMove());
        }

        IEnumerator spikeMove()
        {
            float time = 0;
            int x, y;

            while (true)
            {
                time += Time.deltaTime;

                if (time > 0.8f)
                {
                    time = 0;

                    for (int i = 0; i < _trapCnt; i++)
                    {
                        _pos[i].set(-6, -6);
                    }

                    for (int i = 0; i < _trapCnt; i++)
                    {
                        x = Random.Range(-5, 6);
                        y = Random.Range(-5, 6);

                        if (x * x + y * y < 2)
                        {
                            i--;
                            continue;
                        }

                        _pos[i].set(x, y);

                        for (int j = 0; j < _trapCnt; j++)
                        {
                            if (i == j) continue;

                            if (_pos[i] == _pos[j])
                            {
                                i--;
                                break;
                            }
                        }
                    }

                    for (int i = 0; i < _trapCnt; i++)
                    {
                        _spikes[i].transform.position = transform.position + new Vector3(_pos[i]._x * 0.6f, _pos[i]._y * 0.6f);
                        _spikes[i].SetTrigger("spike");
                    }                    
                }

                yield return new WaitUntil(() => (_gs.Pause == false && _onTrap));
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                Vector3 knock = (_player.transform.position - transform.position).normalized;
                _player.getDamaged(15f);
                _player.getKnock(knock, 0.1f, 0.2f);
            }
        }

        public override void onPause(bool bl)
        {
            for (int i = 0; i < _trapCnt; i++)
            {
                _spikes[i].speed = (bl) ? 0f : 1f;
            }
        }
    }
}