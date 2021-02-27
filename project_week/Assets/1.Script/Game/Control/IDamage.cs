using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week {
    public interface IDamage
    {
        float getDamaged(float f, bool ignoreDef);
        void getKnock(Vector3 v, float p, float d);
        void setFrozen(float f);
        void setBuff(eBuff bff, float val);

        float getHp();
    }
}