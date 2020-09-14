using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class seasonlyBase : MonoBehaviour
    {
        public abstract void FixedInit();
        public abstract void setSeason(season ss);
    }
}