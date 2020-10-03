using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class ScurvedPorjCtrl : BaseProjControl
    {
        [Header("Child")]
        [SerializeField] Transform _bomb;
        [SerializeField] secondEffCtrl _eff;
        [SerializeField] GameObject _shadow;

        Vector3 _direct;
        float _dest;

        SpriteRenderer _bombSp;

        protected override void whenFixedInit()
        {
            _bombSp = _bomb.GetComponent<SpriteRenderer>();
            _eff.setting(_gs);
        }

        public override void setTarget(Vector3 target, float addAngle = 0f, bool rand = false)
        {
            _direct = target;
            _dest = Vector3.Distance(target, transform.position) * 0.5f;
        }

        public override void repeatInit(float dmg, float size, float speed = 1f, float keep = 0.7f) 
        {
            _dmg = dmg;
            _speed = gameValues._defaultCurvProjSpeed * speed;
            _keep = keep;

            // transform.localScale = Vector3.one * size;

            preInit();

            _bomb.gameObject.SetActive(true);
            _eff.gameObject.SetActive(false);
            _shadow.SetActive(true);

            switch (getSkillType)
            {
                case SkillKeyList.snowbomb:
                    StartCoroutine(twoBounce(bombExplored));
                    break;
                case SkillKeyList.poison:
                case SkillKeyList.blackhole:
                    StartCoroutine(oneBounce(bombExplored));
                    break;
                case SkillKeyList.mine:
                    StartCoroutine(rotateBounce(bombExplored));
                    break;
                default:
                    Debug.LogError("잘못된 요청 : " + gameObject.name + "/" + getSkillType);
                    break;
            }
        }

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

        void bombExplored()
        {
            _bomb.gameObject.SetActive(false);
            _shadow.SetActive(false);

            _dmg *= BaseManager.userGameData.SkinFval[(int)skinFvalue.mine];
            _eff.Init(_dmg, _keep, () =>
             {
                 Destroy();
             });
        }

        public override void onPause(bool bl)
        {
            _eff.onPause(bl);
        }
    }
}