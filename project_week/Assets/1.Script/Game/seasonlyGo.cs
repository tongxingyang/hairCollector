using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class seasonlyGo : seasonlyBase
    {
        [SerializeField] GameObject[] objs;

        public override void FixedInit()
        {
        }

        public override void setSeason(season ss)
        {
            for (season i = season.spring; i < season.max; i++)
            {
                objs[(int)i].SetActive(i == ss);
            }
        }
    }
}