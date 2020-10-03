using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class tornado : MonoBehaviour
    {
        Transform _sprite;
        List<MobControl> _mobs;
        float _dmg = 1f;
        float _tick = 1f;

        private void Awake()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>().transform;
            _mobs = new List<MobControl>();
            gameObject.SetActive(false);
        }

        public void Init(float dmg, float tick, float size)
        {
            _dmg = dmg * BaseManager.userGameData.o_Att;
            _tick = tick;
            transform.localScale = Vector3.one * size;

            gameObject.SetActive(true);
            StartCoroutine(tornadoUpdate());
        }

        IEnumerator tornadoUpdate()
        {
            float time = 0;
            while (true)
            {
                yield return new WaitForSeconds(0);

                _sprite.Rotate(Vector3.forward * 1);

                time += Time.deltaTime * Time.timeScale;

                if (time > _tick)
                {
                    time = 0;
                    for(int i = 0; i < _mobs.Count; i++)
                    {
                        _mobs[i].getDamaged(_dmg);
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                _mobs.Add(collision.gameObject.GetComponent<MobControl>());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                _mobs.Remove(collision.gameObject.GetComponent<MobControl>());
            }
        }
    }
}