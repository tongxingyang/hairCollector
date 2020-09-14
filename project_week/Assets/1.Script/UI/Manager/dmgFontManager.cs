using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class dmgFontManager : MonoBehaviour
    {
        [SerializeField] GameObject _fontPrefabs;
        [SerializeField] Camera _main;

        List<dmgFontControl> _fonts;

        public bool Toggle { get; set; }

        // Start is called before the first frame update
        public void Init()
        {
            _fonts = new List<dmgFontControl>();
            Toggle = false;
        }

        /// <summary> 데미지 폰트 pool </summary>
        /// <param name="tr"> 생성위치 </param>
        /// <param name="val"> 값 </param>
        /// <param name="type"> 타입 </param>
        /// <param name="force"> 강제표시여부 </param>
        public void getText(Transform tr, int val, dmgTxtType type = dmgTxtType.standard, bool force = false)
        {
            if (!force && !Toggle)
            {
                return;
            }

            for (int i = 0; i < _fonts.Count; i++)
            {
                if (_fonts[i].isUse == false)
                {
                    _fonts[i].transform.position = tr.position; //_main.WorldToScreenPoint(tr.position + setPos());
                    _fonts[i].Init(val, type);
                    return;
                }
            }

            dmgFontControl dfc = Instantiate(_fontPrefabs).GetComponent<dmgFontControl>();
            dfc.transform.SetParent(transform);
            dfc.transform.localScale = Vector3.one;
            _fonts.Add(dfc);
            dfc.transform.position = tr.position;// _main.WorldToScreenPoint(tr.position + setPos());
            dfc.Init(val, type);
        }

        Vector3 setPos()
        {
            return Vector3.zero;
            Vector3 pos = Random.insideUnitCircle;
            pos.y = Mathf.Abs(pos.y);
            return pos * 0.3f;
        }
    }
}