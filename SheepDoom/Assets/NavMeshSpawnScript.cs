using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class NavMeshSpawnScript : NetworkBehaviour
    {
        [SerializeField] private GameObject navMeshObject;

        public override void OnStartServer()
        {
            GameObject navMesh = Instantiate(navMeshObject, this.transform.position, this.transform.rotation);
            navMesh.transform.SetParent(null, false);
            navMesh.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            NetworkServer.Spawn(navMesh);
        }
    }

}
