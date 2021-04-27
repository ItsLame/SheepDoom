using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class Character2 : NetworkBehaviour
    {
        [SerializeField]
        private GameObject normalAtkMelee, normalSpecial, altSpecial;
        [SerializeField]
        private float meleeAttackDuration1, meleeAttackSpeedMultiplier;
        private int meleeCombo = 1;

        [Client]
        public void normalAtk(bool _multiplier)
        {
            CmdNormalAtk();
            ObjectMovementScript comp = normalAtkMelee.GetComponent<ObjectMovementScript>();
            if (meleeCombo == 1)
            {
                if(!_multiplier)
                {
                    comp.SetMoveSpeed(120);
                    comp.move(meleeAttackDuration1, "right");
                }
                else if (_multiplier)
                {
                    comp.SetMoveSpeed(120 * meleeAttackSpeedMultiplier);
                    comp.move((meleeAttackDuration1 / meleeAttackSpeedMultiplier), "right");
                }
                meleeCombo = 2;
            }
            else if(meleeCombo == 2)
            {
                if(!_multiplier)
                {
                    comp.GetComponent<ObjectMovementScript>().SetMoveSpeed(120);
                    comp.GetComponent<ObjectMovementScript>().move(meleeAttackDuration1, "left");
                }
                else if(_multiplier)
                {
                    comp.SetMoveSpeed(120 * meleeAttackSpeedMultiplier);
                    comp.move((meleeAttackDuration1 / meleeAttackSpeedMultiplier), "left");
                }
                meleeCombo = 1;
            }
        }

        [Command]
        void CmdNormalAtk()
        {
            normalAtkMelee.GetComponent<OnTouchHealth>().SetHitBox(true);
        }

        [Client] // call from client to tell server
        public void ServerSetHitBox(bool _hitBox)
        {
            if (!hasAuthority) return;
            CmdSetHitBox(_hitBox);
        }

        [Command]
        void CmdSetHitBox(bool _hitBox)
        {
            normalAtkMelee.GetComponent<OnTouchHealth>().SetHitBox(_hitBox);
        }

        [Client]
        public void SpecialAtk(bool _isAltSpecial)
        {
            if(!_isAltSpecial)
            {
                Debug.Log("AA");
                CmdSpecialAtk();
            }
        }

        [Command]
        void CmdSpecialAtk()
        {
            if(normalSpecial != null)
            {
                Debug.Log("BB");
                normalSpecial.GetComponent<MeshRenderer>().enabled = true;
                normalSpecial.GetComponent<BoxCollider>().enabled = true;
                normalSpecial.GetComponent<PlayerChild>().refreshDuration();
                RpcEnableShieldSkill();
            }
        }

        [ClientRpc]
        void RpcEnableShieldSkill()
        {
            if (normalSpecial != null)
            {
                Debug.Log("CC");
                normalSpecial.GetComponent<MeshRenderer>().enabled = true;
                normalSpecial.GetComponent<BoxCollider>().enabled = true;
                normalSpecial.GetComponent<PlayerChild>().refreshDuration();
            }
        }
    }
}
