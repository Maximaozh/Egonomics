using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVisibilityBlock : MonoBehaviour
{
    public GameObject uiBlock;
    public int playerID = 0;

    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += OnTurnChanged;
        UpdateVisibility();
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnTurnChanged -= OnTurnChanged;
    }

    private void OnTurnChanged(int currentPlayerID)
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        uiBlock.SetActive(TurnManager.Instance.CurrentPlayerIndex == playerID);
    }
}