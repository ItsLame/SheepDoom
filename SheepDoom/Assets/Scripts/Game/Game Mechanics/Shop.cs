using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class Shop : NetworkBehaviour
    {
        //linking to player
        public GameObject Player;
        public float PlayerGold;

        [Space(15)]
        public float SpecialCost;
        public float UltiCost;

        [Space(15)]
        //shop's UI
        public GameObject ShopMenuUI;

        [Space(15)]
        //shops buttons
        public GameObject SpecialButton1;
        public GameObject SpecialButton2;
        public GameObject UltiButton1;
        public GameObject UltiButton2;

        [Space(15)]
        //game's control UI
        public GameObject GameUI;
        //player special & ulti button
        public GameObject PlayerSpecialButton;
        public GameObject PlayerUltiButton;

        [Space(15)]
        //control checks for if player purchased special / ulti
        public bool hasPurchasedSpecial = false;
        public bool hasPurchasedUlti = false;

        private void Start()
        {
            ShopMenuUI = GameObject.Find("ShopUI");
        }

        //for pressing the shop with raycast
        private void Update()
        {

            //if more than one touch and at the beginning of the touch
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                //get a raycast to where you are touching
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                //store the info of hit object
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //if hit something
                    if (hit.collider.gameObject.CompareTag("Shop"))
                    {
                        Debug.Log("Shop Pressed!");
                        OpenShopUI();
                    }
                }
            }

#if UNITY_EDITOR  //<-- only in unity editor
            //for PC 
            if (Input.GetMouseButtonDown(0))
            {
                //store the info of hit object
                RaycastHit hit;
 //               Debug.Log("Mouse 0 down");
                //get a raycast to where you are touching
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.transform.name + " was clicked");

                    //if hit something
                    if (hit.collider.gameObject.CompareTag("Shop"))
                    {
                        Debug.Log("Shop Pressed, running OpenShopUI()");
                        OpenShopUI();
                    }
                }
            }
#endif

        }

        //opening shop
        public void OpenShopUI()
        {
            Debug.Log("Opening Shop UI");
            //get player's current gold
            PlayerGold = Player.GetComponent<CharacterGold>().GetCurrentGold();

            //disable the controlling UI
            GameUI.GetComponent<Canvas>().enabled = false;

            //enable shopUI
            ShopMenuUI.GetComponent<Canvas>().enabled = true;
        }

        public void CloseShopUI()
        {
            //enable control UI
            GameUI.GetComponent<Canvas>().enabled = true;
            //disable shopUI
            ShopMenuUI.GetComponent<Canvas>().enabled = false;
        }


        //first special selection button
        public void SelectFirstSpecial()
        {
            //can purchase only if havent purchased and u have enough gold
            if (!hasPurchasedSpecial && (PlayerGold - SpecialCost >= 0))
            {
                //deduct gold
                Player.GetComponent<CharacterGold>().varyGold(-SpecialCost);

                //if choose 1st, disable 2nd
                SpecialButton2.GetComponent<Button>().interactable = false;
                //enable special button
//                PlayerSpecialButton.GetComponent<Button>().interactable = true;
                hasPurchasedSpecial = true;

                //set player bool 'haspurchasedspecial' to true
                Player.GetComponent<PlayerAttack>().CMD_playerHasPurchasedSpecial();
            }

        }

        //second special selection button
        public void SelectSecondSpecial()
        {
            //can purchase only if havent purchased
            if (!hasPurchasedSpecial && (PlayerGold - SpecialCost >= 0))
            {
                //deduct gold
                Player.GetComponent<CharacterGold>().varyGold(-SpecialCost);

                //if choose 2nd, disable 1st
                SpecialButton1.GetComponent<Button>().interactable = false;
 //               PlayerSpecialButton.GetComponent<Button>().interactable = true;
                hasPurchasedSpecial = true;

                //set player bool 'haspurchasedspecial' to true
                Player.GetComponent<PlayerAttack>().CMD_playerHasPurchasedSpecial();
            }
        }

        public void SelectFirstUlti()
        {
            //can purchase only if havent purchased
            if (!hasPurchasedUlti && (PlayerGold - UltiCost >= 0))
            {
                Player.GetComponent<CharacterGold>().varyGold(-UltiCost);

                //if choose 1st, disable 2nd
                UltiButton2.GetComponent<Button>().interactable = false;
//                PlayerUltiButton.GetComponent<Button>().interactable = true;
                hasPurchasedUlti = true;

                //set player bool 'haspurchasedulti' to true
                Player.GetComponent<PlayerAttack>().CMD_playerHasPurchasedUlti();
            }
        }

        public void SelectSecondUlti()
        {
            //can purchase only if havent purchased
            if (!hasPurchasedUlti && (PlayerGold - UltiCost >= 0))
            {
                Player.GetComponent<CharacterGold>().varyGold(-UltiCost);
                //if choose 2nd, disable 1st
                UltiButton1.GetComponent<Button>().interactable = false;
  //              PlayerUltiButton.GetComponent<Button>().interactable = true;
                hasPurchasedUlti = true;

                //set player bool 'haspurchasedulti' to true
                Player.GetComponent<PlayerAttack>().CMD_playerHasPurchasedUlti();
            }
        }
    }
}