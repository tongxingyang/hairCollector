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
        [SerializeField] GameObject[] _trees;
        [SerializeField] GameObject[] _bases;
        [SerializeField] SkeletonAnimation[] _spines;

        season _season;
        float _hp = 20f;

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
                _spines[(int)_season].state.SetAnimation(0, name, loop).TimeScale = speed;
                cur_animation = name;
            }
        }

        protected void spinePause(bool bl)
        {
            _spines[(int)_season].state.TimeScale = (bl) ? 0 : 1f;
        }

        #endregion

        public override void FixedInit()
        {
            for (int i = 0; i < 4; i++)
            {                
                _spines[i].AnimationState.Event += delegate (TrackEntry trackEntry, Spine.Event e)
                {
                    if (e.Data.Name.Equals("endMotion"))
                    {
                        SetAnimation("idle", true, 0.5f);
                    }
                };
            }        
        }

        public override void setSeason(season ss)
        {
            _season = ss; 
            
            _hp = 20f;
            for (season i = season.spring; i < season.max; i++)
            {
                _trees[(int)i].SetActive(_season == i);
            }
            _shadow.SetActive(true);
            SetAnimation("idle", true, 0.5f);
        }

        public float getDamaged(float val, bool igD = false)
        {
            _hp -= val;

            if (_hp <= 0)
            {
                Destroy();
                return 0;
            }

            SetAnimation("shake", false, 1f);
            return val;
        }

        void Destroy()
        {
            StartCoroutine(breakdown());
        }

        IEnumerator breakdown()
        {
            SetAnimation("break", false, 0.7f);

            yield return new WaitForSeconds(1.5f);
            
            _trees[(int)_season].SetActive(false);
            _shadow.SetActive(false);
        }

        public void getKnock(Vector3 v, float p, float d) { }

        public void setFrozen(float f) { }
    }
}