using System;
using UnityEngine;

[Serializable]
public class SaveInt 
{
    [SerializeField]
    string key;
    [SerializeField]
    int value;
    public SaveInt(string _key)
    {
        //key = Application.dataPath + GetType() + _key;
        key = GetType() + _key;

        if (PlayerPrefs.HasKey(key))
            value = PlayerPrefs.GetInt(key);
        else
            value = 0;
    }

    public int Value
    {
        get
        {
            return value;
        }
        set
        {
            if (this.value != value)
            {
                PlayerPrefs.SetInt(key, value);
                PlayerPrefs.Save();
            }
            this.value = value;
        }
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}