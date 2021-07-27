using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameStateType
{
    NotInit, //초기화 전
    SelectPlayer, //조정할 아군 선택, 선택된 플레이어가 갈 수 있는 영역과 공격 가능한 영역 표시
    SelectedPlayerMoveOrAct, // 이동 혹은 공격 타겟을 선택 
    IngPlayerMove, // 플레이어 이동중
    SelectToAttackTarget, // 이동 후 공격할 타겟 선택, 공격할 타겟 없으면 SelectPlayer로
    AttackTartget,
    MonsterTurn, // 모든 플레이어 턴 종료 후 몬스터 턴 전환
}
public class StageManager : SingletonMonoBehavior<StageManager>
{
    [SerializeField] private GameStateType gameState;

    public static GameStateType GameState
    {
        get => Instance.gameState;
        set
        {
            Debug.Log($"{Instance.gameState} -> {value}");
            NotifyUI.Instance.Show(value.ToString(), 10);
            Instance.gameState = value;
        }
    }
    void Start()
    {
        OnStartTurn();
        //CenterNotifyUI.Instance.Show("게임이 시작되었습니다.", 1.5f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            ContextMenuUI.Instance. ShoStageMenu(Input.mousePosition);
    }

    public void EndTurnPlayer()
    {
        GameState = GameStateType.MonsterTurn;
        StartCoroutine(MonsterTurnCo());
    }

    IEnumerator MonsterTurnCo()
    {
        foreach (var monster in Monster.Monsters)
        {
            yield return monster.AutoAttackCo();
        }
        ProcessNextTurn();
    }
    
    int turn = 1;
    private void ProcessNextTurn()
    {
        // 몇 번째 턴인지 보여주자
        turn++;

        // 턴이 시작되면
        OnStartTurn();
    }

    private void OnStartTurn()
    {
        FollowTarget.Instance.SetTarget(Player.Players[0].transform);
        // 게임 상태를 SelectPlayer
        ShowCurrentTurn();
        // 턴 정보 초기화 (completeMove, Act)
        GameState = GameStateType.SelectPlayer;
    }

    private void ShowCurrentTurn()
    {
        CenterNotifyUI.Instance.Show($"{turn}번째 턴이 시작되었습니다");
    }
}
