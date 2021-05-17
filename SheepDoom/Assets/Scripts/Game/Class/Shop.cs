using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace SheepDoom
{
    public abstract class Shop : MonoBehaviour
    {
        public AudioClip Sound1;
        public AudioClip Sound2;
        public AudioClip Sound3;
        public AudioSource ShopSound;

        //linking to player
        private GameObject shopPlayer;
        [SerializeField] private GameObject skillIconSetter;
        private float playerGold;
        private int shopTeamIndex;

        [Space(15)]
        private float specialCost;
        private float ultiCost;

        [Space(15)]
        //shop's UI
        public GameObject shopMenuUI;

        [Space(15)]
        //shops buttons
        private GameObject specialButton1;
        private GameObject specialButton2;
        private GameObject ultiButton1;
        private GameObject ultiButton2;

        [Space(15)]
        //game's control UI
        private GameObject gameUI;
        //player special & ulti button
        private GameObject playerSpecialButton;
        private GameObject playerUltiButton;

        [Space(15)]
        //control checks for if player purchased special / ulti
        private bool hasPurchasedSpecial = false;
        private bool hasPurchasedUlti = false;

        #region Properties

        public int P_shopTeamIndex
        {
            get{return shopTeamIndex;}
            set{shopTeamIndex = value;}
        }

        public virtual GameObject P_shopPlayer
        {
            get{return shopPlayer;}
            set{shopPlayer = value;}
        }

        public virtual float P_playerGold
        {
            get{return playerGold;}
            set{playerGold = value;}
        }
        public float P_specialCost
        {
            get{return specialCost;}
            set{specialCost = value;}
        }

        public float P_ultiCost
        {
            get{return ultiCost;}
            set{ultiCost= value;}
        }
        public GameObject P_shopMenuUI
        {
            get{return shopMenuUI;}
            set{shopMenuUI = value;}
        }

        public GameObject P_specialButton1
        {
            get{return specialButton1;}
            set{specialButton1 = value;}
        }
        
        public GameObject P_specialButton2
        {
            get{return specialButton2;}
            set{specialButton2 = value;}
        }
        
        public GameObject P_ultiButton1
        {
            get{return ultiButton1;}
            set{ultiButton1 = value;}
        }
        
        public GameObject P_ultiButton2
        {
            get{return ultiButton2;}
            set{ultiButton2 = value;}
        }

        public GameObject P_gameUI
        {
            get{return gameUI;}
            set{gameUI = value;}
        }
        //player special & ulti button
        
        public GameObject P_playerSpecialButton
        {
            get{return playerSpecialButton;}
            set{playerSpecialButton = value;}
        }
        
        public GameObject P_playerUltiButton
        {
            get{return playerUltiButton;}
            set{playerUltiButton = value;}
        }

        //protected bool hasPurchasedSpecial = false;
        //protected bool hasPurchasedUlti = false;

        #endregion

        private void Start()
        {
            InitShop();
        }

        protected abstract void InitShop();
        
        //for pressing the shop with raycast
        private void Update()
        {
            //if more than one touch and at the beginning of the touch
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                //store the info of hit object
                RaycastHit hit;
                //get a raycast to where you are touching
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                
                if (Physics.Raycast(ray, out hit) && P_shopPlayer != null)
                {
                    //if hit something
                    if (hit.collider.CompareTag("Shop"))
                    {
                        //if (hit.collider.gameObject.layer == 8 && Player.layer == 8)
                        if (P_shopTeamIndex == P_shopPlayer.GetComponent<PlayerAdmin>().getTeamIndex())
                            OpenShopUI();
                        //else if (hit.collider.gameObject.layer == 9 && Player.layer == 9)
                            //OpenShopUI();
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

                if (Physics.Raycast(ray, out hit) && P_shopPlayer != null)
                {
 //                   Debug.Log(hit.transform.name + " was clicked");

                    if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        //if hit something
                        if (hit.collider.CompareTag("Shop"))
                        {
                            if (P_shopTeamIndex == P_shopPlayer.GetComponent<PlayerAdmin>().getTeamIndex())
                                OpenShopUI();
                            //if (hit.collider.gameObject.layer == Player.layer)
                                //OpenShopUI();
                        }
                    }
                }
            }

            //spacebar to open shop for debugging
            if (Input.GetKeyDown(KeyCode.Space))
                OpenShopUI();
#endif
        }

        //opening shop
        private void OpenShopUI()
        {
            if (skillIconSetter.GetComponent<ShopSkillsIconScript>().hasSet == false)
            {
                skillIconSetter.GetComponent<ShopSkillsIconScript>().setIcons();
            }

      //      Debug.Log("Player Gold: " + P_playerGold);
            //get player's current gold
            P_playerGold = P_shopPlayer.GetComponent<CharacterGold>().GetCurrentGold();

       //     Debug.Log("Opening Shop UI");
    //        Debug.Log("Player Gold: " + P_playerGold);

            //disable the controlling UI
            P_gameUI.GetComponent<Canvas>().enabled = false;

            //enable shopUI
            P_shopMenuUI.GetComponent<Canvas>().enabled = true;

            ShopSound.clip = Sound1;
            ShopSound.PlayOneShot(ShopSound.clip, ShopSound.volume);
        }

        public void CloseShopUI()
        {
            //enable control UI
            P_gameUI.GetComponent<Canvas>().enabled = true;
            //disable shopUI
            P_shopMenuUI.GetComponent<Canvas>().enabled = false;

            ShopSound.clip = Sound2;
            ShopSound.PlayOneShot(ShopSound.clip, ShopSound.volume);
        }


        //first special selection button
        public void SelectFirstSpecial()
        {
   //         Debug.Log("First Special In Shop Selected");
            //can purchase only if havent purchased and u have enough gold

            if (!hasPurchasedSpecial)
            {
     //           Debug.Log("Havent purchased special");
            }

            if (P_playerGold - P_specialCost >= 0)
            {
     //           Debug.Log("Gold is enough");
            }

     //       else { Debug.Log("Not enough Player Gold: " + P_playerGold + ", Special Cost: " + P_specialCost); }

            if (!hasPurchasedSpecial && (P_playerGold - P_specialCost >= 0))
            {
                //deduct gold
                P_shopPlayer.GetComponent<CharacterGold>().CmdVaryGold(-P_specialCost);

                //if choose 1st, disable 2nd
                P_specialButton2.GetComponent<Button>().interactable = false;
                //enable special button
//                PlayerSpecialButton.GetComponent<Button>().interactable = true;
                hasPurchasedSpecial = true;

                //set player bool 'haspurchasedspecial' to true
                P_shopPlayer.GetComponent<PlayerAttack>().PlayerHasPurchasedSpecial(false);


                ShopSound.clip = Sound3;
                ShopSound.PlayOneShot(ShopSound.clip, ShopSound.volume);
                //           Debug.Log("First Special Purchased");
            }

            else
            {
      //          Debug.Log("unable to buy first special");
            }

            //update player gold
            P_playerGold = P_shopPlayer.GetComponent<CharacterGold>().GetCurrentGold();
        }

        //second special selection button
        public void SelectSecondSpecial()
        {
            //can purchase only if havent purchased
            if (!hasPurchasedSpecial && (P_playerGold - P_specialCost >= 0))
            {
                //deduct gold
                P_shopPlayer.GetComponent<CharacterGold>().CmdVaryGold(-P_specialCost);

                //set alternative special chosen
                P_shopPlayer.GetComponent<PlayerAttack>().PlayerHasPurchasedSpecial(true);

                //if choose 2nd, disable 1st
                P_specialButton1.GetComponent<Button>().interactable = false;
 //               PlayerSpecialButton.GetComponent<Button>().interactable = true;
                hasPurchasedSpecial = true;

                //set player bool 'haspurchasedspecial' to true
                //Player.GetComponent<PlayerAttack>().PlayerHasPurchasedSpecial();

    //            Debug.Log("Second Special Purchased");

                //update player gold
                P_playerGold = P_shopPlayer.GetComponent<CharacterGold>().GetCurrentGold();

                ShopSound.clip = Sound3;
                ShopSound.PlayOneShot(ShopSound.clip, ShopSound.volume);
            }
        }

        public void SelectFirstUlti()
        {
            //can purchase only if havent purchased
            if (!hasPurchasedUlti && (P_playerGold - P_ultiCost >= 0))
            {
                P_shopPlayer.GetComponent<CharacterGold>().CmdVaryGold(-P_ultiCost);

                //set player bool 'haspurchasedulti' to true
                P_shopPlayer.GetComponent<PlayerAttack>().PlayerHasPurchasedUlti(false);

                //if choose 1st, disable 2nd
                P_ultiButton2.GetComponent<Button>().interactable = false;
//                PlayerUltiButton.GetComponent<Button>().interactable = true;
                hasPurchasedUlti = true;

     //           Debug.Log("First Ulti Purchased");
                //update player gold
                P_playerGold = P_shopPlayer.GetComponent<CharacterGold>().GetCurrentGold();

                ShopSound.clip = Sound3;
                ShopSound.PlayOneShot(ShopSound.clip, ShopSound.volume);
            }
        }

        public void SelectSecondUlti()
        {
            //can purchase only if havent purchased
            if (!hasPurchasedUlti && (P_playerGold - P_ultiCost >= 0))
            {
                P_shopPlayer.GetComponent<CharacterGold>().CmdVaryGold(-P_ultiCost);
                //if choose 2nd, disable 1st
                P_ultiButton1.GetComponent<Button>().interactable = false;
  //              PlayerUltiButton.GetComponent<Button>().interactable = true;
                hasPurchasedUlti = true;

                //set player bool 'haspurchasedulti' to true
                P_shopPlayer.GetComponent<PlayerAttack>().PlayerHasPurchasedUlti(true);

      //          Debug.Log("Second Ulti Purchased");

                //update player gold
                P_playerGold = P_shopPlayer.GetComponent<CharacterGold>().GetCurrentGold();

                ShopSound.clip = Sound3;
                ShopSound.PlayOneShot(ShopSound.clip, ShopSound.volume);
            }
        }
    }
}