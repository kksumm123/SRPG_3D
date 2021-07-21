using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorStateUI : SingletonMonoBehavior<ActorStateUI>
{
    Text nickName;
    Text status;

    public void Show(Actor actor)
    {
        base.Show();
        nickName = transform.Find("Status").GetComponent<Text>();
        status = transform.Find("Status").GetComponent<Text>();

        nickName.text = actor.nickName;
        status.text = actor.status.ToString();
    }
}
