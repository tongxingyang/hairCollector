using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class LaunchSkillCtrl : shotBaseCtrl
    {
        [SerializeField] SpriteRenderer _render;                                
                
        #region [ add launch status ]

        int _bounce; // 튕기기
        int _bounceChk;

        Action<float> dmgAction;
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
        public LaunchSkillCtrl repeatInit(SkillKeyList skillType, float dmg, float size = 1f, float speed = 1f, float keep = 1f)
        {
            // 타입
            _skillType = skillType;

            // 데미지
            _dmg = dmg;
            // 크기            // _size = size;
            transform.localScale = Vector3.one * size;
            // 투사체 속도
            _speed = gameValues._defaultProjSpeed * speed;
            // 지속시간
            _keep = keep;

            _bounce = 0;

            // 이미지 설정
            _render.sprite = DataManager.LaunchImg[skillType];

            preInit();
            return this;
        }

        /// <summary> 햄머 초기화 </summary>
        public LaunchSkillCtrl setHammer(int bounce)
        {
            _bounce = bounce;

            return this;
        }

        public LaunchSkillCtrl setSnowSprite(snowballType sbt)
        {
            _render.sprite = DataManager.SnowballImg[sbt];

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

            while (_isUse)
            {
                // 이부분에서 눈표창, 도탄첫탄은 각도 틀기

                transform.Translate(Vector3.up * _speed * Time.deltaTime, Space.Self);

                yield return new WaitUntil(() => _gs.Pause == false);

                time += Time.deltaTime;
                if (time > _keep)
                {
                    Destroy();
                }

                //if (Vector3.Distance(Player.transform.position, transform.position) > 3f)
                //{
                //    Destroy();
                //}
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
                onTriggerEnemy(collision.gameObject);
            }
            else if (collision.gameObject.tag.Equals("obstacle"))
            {
                _efm.makeEff(effAni.attack, transform.position);
                Destroy();
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
            

            float val = id.getDamaged(_dmg, false);

            dmgAction?.Invoke(val);
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
                    Destroy();
                    break;
                case SkillKeyList.IcicleSpear:
                case SkillKeyList.FrostDrill:
                case SkillKeyList.GigaDrill:
                    break;
                case SkillKeyList.Hammer:
                    Vector3 mob = _gs.mostCloseEnemy(transform, 0.5f);
                    if (Vector3.Distance(transform.position, mob) < 2f)
                    {
                        setTarget(mob);
                        _bounceChk++;
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