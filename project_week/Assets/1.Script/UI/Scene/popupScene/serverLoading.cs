using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class serverLoading : MonoBehaviour, UIInterface
    {
        public bool IsOpen { get; set; }

        void Awake()
        {
            close();
        }

        public void open()
        {
            IsOpen = true;
            gameObject.SetActive(true);
        }

        public void close()
        {
            IsOpen = false;
            gameObject.SetActive(false);
        }
    }
}