using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace week
{
    public class BaseObstacle : poolingObject, IPause
    {
        protected enemyManager _enm;
        Transform _homePos;

        public void setting(enemyManager enm)
        {
            _enm = enm;
        }

        public void Init(tileBase tile)
        {
            preInit();

            _homePos = tile.transform;    
            tile.reclaim += Destroy;

            otherSetInit();
        }

        protected virtual void otherSetInit() { }


        protected override void Destroy()
        {
            preDestroy();
            otherInDestroy();
        }

        protected virtual void otherInDestroy()
        { 
        
        }

        public virtual void onPause(bool bl) { }
    }
}