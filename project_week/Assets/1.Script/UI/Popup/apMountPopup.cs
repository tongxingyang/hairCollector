using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace week
{
    public class apMountPopup : MonoBehaviour, UIInterface
    {
        [SerializeField] TextMeshProUGUI _mount;

        snowmanComp _snow;

        public void open()
        {
            gameObject.SetActive(true);
        }

        public void close()
        {
            _snow.refresh_AboutAp();
            gameObject.SetActive(false);
        }

        public void Init(snowmanComp snow)
        {
            _snow = snow;

            close();
        }

        public void setOpen()
        {
            open();

            _mount.text = _snow._reservAp.ToString();
        }

        public void updown_apMount(int add)
        {
            int max = BaseManager.userGameData.Coin / 1000;

            if (_snow._reservAp > 0 && _snow._reservAp < max)
            {
                _snow._reservAp += add;
            }
            else if (_snow._reservAp < 1)
            {
                _snow._reservAp = 1;
            }
            else
            {
                _snow._reservAp = max;
            }

            _mount.text = _snow._reservAp.ToString();
            _snow.refresh_AboutAp();
        }
    }
}