using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week 
{
    public class trapObject : IobstacleObject
    {
        [SerializeField] SpriteRenderer _trapImg;
        [SerializeField] Sprite[] _imgs;
        PlayerCtrl _player;

        float _dmgRate = 0.01f;

        public override IobstacleObject FixedInit(GameScene gs, obstacleKeyList type)
        {
            _gs = gs;
            _player = _gs.Player;
            getType = type;

            return RepeatInit();
        }

        public override IobstacleObject RepeatInit()
        {
            preInit();

            if (_gs.ClockMng.NowSeason != season.dark)
            {
                _dmgRate = 0.01f;
                _trapImg.sprite = _imgs[0];
            }
            else
            {
                if (Random.Range(0, 3) == 0)
                {
                    _dmgRate = 0.01f;
                    _trapImg.sprite = _imgs[0];
                }
                else 
                {
                    _dmgRate = 0.015f;
                    _trapImg.sprite = _imgs[1];
                }
            }

            return this;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log(collision.tag);
            if (collision.tag.Equals("Player"))
            {
                float dmg = _player.MaxHp * _dmgRate * (_gs.ClockMng.RecordDay + 1); // (날짜)n%만큼 데미지

                _player.getDamaged(null, dmg);
            }
        }

        protected override void Destroy()
        {
            preDestroy();
        }
    }
}