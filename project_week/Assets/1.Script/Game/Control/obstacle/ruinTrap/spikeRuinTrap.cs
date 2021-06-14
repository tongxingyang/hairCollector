using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace week
{
    public class spikeRuinTrap : baseRuinTrap
    {
        /// <summary> 스파이크 위치 클래스 </summary>
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

                _x = -10;
                _y = -10;
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
        Vector3[] _originPos;
        int _trapCnt;

        /// <summary> 함정 생성 초기화 - 추가작업 필요시 </summary>
        protected override void whenFixedInit()
        {
            _trapCnt = _spikes.Length;

            _pos = new pair[_trapCnt];
            _originPos = new Vector3[_trapCnt];

            for (int i = 0; i < _trapCnt; i++)
            {
                _originPos[i] = _spikes[i].transform.localPosition;
                _pos[i] = new pair(0, 0);
            }

            _att = 15f;
        }

        /// <summary> 함정 재사용 초기화 - 추가작업 필요시 </summary>
        protected override void whenRepeatInit()
        {
            for (int i = 0; i < _trapCnt; i++)
            {
                _spikes[i].transform.localPosition = _originPos[i];
                _spikes[i].SetTrigger("idle");
            }

            Att = _att * _dmgRate * _increase;

            //if (gameObject.activeSelf)
            //    StartCoroutine(spikeMove());
        }

        /// <summary> 스파이크 컨트롤 </summary>
        protected override IEnumerator trapPlay()
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
                        _pos[i].set(-10, -10);
                    }

                    for (int i = 0; i < _trapCnt; i++)
                    {
                        x = Random.Range(-10, 11);
                        y = Random.Range(-10, 11);

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

                yield return new WaitUntil(() => (_gs.Pause == false));
            }
        }

        /// <summary> 충돌 </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                Vector3 knock = (transform.position - _player.transform.position).normalized;
                _player.getDamaged(Att);
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