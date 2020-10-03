using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class EnPoisonAreaCtrl : MonoBehaviour
    {
        GameScene _gs;
        PlayerCtrl _player;

        bool _onPlayer;

        public void Init(GameScene gs)
        {
            _gs = gs;
            _player = gs.Player;

            _onPlayer = false;
            StartCoroutine(chk());
        }

        IEnumerator chk()
        {
            while (true)
            {
                if (_onPlayer)
                {
                    _player.DotDmg.setDotDmg(3f, 3f);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                Debug.Log("후후");
                _onPlayer = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _onPlayer = false;
            }
        }
    }
}