using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CharacterGold : NetworkBehaviour
{
    //float to hold player's current gold
    [SyncVar]
    public float CurrentGold;

    //UI showing gold amount
    public Text CurrentGoldUI;
    public Text CurrentGoldInShopUI;

    //gold call function
    public float GetCurrentGold()
    {
        return CurrentGold;
    }

    //gold number manipulation function
    [TargetRpc]
    public void varyGold(float goldValueChange)
    {
        Debug.Log("Gold increased by " + goldValueChange);
        CurrentGold += goldValueChange;
        CurrentGoldUI.text = CurrentGold.ToString();
//        CurrentGoldInShopUI.text = CurrentGold.ToString();
    }


    // Update is called once per frame
    void Start()
    {
        if (!hasAuthority) return;
        CurrentGoldUI = GameObject.Find("PlayerGoldText").GetComponent<Text>();
        CurrentGoldUI.text = CurrentGold.ToString();
 //       CurrentGoldInShopUI.text = CurrentGold.ToString();
    }
}
