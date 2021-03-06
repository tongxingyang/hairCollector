using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class curvedShotCtrl : shotBaseCtrl
    {
        [SerializeField] SpriteRenderer _render;
        [Header("Child")]
        [SerializeField] Transform _bullet;

        playerSkillManager _psm;

        Vector3 _direct;
        float _dest;

        float _dotRate = 0;

        public SkillKeyList SkillType { get => _skillType; }
        public float Damage { get => _dmg; }
        public float Keep { get => _keep; }

        #region [ Init ]

        /// <summary> 첫 생성 </summary>
        public void fixedInit(GameScene gs, playerSkillManager psm)
        {
            _gs = gs;
            _player = gs.Player;
            _psm = psm;
            _efm = gs.EfMng;
        }

        /// <summary> 재사용 및 투사체 데이터 설정 </summary>
        public curvedShotCtrl repeatInit(SkillKeyList skillType, float dmg)
        {
            // 타입
            _skillType = skillType;

            // 데미지
            _dmg = dmg;
            _speed = gameValues._defaultCurvProjSpeed;

            // 이미지 설정
            ShotList sl = EnumHelper.StringToEnum<ShotList>(D_skill.GetEntity(skillType.ToString()).f_shot);
            // Debug.Log(sl.ToString());
            _render.sprite = DataManager.CurveImgs[sl];

            preInit();
            return this;
        }

        public curvedShotCtrl setSpeed(float speed)
        {
            // 투사체 속도
            _speed = gameValues._defaultCurvProjSpeed * speed;

            return this;
        }

        public curvedShotCtrl setSize(float size)
        {
            // 크기            
            _size = size;

            return this;
        }

        public curvedShotCtrl setKeep(float keep)
        {
            // 지속시간
            _keep = keep;

            return this;
        }
        public curvedShotCtrl setDotRate(float rate)
        {
            _dotRate = rate;

            return this;
        }

        /// <summary> 시작 </summary>
        public void play()
        {
            switch (_skillType)
            {
                case SkillKeyList.IceBall:
                case SkillKeyList.SinkHole:
                case SkillKeyList.Crevasse:
                case SkillKeyList.Poison:
                case SkillKeyList.Vespene:
                    StartCoroutine(oneBounce());
                    break;
                case SkillKeyList.Mine:
                case SkillKeyList.Present:
                    StartCoroutine(rotateBounce());
                    break;
                default:
                    Debug.LogError("잘못된 요청 : " + gameObject.name + "/" + _skillType);
                    break;
            }
        }

        #region [ bounce ]

        IEnumerator oneBounce()
        {
            SoundManager.instance.PlaySFX(SFX.shot);

            float time = 0f;
            float spdRate = _speed / _dest;
            Vector3 origin = transform.position;
            Vector3 bounce = (_direct - origin).normalized * 0.5f;
            float progress = 0f;    // 0 ~ 1
            float d;

            Func<bool> chkProgress = ()=> {
                progress = time * spdRate;
                return (progress <= 1f);
            };

            while (chkProgress())
            {
                time += Time.deltaTime;

                transform.position = Vector3.Lerp(origin, _direct, progress);

                d = 4 * _dest * _dest * progress * (1f - progress);
                if (true || d >= 0)
                {

                    _render.sortingOrder = (progress > 0.2f && progress < 0.8f) ? 32 : 24;
                    _bullet.transform.localPosition = new Vector3(0, d, -2 * d);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return new WaitForSeconds(0f);

            bombExplored();
        }

        // Update is called once per frame
        IEnumerator twoBounce()
        {
            SoundManager.instance.PlaySFX(SFX.shot);

            float time = 0;
            float spdRate = _speed / _dest;
            Vector3 origin = transform.position;
            Vector3 bounce = (_direct - origin).normalized * 0.5f;
            float with = 0f;
            float d;

            while (with <= 1f)
            {
                with = time * spdRate;
                transform.position = Vector3.Lerp(origin, _direct, with);
                time += Time.deltaTime;

                d = 4 * _dest * _dest * with * (1 - with);
                if (d >= 0)
                {
                    float root = Mathf.Sqrt(d);
                    _render.sortingOrder = (root > 1) ? 11 : 8;
                    _bullet.transform.localPosition = new Vector3(0, root, root * -0.5f);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            time = 0;
            spdRate = 2; // 0.5초
            origin = transform.position;
            _direct = origin + bounce;
            _dest = Vector3.Distance(Vector3.zero, bounce) * 0.5f;
            with = 0f;
            d = 0;

            while (with <= 1f)
            {
                with = time * spdRate;
                transform.position = Vector3.Lerp(origin, _direct, with);
                time += Time.deltaTime;

                d = 4 * _dest * _dest * with * (1 - with);

                if (d >= 0)
                {
                    float root = Mathf.Sqrt(d);
                    _bullet.transform.localPosition = new Vector3(0, root, root * -0.5f);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return new WaitForSeconds(0f);

            bombExplored();
        }

        IEnumerator rotateBounce()
        {
            SoundManager.instance.PlaySFX(SFX.shot);

            float time = 0;
            float spdRate = _speed / _dest;
            Vector3 origin = transform.position;
            Vector3 bounce = (_direct - origin).normalized * 0.5f;
            float with = 0f;
            float d;

            while (with <= 1f)
            {
                with = time * spdRate;
                transform.position = Vector3.Lerp(origin, _direct, with);
                _bullet.rotation = Quaternion.Euler(0, 0, 360f * with);
                time += Time.deltaTime;

                d = 4 * _dest * _dest * with * (1f - with);
                if (d >= 0)
                {
                    float root = Mathf.Sqrt(d);
                    _render.sortingOrder = (root > 1) ? 32 : 24;
                    _bullet.transform.localPosition = new Vector3(0, root, (root * -0.5f) * 2f); 
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return new WaitForSeconds(0f);

            bombExplored();
        }

        #endregion

        protected override void Destroy()
        {
            preDestroy();

            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        #endregion

        #region [ 움직임 설정 ]

        public void setTarget(Vector3 target, float addAngle = 0f, bool rand = false)
        {
            Vector3 vec = (target - transform.position) * 0.6f;
            target = transform.position + vec;

            _direct = target;
            _dest = Vector3.Distance(target, transform.position) * 0.5f;
        }

        #endregion

        #region [ after-effect ]

        void bombExplored()
        {
            skillMarkCtrl smc = _psm.getStamp();

            smc.transform.position = transform.position;
            smc.repeatInit(_skillType, _dmg)
                .setKeep(_keep)
                .setDotRate(_dotRate)
                .play();

            Destroy();
        }

        #endregion

        public override void onPause(bool bl)
        {
            //_eff.onPause(bl);
        }
    }
}