using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


namespace week
{
    public class spineBossControl : bossControl
    {
        [SerializeField] protected SkeletonAnimation _spine;

        protected string cur_animation = "";

        #region override


        protected override void otherWhenDie()
        {
        }

        protected override void otherWhenFixInit()
        {
        }

        protected override void otherWhenRepeatInit()
        {
            _spine.skeleton.SetColor(Color.white);
        }

        protected override void switchStat(stat st)
        {
        }

        #endregion

        protected void SetAnimation(string name, bool loop, float speed)
        {
            if (name.Equals(cur_animation))
            {
                return;
            }
            else
            {
                _spine.state.SetAnimation(0, name, loop).TimeScale = speed;
                cur_animation = name;
            }
        }

        protected override IEnumerator damageAni()
        {
            _isDmgAction = true;
            _spine.skeleton.SetColor(new Color(1, 0.4f, 0.4f));

            yield return new WaitForSeconds(0.1f);

            _spine.skeleton.SetColor(_originColor);

            yield return new WaitForSeconds(0.1f);

            _spine.skeleton.SetColor(new Color(1, 0.4f, 0.4f));

            yield return new WaitForSeconds(0.1f);

            _spine.skeleton.SetColor(_originColor);
            _isDmgAction = false;
        }

        public override void onPause(bool bl)
        {
            _spine.state.TimeScale = (bl) ? 0 : 1f;
        }
    }
}