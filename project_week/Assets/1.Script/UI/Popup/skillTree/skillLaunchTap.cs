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
            b_launchUp,
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

        [Header("Item")]
        [SerializeField] GameObject _tems;

        public bool _getHmTem()
        {
            // 플레이어? 템리스트에서 해머 있는지 체크 반환

            return false;
        }

        #region [ override ]
        public override void mBoxSetting()
        {
            List<SkillKeyList>[] skl 
                = new List<SkillKeyList>[1] { new List<SkillKeyList>() { SkillKeyList.IcicleSpear, SkillKeyList.IceFist, SkillKeyList.HalfIcicle } };

            mBoxes[(int)e_box.b_launchBase] .Init(_tree, _player.Skills[SkillKeyList.Snowball])      .setDefault(skl);
            mBoxes[(int)e_box.b_launchUp]   .Init(_tree, _player.Skills[SkillKeyList.Snowball])      .setRein();
            mBoxes[(int)e_box.b_Spear]      .Init(_tree, _player.Skills[SkillKeyList.IcicleSpear])   .setFrom(new SkillKeyList[] { SkillKeyList.Snowball });
            mBoxes[(int)e_box.b_Drill]      .Init(_tree, _player.Skills[SkillKeyList.FrostDrill])    .setFrom(new SkillKeyList[] { SkillKeyList.IcicleSpear });
            mBoxes[(int)e_box.b_Fist]       .Init(_tree, _player.Skills[SkillKeyList.IceFist])       .setFrom(new SkillKeyList[] { SkillKeyList.Snowball });
            mBoxes[(int)e_box.b_Knuckle]    .Init(_tree, _player.Skills[SkillKeyList.IceKnuckle])    .setFrom(new SkillKeyList[] { SkillKeyList.IceFist });
            mBoxes[(int)e_box.b_Half]       .Init(_tree, _player.Skills[SkillKeyList.HalfIcicle])    .setFrom(new SkillKeyList[] { SkillKeyList.Snowball });
            mBoxes[(int)e_box.b_Dart]       .Init(_tree, _player.Skills[SkillKeyList.SnowDart])      .setFrom(new SkillKeyList[] { SkillKeyList.HalfIcicle });
            mBoxes[(int)e_box.b_Hammer]     .Init(_tree, _player.Skills[SkillKeyList.Hammer])        .setFrom(new SkillKeyList[] { SkillKeyList.Snowball })     .setTem(_tems, _getHmTem);
            mBoxes[(int)e_box.b_Giga]       .Init(_tree, _player.Skills[SkillKeyList.GigaDrill])     .setFrom(new SkillKeyList[] { SkillKeyList.FrostDrill }, new SkillKeyList[] { SkillKeyList.IceKnuckle });
            mBoxes[(int)e_box.b_Rico]       .Init(_tree, _player.Skills[SkillKeyList.Ricoche])       .setFrom(new SkillKeyList[] { SkillKeyList.SnowDart }, new SkillKeyList[] { SkillKeyList.Hammer });
        }

        #endregion
    }
}