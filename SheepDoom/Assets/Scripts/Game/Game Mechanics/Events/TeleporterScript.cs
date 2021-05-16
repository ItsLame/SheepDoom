using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class TeleporterScript : MonoBehaviour
    {
        [Header("Affinity + Position")]
        [SerializeField] private GameObject EndPositionObject;
        [SerializeField] private bool isBlueSide;
        [SerializeField] private bool isRedSide;

        [Header("Cooldown time?")]
        [Space(15)]
        [SerializeField] private float cooldown;
        [SerializeField] private float cooldownInGame;

        //teleports only blue team on blue side n vice versa
        [ClientCallback]
        private void OnTriggerEnter(Collider other)
        {
            //only tele if off cd
            if (cooldownInGame > 0.1) return;

            if (other.CompareTag("Player"))
            {
                if (isBlueSide)
                {
                    if (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 1)
                        other.gameObject.transform.position = EndPositionObject.transform.position;
                }
                else if (isRedSide)
                {
                    if (other.gameObject.GetComponent<PlayerAdmin>().getTeamIndex() == 2)
                        other.gameObject.transform.position = EndPositionObject.transform.position;
                }
            }
        }

    }
}

