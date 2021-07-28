using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerLevelData
{
    public int level;
    public int hp;
    public int mp;
    public int maxExp;
}

public class GlobalData : SingletonMonoBehavior<GlobalData>
{
    public List<PlayerLevelData> playerDatas;
}
