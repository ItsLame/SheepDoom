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
        [SerializeField]
        private float SpecialCost;
        [SerializeField]
        private float UltiCost;

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

                if (Physics.Raycast(ray, out hit) && !Player)
                {
                    //if hit something
                    if (hit.collider.CompareTag("Shop"))
                    {
                        if (hit.collider.gameObject.layer == 8 && Player.layer == 8)
                            OpenShopUI();
                        else if (hit.collider.gameObject.layer == 9 && Player.layer == 9)
                            OpenShopUI();
                        //Debug.Log("Shop Pressed!");
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

                if (Physics.Raycast(ray, out hit) && Player != null)
                {
 //                   Debug.Log(hit.transform.name + " was clicked");

                    if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        //if hit something
                        if (hit.collider.CompareTag("Shop"))
                        {
                            if (hit.collider.gameObject.layer == 8 && Player.layer == 8)
                                OpenShopUI();
                            else if (hit.collider.gameObject.layer == 9 && Player.layer == 9)
                                OpenShopUI();
                            //Debug.Log("Shop Pressed!");
                        }
                    }
                }
            }

            //spacebar to open shop for debugging
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OpenShopUI();
            }
#endif
        }

        public void SetPlayer(GameObject _player)
        {
            Player = _player;
        }

        //opening shop
        public void OpenShopUI()
        {
            Debug.Log("Player Gold: " + PlayerGold);
            //get player's current gold
            PlayerGold = Player.GetComponent<CharacterGold>().GetCurrentGold();

            Debug.Log("Opening Shop UI");
            Debug.Log("Player Gold: " + PlayerGold);

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
            Debug.Log("First Special In Shop Selected");
            //can purchase only if havent purchased and u have enough gold

            if (!hasPurchasedSpecial)
            {
                Debug.Log("Havent purchased special");
            }

            if (PlayerGold - SpecialCost >= 0)
            {
                Debug.Log("Gold is enough");
            }

            else { Debug.Log("Not enough Player Gold: " + PlayerGold + ", Special Cost: " + SpecialCost); }

            if (!hasPurchasedSpecial && (PlayerGold - SpecialCost >= 0))
            {
                //deduct gold
                Player.GetComponent<CharacterGold>().CmdVaryGold(-SpecialCost);

                //if choose 1st, disable 2nd
                SpecialButton2.GetComponent<Button>().interactable = false;
                //enable special button
//                PlayerSpecialButton.GetComponent<Button>().interactable = true;
                hasPurchasedSpecial = true;

                //set player bool 'haspurchasedspecial' to true
                Player.GetComponent<PlayerAttack>().CMD_playerHasPurchasedSpecial();

                Debug.Log("First Special Purchased");
            }

            else
            {
                Debug.Log("unable to buy first special");
            }

            //update player gold
            PlayerGold = Player.GetComponent<CharacterGold>().GetCurrentGold();

        }

        //second special selection button
        public void SelectSecondSpecial()
        {
            //can purchase only if havent purchased
            if (!hasPurchasedSpecial && (PlayerGold - SpecialCost >= 0))
            {
                //deduct gold
                Player.GetComponent<CharacterGold>().CmdVaryGold(-SpecialCost);

                //set alternative special chosen
                Player.GetComponent<PlayerAttack>().CMD_AlternateSpecial();

                //if choose 2nd, disable 1st
                SpecialButton1.GetComponent<Button>().interactable = false;
 //               PlayerSpecialButton.GetComponent<Button>().interactable = true;
                hasPurchasedSpecial = true;

                //set player bool 'haspurchasedspecial' to true
                Player.GetComponent<PlayerAttack>().CMD_playerHasPurchasedSpecial();

                Debug.Log("Second Special Purchased");

                //update player gold
                PlayerGold = Player.GetComponent<CharacterGold>().GetCurrentGold();
            }
        }

        public void SelectFirstUlti()
        {
            //can purchase only if havent purchased
            if (!hasPurchasedUlti && (PlayerGold - UltiCost >= 0))
            {
                Player.GetComponent<CharacterGold>().CmdVaryGold(-UltiCost);

                //if choose 1st, disable 2nd
                UltiButton2.GetComponent<Button>().interactable = false;
//                PlayerUltiButton.GetComponent<Button>().interactable = true;
                hasPurchasedUlti = true;

                //set player bool 'haspurchasedulti' to true
                Player.GetComponent<PlayerAttack>().CMD_playerHasPurchasedUlti();

                Debug.Log("First Ulti Purchased");
                //update player gold
                PlayerGold = Player.GetComponent<CharacterGold>().GetCurrentGold();
            }
        }

        public void SelectSecondUlti()
        {
            //can purchase only if havent purchased
            if (!hasPurchasedUlti && (PlayerGold - UltiCost >= 0))
            {
                Player.GetComponent<CharacterGold>().CmdVaryGold(-UltiCost);

                //set alternate ulti true
                Player.GetComponent<PlayerAttack>().CMD_AlternateUlti(); 
                //if choose 2nd, disable 1st
                UltiButton1.GetComponent<Button>().interactable = false;
  //              PlayerUltiButton.GetComponent<Button>().interactable = true;
                hasPurchasedUlti = true;

                //set player bool 'haspurchasedulti' to true
                Player.GetComponent<PlayerAttack>().CMD_playerHasPurchasedUlti();

                Debug.Log("Second Ulti Purchased");

                //update player gold
                PlayerGold = Player.GetComponent<CharacterGold>().GetCurrentGold();
            }
        }
    }
}