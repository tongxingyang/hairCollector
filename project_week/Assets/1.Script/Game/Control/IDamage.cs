using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    bool getDamaged(float f);
    void getKnock(Vector3 v, float p, float d);
    void setFrozen(float f);
}
