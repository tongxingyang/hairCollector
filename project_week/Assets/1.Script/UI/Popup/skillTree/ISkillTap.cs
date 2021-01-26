using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class ISkillTap : UIBase
    {
        #region [uibase]
        
        public skillBox[] mBoxes;

        #endregion

        protected PlayerCtrl _player;
        protected skillTreeComp _tree;
        
        public void Init(PlayerCtrl player, skillTreeComp tree)
        {
            _player = player;
            _tree = tree;

            mBoxSetting();
        }
        public abstract void mBoxSetting();

        public void OnOpen()
        {
            gameObject.SetActive(true);
            for (int i = 0; i < mBoxes.Length; i++)
            {
                mBoxes[i].OnOpen();
            }
        }

        public void refreshSkill()
        {
            for (int i = 0; i < mBoxes.Length; i++)
            {
                mBoxes[i].OnSelect(_tree._nowData);
            }
        }
    }
}