using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class rushCollider : MonoBehaviour
    {
        [SerializeField] SkillKeyList _skill;

        GameScene _gs;
        RushCtrl _rush;
        ParticleSystem _part;
        public bool OnStorm { get; set; }

        public SkillKeyList Skill { get => _skill; set => _skill = value; }
        public int StormCnt { get; set; }

        public void Init(RushCtrl rush, GameScene gs)
        {
            _gs = gs;
            _rush = rush;
            _part = GetComponentInChildren<ParticleSystem>();

            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.tag.Equals("Enemy"))
            {
                _rush.onTriggerEnemy(coll.gameObject, _skill);
            }
            else if (coll.gameObject.tag.Equals("Boss"))
            {
                _rush.onTriggerEnemy(coll.gameObject, _skill, true);
            }
        }

        public void endOfAni()
        {
            if (_skill == SkillKeyList.RotateStorm)
            {
                if (OnStorm && StormCnt < 3)
                {
                    transform.localScale *= 1.5f;
                    ParticleSystem.ShapeModule md = _part.shape;
                    md.radius *= 1.5f;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void endStorm()
        {
            _gs.EfMng.makeEff("storm", transform.position);
            StormCnt++;
        }

        public void sizeReset(SkillKeyList skl)
        {
            float v = 1f;

            if (skl == SkillKeyList.LockOn)
                v = 1.6f;
            else if (skl == SkillKeyList.RotateStorm)
            {
                OnStorm = false;
                StormCnt = 0;
            }

            transform.localScale = Vector3.one * v;
            ParticleSystem.ShapeModule md = _part.shape;
            md.radius = v;
        }
    }
}