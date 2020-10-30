using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class BuffParticleManager : MonoBehaviour
    {
        [SerializeField] GameObject[] _particleFabs;

        GameObject[] _particle;

        private void Awake()
        {
            _particle = new GameObject[5];
        }

        public void getBuffParticle(BuffEffect.buffNamed name)
        {
            int num = (int)name;
            if (_particle[num] == null)
            {
                _particle[num] = Instantiate(_particleFabs[num]);
                _particle[num].transform.parent = transform;
                _particle[num].transform.localPosition = Vector3.zero;
            }

            _particle[num].SetActive(true);
        }

        public void Buffoff(BuffEffect.buffNamed name)
        {
            int num = (int)name;
            if (_particle[num] != null)
            {
                _particle[num].SetActive(false);
            }
        }
    }
}