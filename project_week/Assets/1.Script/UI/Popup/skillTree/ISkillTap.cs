using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public interface ISkillTap
    {
        void Init(PlayerCtrl player, skillTreeComp tree);
        void refreshSkill();
        void OnOpen();
    }
}