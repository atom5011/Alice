using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class BlinkIcon : MonoBehaviour
{
    public float duration;
    public Ease easeType;

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.DOFade(0.0f, duration).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
    }
}