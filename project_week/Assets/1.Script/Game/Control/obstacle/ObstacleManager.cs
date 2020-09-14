using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class ObstacleManager : MonoBehaviour
    {
        GameScene _gs;
        enemyManager _enm;
        clockManager _clm;

        List<LandObject> _obstacleList;
        List<baseRuinTrap> _trapList;

        public void Init(GameScene gs)
        {
            _gs = gs;
            _enm = _gs.EnemyMng;
            _clm = _gs.ClockMng;

            _obstacleList = new List<LandObject>();
            _trapList = new List<baseRuinTrap>();

        }

        public LandObject getObstacle(mapObstacle type, tileBase tile, Vector3 pos)
        {
            // 있으면 찾아쓰고
            foreach (LandObject lo in _obstacleList)
            {
                if (lo.getType == type && lo.IsUse == false)
                {
                    lo.transform.position = pos;
                    lo.RepeatInit(tile, _clm.Season);

                    return lo;
                }
            }

            // 없으면 생성
            LandObject lob = Instantiate(DataManager.ObstacleFabs[type]).GetComponent<LandObject>();
            _obstacleList.Add(lob);
            lob.transform.position = pos;
            lob.FixInit(_gs, tile, _clm.Season);

            lob.transform.parent = transform;
            return lob;
        }
    }
}