using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorStateUI : SingletonMonoBehavior<ActorStateUI>
{
    Text nickName;
    Text status;
    RectTransform hpBarGauge;
    RectTransform hpBar;
    RectTransform mpBarGauge;
    RectTransform mpBar;
    Image hpBarGaugeImage;
    Image mpBarGaugeImage;


    public void Show(Actor actor)
    {
        base.Show();
        nickName = transform.Find("Name").GetComponent<Text>();
        status = transform.Find("State").GetComponent<Text>();

        hpBarGauge = transform.Find("HPBar/Gauge").GetComponent<RectTransform>();
        hpBar = transform.Find("HPBar/BG").GetComponent<RectTransform>();
        mpBarGauge = transform.Find("MPBar/Gauge").GetComponent<RectTransform>();
        mpBar = transform.Find("MPBar/BG").GetComponent<RectTransform>();
        hpBarGaugeImage = hpBarGauge.GetComponent<Image>();
        mpBarGaugeImage = mpBarGauge.GetComponent<Image>();

        var size = hpBarGauge.sizeDelta;
        size.x = actor.maxHP;
        hpBarGauge.sizeDelta = size;
        hpBar.sizeDelta = size;

        size = mpBarGauge.sizeDelta;
        size.x = actor.maxMP;
        mpBarGauge.sizeDelta = size;
        mpBar.sizeDelta = size;

        hpBarGaugeImage.fillAmount = actor.hp / actor.maxHP;
        mpBarGaugeImage.fillAmount = actor.mp / actor.maxMP;

        nickName.text = actor.nickName;
        status.text = actor.status.ToString();
    }
}
