using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace week
{
    public class dmgFontControl : MonoBehaviour
    {
        TextMeshProUGUI _dmgText;

        public bool isUse { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            _dmgText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Update is called once per frame
        public void Init(string val, dmgTxtType type = dmgTxtType.standard)
        {
            isUse = true;            
            _dmgText.text = val;

            switch (type)
            {
                case dmgTxtType.standard:
                    _dmgText.color = Color.red;
                    break;
                case dmgTxtType.shield:
                    _dmgText.color = Color.cyan * 0.9f;
                    break;
                case dmgTxtType.heal:
                    _dmgText.color = Color.green * 0.9f;
                    break;
                case dmgTxtType.att:
                    _dmgText.color = new Color(1f, 1f, 0f) * 0.9f;
                    break;
                case dmgTxtType.def:
                    _dmgText.color = Color.blue * 0.9f;
                    break;
            }

            gameObject.SetActive(true);
            StartCoroutine(fontAni());
        }

        IEnumerator fontAni()
        {
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime;
                transform.position += Vector3.up * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            gameObject.SetActive(false);
            isUse = false;
        }
    }
}