using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
 {
     public GameObject Projectile;   
     public Transform SpawnPoint; 
 
     private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Space))
         {
             Instantiate(Projectile, SpawnPoint.position, SpawnPoint.rotation);
         }
     }
 }
