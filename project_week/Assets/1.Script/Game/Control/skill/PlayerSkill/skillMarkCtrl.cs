using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class skillMarkCtrl : poolingObject, IPause
    {
        #region [ references ]

        protected GameScene _gs;
        protected effManager _efm;
        protected playerSkillManager _psm;

        #endregion

        [SerializeField] GameObject[] _marks;
        SpriteRenderer _sprites;
        CapsuleCollider2D _cols;

        SkillKeyList _skillType;
        float _dmg, _dmg2;
        float _keep;
        float _size;
        float _dotRate = 1f;

        attackData _adata = new attackData();

        bool _targetEnemy = true;

        Action _midEff;
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
            _sprites = _marks[0].GetComponent<SpriteRenderer>();
            _cols = _marks[0].GetComponent<CapsuleCollider2D>();
            // _sprites = new SpriteRenderer[3];

            //_cols = new CircleCollider2D[3];
            //for (int i = 0; i < 3; i++)
            //{
            //    _sprites[i] = _marks[i].GetComponent<SpriteRenderer>();
            //    _cols[i] = _marks[i].GetComponent<CircleCollider2D>();
            //}
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
        public skillMarkCtrl repeatInit(SkillKeyList skilln, float dmg, float dmg2 = 0)
        {
            _skillType = skilln;
            _dmg = dmg;
            _dmg2 = dmg2;

            _marks[1].SetActive(false);
            _marks[2].SetActive(false);

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
        public skillMarkCtrl setDotRate(float rate)
        {
            _dotRate = rate;
            return this;
        }
        public skillMarkCtrl setMidEff(Action midEff)
        {
            _midEff = midEff;
            return this;
        }

        public void play()
        {
            _targetEnemy = (_skillType == SkillKeyList.Present) ? false : true;
            switch (_skillType)
            {
                case SkillKeyList.IceBall:  // 지속시간 변경불가
                case SkillKeyList.Hail:
                case SkillKeyList.Meteor: 
                case SkillKeyList.IceWall:
                case SkillKeyList.Shard:
                    Ani.SetTrigger(_skillType.ToString());
                    break;
                case SkillKeyList.Lightning:
                    SoundManager.instance.PlaySFX(SFX.lightning);
                    Ani.SetTrigger(_skillType.ToString());
                    break;
                case SkillKeyList.Poison:
                    SoundManager.instance.PlaySFX(SFX.poison);
                    Ani.SetTrigger(_skillType.ToString());
                    break;
                case SkillKeyList.Circle:
                case SkillKeyList.Thuncall:
                    SoundManager.instance.PlaySFX(SFX.magic);
                    Ani.SetTrigger(_skillType.ToString());
                    break;
                case SkillKeyList.SinkHole: 
                case SkillKeyList.Vespene:
                    Ani.SetTrigger(_skillType.ToString());
                    break;
                case SkillKeyList.Crevasse:
                    SoundManager.instance.PlaySFX(SFX.crevasse);
                    Ani.SetTrigger(_skillType.ToString());
                    _gs.Player.cameraShake();
                    break;
                case SkillKeyList.IceBerg:
                    SoundManager.instance.PlaySFX(SFX.iceberg);
                    Ani.SetTrigger(_skillType.ToString());
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

        protected override void Destroy()
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

        public void getEff()
        {
            string str = D_skill.GetEntity(_skillType.ToString()).f_eff;

            if (string.IsNullOrWhiteSpace(str) == false)
            {
                _efm.makeEff(str, transform.position);
            }

            _midEff?.Invoke();
        }

        /// <summary> 파편 애니 </summary>
        public void getShard()
        {
            if (_skillType == SkillKeyList.Shard)
            {
                shotCtrl _shotBullet;
                int num = _gs.Player.Skills[SkillKeyList.Shard].Lvl + 2;
                float rAngle = 360f / num;
                float _angle = UnityEngine.Random.Range(0f, rAngle);

                for (int i = 0; i < num; i++)
                {
                    _shotBullet = _psm.getLaunch(SkillKeyList.Shard);
                    _shotBullet.transform.position = transform.position;
                    _shotBullet.setTarget(Vector3.up, _angle + rAngle * i);
                    _shotBullet.repeatInit(SkillKeyList.Shard, _dmg2, 1f)
                    .play();
                }
            }
        }

        public void setVespene()
        {
            StartCoroutine(vespene());
        }

        IEnumerator vespene()
        {
            skillMarkCtrl _stamp;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    _stamp = _psm.getStamp();
                    _stamp.transform.position = transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f);
                    _stamp.repeatInit(SkillKeyList.Vespene, _dmg2)
                          .setKeep(_gs.Player._skills[SkillKeyList.Vespene].keep)
                          .setDotRate(_gs.Player._skills[SkillKeyList.Vespene].val1)
                          .play();

                    yield return new WaitUntil(() => _gs.Pause == false);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        public void EndAni()
        {
            Destroy();

            _midEff = null;

            for (int i = 0; i < _marks.Length; i++)
            {
                _marks[i].transform.localPosition = Vector3.zero;
                _marks[i].transform.rotation = Quaternion.Euler(Vector3.zero);
                _marks[i].transform.localScale = Vector3.one;
            }
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
                            _gs.Player.getAddStat(SkillKeyList.ATT, 1.01f);
                            break;
                        case 9:
                            _gs.Player.getAddStat(SkillKeyList.DEF, 0.1f);
                            break;
                        default:
                            _gs.Player.getHealed(_dmg);
                            break;
                    }

                    Destroy();
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (_targetEnemy)
            {
                if (coll.gameObject.tag.Equals("Enemy"))
                {
                    ec = coll.gameObject.GetComponentInParent<EnemyCtrl>();
                    if (ec == null)
                    {
                        return;
                    }

                    chkCollision(ec);
                }
                else if (coll.gameObject.tag.Equals("Boss"))
                {
                    ec = coll.gameObject.GetComponentInParent<EnemyCtrl>();
                    if (ec == null)
                    {
                        return;
                    }

                    chkCollision(ec, false);
                }
            }
        }

        void chkCollision(EnemyCtrl ec, bool isMob = true)
        {
            if (isMob && ((MobControl)ec).EnemyType == Mob.ash)
                return;

            switch (_skillType)
            {
                case SkillKeyList.IceBall:  // 닿으면 바로 데미지
                case SkillKeyList.Hail:
                case SkillKeyList.Meteor:
                case SkillKeyList.Lightning:
                case SkillKeyList.SinkHole:
                case SkillKeyList.Crevasse:
                case SkillKeyList.Circle:
                case SkillKeyList.IceWall:
                case SkillKeyList.IceBerg:
                case SkillKeyList.Shard:
                    _adata.set(_dmg, _skillType, false);
                    ec.getDamaged(_adata);
                    break;
                case SkillKeyList.Poison:   // 데미지 없고 중독
                    ec.DotDmg.setDotDmg(_dotRate, _keep);
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
                case SkillKeyList.Vespene:   // 데미지 + 중독
                    ec.DotDmg.setDotDmg(_dotRate, _keep);
                    _adata.set(_dmg, _skillType, false);
                    ec.getDamaged(_adata);      
                    break;
                case SkillKeyList.Present:
                    //
                    break;
                case SkillKeyList.Mine:
                    _adata.set(_dmg, _skillType, false);
                    ec.getDamaged(_adata);
                    getEff();                    
                    Destroy();
                    break;
                default:
                    Debug.LogError("잘못된 입력 : " + _skillType.ToString() + "/" + gameObject.name);
                    break;
            }
        }

        #endregion

        public virtual void onPause(bool bl)
        {
            if (Ani != null)
            {
                Ani.speed = (bl) ? 0 : 1f;
            }
        }
    }
}