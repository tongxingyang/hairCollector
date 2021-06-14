using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class LandNpc : IobstacleObject
    {
        bool _npcIsUsed;

        /// <summary> 생성 초기화 </summary>
        public override IobstacleObject FixedInit(GameScene gs, obstacleKeyList type)
        {
            _gs = gs;
            getType = type;

            return RepeatInit();
        }

        /// <summary> 재사용 초기화 </summary>
        public override IobstacleObject RepeatInit()
        {
            _npcIsUsed = false;
            preInit();
            return this;
        }

        // 충돌시 판정
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                if (collision.GetComponent<PlayerCtrl>() == null)
                {
                    Debug.Log("누구냐 : " + collision.name);
                }

                if (_npcIsUsed == false)
                {
                    _gs.getInQuest(); // 퀘스트 창 오픈
                }
                _npcIsUsed = true;
            }
        }
        protected override void Destroy()
        {
            preDestroy();
        }
    }
}