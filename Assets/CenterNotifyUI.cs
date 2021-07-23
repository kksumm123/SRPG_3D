using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CenterNotifyUI : SingletonMonoBehavior<CenterNotifyUI>
{
    Text contentText;
    CanvasGroup canvasGroup;
    protected override void OnInit()
    {
        base.OnInit();
        canvasGroup = GetComponent<CanvasGroup>();
        contentText = transform.Find("ContentText").GetComponent<Text>();
    }

    public void Show(string text, float visibleTime = 3f)
    {
        base.Show();
        canvasGroup.alpha = 1;
        contentText.text = text;

        canvasGroup.DOFade(0, 1).SetDelay(visibleTime).OnComplete(Close);
    }
}
