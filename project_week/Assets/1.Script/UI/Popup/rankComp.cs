﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class rankComp : MonoBehaviour, UIInterface
    {
        [SerializeField] GameObject _rankBoxFab;
        [SerializeField] Transform _boxParent;

        List<rankBox> _boxes;
        
        float _refTime;
        float _reqTime;
        readonly float _cool = 5f;

        public void Init()
        {
            _boxes = new List<rankBox>();
            _refTime = _cool + 1f;
            _reqTime = _cool + 1f;

            settingRankBoxes();
        }

        /// <summary> 랭킹 로드 </summary>
        public void setRankBoxes()
        {
            if (AuthManager.instance.networkCheck() == false)
            {
                WindowManager.instance.Win_message.showMessage("네트워크가 없어눈~");
                return;
            }

            if (_refTime > _cool)
            {
                _refTime -= _cool;
                settingRankBoxes();
            }
            else
            {
                WindowManager.instance.Win_message.showMessage($"잠시만요~ (쿨타임 : {Convert.ToInt32(_cool - _refTime)}초)");
            }
        }

        void settingRankBoxes()
        {
            // yield return StartCoroutine(AuthManager.instance.loadRankDataFromFB());
            NanooManager.instance.getRankingTotal((dictionary) => 
            {
                ArrayList list = (ArrayList)dictionary["list"];
                int i = 0;

                foreach (Dictionary<string, object> rank in list)
                {
                    if (_boxes.Count <= i)
                    {
                        rankBox box = Instantiate(_rankBoxFab).GetComponent<rankBox>();
                        box.transform.SetParent(_boxParent);
                        _boxes.Add(box);
                    }

                    rankData data = JsonUtility.FromJson<rankData>((string)rank["data"]);

                    _boxes[i].setRankBox(i, rank["nickname"].ToString(), int.Parse((string)rank["score"]), data);
                    i++; 
                }
            });
        }

        /// <summary> 내 랭킹 어필 </summary>
        public void uploadRequest()
        {
            if (AuthManager.instance.networkCheck() == false)
            {
                WindowManager.instance.Win_message.showMessage("네트워크가 없어용~");
                return;
            }

            if (BaseManager.userGameData._minRank < BaseManager.userGameData.TimeRecord)
            {
                if (_reqTime > _cool)
                {
                    _reqTime -= _cool;
                    // AuthManager.instance.saveRankDataFromFB();
                    WindowManager.instance.Win_message.showMessage("접수되었습니다." + System.Environment.NewLine + "반영까지 쬐금만 기달려주세요");
                }
                else
                {
                    WindowManager.instance.Win_message.showMessage($"잠시만요~ (쿨타임 : {Convert.ToInt32(_cool - _refTime)}초)");
                }
            }
            else
            {
                WindowManager.instance.Win_message.showMessage("랭킹 반영안된거 같을때 눌러줭");
            }
        }

        void Update()
        {
            if (_refTime <= _cool)
            {
                _refTime += Time.deltaTime;
            }

            if (_reqTime <= _cool)
            {
                _reqTime += Time.deltaTime;
            }
        }

        public void open()
        {
            gameObject.SetActive(true);

            if (_refTime > _cool)
            {
                _refTime -= _cool;
                settingRankBoxes();
            }
        }

        public void close()
        {
            gameObject.SetActive(false);
        }
    }
}