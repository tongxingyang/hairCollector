using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class LandItem : IobstacleObject
    {
        [SerializeField] GameObject _tem;

        PlayerCtrl _player;

        public gainableTem _temtype { get; set; } = gainableTem.heal;

        float regenTime = 15f; // 힐
        bool _isMineOff = true; // 지뢰 (습득 가능 상태 여부)
        string _aniName;

        /// <summary> 생성 초기화 </summary>
        public override IobstacleObject FixedInit(GameScene gs, obstacleKeyList type)
        {
            _gs = gs;
            _player = gs.Player;
            getType = type;

            _temtype = D_obstacle.GetEntity(type.ToString()).f_tem;

            return RepeatInit();
        }

        /// <summary> 재사용 초기화 </summary>
        public override IobstacleObject RepeatInit()
        {
            preInit();

            //if (_temtype == gainableTem.dropMine)
            //{
            //    _isMineOff = (UnityEngine.Random.Range(0, 4) == 0);
            //    _aniName = (_isMineOff) ? "offMine" : "onMine";
                
            //    GetComponentInChildren<Animator>().SetTrigger(_aniName);
            //}
            
            _tem.SetActive(true);

            return this;
        }

        /// <summary> 충돌 (아이템습득) </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                if (collision.GetComponent<PlayerCtrl>() == null)
                {
                    Debug.Log("누구냐 : " + collision.name);
                }

                apply_ItemEffect();
            }
        }

        /// <summary> 템 효과 적용 (및 리스폰 체크) </summary>
        void apply_ItemEffect()
        {
            // 습득 실패류
            if (_temtype == gainableTem.sward && BaseManager.userGameData.Skin != SkinKeyList.heroman) // 칼인데 용사아닐때
                return;

            // 습득 성공류
            if (_temtype != gainableTem.non)
                _player.getTem(_temtype);

            if (_temtype == gainableTem.heal)
            {
                StartCoroutine(respone());
            } 

            temOff();
        }

        public void temOff()
        {
            _tem.SetActive(false);
        }

        /// <summary> 템 리스폰 (시간) </summary>
        IEnumerator respone()
        {
            float t = 0f;

            while (true)
            {
                t += Time.deltaTime;

                if (t > regenTime)
                {
                    t = 0f; 
                    _tem.SetActive(true);
                }

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        protected override void Destroy()
        {
            preDestroy();
        }
    }
}