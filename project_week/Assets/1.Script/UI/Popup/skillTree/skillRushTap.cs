using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace week
{
    public class skillRushTap : ISkillTap
    {
        #region [uibase]

        enum e_box
        {
            b_batBase,
            b_batUp,
            b_openRoader,
            b_icePowder,
            b_iceBalt,
            b_recovery,
            b_flurry,
            b_eyeOfF,
            b_coldStorm,
            b_rotateStorm,
            b_lockOn
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
                = new List<SkillKeyList>[1] { new List<SkillKeyList>() { SkillKeyList.OpenRoader, SkillKeyList.IceBalt, SkillKeyList.Flurry, SkillKeyList.ColdStorm } };

            mBoxes[(int)e_box.b_batBase]    .Init(_tree, _player.Skills[SkillKeyList.IceBat])       .setDefault(skl);
            mBoxes[(int)e_box.b_batUp]      .Init(_tree, _player.Skills[SkillKeyList.IceBat])       .setRein();
            
            mBoxes[(int)e_box.b_openRoader] .Init(_tree, _player.Skills[SkillKeyList.OpenRoader])
                .setSelectType(new SkillKeyList[1] { SkillKeyList.IceBalt })                        .setFrom(new SkillKeyList[] { SkillKeyList.IceBat });
            mBoxes[(int)e_box.b_icePowder]  .Init(_tree, _player.Skills[SkillKeyList.IcePowder])    .setFrom(new SkillKeyList[] { SkillKeyList.OpenRoader });
            mBoxes[(int)e_box.b_iceBalt].Init(_tree, _player.Skills[SkillKeyList.IceBalt])
                .setSelectType(new SkillKeyList[1] { SkillKeyList.OpenRoader })                     .setFrom(new SkillKeyList[] { SkillKeyList.IceBat });
            mBoxes[(int)e_box.b_recovery]   .Init(_tree, _player.Skills[SkillKeyList.Recovery])     .setFrom(new SkillKeyList[] { SkillKeyList.IceBalt });
            mBoxes[(int)e_box.b_flurry]     .Init(_tree, _player.Skills[SkillKeyList.Flurry])
                .setSelectType(new SkillKeyList[1] { SkillKeyList.ColdStorm })                      .setFrom(new SkillKeyList[] { SkillKeyList.IceBat });
            mBoxes[(int)e_box.b_eyeOfF]     .Init(_tree, _player.Skills[SkillKeyList.EyeOfFlurry])  .setFrom(new SkillKeyList[] { SkillKeyList.Flurry });
            mBoxes[(int)e_box.b_coldStorm]  .Init(_tree, _player.Skills[SkillKeyList.ColdStorm])
                .setSelectType(new SkillKeyList[1] { SkillKeyList.Flurry })                         .setFrom(new SkillKeyList[] { SkillKeyList.IceBat });
            mBoxes[(int)e_box.b_rotateStorm].Init(_tree, _player.Skills[SkillKeyList.RotateStorm])  .setFrom(new SkillKeyList[] { SkillKeyList.ColdStorm });
            
            mBoxes[(int)e_box.b_lockOn]     .Init(_tree, _player.Skills[SkillKeyList.LockOn])       .setFrom(new SkillKeyList[] { SkillKeyList.Recovery }, new SkillKeyList[] { SkillKeyList.RotateStorm });
        }

        #endregion
    }
}