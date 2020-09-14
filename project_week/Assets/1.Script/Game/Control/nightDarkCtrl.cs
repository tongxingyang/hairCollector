using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class nightDarkCtrl : MonoBehaviour
    {
        [SerializeField] Image _dark;

        GameScene _gs;
        float _alpha;

        public void Init(GameScene gs)
        {
            _gs = gs;
        }

        public void startNight(float a)
        {
            _alpha = a;
            gameObject.SetActive(true);
            StartCoroutine(startNight());
        }

        IEnumerator startNight()
        {
            Color col = Color.white;
            col.a = 0;
            _dark.color = col;

            while (col.a < _alpha)
            {
                col.a += Time.deltaTime * _alpha;
                _dark.color = col;

                yield return new WaitUntil(() => _gs.Pause == false);
            }
        }

        public void endNight()
        {
            StartCoroutine(IEendNight());
        }

        IEnumerator IEendNight()
        {
            Color col = Color.white;
            col.a = _dark.color.a;

            while (col.a > 0)
            {
                col.a -= Time.deltaTime * _alpha;
                _dark.color = col;

                yield return new WaitUntil(() => _gs.Pause == false);
            }
            gameObject.SetActive(false);
        }
    }
}