using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CharacterGold : NetworkBehaviour
{
    //float to hold player's current gold
    [SyncVar(hook = nameof(SyncPlayerGold))]
    public float CurrentGold;

    //UI showing gold amount
    public Text CurrentGoldUI;
    public Text CurrentGoldInShopUI;

    //to sync player gold ui
    public void SyncPlayerGold(float oldValue, float newValue)
    {
        if (hasAuthority)
        {
            CurrentGoldUI.text = CurrentGold.ToString();
            CurrentGoldInShopUI.text = CurrentGold.ToString();
        }
    }

    //gold call function
    public float GetCurrentGold()
    {
        return CurrentGold;
    }

    //gold number manipulation function
    public void varyGold(float goldValueChange)
    {
        Debug.Log("Current Gold: " + CurrentGold);
        Debug.Log("Gold increased by " + goldValueChange);
        CurrentGold += goldValueChange;
        Debug.Log("Current Gold after: " + CurrentGold);
    }


    // Update is called once per frame
    void Start()
    {
        if (!hasAuthority) return;
        //get the gold text in game UI
        CurrentGoldUI = GameObject.Find("PlayerGoldText").GetComponent<Text>();
        CurrentGoldUI.text = CurrentGold.ToString();

        //get the gold text in game shop
        CurrentGoldInShopUI = GameObject.Find("Shop_Gold_Amount").GetComponent<Text>();
        CurrentGoldInShopUI.text = CurrentGold.ToString();
    }
}
