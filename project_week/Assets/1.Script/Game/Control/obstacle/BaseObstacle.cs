using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace week
{
    public class BaseObstacle : poolingObject
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


        public override void Destroy()
        {
            preDestroy();
            otherInDestroy();
        }

        protected virtual void otherInDestroy()
        { 
        
        }

        public override void onPause(bool bl)
        {
        }
    }
}