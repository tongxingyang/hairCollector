using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class EnSkill_Curved : EnSkillControl
    {
        [SerializeField] EnCurvedEffBase _eff;
        [SerializeField] Transform _bullet;
        [SerializeField] GameObject _shadow;
        [SerializeField] float _isHigh = 0f;
        SpriteRenderer _render;

        Vector3 _direct;
        float _dest;

        Action dest;

        protected override void whenInit()
        {
            _render = _bullet.GetComponentInChildren<SpriteRenderer>();
            _eff.setting(_gs);
        }

        protected override void whenRecycleInit()
        {

        }

        public override void operation(Vector3 target, float addAngle = 0f)
        {
            setTarget(target, addAngle);

            StartCoroutine(projMove(_lookRotate, _isHigh));
        }
        /// <summary> 사용 되지 않음 </summary>
        public override void operation(float addAngle = 0)
        {
        }

        protected override void setTarget(Vector3 target, float addAngle = 0f)
        {
            _direct = target;
            _dest = Vector3.Distance(target, transform.position) * 0.5f;
        }

        ///// <summary> 투사체 이동중 회전없음 </summary>
        //protected IEnumerator standardMove()
        //{
        //    float time = 0;
        //    float spdRate = _speed / _dest;
        //    Vector3 origin = transform.position;
        //    Vector3 bounce = (_direct - origin).normalized * 0.5f;
        //    float with = 0f;
        //    float d;

        //    while (with <= 1f)
        //    {
        //        with = time * spdRate;
        //        transform.position = Vector3.Lerp(origin, _direct, with);
        //        time += Time.deltaTime;

        //        d = 4 * _dest * _dest * with * (1 - with);
        //        if (d >= 0)
        //        {
        //            float root = Mathf.Sqrt(d);
        //            _render.sortingOrder = (root > 1) ? 11 : 8;
        //            _bullet.transform.localPosition = new Vector3(0, root * 1.5f, root * -0.5f);
        //        }

        //        yield return new WaitUntil(() => _gs.Pause == false);
        //    }

        //    yield return new WaitForSeconds(0f);

        //    //dest();
        //}

        /// <summary> 투사체 던지기 [1: 회전여부, 2: 발사 높이] </summary>
        protected IEnumerator projMove(bool _isRotate, float addHight = 0)
        {
            float time = 0;
            float spdRate = _speed / _dest;
            Vector3 origin = transform.position;
            Vector3 bounce = (_direct - origin).normalized * 0.5f;
            float with = 0f;
            float d;
            float ah;

            while (with < 1f)
            {
                with = time * spdRate;
                transform.position = Vector3.Lerp(origin, _direct, with);
                time += Time.deltaTime;

                if (_isRotate)
                {
                    _bullet.rotation = Quaternion.Euler(0, 0, 360f * with);
                }

                ah = addHight * with;

                if (with > 0.99f)
                {
                    with = 1f;
                }

                d = 4 * _dest * _dest * with * (1 - with);
                if (d >= 0)
                {
                    float root = Mathf.Sqrt(d);
                    _render.sortingOrder = (root > 1) ? 11 : 8;
                    _bullet.transform.localPosition = new Vector3(0, (addHight - ah) + root * 1.2f, root * -0.5f);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return new WaitForSeconds(0f);

            bombExplored();
        }

        /// <summary> 투사체 이동중 각도 바라보기 </summary>
        protected IEnumerator followAngleMove()
        {
            float time = 0;
            float spdRate = _speed / _dest;
            Vector3 origin = transform.position;
            Vector3 bounce = (_direct - origin).normalized * 0.5f;
            float with = 0f;
            float d;

            Vector3 _lookDirect;
            Vector3 _prevPos;
            float angle;

            while (with <= 1f)
            {
                with = time * spdRate;
                _prevPos = _bullet.transform.position;
                transform.position = Vector3.Lerp(origin, _direct, with);
                time += Time.deltaTime;

                _lookDirect = _prevPos - _bullet.transform.position;
                angle = Mathf.Atan2(_lookDirect.x, _lookDirect.y) * Mathf.Rad2Deg;
                _bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);

                d = 4 * _dest * _dest * with * (1 - with);
                if (d >= 0)
                {
                    float root = Mathf.Sqrt(d);
                    _render.sortingOrder = (root > 1) ? 11 : 8;
                    _bullet.transform.localPosition = new Vector3(0, root, root * -0.5f);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }

            yield return new WaitForSeconds(0f);

            //dest();
        }

        void bombExplored()
        {
            _bullet.gameObject.SetActive(false);
            _shadow.SetActive(false);

            _eff.Init(_dmg, 12f, () =>
            {
                Destroy();
            });
        }

        /// <summary> 일시정지 </summary>
        public override void onPause(bool bl)
        {
            if (Ani != null)
            {
                Ani.speed = (bl) ? 0f : 1f;
            }
        }        
    }
}