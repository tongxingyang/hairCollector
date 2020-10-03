using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class environmentObject : MonoBehaviour
    {
        GameScene _gs;
        PlayerCtrl _player;
        BuffEffect _bff;
        public void Init(GameScene gs)
        {
            _gs = gs;
            _player = gs.Player;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                BuffEffect bf = _player.setDeBuff(snowStt.speed, 1, 0.8f, BuffEffect.buffTermType.infinity);
                if (bf != null)
                {
                    _bff = bf;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                Debug.Log("늪에서 나옴");
                _player.manualRemoveDeBuff(_bff);
                _bff = null;
            }
        }
    }
}