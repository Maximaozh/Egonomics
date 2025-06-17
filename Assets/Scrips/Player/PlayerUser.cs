using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUser : PlayerBase
{
    public override void TakeTurn()
    {
        base.TakeTurn();

    }

    public override void EndTurn()
    {
        base.EndTurn();
    }

    //public void Update()
    //{
    //    if (TurnManager.Instance.CurrentPlayerIndex == 0 && TurnManager.Instance.TurnCount < 31)
    //    {
    //        DataLogger.Instance.Clear();
    //        EndTurn();
    //    }
    //    else if (TurnManager.Instance.TurnCount >= 31)
    //    {
    //        string currentSceneName = SceneManager.GetActiveScene().name;
    //        SceneManager.LoadScene(currentSceneName);
    //    }
    //}
}
