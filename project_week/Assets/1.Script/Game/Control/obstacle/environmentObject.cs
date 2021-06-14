using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class environmentObject : IobstacleObject
    {
        [SerializeField] LandItem _gemTem;

        PlayerCtrl _player;
        BuffEffect _bff;

        public override IobstacleObject FixedInit(GameScene gs, obstacleKeyList type)
        {
            _gs = gs;
            _player = gs.Player;
            getType = type;

            _gemTem.FixedInit(_gs, type);

            return RepeatInit();
        }

        public override IobstacleObject RepeatInit()
        {
            _gemTem.RepeatInit();

            preInit();

            return this;
        }
        protected override void Destroy()
        {
            preDestroy();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                BuffEffect bf = _player.setDeBuff(SkillKeyList.SPEED, 1, 0.8f, BuffEffect.buffTermType.infinity);
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
                _player.manualRemoveDeBuff(_bff);
                _bff = null;
            }
        }
    }
}