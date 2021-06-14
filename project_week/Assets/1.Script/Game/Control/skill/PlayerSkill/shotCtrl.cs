using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class shotCtrl : shotBaseCtrl
    {
        [SerializeField] SpriteRenderer _render;
        [SerializeField] CircleCollider2D _col;
        [SerializeField] TrailRenderer[] _trail;

        #region [ add launch status ]

        int _bounce; // 튕기기
        int _bounceChk;
        float _perDem;
        int _hitCount; // 몇번 적중했는지

        Action<float> _dmgAction;
        Action<IDamage> idmgAction;

        Animator _ani;
        snowballType _sbt = snowballType.standard;

        #endregion

        #region [ Init ]

        /// <summary> 첫 생성 </summary>
        public void fixedInit(GameScene gs, effManager efm)
        {
            _gs = gs;
            _player = gs.Player;
            _efm = efm;
            _ani = GetComponent<Animator>();

            Destroy();
        }

        /// <summary> 재사용 및 투사체 데이터 설정 </summary>
        public shotCtrl repeatInit(SkillKeyList skillType, float dmg, float size)
        {            
            _skillType = skillType;                                                                             // 스킬 타입
            _bulletType = EnumHelper.StringToEnum<ShotList>(D_skill.GetEntity(skillType.ToString()).f_shot);    // 투사체 이미지 타입

            _dmg = dmg;                 // 데미지
    
            transform.localScale    = Vector3.one * size;                                                               // 크기
            _col.radius             = 0.1f * DataManager.ShotBulletData[_bulletType].ColSize;                           // 투사체 콜라이더 크기
            _speed                  = gameValues._defaultProjSpeed * DataManager.ShotBulletData[_bulletType].Speed;     // 투사체 속도      
            _keep                   = 1.3f / DataManager.ShotBulletData[_bulletType].Speed;                             // 지속시간 (속도에 반비례)                   

            _perDem = 0;
            _bounce = 0;
            _hitCount = 0;
            _dmgAction = null;

            // 이미지 설정            
            if (_skillType == SkillKeyList.SnowBall && _sbt != snowballType.standard)
            { 
                _render.sprite = DataManager.SnowballImg[_sbt]; 
            }
            else
            {
                _render.sprite = DataManager.ShotBulletData[_bulletType].Img;
            }            

            // 꼬리 두께
            for (int i = 0; i < _trail.Length; i++)
            {
                _trail[i].enabled = true;

                _trail[i].widthMultiplier = 0.3f * DataManager.ShotBulletData[_bulletType].TailSize;
                _trail[i].Clear();
            }

            preInit();
            return this;
        }

        public shotCtrl setSpeed(float speed)
        {
            _speed = gameValues._defaultProjSpeed * speed;
            return this;
        }
        public shotCtrl setKeep(float keep)
        {
            _keep = keep;
            return this;
        }
        
        /// <summary> 눈덩이 이미지 세팅 </summary>
        public shotCtrl setSnowSprite(snowballType sbt)
        {
            _sbt = sbt;

            return this;
        }

        /// <summary> 햄머 초기화 </summary>
        public shotCtrl setHammer(int bounce)
        {
            _bounce = bounce;
            _bounceChk = 0;

            return this;
        }

        public shotCtrl setPerDamage(float val)
        {
            _perDem = val;
            return this;
        }

        public shotCtrl setDmgAction(Action<float> value)
        { 
            _dmgAction = value;
            return this;
        }

        /// <summary> 시작 </summary>
        public void play()
        {
            StartCoroutine(LaunchUpdate());
        }

        /// <summary> 업데이트 </summary>
        IEnumerator LaunchUpdate()
        {
            SoundManager.instance.PlaySFX(SFX.shot);
            float time = 0;

            string aniname = DataManager.ShotBulletData[_bulletType].Ani;
            _ani.SetTrigger(aniname);
                        
            
            int i = UnityEngine.Random.Range(0, 2);
            float degree = (i == 0) ? 1.5f : -1.5f;

            while (IsUse)
            {
                // 이부분에서 눈표창, 도탄첫탄은 각도 틀기
                if (_skillType == SkillKeyList.SnowDart ||
                    (_skillType == SkillKeyList.Ricoche && _bounceChk == 0))
                {
                    transform.Rotate(0f, 0f, degree);
                }

                transform.Translate(Vector3.up * _speed * Time.deltaTime, Space.Self);
                
                yield return new WaitUntil(() => _gs.Pause == false);
                
                time += Time.deltaTime;
                if (time > _keep)
                {
                    if (_skillType == SkillKeyList.Recovery || _skillType == SkillKeyList.LockOn)
                    {
                        yield return StartCoroutine(backToPlayer());
                    }

                    Destroy();
                }
            }
        }

        IEnumerator backToPlayer()
        {
            while (IsUse)
            {
                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, gameValues._defaultProjSpeed * 1.5f * Time.deltaTime);

                yield return new WaitUntil(() => _gs.Pause == false);

                if (Vector3.Distance(transform.position, _player.transform.position) < 0.1f)
                {
                    break;
                }
            }
        }

        /// <summary> 사용종료 </summary>
        protected override void Destroy()
        {
            for (int i = 0; i < _trail.Length; i++)
            {
                _trail[i].enabled = false;
            }

            preDestroy();

            transform.rotation = new Quaternion(0, 0, 0, 0);
        }        

        #endregion

        #region [ 움직임 설정 ]
        public virtual void setTarget(Vector3 target, float addAngle = 0f, bool rand = false)
        {
            _render.transform.rotation = Quaternion.Euler(Vector3.zero);

            Vector3 _direct = target - transform.position;

            float angle = Mathf.Atan2(_direct.x, _direct.y) * Mathf.Rad2Deg;
            float add = (rand) ? UnityEngine.Random.Range(-addAngle, addAngle) : addAngle;
            transform.rotation = Quaternion.AngleAxis(angle + add, Vector3.back);
        }

        #endregion

        #region [ 적중 ]

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                onTriggerEnemy(collision.gameObject);
            }
            else if (collision.gameObject.tag.Equals("Boss"))
            {
                onTriggerEnemy(collision.gameObject, true);
            }
            else if (collision.gameObject.tag.Equals("interOb"))
            {
                onTriggerInterOb(collision.gameObject);
            }
            else if (collision.gameObject.tag.Equals("obstacle"))
            {
                getEff();

                destroyChk(false);
            }
        }

        /// <summary> 상호작용 가능한 오브젝트만 </summary>
        void onTriggerEnemy(GameObject go, bool isboss = false)
        {
            IDamage id = go.GetComponentInParent<IDamage>();
            if (id == null)
            {
                return;
            }

            // 아직 크리티컬 없음
            _hitCount++;

            // 스킬별 효과
            switch (_skillType)
            {
                case SkillKeyList.IceKnuckle:
                    _dmg += id.getHp() * _perDem * 0.01f;
                    break;
                case SkillKeyList.SnowPoint:
                    id.setBuff(enemyStt.DEF, _player.Skills[_skillType].val1 * -1f);
                    break;
                case SkillKeyList.Pet2:
                case SkillKeyList.BPet:
                    id.setFrozen(1.5f);
                    break;
            }

            _aData.set(_dmg, _skillType, false);
            float val = id.getDamaged(_aData);

            _dmgAction?.Invoke(val);
            idmgAction?.Invoke(id);

            // 몹만 밀치기
            if (isboss == false)
            {
                Vector3 nor = (go.transform.position - transform.position).normalized * 0.05f;
                id.getKnock(nor, 0.05f, 0.1f);
            }

            getEff();

            destroyChk(true);
        }

        /// <summary> 상호작용 장애물 </summary>
        void onTriggerInterOb(GameObject go)
        {
            IDamage id = go.GetComponentInParent<IDamage>();
            if (id == null)            
                return;

            _aData.set(_dmg, _skillType, true);
            float val = id.getDamaged(_aData);

            _dmgAction?.Invoke(val);
            idmgAction?.Invoke(id);

            getEff();

            destroyChk(false);
        }

        #endregion

        #region 

        /// <summary> 상호작용 가능한 오브젝트만 [ 0 : none / 1 : mob / 2 : boss ] </summary>
        void destroyChk(bool ismob, bool isboss = false)
        {
            switch (_skillType)
            {
                case SkillKeyList.SnowBall:
                    _gs.setInQuestData(inQuest_goal_key.skill, inQuest_goal_valtype.conti, (ismob) ? _hitCount : 0);
                    Destroy();
                    break;
                case SkillKeyList.IceFist:
                case SkillKeyList.IceKnuckle:
                case SkillKeyList.HalfIcicle:
                case SkillKeyList.SnowDart:
                case SkillKeyList.SnowBullet:
                case SkillKeyList.SnowPoint:
                case SkillKeyList.ThornShield:
                case SkillKeyList.ReflectShield:
                case SkillKeyList.Shard:
                case SkillKeyList.Pet:
                case SkillKeyList.Pet2:
                case SkillKeyList.BPet:
                    Destroy();
                    break;
                case SkillKeyList.Icicle:   // 고드름 몹만 관통
                    if (ismob == false)
                        Destroy();
                    break;
                case SkillKeyList.FrostDrill:
                case SkillKeyList.GigaDrill:
                case SkillKeyList.IceBalt:
                case SkillKeyList.Recovery:
                case SkillKeyList.LockOn:
                    break;
                case SkillKeyList.Hammer:
                case SkillKeyList.Ricoche:
                    if (ismob)
                    {
                        Vector3 mob = _gs.mostCloseEnemy(transform, 0.5f);
                        if (Vector3.Distance(transform.position, mob) < gameValues._ricocheRange)
                        {
                            _bounceChk++;
                            setTarget(mob);
                            if (_bounceChk > _bounce)
                            {
                                Destroy();
                            }
                        }
                        else
                        {
                            Destroy();
                        }
                    }
                    else
                    {
                        Destroy();
                    }
                    break;
                default:
                    Debug.LogError("잘못된 스킬 : " + _skillType.ToString());
                    break;
            }
        }

        public override void onPause(bool bl)
        {
        }

        #endregion
    }
}