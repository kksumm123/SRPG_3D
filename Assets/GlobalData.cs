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

    public override string ToString()
    {
        return $"ID/allowLevel/sellPrice/buyPrice : {ID}/{allowLevel}/{iconName}/{sellPrice}/{buyPrice}";
    }
}
[System.Serializable]
public class ItemDropInfo
{
    public int dropItemID;
    public float ratio; // 드랍확률
    public override string ToString()
    {
        return $"{dropItemID}, {ratio}";
    }
}
[System.Serializable]
public class DropItemGroupData
{
    public int ID; //그룹에 대한 ID
    public List<ItemDropInfo> dropItems;
}

public class GlobalData : MonoBehaviour
{
    public static GlobalData Instance;
    private void Awake()
    {
        Instance = this;
        playerDataMap = playerDatas.ToDictionary(x => x.level);
        itemDataMap = itemDatas.ToDictionary(x => x.ID);
        dropItemGroupDataMap = dropItemGroupData.ToDictionary(x => x.ID);
    }
    [SerializeField] List<PlayerLevelData> playerDatas; // = new List<PlayerLevelData>();
    public Dictionary<int, PlayerLevelData> playerDataMap;

    [SerializeField] List<ItemData> itemDatas = new List<ItemData>();
    public Dictionary<int, ItemData> itemDataMap;

    [SerializeField] List<DropItemGroupData> dropItemGroupData = new List<DropItemGroupData>();
    public Dictionary<int, DropItemGroupData> dropItemGroupDataMap;
    //protected override void OnInit()
    //{ // 커밋했습니다 ! 
    //    playerDataMap = playerDatas.ToDictionary(x => x.level);
    //    itemDataMap = itemDatas.ToDictionary(x => x.ID);
    //    dropItemGroupDataMap = dropItemGroupData.ToDictionary(x => x.ID);
    //}
}
