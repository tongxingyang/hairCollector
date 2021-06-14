using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week {
    public interface IDamage
    {
        float getDamaged(attackData data);
        void getKnock(Vector3 v, float p, float d);
        void setFrozen(float f);
        void setBuff(enemyStt bff, float val);

        float getHp();
    }
    
    public class attackData
    {
        public float damage;
        public SkillKeyList type;
        public bool def_Ignore;

        /// <summary> 일반공격 정보 </summary>
        public void set(float damage, SkillKeyList type, bool def_Ignore)
        {
            this.damage = damage;
            this.type = type;
            this.def_Ignore = def_Ignore;
        }

        /// <summary> 독뎀시 </summary>
        public void setDot(float damage)
        {
            this.damage = damage;
            this.type = SkillKeyList.non;
            this.def_Ignore = false;
        }
    }
}