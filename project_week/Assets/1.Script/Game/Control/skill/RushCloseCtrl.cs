using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class RushCloseCtrl : MonoBehaviour
    {        
        [SerializeField] Transform[] _skillObjs;
        Animator _ani;

        GameScene _gs;
        PlayerCtrl _player;
        playerSkillManager _psm;

        float _dmg;
        float _size;
        attackData _adata = new attackData();

        private void Awake()
        {
            _ani = GetComponent<Animator>();
            for(int i = 0; i < _skillObjs.Length;i++)
            {
                _skillObjs[i].gameObject.SetActive(false);
            }
        }

        public void fixedInit(GameScene gs)
        {
            _gs = gs;
            _player = gs.Player;
            _psm = gs.SkillMng;
        }

        public void getBat()
        {
            _skillObjs[0].gameObject.SetActive(true);
            _ani.SetTrigger("shot");
        }

        public RushCloseCtrl repeatInit(float dmg, float size = 1f)
        {
            _dmg = dmg;
            _size = size;

            return this;
        }

        public void play()
        {
            _ani.SetTrigger("shot");
        }

        

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                //if (_skillType == SkillKeyList.IceBat)
                //{
                //    onTriggerEnemy(collision.gameObject, (IDamage id) => {
                //        Vector3 nor = (collision.gameObject.transform.position - _player.transform.position).normalized * 0.05f;
                //        id.getKnock(nor, 0.05f, 0.1f);
                //    });
                //}
                //else if (_skillType == SkillKeyList.IceBat)
                //{
                //    onTriggerEnemy(collision.gameObject, (IDamage id) => {
                //        Vector3 nor = (collision.gameObject.transform.position - _player.transform.position).normalized * 0.05f;
                //        id.getKnock(nor, 0.05f, 0.1f);
                //    });
                //}
                //else
                //{
                //    onTriggerEnemy(collision.gameObject);
                //}
            }
            else if (collision.gameObject.tag.Equals("Boss"))
            {
                onTriggerEnemy(collision.gameObject);
            }
            else if (collision.gameObject.tag.Equals("interOb"))
            {
                onTriggerEnemy(collision.gameObject);
            }
        }

        /// <summary> 상호작용 가능한 오브젝트만 </summary>
        void onTriggerEnemy(GameObject go, Action<IDamage> knock = null)
        {
            IDamage id = go.GetComponentInParent<IDamage>();
            if (id == null)
            {
                return;
            }

            // 아직 크리티컬 없음
            _adata.set(_dmg, SkillKeyList.IceBat, false);
            float val = id.getDamaged(_adata);            

            // 밀치기
            knock?.Invoke(id);

            // _efm.makeEff(effAni.attack, transform.position);            
        }
    }
}