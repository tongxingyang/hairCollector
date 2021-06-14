using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class IobstacleObject : poolingObject
    {
        protected GameScene _gs;
        public obstacleKeyList getType { get; protected set; }
        public abstract IobstacleObject FixedInit(GameScene gs, obstacleKeyList type);
        public abstract IobstacleObject RepeatInit();

        /// <summary> 외부에서 삭제 </summary>
        public void forceDestroy()
        {
            Destroy();
        }
    }
}