﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functions to complete:
/// - Attack
/// </summary>
public class FightManager : MonoBehaviour
{
    public BattleSystem battleSystem; //A reference to our battleSystem script in our scene
    public Color drawCol = Color.gray; // A colour you might want to set the battle log message to if it's a draw.
    private float fightAnimTime = 2; //An amount to wait between initiating the fight, and the fight begining, so we can see some of that sick dancing.

    //TODO this function is all you need to modify, in this script.
    //You just need determine who wins/loses/draws etc.
    IEnumerator Attack(Character teamACharacter, Character teamBCharacter)
    {
        float outcome = 0; // the outcome from the fight, i.e. the % that the winner has won by...fractions could help us calculate this, but start with whole numbers i.e. 0 = draw, and 1 = 100% win.
        Character winner = teamACharacter; //Creating the winner variable, defaulting to Team A to assist with draw outcomes
        Character defeated = teamBCharacter; //Creating the defeated variable, defaulting to Team B to assist with draw outcomes

        // Tells each dancer that they are selcted and sets the animation to dance.
        SetUpAttack(teamACharacter);
        SetUpAttack(teamBCharacter);

        // Tells the system to wait X number of seconds until the fight to begins.
        yield return new WaitForSeconds(fightAnimTime);

        // Create character results from the fight
        int teamABattlePoints = teamACharacter.ReturnBattlePoints();
        int teamBBattlePoints = teamBCharacter.ReturnBattlePoints();
        
        //If Team A has more points than Team B, Team A wins and the outcome is determined.
        if (teamABattlePoints > teamBBattlePoints)
        {
            Debug.Log("Team A has won with" + teamABattlePoints + "points!");
            outcome = 1 - ((float)teamBBattlePoints / (float)teamABattlePoints);
            winner = teamACharacter;
            defeated = teamBCharacter;
        }
        //If Team B has more points than Team A, Team B wins and the outcome is determined.
        else if (teamBBattlePoints > teamABattlePoints)
        {
            Debug.Log("Team B has won with" + teamBBattlePoints + "points!");
            outcome = 1 - ((float)teamABattlePoints / (float)teamBBattlePoints);
            winner = teamBCharacter;
            defeated = teamACharacter;
        }
        //If Team A and Team B have the same amount of points, it's a draw and the outcome is set.
        else if (teamBBattlePoints == teamABattlePoints)
        {
            Debug.Log("It's a draw!");
            outcome = 0.1f;

        }
        else
        {
            Debug.LogWarning("Player or NPC battle points is 0, most likely something has gone wrong with the logic");
        }

        // We need to do some logic hear to check who wins based on the battle points, we want to handle team A winning, team B winning and draw scenarios.

        Debug.LogWarning("Attack called, may want to use the BattleLog.Log() to report the dancers and the outcome of their dance off.");

        // Pass on the winner/loser and the outcome to our fight completed function.
        FightCompleted(winner, defeated, outcome);
        yield return null;
    }

    #region Pre-Existing - No Modes Required
    /// <summary>
    /// Is called when two dancers have been selected and begins a fight!
    /// </summary>
    /// <param name="data"></param>
    public void Fight(Character TeamA, Character TeamB)
    {
        StartCoroutine(Attack(TeamA, TeamB));
    }

    /// <summary>
    /// Passes in a winning character, and a defeated character, as well as the outcome between -1 and 1
    /// </summary>
    /// <param name="winner"></param>
    /// <param name="defeated"></param>
    /// <param name="outcome"></param>
    private void FightCompleted(Character winner, Character defeated, float outcome)
    {
        var results = new FightResultData(winner, defeated, outcome);

        winner.isSelected = false;
        defeated.isSelected = false;

        battleSystem.FightOver(winner,defeated,outcome);
        winner.animController.BattleResult(winner,defeated,outcome);
        defeated.animController.BattleResult(winner, defeated, outcome);
    }

    /// <summary>
    /// Sets up a dancer to be selected and the animation to start dancing.
    /// </summary>
    /// <param name="dancer"></param>
    private void SetUpAttack(Character dancer)
    {
        dancer.isSelected = true;
        dancer.GetComponent<AnimationController>().Dance();
        BattleLog.Log(dancer.charName.GetFullCharacterName() + " Selected", dancer.myTeam.teamColor);
    }
    #endregion  
}
