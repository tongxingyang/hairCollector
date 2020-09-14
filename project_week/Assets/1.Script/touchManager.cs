using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class touchManager : MonoBehaviour
    {
        public void Init()
        {
            StartCoroutine(touch());
        }

        IEnumerator touch()
        {
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SoundManager.instance.PlaySFX(SFX.click);
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}