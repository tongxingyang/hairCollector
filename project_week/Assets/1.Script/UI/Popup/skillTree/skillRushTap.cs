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

            b_snowBullet,
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
            gameObject.SetActive(false);

            mBoxes[(int)e_box.b_batBase]    .Init(_tree, _player.Skills[SkillKeyList.IceBat])       
                .setRoot( new List<SkillKeyList>[] { 
                    new List<SkillKeyList>() { SkillKeyList.SnowBullet, SkillKeyList.IceBalt }, 
                    new List<SkillKeyList>() { SkillKeyList.Flurry, SkillKeyList.ColdStorm } });
            
            mBoxes[(int)e_box.b_snowBullet] .Init(_tree, _player.Skills[SkillKeyList.SnowBullet])   .setChoiceType(new SkillKeyList[1] { SkillKeyList.IceBalt })    .setFrom();
            mBoxes[(int)e_box.b_icePowder]  .Init(_tree, _player.Skills[SkillKeyList.SnowPoint])    .setFrom();
            
            mBoxes[(int)e_box.b_iceBalt].Init(_tree, _player.Skills[SkillKeyList.IceBalt])          .setChoiceType(new SkillKeyList[1] { SkillKeyList.SnowBullet }) .setFrom();
            mBoxes[(int)e_box.b_recovery]   .Init(_tree, _player.Skills[SkillKeyList.Recovery])     .setFrom()
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList>() { SkillKeyList.SnowBullet, SkillKeyList.LockOn } });
            
            mBoxes[(int)e_box.b_flurry]     .Init(_tree, _player.Skills[SkillKeyList.Flurry])       .setChoiceType(new SkillKeyList[1] { SkillKeyList.ColdStorm })  .setFrom();
            mBoxes[(int)e_box.b_eyeOfF]     .Init(_tree, _player.Skills[SkillKeyList.EyeOfFlurry])  .setFrom();
            
            mBoxes[(int)e_box.b_coldStorm]  .Init(_tree, _player.Skills[SkillKeyList.ColdStorm])    .setChoiceType(new SkillKeyList[1] { SkillKeyList.Flurry })     .setFrom();
            mBoxes[(int)e_box.b_rotateStorm].Init(_tree, _player.Skills[SkillKeyList.RotateStorm])  .setFrom()
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList>() { SkillKeyList.SnowBullet, SkillKeyList.LockOn } });

            mBoxes[(int)e_box.b_lockOn]     .Init(_tree, _player.Skills[SkillKeyList.LockOn])       .setFrom();
        }

        #endregion
    }
}