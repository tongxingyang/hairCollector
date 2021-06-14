using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class recommendPopup : MonoBehaviour, UIInterface
    {
        [SerializeField] GameObject recommend;
        [SerializeField] GameObject thanks;

        bool _isGoRecom;

        private void Awake()
        {
            _isGoRecom = false;
            open();
            gameObject.SetActive(false);
        }

        public void open()
        {
            gameObject.SetActive(true);
            recommend.SetActive(true);
            thanks.SetActive(false);
        }

        public void close()
        {
            gameObject.SetActive(false);
        }

        /// <summary> 리뷰링크이동 </summary>
        public void goRecommend()
        {
            // 링크이동

            _isGoRecom = true;

            BaseManager.userGameData.Success_Recommend = true;
            BaseManager.instance.saveDeviceData();

            recommend.SetActive(false);
            thanks.SetActive(true);
        }

        public void backPanel()
        {
            if (_isGoRecom)
            {
                close();
            }
        }
    }
}