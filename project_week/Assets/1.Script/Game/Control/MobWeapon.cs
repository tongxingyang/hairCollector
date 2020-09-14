using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class MobWeapon : MonoBehaviour
    {
        [SerializeField] CircleCollider2D[] col;

        PlayerCtrl _player;
        float _dmg;

        public void setting(PlayerCtrl player, float dmg)
        {
            _player = player;
            _dmg = dmg;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {          
            if (collision.gameObject.tag.Equals("Player"))
            {                
                _player.getDamaged(_dmg);
                if (_player == null)
                {
                    Debug.LogError("플레이어가 왜없어?!");
                }
            }
        }

        public void active(bool act)
        {
            foreach (Collider2D co in col)
            {
                co.gameObject.SetActive(act);
            }
        }
    }
}