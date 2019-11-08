using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functions to complete:
/// - Do Round
/// - Fight Over
/// </summary>
public class BattleSystem : MonoBehaviour
{
    public DanceTeam teamA,teamB; //References to TeamA and TeamB
    public FightManager fightManager; // References to our FightManager.

    public float battlePrepTime = 2;  // the amount of time we need to wait before a battle starts
    public float fightCompletedWaitTime = 2; // the amount of time we need to wait till a fight/round is completed.
    
    /// <summary>
    /// This occurs every round or every X number of seconds, is the core battle logic/game loop.
    /// </summary>
    /// <returns></returns>
    IEnumerator DoRound()
    {
       
        // waits for a float number of seconds....
        yield return new WaitForSeconds(battlePrepTime);

        //checking for no dancers on either team
        if(teamA.allDancers.Count == 0 && teamB.allDancers.Count == 0)
        {
            Debug.LogWarning("DoRound called, but there are no dancers on either team. DanceTeamInit needs to be completed");
            // This will be called if there are 0 dancers on both teams.

        }
        else if (teamA.activeDancers.Count > 0 && teamB.activeDancers.Count > 0)
        {
            Debug.LogWarning("DoRound called, it needs to select a dancer from each team to dance off and put in the FightEventData below");

            //Randomly chooses team members to compete against each other
            Character teamAChosenOne = teamA.activeDancers[Random.Range(0, teamA.activeDancers.Count)];
            Character teamBChosenOne = teamB.activeDancers[Random.Range(0, teamB.activeDancers.Count)];

            fightManager.Fight(teamAChosenOne, teamBChosenOne);

        }
        else
        {
            //Sets the winning team based off who still has dancers.
            DanceTeam winner = null;

            if (teamA.activeDancers.Count > 0)
            {
                winner = teamA;
            }
            else if (teamB.activeDancers.Count > 0)
            {
                winner = teamB;
            }

            //Enables the win effects, and logs it out to the console.
            winner.EnableWinEffects();
            BattleLog.Log(winner.ToString(), winner.teamColor);

            Debug.Log("DoRound called, but we have a winner so Game Over");
          
        }
    }

    // This is where we can handle what happens when we win or lose.
    public void FightOver(Character winner, Character defeated, float outcome)
    {
        Debug.LogWarning("FightOver called, may need to check for winners and/or notify teams of zero mojo dancers");

        //calling the coroutine so we can put waits in for anims to play
        StartCoroutine(HandleFightOver());
        
        //If the outcome is a tie, both competetitors gain XP, otherwise the winner gains XP and the defeated gets damaged.
        if (outcome == 0.1f)
        {
            winner.CalculateXP(outcome);
            defeated.CalculateXP(outcome);
        }
        else
        {
            winner.CalculateXP(outcome);
            defeated.DealDamage((outcome * 10) * Random.Range(1, 5));
        }

    }



    /// <summary>
    /// Used to Request A round.
    /// </summary>
    public void RequestRound()
    {
        //calling the coroutine so we can put waits in for anims to play
        StartCoroutine(DoRound());
    }

    /// <summary>
    /// Handles the end of a fight and waits to start the next round.
    /// </summary>
    /// <returns></returns>
    IEnumerator HandleFightOver()
    {
        yield return new WaitForSeconds(fightCompletedWaitTime);
        teamA.DisableWinEffects();
        teamB.DisableWinEffects();
        Debug.LogWarning("HandleFightOver called, may need to prepare or clean dancers or teams and checks before doing GameEvents.RequestFighters()");
        RequestRound();
    }
}
