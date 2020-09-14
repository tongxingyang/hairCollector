using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace week
{
    public class spineCtrl : MonoBehaviour
    {
        [SerializeField] protected SkeletonAnimation _spine;

        protected string cur_animation = "";

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

        protected void spinePause(bool bl)
        {
            _spine.state.TimeScale = (bl) ? 0 : 1f;
        }
    }
}