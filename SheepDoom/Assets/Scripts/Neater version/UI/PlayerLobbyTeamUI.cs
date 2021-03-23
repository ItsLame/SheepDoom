using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class SwitchTeam : NetworkBehaviour
    {
        [SyncVar] GameObject myToTeam1Button;
        [SyncVar] GameObject myToTeam2Button;

        public void InitTeam()
        {
            PlayerObj.instance.SetTeamIndex(1);

            PlayerObj.instance.SetTeamIndex(2);
        }

        public void StartSwitchTeam()
        {

        }
    }
}