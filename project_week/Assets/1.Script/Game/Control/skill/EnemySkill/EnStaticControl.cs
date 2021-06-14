using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class EnStaticControl : EnSkillControl
    {
        [SerializeField] GameObject[] _circles;
        [SerializeField] Transform[] _fires0;
        [SerializeField] Transform[] _fires1;
        Vector3 _home;
        Vector3[] _vec;
        protected override void whenInit()
        {
            Transform[] tt = _circles[0].GetComponentsInChildren<Transform>();

            _vec = new Vector3[8];
            for (int i = 0; i < 8; i++)
            {
                _vec[i] = _fires0[i].position;
            }
        }

        protected override void whenRecycleInit()
        {
        }

        public override void operation(Vector3 target, float addAngle = 0)
        {
            operation(addAngle);
        }

        public override void operation(float addAngle = 0)
        {
            _home = transform.position;

            StartCoroutine(play());
        }

        protected override void setTarget(Vector3 target, float addAngle = 0)
        {
        }

        IEnumerator play()
        {
            float time = 0;
            float sz = 0;
            float term = 0.3f;
            float rate = 1 / term;

            _circles[0].SetActive(true);
            _circles[1].SetActive(false);

            float max = 1f;
            for (int i = 0; i < 8; i++)
            {
                _fires0[i].position = _home + _vec[i];
            }
            while (time < 1f)
            {
                time += Time.deltaTime * rate;

                for (int i = 0; i < 8; i++)
                {
                    _fires0[i].localScale = new Vector3(max * time, max * time, 1f);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            _circles[1].SetActive(true);
            float dst = 1f;
            int repeat = 10;

            for (int p = 0; p < repeat; p++)
            {
                sz = max;
                max += 0.05f;
                time = 0;
                dst += 1f;

                for (int i = 0; i < 8; i++)
                {
                    if (p % 2 == 0)
                    {
                        _fires1[i].position = _home + _vec[i] * dst;
                    }
                    else
                    {
                        _fires0[i].position = _home + _vec[i] * dst;
                    }
                }
                while (time < 1f)
                {
                    time += Time.deltaTime * rate * 1.1f;

                    for (int i = 0; i < 8; i++)
                    {
                        if (p % 2 == 0)
                        {
                            _fires0[i].localScale = new Vector3(sz * (1 - time), sz * (1 - time), 1f);
                            _fires1[i].localScale = new Vector3(max * time, max * time, 1f);
                        }
                        else
                        {
                            _fires1[i].localScale = new Vector3(sz * (1 - time), sz * (1 - time), 1f);
                            _fires0[i].localScale = new Vector3(max * time, max * time, 1f);
                        }
                    }

                    yield return new WaitUntil(() => _gs.Pause == false);
                }

                SoundManager.instance.PlaySFX(SFX.crowfire);
            }

            sz = max;
            time = 0;
            while (time < 1f)
            {
                time += Time.deltaTime * rate;

                for (int i = 0; i < 8; i++)
                {
                    if (repeat % 2 == 0)
                    {
                        _fires0[i].localScale = new Vector3(sz * (1 - time), sz * (1 - time), 1f);
                    }
                    else
                    {
                        _fires1[i].localScale = new Vector3(sz * (1 - time), sz * (1 - time), 1f);
                    }
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            _circles[0].SetActive(false);

            Destroy();
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(_dmg);
            }
        }

        public override void onPause(bool bl)
        {
        }
    }
}