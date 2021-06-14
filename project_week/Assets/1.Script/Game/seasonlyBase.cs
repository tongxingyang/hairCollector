using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class seasonlyBase : IobstacleObject, IPause
    {
        protected season _season;

        /// <summary> 최초 초기화 </summary>
        public override IobstacleObject FixedInit(GameScene gs, obstacleKeyList type) 
        {
            _gs = gs;
            getType = type;

            whenFixedInit();

            return RepeatInit();
        }
        protected abstract void whenFixedInit();

        /// <summary> 재사용 초기화 </summary>
        public override IobstacleObject RepeatInit()
        {
            preInit();
            whenRepeatInit();
            return this;
        }
        protected abstract void whenRepeatInit();
        protected override void Destroy()
        {
            preDestroy();
        }

        /// <summary> 계절설정 </summary>
        protected abstract void setSeason();

        public virtual void onPause(bool bl) { }
    }
}