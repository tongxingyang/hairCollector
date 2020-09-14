using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class environmentObject : MonoBehaviour
    {
        GameScene _gs;
        PlayerCtrl _player;
        public void Init(GameScene gs)
        {
            _gs = gs;
            _player = gs.Player;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                Debug.Log("늪에빠짐 플");
                _player.EnvironmentSpeed = 0.8f;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                Debug.Log("늪에서 나옴");
                _player.EnvironmentSpeed = 1f;
            }
        }
    }
}