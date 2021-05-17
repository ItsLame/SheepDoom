using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class CharacterGold : NetworkBehaviour
    {
        //float to hold player's current gold
        [SerializeField]
        [SyncVar(hook = nameof(SyncPlayerGold))]
        private float CurrentGold;

        //UI showing gold amount
        [SerializeField]
        private Text CurrentGoldUI;
        [SerializeField]
        private Text CurrentGoldInShopUI;

        //to sync player gold ui
        void SyncPlayerGold(float oldValue, float newValue)
        {
            if (hasAuthority)
            {
                CurrentGoldUI.text = newValue.ToString();
                CurrentGoldInShopUI.text = newValue.ToString();
            }
        }

        //gold call function
        public float GetCurrentGold()
        {
            return CurrentGold;
        }

        //gold number manipulation function
        [Command]
        public void CmdVaryGold(float goldValueChange)
        {
            CurrentGold += goldValueChange;
        }

        public void ServerVaryGold(float goldValueChange)
        {
            CurrentGold += goldValueChange;
        }

        public override void OnStartClient()
        {
            if (!hasAuthority) return;
            CurrentGoldUI = FindMe.instance.P_PlayerGold.GetComponent<Text>();
            CurrentGoldUI.text = CurrentGold.ToString();

            //get the gold text in game shop
            CurrentGoldInShopUI = FindMe.instance.P_ShopGold.GetComponent<Text>();
            CurrentGoldInShopUI.text = CurrentGold.ToString();
        }
    }
}
