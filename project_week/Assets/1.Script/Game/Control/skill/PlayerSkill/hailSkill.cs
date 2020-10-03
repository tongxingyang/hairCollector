using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class hailSkill : MonoBehaviour
    {
        [SerializeField] CircleCollider2D _collider;
        [SerializeField] GameObject _ice;
        [SerializeField] SpriteRenderer _mark;

        GameScene _gs;
        effManager _efm;

        bool _isUse = false;
        Vector3 _pos;
        float _dmg;
        public bool IsUse { get => _isUse; }

        public void select()
        {
            _isUse = true;
        }

        public void setting(GameScene gs, effManager efm)
        {
            _gs = gs;
            _efm = efm;
        }

        public void Init(Vector3 pos, float dmg)
        {
            _pos = pos;
            _dmg = dmg * BaseManager.userGameData.o_Att;
            transform.localScale = Vector3.one;

            _collider.enabled = false;

            gameObject.SetActive(true);
            StartCoroutine(fallHail());
        }

        // Update is called once per frame
        IEnumerator fallHail()
        {
            _ice.SetActive(true);
            _mark.gameObject.SetActive(false);

            float time = 0;
            //Vector3 vec = (_pos - transform.position).normalized;
            while (Vector3.Distance(transform.position, _pos) > 0.01f)
            {
                time += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _pos, time / 2);

                yield return new WaitUntil(()=>_gs.Pause == false);
            }

            _collider.enabled = true;
            _efm.makeEff(effAni.hail, transform.position);

            //yield return new WaitForSeconds(0.1f);

            _ice.SetActive(false);
            _mark.gameObject.SetActive(true);
            Color col = Color.white;

            while (col.a > 0f)
            {
                col.a -= Time.deltaTime;
                _mark.color = col;
                yield return new WaitUntil(() => _gs.Pause == false);
            }

            Destroy();
        }

        public void Destroy()
        {
            _isUse = false;
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                MobControl mc = collision.gameObject.GetComponentInParent<MobControl>();
                if (mc)
                {
                    mc.getDamaged(_dmg);
                }
                else
                {
                    Debug.LogError(collision + " : MobControl() 없음");
                }

                Destroy();
            }
        }
    }
}