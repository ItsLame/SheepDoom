using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGold : MonoBehaviour
{
    //float to hold player's current gold
    private float CurrentGold;

    //UI showing gold amount
    public Text CurrentGoldUI;
    public Text CurrentGoldInShopUI;

    //gold call function
    public float GetCurrentGold()
    {
        return CurrentGold;
    }

    //gold number manipulation function
    public void varyGold(float goldValueChange)
    {
        CurrentGold += goldValueChange;
    }



    // Update is called once per frame
    void Update()
    {
        CurrentGoldUI.text = CurrentGold.ToString();
        CurrentGoldInShopUI.text = CurrentGold.ToString();
    }
}
