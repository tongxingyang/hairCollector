using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class RangeSkillCtrl : shotBaseCtrl
    {
        [SerializeField] SpriteRenderer _render;
        [Header("Child")]
        [SerializeField] Transform _bomb;
        // [SerializeField] secondEffCtrl _eff;
        [SerializeField] GameObject _shadow;

        playerSkillManager _psm;

        Vector3 _direct;
        float _dest;

        SpriteRenderer _bombSp;

        public SkillKeyList SkillType { get => _skillType; }
        public float Damage { get => _dmg; }
        public float Keep { get => _keep; }

        #region [ Init ]

        /// <summary> 첫 생성 </summary>
        public void fixedInit(GameScene gs,playerSkillManager psm, effManager efm)
        {
            _gs = gs;
            _player = gs.Player;
            _psm = psm;
            _efm = efm;

            _bombSp = _bomb.GetComponent<SpriteRenderer>();
            // _eff.setting(_gs);
        }

        /// <summary> 재사용 및 투사체 데이터 설정 </summary>
        public RangeSkillCtrl repeatInit(SkillKeyList skillType, float dmg, float size = 1f, float speed = 1f, float keep = 0.7f)
        {
            // 타입
            _skillType = skillType;

            // 데미지
            _dmg = dmg;
            // 크기            // _size = size;
            transform.localScale = Vector3.one * size;
            // 투사체 속도
            _speed = gameValues._defaultCurvProjSpeed * speed;
            // 지속시간
            _keep = keep;

            // 이미지 설정
            _render.sprite = DataManager.LaunchImg[skillType];

            //=========[ 자식 에프터-이펙트 초기화 ]========================
            _bomb.gameObject.SetActive(true);
            _shadow.SetActive(true);

            //_eff.gameObject.SetActive(false);

            preInit();
            return this;
        }

        /// <summary> 시작 </summary>
        public void play()
        {
            switch (_skillType)
            {
                case SkillKeyList.Iceball:
                case SkillKeyList.Crevasse:
                case SkillKeyList.Poison:
                case SkillKeyList.sulfurous:
                    StartCoroutine(oneBounce(bombExplored));
                    break;
                case SkillKeyList.SnowBomb:
                    StartCoroutine(twoBounce(bombExplored));
                    break;
                case SkillKeyList.Mine:
                case SkillKeyList.present:
                    StartCoroutine(rotateBounce(bombExplored));
                    break;
                default:
                    Debug.LogError("잘못된 요청 : " + gameObject.name + "/" + _skillType);
                    break;
            }
        }

        #region [ bounce ]

        IEnumerator oneBounce(Action dest)
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
                    _bombSp.sortingOrder = (root > 1) ? 11 : 8;
                    _bomb.transform.localPosition = new Vector3(0, root, root * -0.5f);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return new WaitForSeconds(0f);

            dest();
        }

        // Update is called once per frame
        IEnumerator twoBounce(Action dest)
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
                    _bombSp.sortingOrder = (root > 1) ? 11 : 8;
                    _bomb.transform.localPosition = new Vector3(0, root, root * -0.5f);
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
                    _bomb.transform.localPosition = new Vector3(0, root, root * -0.5f);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return new WaitForSeconds(0f);

            dest();
        }

        IEnumerator rotateBounce(Action dest)
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
                _bomb.rotation = Quaternion.Euler(0, 0, 360f * with);
                time += Time.deltaTime;

                d = 4 * _dest * _dest * with * (1 - with);
                if (d >= 0)
                {
                    float root = Mathf.Sqrt(d);
                    _bombSp.sortingOrder = (root > 1) ? 11 : 8;
                    _bomb.transform.localPosition = new Vector3(0, root, root * -0.5f);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return new WaitForSeconds(0f);

            dest();
        }

        #endregion

        public override void Destroy()
        {
            preDestroy();

            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        #endregion

        #region [ 움직임 설정 ]

        public void setTarget(Vector3 target, float addAngle = 0f, bool rand = false)
        {
            _direct = target;
            _dest = Vector3.Distance(target, transform.position) * 0.5f;
        }

        #endregion

        #region [ after-effect ]

        void bombExplored()
        {
            _bomb.gameObject.SetActive(false);
            _shadow.SetActive(false);

            //if (_skillType == SkillKeyList.Mine)
            //{
            //    _dmg *= BaseManager.userGameData.SkinFval[(int)skinFvalue.mine];
            //}

            //_eff.Init(this, () =>
            //{
            //    Destroy();
            //});
        }

        #endregion

        public override void onPause(bool bl)
        {
            //_eff.onPause(bl);
        }
    }
}