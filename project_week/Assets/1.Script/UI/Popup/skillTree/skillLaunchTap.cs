using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace week
{
    public class skillLaunchTap : UIBase, ISkillTap
    {
        #region [uibase]

        enum e_Line 
        {
            l_fromball,
            l_toUp,
            l_toSpear,
            l_toFist,
            l_toHalf,
            l_toHM,            
            l_toDrill,
            l_toKnuckle,
            l_toDart,
            l_toGiga,
            l_toRico
        }

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

        Enum GetEnumLine()
        {
            return new e_Line();
        }

        Enum GetEnumBox()
        {
            return new e_box();
        }

        public UILineRenderer[] mLines;
        public skillBox[] mBoxes;

        protected override void OtherSetContent()
        {
            if (GetEnumLine() != null)
            {
                mLines = SetComponent<UILineRenderer>(GetEnumLine());
            }

            if (GetEnumBox() != null)
            {
                mBoxes = SetComponent<skillBox>(GetEnumBox());
            }
        }

        #endregion

        [SerializeField] GameObject _tems;

        PlayerCtrl _player;
        skillTreeComp _tree;

        public bool _getHmTem()
        {
            // 플레이어? 템리스트에서 해머 있는지 체크 반환

            return false;
        }

        // Start is called before the first frame update
        public void Init(PlayerCtrl player, skillTreeComp tree)
        {
            _player = player;
            _tree = tree;

            List<SkillKeyList>[] skl 
                = new List<SkillKeyList>[1] { new List<SkillKeyList>() { SkillKeyList.IcicleSpear, SkillKeyList.IceFist, SkillKeyList.HalfIcicle } };

            mBoxes[(int)e_box.b_launchBase] .Init(tree, _player.Skills[SkillKeyList.Snowball])      .setDefault(skl);
            mBoxes[(int)e_box.b_launchUp]   .Init(tree, _player.Skills[SkillKeyList.Snowball])      .setRein();
            mBoxes[(int)e_box.b_Spear]      .Init(tree, _player.Skills[SkillKeyList.IcicleSpear])   .setFrom(SkillKeyList.Snowball);
            mBoxes[(int)e_box.b_Drill]      .Init(tree, _player.Skills[SkillKeyList.FrostDrill])    .setFrom(SkillKeyList.IcicleSpear);
            mBoxes[(int)e_box.b_Fist]       .Init(tree, _player.Skills[SkillKeyList.IceFist])       .setFrom(SkillKeyList.Snowball);
            mBoxes[(int)e_box.b_Knuckle]    .Init(tree, _player.Skills[SkillKeyList.IceKnuckle])    .setFrom(SkillKeyList.IceFist);
            mBoxes[(int)e_box.b_Half]       .Init(tree, _player.Skills[SkillKeyList.HalfIcicle])    .setFrom(SkillKeyList.Snowball);
            mBoxes[(int)e_box.b_Dart]       .Init(tree, _player.Skills[SkillKeyList.SnowDart])      .setFrom(SkillKeyList.HalfIcicle);
            mBoxes[(int)e_box.b_Hammer]     .Init(tree, _player.Skills[SkillKeyList.Hammer])        .setFrom(SkillKeyList.Snowball)     .setTem(_tems, _getHmTem);
            mBoxes[(int)e_box.b_Giga]       .Init(tree, _player.Skills[SkillKeyList.GigaDrill])     .setFrom(SkillKeyList.FrostDrill, SkillKeyList.IceKnuckle);
            mBoxes[(int)e_box.b_Rico]       .Init(tree, _player.Skills[SkillKeyList.Ricoche])       .setFrom(SkillKeyList.SnowDart, SkillKeyList.Hammer);
        }

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