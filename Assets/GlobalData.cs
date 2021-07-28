using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[System.Serializable]
public class PlayerLevelData
{
    public int level;
    public int maxHp;
    public int maxMp;
    public int maxExp;
}
[System.Serializable]
public class ItemData
{
    public int ID;
    public int allowLevel;
    public int sellPrice;
    public int buyPrice;
    public string iconName;
}
[System.Serializable]
public class ItemDropInfo
{
    public int dropItemID;
    public float ratio; // 드랍확률
}
[System.Serializable]
public class DropItemGroupData
{
    public int ID;
    public List<ItemDropInfo> dropItems;
}

public class GlobalData : SingletonMonoBehavior<GlobalData>
{
    [SerializeField] List<PlayerLevelData> playerDatas = new List<PlayerLevelData>();
    public Dictionary<int, PlayerLevelData> playerDataMap;

    [SerializeField] List<ItemData> itemDatas = new List<ItemData>();
    public Dictionary<int, ItemData> itemDataMap;

    [SerializeField] List<DropItemGroupData> dropItemGroupData = new List<DropItemGroupData>();
    public Dictionary<int, DropItemGroupData> dropItemGroupDataMap;
    protected override void OnInit()
    {
        playerDataMap = playerDatas.ToDictionary(x => x.level);
        itemDataMap = itemDatas.ToDictionary(x => x.ID);
        dropItemGroupDataMap = dropItemGroupData.ToDictionary(x => x.ID);
    }
}
