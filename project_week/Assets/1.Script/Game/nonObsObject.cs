using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class nonObsObject : IobstacleObject
    {
        protected LandItem _tem;
        public override IobstacleObject FixedInit(GameScene gs, obstacleKeyList type)
        {
            _gs = gs;
            getType = type;

            _tem = GetComponentInChildren<LandItem>();
            if (_tem != null)
            {
                _tem.FixedInit(_gs, type);
            }

            return RepeatInit();
        }

        public override IobstacleObject RepeatInit()
        {
            preInit();

            if (_tem != null)
            {
                if (Random.Range(0, 10) == 7)
                {
                    _tem.RepeatInit();
                }
                else
                    _tem.temOff();
            }

            return this;
        }

        protected override void Destroy()
        {
            preDestroy();
        }
    }
}