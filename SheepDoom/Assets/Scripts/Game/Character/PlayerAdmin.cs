using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAdmin : NetworkBehaviour
{
    // Mainly player attributes 

    [Header("Team affiliation (1 for Blue, 2 for Red")]
    [SerializeField]
    private float TeamIndex;

    [Header("Player scores")]
    public float PlayerKills;
    public float PlayerDeaths;
    public float TowerCaptures;

    //accessor method
    public float getTeamIndex()
    {
        return TeamIndex;
    }
}
