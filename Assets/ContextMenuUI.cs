﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContextMenuUI : BaseUI<ContextMenuUI>
{

    public GameObject baseItem;

    protected override void OnInit()
    {
        Dictionary<string, UnityAction> menus = new Dictionary<string, UnityAction>();
        baseItem = transform.Find("BG/Button").gameObject;

        menus.Add("턴 종료 (F10)", EndTurnPlayer);
        menus.Add("테스트 메뉴", TestMenu);
        menus.Add("무명 함수 테스트", () => Debug.Log("무명함수"));

        foreach (var item in menus)
        {
            GameObject go = Instantiate(baseItem, baseItem.transform.parent);
            go.GetComponentInChildren<Text>().text = item.Key;
            go.GetComponent<Button>().AddListener(this, item.Value);
        }
        baseItem.SetActive(false);
    }


    private void EndTurnPlayer()
    {
        throw new NotImplementedException();
    }
    private void TestMenu()
    {
        throw new NotImplementedException();
    }
}
