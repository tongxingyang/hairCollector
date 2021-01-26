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

        [SerializeField] GameObject[] _marks;
        SpriteRenderer[] _sprites;
        CircleCollider2D[] _cols;

        SkillKeyList _skillType;
        float _dmg;
        float _keep;
        float _size;

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

        private void Awake()
        {
            _sprites = new SpriteRenderer[3];
            _cols = new CircleCollider2D[3];
            for (int i = 0; i < 3; i++)
            {
                _sprites[i] = _marks[i].GetComponent<SpriteRenderer>();
                _cols[i] = _marks[i].GetComponent<CircleCollider2D>();
            }
        }

        /// <summary> 재사용 </summary>
        public skillMarkCtrl fixedInit(GameScene gs, playerSkillManager psm)
        {
            _gs = gs;
            _efm = gs.EfMng;
            _psm = psm;

            return this;
        }

        /// <summary> 재사용 </summary>
        public skillMarkCtrl repeatInit(SkillKeyList sk, float dmg)
        {
            _skillType = sk;
            _dmg = dmg;

            _marks[1].SetActive(false);
            _marks[2].SetActive(false);
            _cols[0].isTrigger = true;
            for (int i = 0; i < 3; i++)
            {
                _marks[i].transform.localPosition = Vector3.zero;
                _marks[i].transform.rotation = Quaternion.Euler(Vector3.zero);
                _marks[i].transform.localScale = Vector3.one;

                _sprites[i].color = Color.white * 0f;
                _cols[i].enabled = false;
            }

            Ani.enabled = true;

            preInit();

            return this;
        }

        public skillMarkCtrl setKeep(float keep)
        {
            _keep = keep;
            return this;
        }

        public skillMarkCtrl setSize(float size)
        {
            _size = size;
            return this;
        }

        public void play()
        {
            _targetEnemy = (_skillType == SkillKeyList.Present) ? false : true;
            switch (_skillType)
            {
                case SkillKeyList.Iceball:  // 지속시간 변경불가
                case SkillKeyList.Hail:
                case SkillKeyList.Meteor:
                case SkillKeyList.Poison:
                case SkillKeyList.Lightning:
                case SkillKeyList.Circle:   
                case SkillKeyList.Thuncall:
                case SkillKeyList.IceWall:
                case SkillKeyList.Iceberg:
                    Ani.SetTrigger(_skillType.ToString());
                    break;
                case SkillKeyList.SnowBomb:     // 지속시간 변경가능
                case SkillKeyList.SnowMissile:
                case SkillKeyList.PoisonBomb:
                    StartCoroutine(timeNeff());
                    break;
                case SkillKeyList.Mine:                  
                case SkillKeyList.Present:
                    StartCoroutine(timechk());
                    break;
                default:
                    Debug.LogError("잘못된 요청 : " + _skillType);
                    break;
            }
        }

        public override void Destroy()
        {
            preDestroy();
        }

        #endregion

        /// <summary> 시간초 체크 있음 </summary>
        IEnumerator timechk()
        {
            float time = 0;
            Ani.SetTrigger(_skillType.ToString());

            while (time < _keep)
            {
                time += Time.deltaTime;
                yield return new WaitUntil(() => _gs.Pause == false);
            }

            EndAni();
        }

        /// <summary> 시간초 체크 있음 </summary>
        IEnumerator timeNeff()
        {
            float time = 0;
            Ani.SetTrigger(_skillType.ToString());

            while (time < _keep)
            {
                time += Time.deltaTime;
                yield return new WaitUntil(() => _gs.Pause == false);
            }

            Ani.enabled = false;

            time = 0;
            _cols[0].enabled = false;
            _sprites[0].sortingOrder = 3;

            while (time < 0.5f)
            {
                time += Time.deltaTime;
                _sprites[0].color = new Color(1f, 1f, 1f, 1f - time / 0.5f);

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            // Ani.SetTrigger("End");
            EndAni();
        }

        public void getEff()
        {
            _efm.makeEff(effAni.hail, transform.position);
        }

        ///// <summary> 한번 </summary>
        //IEnumerator oneEff(float open, float end)
        //{
        //    float time = 0;
        //    while (time < open+_keep+end)
        //    {
        //        time += Time.deltaTime;

        //        if (time < open)
        //        {
        //            _sprite.color = Color.white * (time / open);
        //            _col.enabled = true;
        //        }
        //        else if (time < open + _keep)
        //        {
        //            _sprite.color = Color.white;
                    
        //        }
        //        else if (time < open + _keep + end)
        //        {
        //            _col.enabled = false;
        //            _sprite.color = Color.white * (1 - ((time - open - _keep) / end));
        //        }
        //        else
        //        {
        //            _sprite.color = Color.white * 0f;
        //        }

        //        yield return new WaitUntil(() => _gs.Pause == false);
        //    }

        //    Destroy();
        //}

        ///// <summary>  </summary>
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

        public void EndAni()
        {
            Destroy();
        }

        #region [ 충돌 ]

        EnemyCtrl ec;
        private void OnTriggerEnter2D(Collider2D coll)
        {
            // Debug.Log(coll.tag);
            if (_targetEnemy)
            {
                if (coll.tag.Equals("Enemy") || coll.tag.Equals("Boss"))
                {
                    ec = coll.gameObject.GetComponentInParent<EnemyCtrl>();
                    if (ec == null)
                    {
                        Debug.LogError(coll.name);
                        return;
                    }

                    chkCollision(ec);
                }
            }
            else
            {
                if (coll.tag.Equals("Player"))
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
                case SkillKeyList.Iceball:  // 닿으면 바로 데미지
                case SkillKeyList.Hail:
                case SkillKeyList.Meteor:
                case SkillKeyList.Lightning:
                case SkillKeyList.SnowBomb:
                case SkillKeyList.SnowMissile:
                case SkillKeyList.Circle:
                case SkillKeyList.IceWall:
                case SkillKeyList.Iceberg:
                    ec.getDamaged(_dmg);
                    break;
                case SkillKeyList.Poison:   // 데미지 없고 중독
                    ec.DotDmg.setDotDmg(_dmg, _keep);
                    break;
                case SkillKeyList.Thuncall:
                    {
                        skillMarkCtrl _stamp = _psm.getStamp();
                        _stamp.transform.position = ec.transform.position;

                        _stamp.repeatInit(SkillKeyList.Lightning, _dmg)
                            .setKeep(_keep)
                            .play();
                    }
                    break;              
                case SkillKeyList.PoisonBomb:   // 데미지 + 중독
                    ec.getDamaged(_dmg); 
                    ec.DotDmg.setDotDmg(_dmg, _keep);
                    break;
                case SkillKeyList.Present:
                    //
                    break;
                case SkillKeyList.Mine:
                    ec.getDamaged(_dmg);
                    _efm.makeEff(effAni.mine, transform.position);
                    Destroy();
                    break;
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