using ES3Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class SprojCtrl : BaseProjControl
    {
        GameObject _sprite;
        
        int bounce;

        protected override void whenFixedInit()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        }

        public override void repeatInit(float dmg, float size, float speed = 1f, float keep = 1f)
        {
            _dmg = dmg;
            _speed = gameValues._defaultSpeed * speed;
            _keep = keep;
            bounce = 0;

            transform.localScale = Vector3.one * size;

            preInit();

            _sprite.SetActive(true);
            StartCoroutine(projectilUpdate());
        }

        // Update is called once per frame
        IEnumerator projectilUpdate()
        {
            SoundManager.instance.PlaySFX(SFX.shot);
            float time = 0;

            while (_isUse)
            {
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

        public void projDestroy()
        {
            Destroy();
        }

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

        void onTriggerEnemy(GameObject go, bool knock = false)
        {
            IDamage id = go.GetComponentInParent<IDamage>();
            if (id == null)
            {
                return;
            }

            id.getDamaged(_dmg);

            if (getSkillType == getSkillList.pet)
            {
                id.setFrozen(1f);
            }
            else if (knock)
            {
                Vector3 nor = (go.transform.position - transform.position).normalized * 0.05f;
                id.getKnock(nor, 0.05f, 0.1f);
            }

            _efm.makeEff(effAni.attack, transform.position);


            destroyChk();
        }

        #region 

        void destroyChk()
        {
            switch (getSkillType)
            {
                case getSkillList.snowball:
                case getSkillList.icefist:
                case getSkillList.halficicle:
                case getSkillList.pet:
                    projDestroy();
                    break;
                case getSkillList.icicle:
                    break;
                case getSkillList.hammer:
                    Vector3 mob = _gs.mostCloseEnemy(transform, 0.5f);
                    if (Vector3.Distance(transform.position, mob) < 2f)
                    {
                        setTarget(mob);
                        bounce++;
                        if (bounce > skillLvl)
                        {
                            projDestroy();
                        }
                    }
                    else
                    {
                        projDestroy();
                    }
                    break;
                default:
                    Debug.LogError("잘못된 스킬 : " + getSkillType.ToString());
                    break;
            }
        }

        public override void onPause(bool bl)
        {
        }

        #endregion
    }
}