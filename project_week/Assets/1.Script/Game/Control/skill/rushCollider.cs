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

        public SkillKeyList Skill { get => _skill; set => _skill = value; }
        public int StormCnt { get; set; }

        public void Init(RushCtrl rush, GameScene gs)
        {
            _gs = gs;
            _rush = rush;
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            //Debug.Log(coll.name);
            if (coll.gameObject.tag.Equals("Enemy"))
            {
                _rush.onTriggerEnemy(coll.gameObject, _skill);
            }
            else if (coll.gameObject.tag.Equals("Boss"))
            {
                _rush.onTriggerEnemy(coll.gameObject, _skill);
            }
        }

        public void endOfAni()
        {
            if (_skill == SkillKeyList.RotateStorm)
            {
                if (_rush.OnStorm && StormCnt < 3)
                {
                    transform.localScale *= 1.5f;
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
            _gs.EfMng.makeEff(effAni.storm, transform.position);
            StormCnt++;
        }
    }
}