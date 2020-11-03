using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace week
{
    public class tileBase : MonoBehaviour
    {
        [SerializeField] SpriteRenderer[] _renders;

        GameScene _gs;
        ObstacleManager _obm;
        MapManager _mpm;

        mapObstacle _mapType;
        public mapObstacle MapType { get => _mapType; set => _mapType = value; }

        public Action reclaim;


        public void setOBMng(GameScene gs, season ss)
        {
            _gs = gs;
            _obm = _gs.ObtMng;
            _mpm = _gs.MapMng;

            _renders[0].gameObject.SetActive(false);
            _renders[1].gameObject.SetActive(true);

            _renders[1].sprite = _mpm.SeasonMaps[(int)ss];
        }

        public void setTransform(Vector3 vec, bool first = false)
        {
            transform.position = vec;

            if (reclaim != null)
            {
                reclaim();
                reclaim = null;
            }

            makeOb(first);
        }

        public void changeSeasonMap(season ss)
        {
            StartCoroutine(changeSeason(ss));
        }

        IEnumerator changeSeason(season ss)
        {
            _renders[0].gameObject.SetActive(true);
            _renders[1].gameObject.SetActive(true);

            _renders[0].sprite = _renders[1].sprite;
            _renders[1].sprite = _mpm.SeasonMaps[(int)ss];

            Color prev = Color.white;
            Color next = Color.white;
            next.a = 0;

            _renders[0].color = prev;
            _renders[1].color = next;

            while (next.a < 1f)
            {
                prev.a -= Time.deltaTime;
                next.a += Time.deltaTime;

                _renders[0].color = prev;
                _renders[1].color = next;

                yield return new WaitForEndOfFrame();
            }

            _renders[0].gameObject.SetActive(false);
        }

        public void makeOb(bool first = false)
        { 
            int ran = UnityEngine.Random.Range(0, 201);
            mapObstacle mo;

            if (ran < 5) // 2.5퍼
            {
                mo = mapObstacle.bosszone;
            }
            else if(ran > 195) // 2.5퍼
            {
                mo = (mapObstacle)UnityEngine.Random.Range((int)mapObstacle.ruin0, (int)mapObstacle.max);
            }
            else
            {
                mo = (mapObstacle)UnityEngine.Random.Range((int)mapObstacle.map_0, (int)mapObstacle.ruin0);
            }

            if (first)
            {
                mo = (mapObstacle)UnityEngine.Random.Range((int)mapObstacle.map_0, (int)mapObstacle.gem_0);
            }

            _mapType = mo;
            _obm.getObstacle(mo, this, transform.position).transform.position = transform.position;
        }
    }
}