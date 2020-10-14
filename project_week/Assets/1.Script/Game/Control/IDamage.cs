using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    float getDamaged(float f, bool ignoreDef);
    void getKnock(Vector3 v, float p, float d);
    void setFrozen(float f);
}
