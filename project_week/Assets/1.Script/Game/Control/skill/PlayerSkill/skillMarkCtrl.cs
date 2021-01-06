using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class skillMarkCtrl : poolingObject
    {
        #region [ references ]

        protected GameScene _gs;
        protected effManager _efm;
        protected playerSkillManager _psm;

        #endregion

        [SerializeField] SpriteRenderer _sprite;
        [SerializeField] Collider2D _col;
        SkillKeyList _skillType;
        float _dmg;
        float _keep;

        bool _targetEnemy = true;

        protected Animator _ani;
        protected Animator Ani
        {
            get
            {
                if (_ani == null)
                {
                    _ani = GetComponent<Animator>();
                }

                return _ani;
            }
        }

        #region [ Init ]

        /// <summary> 재사용 </summary>
        public skillMarkCtrl fixedInit(GameScene gs, playerSkillManager psm)
        {
            _gs = gs;
            _efm = gs.EfMng;
            _psm = psm;

            return this;
        }

        /// <summary> 재사용 </summary>
        public skillMarkCtrl repeatInit(RangeSkillCtrl rsc)
        {
            _skillType = rsc.SkillType;
            _dmg = rsc.Damage;
            _keep = rsc.Keep;

            // _sprite.sprite = 
            _sprite.color = Color.white * 0f;
            _col.enabled = false;

            gameObject.SetActive(true);

            return this;
        }

        public void play()
        {
            _targetEnemy = (_skillType == SkillKeyList.present) ? false : true;
            switch (_skillType)
            {
                case SkillKeyList.Iceball:  // 한번
                case SkillKeyList.Meteor:
                case SkillKeyList.SnowBomb:
                case SkillKeyList.Crevasse:
                    StartCoroutine(oneEff(0.5f, 0.5f));
                    break;
                case SkillKeyList.Circle:   // 각각 효과
                case SkillKeyList.thuncall:
                    break;                
                case SkillKeyList.sulfurous:// 반복데미지
                    StartCoroutine(secondEff());
                    break;
                case SkillKeyList.Mine:     // 상호작용 있음, 시간체크
                case SkillKeyList.present:
                    _col.enabled = true;
                    StartCoroutine(timechk());
                    break;
                case SkillKeyList.Hail:     // only 자국 상호작용 없음
                case SkillKeyList.Poison:
                case SkillKeyList.Lightning:
                    _col.enabled = false;
                    StartCoroutine(timechk());
                    break;
                default:
                    break;
            }
        }

        public override void Destroy()
        {
            gameObject.SetActive(false);
        }

        #endregion

        /// <summary> 한번 </summary>
        IEnumerator oneEff(float open, float end)
        {
            float time = 0;
            while (time < open+_keep+end)
            {
                time += Time.deltaTime;

                if (time < open)
                {
                    _sprite.color = Color.white * (time / open);
                    _col.enabled = true;
                }
                else if (time < open + _keep)
                {
                    _sprite.color = Color.white;
                    
                }
                else if (time < open + _keep + end)
                {
                    _col.enabled = false;
                    _sprite.color = Color.white * (1 - ((time - open - _keep) / end));
                }
                else
                {
                    _sprite.color = Color.white * 0f;
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            Destroy();
        }

        /// <summary>  </summary>
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

        /// <summary> 시간초만 있음 </summary>
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

        #region [ 충돌 ]

        EnemyCtrl ec;
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (_targetEnemy)
            {
                if (collision.gameObject.tag.Equals("Enemy") || collision.gameObject.tag.Equals("Boss"))
                {
                    ec = collision.gameObject.GetComponentInParent<EnemyCtrl>();
                    if (ec == null)
                    {
                        Debug.LogError(collision.name);
                        return;
                    }

                    chkCollision(ec);
                }
            }
            else
            {
                if (collision.gameObject.tag.Equals("Player"))
                {
                    int num = UnityEngine.Random.Range(0, 10);

                    switch (num)
                    {
                        case 0:
                            _gs.Player.getAddStat(1, 0);
                            break;
                        case 9:
                            _gs.Player.getAddStat(0, 0.001f);
                            break;
                        default:
                            _gs.Player.getHealed(_dmg);
                            break;
                    }

                    Destroy();
                }
            }
        }

        void chkCollision(EnemyCtrl ec)
        {
            switch (_skillType)
            {
                case SkillKeyList.SnowBomb:
                    ec.getDamaged(_dmg);
                    break;
                case SkillKeyList.Poison:
                    ec.DotDmg.setDotDmg(_dmg, _keep);
                    break;
                case SkillKeyList.Mine:
                    ec.getDamaged(_dmg);
                    _efm.makeEff(effAni.mine, transform.position);
                    Destroy();
                    break;
                //case SkillKeyList.blackhole:
                //    _enemies.Add(ec);
                //    break;
                default:
                    Debug.LogError("잘못된 입력 : " + _skillType.ToString() + "/" + gameObject.name);
                    break;
            }
        }

        #endregion

        public override void onPause(bool bl)
        {
            if (Ani != null)
            {
                Ani.speed = (bl) ? 0 : 1f;
            }
        }
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
    }
}