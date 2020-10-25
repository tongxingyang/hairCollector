using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class TweenAnim : MonoBehaviour
{
    public enum AnimType
    {
        Fade,
        Scale,
    }

    public AnimType AnimationType = AnimType.Fade;

    ITweenAnimSub tweenAnimSub;

    [SerializeField] float openTime = 0.3f;
    [SerializeField] float closeTime = 0.2f;
    [SerializeField] bool CloseDisable = true;

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (AnimationType == AnimType.Fade)
        {
            tweenAnimSub = FadeAnim.instance;
        }
        else if (AnimationType == AnimType.Scale)
        {
            tweenAnimSub = ScaleAnim.instance;
        }
    }

    void OnEnable() => tweenAnimSub.OnEnable(this);

    void OnDisable() => tweenAnimSub.OnDisable(this);

    void OnOpenComplete()
    {

    }

    public void Close()
    {
        tweenAnimSub.Close(this);
    }

    void OnCloseComplete()
    {
        if (CloseDisable)
            gameObject.SetActive(false);
    }

    [Button]
    public void AddCanvasGroup() => gameObject.AddComponent<CanvasGroup>();

    interface ITweenAnimSub
    {
        void OnEnable(TweenAnim tweenAnim);
        void Close(TweenAnim tweenAnim);
        void OnDisable(TweenAnim tweenAnim);
    }


    /// <summary>
    /// 트윈 애니메이션을 정의하는 내부 클래스
    /// </summary>
    class FadeAnim : ITweenAnimSub
    {
        public static FadeAnim instance = new FadeAnim();

        public void OnEnable(TweenAnim tweenAnim)
        {
            tweenAnim.canvasGroup.alpha = 0f;
            tweenAnim.canvasGroup.DOFade(1, tweenAnim.openTime)
                .OnComplete(tweenAnim.OnOpenComplete)
                .SetUpdate(true);
        }

        public void Close(TweenAnim tweenAnim)
        {
            tweenAnim.canvasGroup.alpha = 1f;
            tweenAnim.canvasGroup.DOFade(0, tweenAnim.closeTime)
               .OnComplete(tweenAnim.OnCloseComplete)
                .SetUpdate(true);
        }

        public void OnDisable(TweenAnim tweenAnim)
        {
            tweenAnim.canvasGroup.DOKill();
        }
    }

    class ScaleAnim : ITweenAnimSub
    {
        public static ScaleAnim instance = new ScaleAnim();

        public void OnEnable(TweenAnim tweenAnim)
        {
            tweenAnim.transform.localScale = Vector3.zero;
            tweenAnim.transform.DOScale(Vector3.one, tweenAnim.openTime)
                .SetEase(Ease.OutBack)
                .OnComplete(tweenAnim.OnOpenComplete)
                .SetUpdate(true);
        }

        public void Close(TweenAnim tweenAnim)
        {
            tweenAnim.transform.localScale = Vector3.one;
            tweenAnim.transform.DOScale(Vector3.zero, tweenAnim.closeTime)
               .SetEase(Ease.InBack)
               .OnComplete(tweenAnim.OnCloseComplete)
               .SetUpdate(true);
        }

        public void OnDisable(TweenAnim tweenAnim)
        {
            tweenAnim.transform.DOKill();
        }
    }
}