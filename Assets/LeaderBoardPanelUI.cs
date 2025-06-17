using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderBoardPanelUI : MonoBehaviour
{
    public TMP_Text text;
    public TurnManager tm;

    public void Start()
    {
        Configure();
    }
    
    private void OnEnable()
    {
        Configure();
    }
    private void OnDisable()
    {
        text.text = "";
    }


    public void Configure()
    {
        var players = tm.Players.OrderByDescending(x => x.Portfolio.funds);
        text.text = ""; 

        int rank = 1;
        foreach (var player in players)
        {
            Color playerColor = BuildManagerExpanded.Instance.GenerateColorFromString(player.playerName);
            string colorHex = ColorUtility.ToHtmlStringRGB(playerColor);

            text.text += string.Format(
                "#{0}\t<color=#{1}>{2,-15}</color>\t{3}\n",
                rank,
                colorHex,
                player.playerName,
                player.Portfolio.funds
            );
            rank++;
        }
    }
}
