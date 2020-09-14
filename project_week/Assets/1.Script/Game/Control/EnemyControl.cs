using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class EnemyControl : MonoBehaviour
    {
        [SerializeField] Enemy _enemy = Enemy.mop1;
        [SerializeField] int _damage = 1;
        [SerializeField] float _speed = 1;
        Vector3 _direct;

        GameScene _gs;

        public bool isUse { get; set; }
        public Enemy getType { get { return _enemy; } }

        public void setting(GameScene gs)
        {
            _gs = gs;
        }

        public void Init(Vector3 target)
        {
            isUse = true;
            gameObject.SetActive(true);
            _direct = target.normalized;
        }

        public void Destroy()
        {
            isUse = false;
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (isUse)
            {
                transform.position += _direct * _speed * Time.deltaTime;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _gs.getDamaged(_damage);
            }
            else if (collision.tag.Equals("Finish"))
            {
                Destroy();
            }
        }
    }
}