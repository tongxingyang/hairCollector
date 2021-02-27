using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class shotCtrl : shotBaseCtrl
    {
        [SerializeField] SpriteRenderer _render;                                
                
        #region [ add launch status ]

        int _bounce; // 튕기기
        int _bounceChk;
        float _perDem;

        Action<float> _dmgAction;
        Action<IDamage> idmgAction;

        #endregion

        #region [ Init ]

        /// <summary> 첫 생성 </summary>
        public void fixedInit(GameScene gs, effManager efm)
        {
            _gs = gs;
            _player = gs.Player;
            _efm = efm;
        }

        /// <summary> 재사용 및 투사체 데이터 설정 </summary>
        public shotCtrl repeatInit(SkillKeyList skillType, float dmg, float size)
        {            
            _skillType = skillType;     // 타입
            
            _dmg = dmg;                 // 데미지

            transform.localScale = Vector3.one;         // 크기           
            _speed = gameValues._defaultProjSpeed;      // 투사체 속도      
            transform.localScale = Vector3.one * size;  // 크기
            _keep = 1f;                                 // 지속시간

            _perDem = 0;
            _bounce = 0;

            // 이미지 설정
            ShotList sl = EnumHelper.StringToEnum<ShotList>(DataManager.GetTable<string>(DataTable.skill, skillType.ToString(), SkillValData.shot.ToString()));
            _render.sprite = DataManager.ShotImgs[sl];

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
            if (sbt != snowballType.standard)
            {
                _render.sprite = DataManager.SnowballImg[sbt];
            }

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
            // _sprite.SetActive(true);
            StartCoroutine(LaunchUpdate());
        }

        /// <summary> 업데이트 </summary>
        IEnumerator LaunchUpdate()
        {
            SoundManager.instance.PlaySFX(SFX.shot);
            float time = 0;

            int i = UnityEngine.Random.Range(0, 2);
            i = (i == 0) ? 2 : -2;

            while (_isUse)
            {
                // 이부분에서 눈표창, 도탄첫탄은 각도 틀기
                if (_skillType == SkillKeyList.SnowDart ||
                    (_skillType == SkillKeyList.Ricoche && _bounceChk == 0))
                {
                    transform.Rotate(0f, 0f, i);
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
            while (_isUse)
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
        public override void Destroy()
        {
            preDestroy();

            transform.rotation = new Quaternion(0, 0, 0, 0);
        }        

        #endregion

        #region [ 움직임 설정 ]
        public virtual void setTarget(Vector3 target, float addAngle = 0f, bool rand = false)
        {
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
                onTriggerEnemy(collision.gameObject, true);
            }
            else if (collision.gameObject.tag.Equals("Boss"))
            {
                onTriggerEnemy(collision.gameObject);
            }
            else if (collision.gameObject.tag.Equals("interOb"))
            {
                onTriggerInterOb(collision.gameObject);
            }
            else if (collision.gameObject.tag.Equals("obstacle"))
            {
                _efm.makeEff(effAni.attack, transform.position);

                if (_skillType != SkillKeyList.FrostDrill && _skillType != SkillKeyList.GigaDrill 
                    && _skillType != SkillKeyList.Recovery && _skillType != SkillKeyList.LockOn)
                {
                    Destroy();
                }
            }
        }

        /// <summary> 상호작용 가능한 오브젝트만 </summary>
        void onTriggerEnemy(GameObject go, bool knock = false)
        {
            IDamage id = go.GetComponentInParent<IDamage>();
            if (id == null)
            {
                return;
            }

            // 아직 크리티컬 없음

            // 스킬별 효과
            switch (_skillType)
            {
                case SkillKeyList.IceKnuckle:
                    _dmg += id.getHp() * _perDem * 0.01f;
                    break;
                case SkillKeyList.IcePowder:
                    id.setBuff(eBuff.def, _player.Skills[_skillType].val1* -1f);
                    break;
                case SkillKeyList.Pet2:
                    id.setFrozen(1.5f);
                    break;
            }

            float val = id.getDamaged(_dmg, false);

            _dmgAction?.Invoke(val);
            idmgAction?.Invoke(id);

            // 밀치기
            if (knock)
            {
                Vector3 nor = (go.transform.position - transform.position).normalized * 0.05f;
                id.getKnock(nor, 0.05f, 0.1f);
            }

            _efm.makeEff(effAni.attack, transform.position);

            destroyChk();
        }

        /// <summary> 상호작용 장애물 </summary>
        void onTriggerInterOb(GameObject go)
        {
            IDamage id = go.GetComponentInParent<IDamage>();
            if (id == null)            
                return;
            
            float val = id.getDamaged(1, true);

            _dmgAction?.Invoke(val);
            idmgAction?.Invoke(id);

            _efm.makeEff(effAni.attack, transform.position);

            destroyChk();
        }

        #endregion

        #region 

        void destroyChk()
        {
            switch (_skillType)
            {
                case SkillKeyList.Snowball:
                case SkillKeyList.IceFist:
                case SkillKeyList.IceKnuckle:
                case SkillKeyList.HalfIcicle:
                case SkillKeyList.SnowDart:
                case SkillKeyList.OpenRoader:
                case SkillKeyList.IcePowder:
                case SkillKeyList.Shard:
                case SkillKeyList.Pet:
                case SkillKeyList.Pet2:
                case SkillKeyList.BPet:
                    Destroy();
                    break;
                case SkillKeyList.IcicleSpear:
                case SkillKeyList.FrostDrill:
                case SkillKeyList.GigaDrill:
                case SkillKeyList.IceBalt:
                case SkillKeyList.Recovery:
                case SkillKeyList.LockOn:
                    break;
                case SkillKeyList.Hammer:
                case SkillKeyList.Ricoche:
                    Vector3 mob = _gs.mostCloseEnemy(transform, 0.5f);
                    if (Vector3.Distance(transform.position, mob) < 2.5f)
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