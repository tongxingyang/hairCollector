﻿using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class EnSkill_Proj : EnSkillControl
    {
        [SerializeField] bool _lowHitRate;
        [ShowIf("isLowHit")]
        float _randAngle = 0f;
        bool isLowHit { get => (_lowHitRate == true); }

        [SerializeField] bool _triggDestroy = true;

        float _lifeTime = 2.5f;

        Vector3 _target;


        protected override void whenInit()
        {

        }

        protected override void whenRecycleInit()
        {

        }
        public override void operation(Vector3 target, float addAngle = 0f)
        {
            _targeting = true;
            setTarget(target - transform.position, addAngle);

            StartCoroutine(skillUpdate());
        }

        public override void operation(float addAngle = 0)
        {
            _targeting = false;
            setTarget(Vector3.zero, addAngle);

            StartCoroutine(skillUpdate());
        }

        /// <summary> 타겟방향 회전 </summary>
        protected override void setTarget(Vector3 target, float addAngle = 0f)
        {
            if (_lookRotate)
            {
                if (_targeting)
                {
                    Vector3 _direct = target - transform.position;

                    float angle = Mathf.Atan2(_direct.x, _direct.y) * Mathf.Rad2Deg;
                    float add = Random.Range(-_randAngle, _randAngle);
                    transform.rotation = Quaternion.AngleAxis(angle + add, Vector3.back);

                }
                else
                {
                    transform.rotation = Quaternion.AngleAxis(addAngle, Vector3.back);
                }
            }
            else
            {
                _target = target.normalized;
            }
        }

        protected IEnumerator skillUpdate()
        {
            float time = 0;

            while (_isUse)
            {
                time += Time.deltaTime;

                if (_lookRotate)
                {
                    transform.Translate(Vector3.up * _speed * Time.deltaTime, Space.Self);
                }
                else
                {
                    transform.Translate(_target * _speed * Time.deltaTime, Space.Self);
                }

                if (time > _lifeTime)
                {
                    Destroy();
                }

                yield return new WaitUntil(()=>_gs.Pause == false);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(_dmg);
                if (_triggDestroy)
                {
                    Destroy();
                }
            }
            else if (collision.tag.Equals("obstacle"))
            {
                _efm.makeEff(effAni.attack, transform.position);
                Destroy();
            }
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