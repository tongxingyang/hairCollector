using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class TrapController : IobstacleObject, IPause
    {
        baseRuinTrap _trap;
        LandItem _tem;

        /// <summary> 생성 초기화 </summary>
        public override IobstacleObject FixedInit(GameScene gs, obstacleKeyList type)
        {
            _gs = gs;
            getType = type;

            _trap = GetComponentInChildren<baseRuinTrap>();
            _trap?.FixedInit(_gs, type);
            _tem = GetComponentInChildren<LandItem>();
            _tem?.FixedInit(_gs, type);

            return RepeatInit();
        }

        /// <summary> 재사용 초기화 </summary>
        public override IobstacleObject RepeatInit()
        {
            preInit();

            _trap?.RepeatInit(); 
            _trap?.Play();

            _tem?.RepeatInit();

            return this;
        }

        protected override void Destroy()
        {
            preDestroy();
        }

        public void onPause(bool bl)
        {
            _trap.onPause(bl);
        }

        
    }
}