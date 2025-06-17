using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AmbientManager : MonoBehaviour
{
    [Header("Аудиоэлементы")]
    [SerializeField] private List<AudioClip> playerAmbient;
    [SerializeField] private List<AudioClip> enemyAmbient;

    [SerializeField] private AudioSource audioSource;

    private int lastPlayerId;
    private int currentPlayerId;
    private TurnManager tm;

    private bool isPlayerTurn;
    private bool wasPlayerTurn;

    public List<AudioClip> PlayerAmbient { get => playerAmbient; set => playerAmbient = value; }
    public List<AudioClip> EnemyAmbient { get => enemyAmbient; set => enemyAmbient = value; }
    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }

    private bool updateTurnState() => isPlayerTurn = tm.CurrentPlayerIndex == 0;

    public void Start()
    {
        tm = TurnManager.Instance;
        AudioSource = this.gameObject.GetComponent<AudioSource>();
        lastPlayerId = -1;
        currentPlayerId = -1;

        wasPlayerTurn = false;
        updateTurnState();
    }

    public void LateUpdate()
    {
        updateTurnState();
        if (isPlayerTurn != wasPlayerTurn)
        {
            ChangeAmbient();
            wasPlayerTurn = isPlayerTurn;
        }

        if (!AudioSource.isPlaying)
        {
            ContinueAmbient();
        }
    }

    public void ChangeAmbient()
    {
        AudioSource.Stop();

        List<AudioClip> targetList = isPlayerTurn ? PlayerAmbient : EnemyAmbient;
        AudioClip selectedClip = targetList[UnityEngine.Random.Range(0, targetList.Count)];

        if (selectedClip == null)
            return;

        AudioSource.clip = selectedClip;
        AudioSource.Play();
    }


    private void ContinueAmbient()
    {
        
        AudioSource.Stop();

        try
        {

            List<AudioClip> currentList = isPlayerTurn ? PlayerAmbient : EnemyAmbient;


            AudioClip nextClip = currentList.DistinctBy(x => x == AudioSource.clip).ToList()[UnityEngine.Random.Range(0, currentList.Count)];

            if (nextClip == null)
                return;

            AudioSource.clip = nextClip;

        }
        catch (Exception ex)
        { 

        }
        AudioSource.Play();
    }
}
