using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace week
{
    public class skillRangeTap : ISkillTap
    {
        #region [uibase]

        enum e_box
        {
            b_rangeBase,

            b_hail,
            b_sinkhole,
            b_circle,

            b_crevasse,
            b_meteor,

            b_poison,
            b_thunder,

            b_vespene,
            b_thuncall
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

        #region [ override ] --------------------------------------------------------------------
        public override void mBoxSetting()
        {
            gameObject.SetActive(false);

            mBoxes[(int)e_box.b_rangeBase]  .Init(_tree, _player.Skills[SkillKeyList.IceBall])  
                .setRoot(new List<SkillKeyList>[] {
                    new List<SkillKeyList>{SkillKeyList.Hail },
                    new List<SkillKeyList>{SkillKeyList.SinkHole },
                    new List<SkillKeyList> {SkillKeyList.Circle }});

            mBoxes[(int)e_box.b_hail]       .Init(_tree, _player.Skills[SkillKeyList.Hail])     .setFrom();
            mBoxes[(int)e_box.b_sinkhole]   .Init(_tree, _player.Skills[SkillKeyList.SinkHole]) .setFrom();
            mBoxes[(int)e_box.b_circle]     .Init(_tree, _player.Skills[SkillKeyList.Circle])   .setFrom()
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.Thuncall } });

            mBoxes[(int)e_box.b_crevasse]   .Init(_tree, _player.Skills[SkillKeyList.Crevasse]) .setFrom()
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.Vespene } });
            mBoxes[(int)e_box.b_meteor]     .Init(_tree, _player.Skills[SkillKeyList.Meteor])   .setFrom();

            mBoxes[(int)e_box.b_poison]     .Init(_tree, _player.Skills[SkillKeyList.Poison])   .setFrom()//  .setTem(() => getTem(gainableTem.poisonBottle))
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.Vespene } });
            mBoxes[(int)e_box.b_thunder]    .Init(_tree, _player.Skills[SkillKeyList.Lightning]).setFrom()//  .setTem(() => getTem(gainableTem.lightningTem))
                .setGoLine(new List<SkillKeyList>[] { new List<SkillKeyList> { SkillKeyList.Thuncall } });

            mBoxes[(int)e_box.b_vespene]    .Init(_tree, _player.Skills[SkillKeyList.Vespene])  .setFrom();
            mBoxes[(int)e_box.b_thuncall]   .Init(_tree, _player.Skills[SkillKeyList.Thuncall]) .setFrom();
        }

        #endregion
    }
}