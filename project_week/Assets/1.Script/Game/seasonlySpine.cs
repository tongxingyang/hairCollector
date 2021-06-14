using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace week
{
    public class seasonlySpine : seasonlyBase, IDamage
    {
        [SerializeField] GameObject _shadow;
        [SerializeField] SkeletonAnimation _tree;
        //[SerializeField] GameObject[] _trees;
        [SerializeField] SpriteRenderer _base;
        [SerializeField] Sprite[] _baseImgs;

        CircleCollider2D _coll;

        Vector2[] _treePosition = new Vector2[] 
        {
            new Vector2(0.56f,-0.73f),
            new Vector2(0.49f,-0.85f),
            new Vector2(0.5f,-0.73f),
            new Vector2(0.5f,-0.7f),
            new Vector2(0.54f,-0.85f)
        };

        int _hp = 2;
        bool isBroken;
        public float getHp()
        {
            return _hp;
        }

        #region

        protected string cur_animation = "";

        protected void SetAnimation(string name, bool loop, float speed)
        {
            if (name.Equals(cur_animation))
            {
                return;
            }
            else
            {
                _tree.state.SetAnimation(0, name, loop).TimeScale = speed;
                cur_animation = name;
            }
        }

        protected void spinePause(bool bl)
        {
            _tree.state.TimeScale = (bl) ? 0 : 1f;
        }

        #endregion

        /// <summary> 최초 초기화 - 추가 작업 </summary>
        protected override void whenFixedInit()
        {
            _coll = GetComponent<CircleCollider2D>();

            for (int i = 0; i < 4; i++)
            {
                _tree.AnimationState.Event += delegate (TrackEntry trackEntry, Spine.Event e)
                {
                    if (e.Data.Name.Equals("endMotion"))
                    {
                        SetAnimation("idle", true, 0.5f);
                    }
                };
            }

            whenRepeatInit();
        }

        /// <summary> 재사용 초기화 - 추가 작업 </summary>
        protected override void whenRepeatInit()
        {
            isBroken = false;
            _coll.enabled = true;

            setSeason();
        }

        /// <summary> 계절설정 </summary>
        protected override void setSeason()
        {
            _season = _gs.ClockMng.NowSeason;

            _hp = 2;

            _tree.skeleton.SetSkin(_season.ToString());
            _tree.gameObject.SetActive(true);
            _tree.transform.localPosition = _treePosition[(int)_season];

            _base.gameObject.SetActive(true);
            _base.sprite = _baseImgs[(int)_season];

            _shadow.SetActive(true);            

            SetAnimation("idle", true, 0.5f);
        }

        public float getDamaged(attackData data)
        {
            _hp--;
            
            if (isBroken == false)
            {
                SoundManager.instance.PlaySFX(SFX.tree);
                if (_hp <= 0)
                {
                    isBroken = true;
                    _gs.setInQuestData(inQuest_goal_key.kill, inQuest_goal_valtype.tree, 1);

                    _coll.enabled = false;

                    StartCoroutine(breakdown());
                    return 0;
                }

                SetAnimation("shake", false, 1f);
            }
            
            return 0;
        }

        IEnumerator breakdown()
        {
            SetAnimation("break", false, 0.7f);

            yield return new WaitForSeconds(1.5f);
            
            _tree.gameObject.SetActive(false);
            _base.gameObject.SetActive(false);
            _shadow.SetActive(false);
            preDestroy();
        }

        public void getKnock(Vector3 v, float p, float d) { }

        public void setFrozen(float f) { }

        public void setBuff(enemyStt bff, float val)
        {
            Debug.LogError("장애물은 디버프 걸리지 않음");
        }

        protected override void Destroy()
        {
            preDestroy();
        }

        public override void onPause(bool bl)
        {
            spinePause(bl);
        }
    }
}