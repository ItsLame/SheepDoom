using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    //https://gamedevbeginner.com/how-to-change-a-sprite-from-a-script-in-unity-with-examples/
    //https://answers.unity.com/questions/1127944/changing-the-image-of-a-button-with-a-script.html <--
    public class ShopSkillsIconScript : MonoBehaviour
    {
        [Header("Shop Buttons")]
        [SerializeField] private Button S1;
        [SerializeField] private Button S2;
        [SerializeField] private Button U1;
        [SerializeField] private Button U2;
  //      [SerializeField] private SpriteRenderer S1_renderer;
 //       [SerializeField] private SpriteRenderer S2_renderer;
 //       [SerializeField] private SpriteRenderer U1_renderer;
  //      [SerializeField] private SpriteRenderer U2_renderer;


        [Header("Identifying player")]
        [Space(15)]
        [SerializeField] private float characterID;
        public bool hasSet;

        [Header("---- Alma----")]
        [Space(15)]
        [SerializeField] private Sprite AlmaS1;
        [SerializeField] private Sprite AlmaS2;
        [SerializeField] private Sprite AlmaU1;
        [SerializeField] private Sprite AlmaU2;

        [Header("---- Asta----")]
        [SerializeField] private Sprite AstaS1;
        [SerializeField] private Sprite AstaS2;
        [SerializeField] private Sprite AstaU1;
        [SerializeField] private Sprite AstaU2;

        [Header("----Isa----")]
        [SerializeField] private Sprite IsaS1;
        [SerializeField] private Sprite IsaS2;
        [SerializeField] private Sprite IsaU1;
        [SerializeField] private Sprite IsaU2;

        public void Start()
        {
   //         S1_renderer = S1.GetComponent<SpriteRenderer>();
   //         S2_renderer = S2.GetComponent<SpriteRenderer>();
   //         U1_renderer = U1.GetComponent<SpriteRenderer>();
    //        U2_renderer = U2.GetComponent<SpriteRenderer>();
        }

        public void setIcons()
        {
            characterID = FindMe.instance.P_MyPlayer.GetComponent<PlayerAdmin>().getCharID();
      //      Debug.Log("Char ID: " + characterID);
 
            if (characterID == 1)
            {
                S1.GetComponent<Image>().sprite = AlmaS1;
                S2.GetComponent<Image>().sprite = AlmaS2;
                U1.GetComponent<Image>().sprite = AlmaU1;
                U2.GetComponent<Image>().sprite = AlmaU2;

                /*
                S1_renderer.sprite = AlmaS1;
                S2_renderer.sprite = AlmaS2;
                U1_renderer.sprite = AlmaU1;
                U2_renderer.sprite = AlmaU2;*/

           //     Debug.Log("Setting for Alma");
            }

            else if (characterID == 2)
            {
                S1.GetComponent<Image>().sprite = AstaS1;
                S2.GetComponent<Image>().sprite = AstaS2;
                U1.GetComponent<Image>().sprite = AstaU1;
                U2.GetComponent<Image>().sprite = AstaU2;
                /*
                S1_renderer.sprite = AstaS1;
                S2_renderer.sprite = AstaS2;
                U1_renderer.sprite = AstaU1;
                U2_renderer.sprite = AstaU2;*/

         //       Debug.Log("Setting for Asta");
            }

            else if (characterID == 3)
            {
                S1.GetComponent<Image>().sprite = IsaS1;
                S2.GetComponent<Image>().sprite = IsaS2;
                U1.GetComponent<Image>().sprite = IsaU1;
                U2.GetComponent<Image>().sprite = IsaU2;

                /*
                S1_renderer.sprite = IsaS1;
                S2_renderer.sprite = IsaS2;
                U1_renderer.sprite = IsaU1;
                U2_renderer.sprite = IsaU2;*/

         //       Debug.Log("Setting for Isa");
            }

      //      Debug.Log("Setting skill icons complete");
            hasSet = true;
        }



    }

}
