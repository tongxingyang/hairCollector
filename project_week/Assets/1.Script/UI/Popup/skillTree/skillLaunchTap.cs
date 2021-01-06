using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace week
{
    public class skillLaunchTap : UIBase
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

        public Image hm_tem;

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < mBoxes.Length; i++)
            {
                mBoxes[i].Init();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}