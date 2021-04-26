using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Mirror;

namespace SheepDoom
{
    public class PlayerAttachToButtons : NetworkBehaviour
    {
        [Header("UI Attack Buttons")]
        [Space(15)]
        public Button _NormalButton;
        public Button _SpecialButton;
        public Button _UltiButton;

        public Button _ShopSpecialButton1;
        public Button _ShopSpecialButton2;
        public Button _ShopUltiButton1;
        public Button _ShopUltiButton2;

        //player teamID
        private float teamID;

        public override void OnStartClient()
        {
            if (!hasAuthority) return;
            teamID = gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
            assignButtons(gameObject); // no point assigning buttons on server because u dont have a single player view
        }

        [Client]
        public void assignButtons(GameObject player)
        {
            //assign attacking buttons
            _NormalButton = FindMe.instance.P_AtkBtn.GetComponent<Button>();
            _SpecialButton = FindMe.instance.P_SpecialBtn.GetComponent<Button>();
            _UltiButton = FindMe.instance.P_UltimateBtn.GetComponent<Button>();

            //get the action
            UnityAction normalAttack = new UnityAction(player.GetComponent<PlayerAttack>().AttackClick);
            UnityAction specialAttack = new UnityAction(player.GetComponent<PlayerAttack>().SpecialSkillClick);
            UnityAction ultiAttack = new UnityAction(player.GetComponent<PlayerAttack>().UltiClick);

            _NormalButton.onClick.AddListener(normalAttack);
            _SpecialButton.onClick.AddListener(specialAttack);
            _UltiButton.onClick.AddListener(ultiAttack);

            //assign shop buttons based on teamID
            _ShopSpecialButton1 = FindMe.instance.P_ShopSpecial1.GetComponent<Button>();
            _ShopSpecialButton2 = FindMe.instance.P_ShopSpecial2.GetComponent<Button>(); ;
            _ShopUltiButton1 = FindMe.instance.P_ShopUlti1.GetComponent<Button>();
            _ShopUltiButton2 = FindMe.instance.P_ShopUlti2.GetComponent<Button>();


            if (teamID == 1)
            {
                GameObject blueShop = FindMe.instance.P_BlueShop;
                UnityAction buySpecial1 = new UnityAction(blueShop.gameObject.GetComponent<Shop>().SelectFirstSpecial);
                UnityAction buySpecial2 = new UnityAction(blueShop.gameObject.GetComponent<Shop>().SelectSecondSpecial);
                UnityAction buyUlti1 = new UnityAction(blueShop.gameObject.GetComponent<Shop>().SelectFirstUlti);
                UnityAction buyUlti2 = new UnityAction(blueShop.gameObject.GetComponent<Shop>().SelectSecondUlti);

                _ShopSpecialButton1.onClick.AddListener(buySpecial1);
                _ShopSpecialButton2.onClick.AddListener(buySpecial2);
                _ShopUltiButton1.onClick.AddListener(buyUlti1);
                _ShopUltiButton2.onClick.AddListener(buyUlti2);
            }

            if (teamID == 2)
            {
                GameObject redShop = FindMe.instance.P_RedShop;
                UnityAction buySpecial1 = new UnityAction(redShop.gameObject.GetComponent<Shop>().SelectFirstSpecial);
                UnityAction buySpecial2 = new UnityAction(redShop.gameObject.GetComponent<Shop>().SelectSecondSpecial);
                UnityAction buyUlti1 = new UnityAction(redShop.gameObject.GetComponent<Shop>().SelectFirstUlti);
                UnityAction buyUlti2 = new UnityAction(redShop.gameObject.GetComponent<Shop>().SelectSecondUlti);

                _ShopSpecialButton1.onClick.AddListener(buySpecial1);
                _ShopSpecialButton2.onClick.AddListener(buySpecial2);
                _ShopUltiButton1.onClick.AddListener(buyUlti1);
                _ShopUltiButton2.onClick.AddListener(buyUlti2);
            }

        }
    }
}
