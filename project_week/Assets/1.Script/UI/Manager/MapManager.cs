using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class MapManager : MonoBehaviour
    {
        #region

        [Serializable]
        class Tile
        {
            [NonSerialized]
            public bool _isUse;
            public tileBase _map;
            SpriteRenderer _sprite;
            Transform tr;

            public Transform Tr
            {
                get
                {
                    if (tr == null)
                    {
                        tr = _map.GetComponent<Transform>();
                    }
                    return tr;
                }
            }
        }

        #endregion

        [SerializeField] Tile[] _tiles;
        [Space]
        [SerializeField] Sprite[] _seasonMaps;
        gameCompass _compass;

        Tile[] _temp;
        GameScene _gs;
        ObstacleManager _obm;
        clockManager _clm;

        public Vector3 middleTilePos
        {
            get { return _tiles[4].Tr.position; }
        }

        public Sprite[] SeasonMaps { get => _seasonMaps; set => _seasonMaps = value; }

        public void Init(GameScene gs)
        {
            _gs = gs;
            _obm = _gs.ObtMng;
            _clm = _gs.ClockMng;
            // _compass = _gs.Compass;
            _temp = new Tile[3];

            setMapObj(true); 
            
            for (int i = 0; i < 9; i++)
            {
                _clm.changeSS += _tiles[i]._map.changeSeasonMap;
            }
        }

        public void setMapObj(bool first = false)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (_tiles[i * 3 + j]._isUse == false)
                    {
                        _tiles[i * 3 + j]._map.setOBMng(_gs, _clm.Season);
                        _tiles[i * 3 + j]._map.setTransform(_tiles[i * 3 + j].Tr.position, first);
                        _tiles[i * 3 + j]._isUse = true;
                    }
                }
            }

            //_compass.newArea();

            //for (int i = 0; i < 3; i++)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        if (_tiles[i * 3 + j]._map.MapType == mapObstacle.bosszone)
            //        {
            //            _compass.chkCompass(_tiles[i * 3 + j].Tr, true);
            //        }
            //        else if(_tiles[i * 3 + j]._map.MapType >= mapObstacle.ruin0)
            //        {
            //            _compass.chkCompass(_tiles[i * 3 + j].Tr, false);
            //        }
            //    }
            //}
        }

        #region [map move]

        public void playerMoveUp()
        {
            for (int i = 0; i < 3; i++)
            {
                _tiles[6 + i].Tr.position = _tiles[i].Tr.position + new Vector3(0, 20f);
                _tiles[6 + i]._isUse = false;
                _temp[i] = _tiles[6 + i];
            }

            for (int i = 0; i < 6; i++)
            {
                _tiles[8 - i] = _tiles[5 - i];
            }

            for (int i = 0; i < 3; i++)
            {
                _tiles[i] = _temp[i];
            }

            setMapObj();
        }

        public void playerMoveDown()
        {
            for (int i = 0; i < 3; i++)
            {
                _tiles[i].Tr.position = _tiles[6 + i].Tr.position + new Vector3(0, -20f);
                _tiles[i]._isUse = false;
                _temp[i] = _tiles[i];
            }

            for (int i = 0; i < 6; i++)
            {
                _tiles[i] = _tiles[3 + i];
            }

            for (int i = 0; i < 3; i++)
            {
                _tiles[i + 6] = _temp[i];
            }

            setMapObj();
        }

        public void playerMoveRight()
        {
            for (int i = 0; i < 3; i++)
            {
                _tiles[i * 3].Tr.position = _tiles[i * 3 + 2].Tr.position + new Vector3(20f, 0);
                _tiles[i * 3]._isUse = false;
                _temp[i] = _tiles[i * 3];
            }

            for (int i = 0; i < 3; i++)
            {
                _tiles[i * 3] = _tiles[i * 3 + 1];
                _tiles[i * 3 + 1] = _tiles[i * 3 + 2];
            }

            for (int i = 0; i < 3; i++)
            {
                _tiles[i * 3 + 2] = _temp[i];
            }

            setMapObj();
        }

        public void playerMoveLeft()
        {
            for (int i = 0; i < 3; i++)
            {
                _tiles[i * 3 + 2].Tr.position = _tiles[i * 3].Tr.position + new Vector3(-20f, 0);
                _tiles[i * 3 + 2]._isUse = false;
                _temp[i] = _tiles[i * 3 + 2];
            }

            for (int i = 0; i < 3; i++)
            {
                _tiles[i * 3 + 2] = _tiles[i * 3 + 1];
                _tiles[i * 3 + 1] = _tiles[i * 3];
            }

            for (int i = 0; i < 3; i++)
            {
                _tiles[i * 3] = _temp[i];
            }

            setMapObj();
        }

        #endregion
    }
}