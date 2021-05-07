using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class NavMeshSpawnScript : NetworkBehaviour
    {
        [SerializeField] private GameObject navMeshObject;
        private GameObject navMesh;
        private string matchID = string.Empty;

        void Update()
        {
            if(isServer && string.IsNullOrEmpty(matchID))
            {
                matchID = GetComponent<GetGameStatus>().GetGameStatusManager().GetComponent<GameStatus>().P_matchID;
                Debug.Log("whats my matchID2: " + matchID);
                MatchMaker.instance.GetMatches()[matchID].GetSDSceneManager().MoveToNewScene(navMesh, MatchMaker.instance.GetMatches()[matchID].GetScenes()[2]);
                return;
            }
        }


        public override void OnStartServer()
        {
            navMesh = Instantiate(navMeshObject, transform.position, transform.rotation);
        }
    }
}
