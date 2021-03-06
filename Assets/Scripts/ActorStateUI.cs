using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorStateUI : SingletonMonoBehavior<ActorStateUI>
{
    Text nickName;
    Image icon;
    Text status;
    RectTransform hpBarGauge;
    RectTransform hpBar;
    RectTransform mpBarGauge;
    RectTransform mpBar;
    Image hpBarGaugeImage;
    Image mpBarGaugeImage;
    protected override void OnInit()
    {
        base.OnInit();
    }

    public void Show(Actor actor)
    {
        base.Show();
        nickName = transform.Find("Name").GetComponent<Text>();
        icon = transform.Find("Icon").GetComponent<Image>();
        status = transform.Find("State").GetComponent<Text>();

        hpBarGauge = transform.Find("HPBar/Gauge").GetComponent<RectTransform>();
        hpBar = transform.Find("HPBar/BG").GetComponent<RectTransform>();
        mpBarGauge = transform.Find("MPBar/Gauge").GetComponent<RectTransform>();
        mpBar = transform.Find("MPBar/BG").GetComponent<RectTransform>();
        hpBarGaugeImage = hpBarGauge.GetComponent<Image>();
        mpBarGaugeImage = mpBarGauge.GetComponent<Image>();

        var size = hpBarGauge.sizeDelta;
        size.x = actor.maxHp;
        hpBarGauge.sizeDelta = size;
        hpBar.sizeDelta = size;

        size = mpBarGauge.sizeDelta;
        size.x = actor.maxMp;
        mpBarGauge.sizeDelta = size;
        mpBar.sizeDelta = size;

        hpBarGaugeImage.fillAmount = actor.hp / actor.maxHp;
        mpBarGaugeImage.fillAmount = actor.mp / actor.maxMp;

        nickName.text = actor.nickName;
        icon.sprite = Resources.Load<Sprite>($"Icon/{actor.iconName}");
        status.text = actor.status.ToString();
    }
}
