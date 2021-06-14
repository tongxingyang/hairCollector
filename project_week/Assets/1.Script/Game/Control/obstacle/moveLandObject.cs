using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class moveLandObject : IobstacleObject
    {
        [SerializeField] SpriteRenderer _tem;
        PlayerCtrl _player;
        public gainableTem _temtype { get; set; }// = gainableTem.dropBeard;
        bool _isChecked = false;

        Animator _ani;
        float Dist { get => Vector3.Distance(transform.position, _player.transform.position); }
        state _state;
        float _wait, _check;

        public override IobstacleObject FixedInit(GameScene gs, obstacleKeyList type)
        {
            _gs = gs;
            _player = gs.Player;
            getType = type;

            _temtype = D_obstacle.GetEntity(type.ToString()).f_tem;

            _ani = GetComponentInChildren<Animator>();

            return RepeatInit();
        }

        public override IobstacleObject RepeatInit()
        {
            preInit();
            _isChecked = false;

            StartCoroutine(lifeCycle());

            return this;
        }

        enum state { idle, up, run, down }
        IEnumerator lifeCycle()
        {
            Vector3 vec;
            while (IsUse)
            {
                switch (_state)
                {
                    case state.idle:
                        if (Dist < 4f)
                        {
                            changeState(state.up);
                            _isChecked = true;
                        }
                        else if (Dist > 40f)
                        {
                            Destroy();
                        }
                        break;
                    case state.up:
                        waitTime(0.5f);
                        changeState(state.run);
                        break;
                    case state.run:
                        if (Dist > 4f)
                        {
                            changeState(state.down);
                            _isChecked = false;
                        }
                        vec = (_player.transform.position - transform.position) * -1;
                        transform.position = Vector3.MoveTowards(transform.position, transform.position + vec, gameValues._defaultSpeed * 0.8f * Time.deltaTime);
                        break;
                    case state.down:
                        waitTime(0.5f);
                        changeState(state.idle);
                        break;
                }

                _tem.flipX = (_player.transform.position.x < transform.position.x);

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        void changeState(state state)
        {
            _state = state;
            _ani.SetTrigger(state.ToString());
            _wait = _check = 0;
        }

        void waitTime(float time)
        {
            _wait = time;
            _check += Time.deltaTime;
        }

        /// <summary> 충돌 (아이템습득) </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                if (collision.gameObject.GetComponent<PlayerCtrl>() == null)
                {
                    Debug.Log("누구냐 : " + collision.gameObject.name);
                }

                apply_ItemEffect();
            }
        }

        /// <summary> 템 효과 적용 (및 리스폰 체크) </summary>
        void apply_ItemEffect()
        {
            //if (_temtype != gainableTem.dropMine) // 
            //    return;

            // 습득 성공류
            _player.getTem(_temtype);

            Destroy();
        }

        protected override void Destroy()
        {
            preDestroy();
        }
    }
}