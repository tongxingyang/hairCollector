using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace week
{
    public class skillFieldTap : ISkillTap
    {
        #region [uibase]

        enum e_box
        {
            b_fieldBase,
            b_snowStorm,
            b_snowFog,
            b_aurora,
            b_blizzard,
            b_whiteout,
            b_subStorm,
            b_iceage
        }

        Enum GetEnumBox()
        {
            return new e_box();
        }

        protected override void OtherSetContent()
        {
            if (GetEnumBox() != null)
            {
                mBoxes = SetComponent<skillBox>(GetEnumBox());
            }
        }

        #endregion

        #region [ override ]
        public override void mBoxSetting()
        {
            gameObject.SetActive(false);

            mBoxes[(int)e_box.b_fieldBase]  .Init(_tree, _player.Skills[SkillKeyList.Field])
            .setRoot(new List<SkillKeyList>[] { new List<SkillKeyList>() { SkillKeyList.SnowStorm, SkillKeyList.SnowFog } });

            mBoxes[(int)e_box.b_snowStorm]  .Init(_tree, _player.Skills[SkillKeyList.SnowStorm]).setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.SnowFog });
            mBoxes[(int)e_box.b_blizzard]   .Init(_tree, _player.Skills[SkillKeyList.Blizzard]) .setFrom()
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList>() { SkillKeyList.IceAge } });

            mBoxes[(int)e_box.b_snowFog]    .Init(_tree, _player.Skills[SkillKeyList.SnowFog])  .setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.SnowStorm })  
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList>() { SkillKeyList.WhiteOut, SkillKeyList.Aurora } });
            mBoxes[(int)e_box.b_whiteout]   .Init(_tree, _player.Skills[SkillKeyList.WhiteOut]) .setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.Aurora })
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList>() { SkillKeyList.IceAge } });
            mBoxes[(int)e_box.b_aurora]     .Init(_tree, _player.Skills[SkillKeyList.Aurora])   .setFrom()  .setChoiceType(new SkillKeyList[1] { SkillKeyList.WhiteOut });
            
            mBoxes[(int)e_box.b_iceage]     .Init(_tree, _player.Skills[SkillKeyList.IceAge])   .setFrom();
            mBoxes[(int)e_box.b_subStorm]   .Init(_tree, _player.Skills[SkillKeyList.SubStorm]) .setFrom();
        }

        #endregion
    }
}