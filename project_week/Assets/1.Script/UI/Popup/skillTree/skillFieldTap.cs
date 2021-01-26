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
            b_iceage,
            b_circle
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
            List<SkillKeyList>[] skl
                = new List<SkillKeyList>[1] { new List<SkillKeyList>() { SkillKeyList.SnowStorm, SkillKeyList.SnowFog, SkillKeyList.Aurora } };

            mBoxes[(int)e_box.b_fieldBase].Init(_tree, _player.Skills[SkillKeyList.Field]).setDefault(skl);

            mBoxes[(int)e_box.b_snowStorm].Init(_tree, _player.Skills[SkillKeyList.SnowStorm])
                .setSelectType(new SkillKeyList[2] { SkillKeyList.SnowFog, SkillKeyList.Aurora }).setFrom(new SkillKeyList[] { SkillKeyList.Field });
            mBoxes[(int)e_box.b_blizzard].Init(_tree, _player.Skills[SkillKeyList.Blizzard]).setFrom(new SkillKeyList[] { SkillKeyList.SnowStorm });
            mBoxes[(int)e_box.b_snowFog].Init(_tree, _player.Skills[SkillKeyList.SnowFog])
                .setSelectType(new SkillKeyList[2] { SkillKeyList.SnowStorm, SkillKeyList.Aurora }).setFrom(new SkillKeyList[] { SkillKeyList.Field });
            mBoxes[(int)e_box.b_whiteout].Init(_tree, _player.Skills[SkillKeyList.WhiteOut]).setFrom(new SkillKeyList[] { SkillKeyList.SnowFog });
            mBoxes[(int)e_box.b_aurora].Init(_tree, _player.Skills[SkillKeyList.Aurora])
                .setSelectType(new SkillKeyList[2] { SkillKeyList.SnowStorm, SkillKeyList.SnowFog }).setFrom(new SkillKeyList[] { SkillKeyList.Field });
            mBoxes[(int)e_box.b_subStorm].Init(_tree, _player.Skills[SkillKeyList.SubStorm]).setFrom(new SkillKeyList[] { SkillKeyList.Aurora });

            mBoxes[(int)e_box.b_iceage].Init(_tree, _player.Skills[SkillKeyList.IceAge]).setFrom(new SkillKeyList[] { SkillKeyList.Blizzard, SkillKeyList.WhiteOut }, new SkillKeyList[] { SkillKeyList.Circle });

            mBoxes[(int)e_box.b_circle].Init(_tree, _player.Skills[SkillKeyList.Circle]).setChecker();
        }

        #endregion
    }
}