using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class effControl : MonoBehaviour
    {
        [SerializeField] effAni ee;
        SpriteRenderer _renderer;
        Animator _animator;

        public bool isUse { get; set; }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        public void Init(string effName)
        {
            // ee = efa;
            isUse = true;
            gameObject.SetActive(true);
            _animator.SetTrigger(effName.ToString());
        }

        public void endOfAnimation()
        {
            gameObject.SetActive(false);
            isUse = false;
        }

        public void Destroy()
        {
            isUse = false;
            gameObject.SetActive(false);
        }

        public void onPause(bool pause)
        {
            _animator.speed = (pause) ? 0 : 1f;
        }
    }
}