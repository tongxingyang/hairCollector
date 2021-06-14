using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace week
{
    public class skillLaunchTap : ISkillTap
    {
        #region [uibase]

        enum e_box
        { 
            b_launchBase,
            
            b_Spear,
            b_Fist,
            b_Half,
            b_Hammer,

            b_Drill,
            b_Knuckle,
            b_Dart,

            b_Giga,
            b_Rico
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

            mBoxes[(int)e_box.b_launchBase].Init(_tree, _player.Skills[SkillKeyList.SnowBall])
                .setRoot(new List<SkillKeyList>[] { 
                    new List<SkillKeyList> { SkillKeyList.Icicle }, 
                    new List<SkillKeyList> { SkillKeyList.IceFist }, 
                    new List<SkillKeyList> { SkillKeyList.HalfIcicle } });

            mBoxes[(int)e_box.b_Spear]      .Init(_tree, _player.Skills[SkillKeyList.Icicle])       .setFrom();
            mBoxes[(int)e_box.b_Fist]       .Init(_tree, _player.Skills[SkillKeyList.IceFist])      .setFrom();
            mBoxes[(int)e_box.b_Half]       .Init(_tree, _player.Skills[SkillKeyList.HalfIcicle])   .setFrom();

            mBoxes[(int)e_box.b_Drill]      .Init(_tree, _player.Skills[SkillKeyList.FrostDrill])   .setFrom()
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.GigaDrill } });
            mBoxes[(int)e_box.b_Knuckle]    .Init(_tree, _player.Skills[SkillKeyList.IceKnuckle])   .setFrom()
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.GigaDrill } });
            mBoxes[(int)e_box.b_Dart]       .Init(_tree, _player.Skills[SkillKeyList.SnowDart])     .setFrom()
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.Ricoche } });
            mBoxes[(int)e_box.b_Hammer]     .Init(_tree, _player.Skills[SkillKeyList.Hammer])       .setFrom()//  .setTem(() => getTem(gainableTem.hammerTem))
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.Ricoche } });

            mBoxes[(int)e_box.b_Giga]       .Init(_tree, _player.Skills[SkillKeyList.GigaDrill])    .setFrom();
            mBoxes[(int)e_box.b_Rico]       .Init(_tree, _player.Skills[SkillKeyList.Ricoche])      .setFrom();
        }

        #endregion
    }
}