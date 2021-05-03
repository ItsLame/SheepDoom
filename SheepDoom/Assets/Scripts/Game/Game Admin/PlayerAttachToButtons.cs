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
        [SerializeField] private Button _NormalButton;
        [SerializeField] private Button _SpecialButton;
        [SerializeField] private Button _UltiButton;

        [Header("UI Shop Buttons")]
        [Space(15)]
        [SerializeField] private Button _ShopSpecialButton1;
        [SerializeField] private Button _ShopSpecialButton2;
        [SerializeField] private Button _ShopUltiButton1;
        [SerializeField] private Button _ShopUltiButton2;
        [SerializeField] private Button _ShopX;

        private GameObject shop;

        private UnityAction normalAttack, specialAttack, ultiAttack, buySpecial1, buySpecial2, buyUlti1, buyUlti2, closeShop;

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
            normalAttack = new UnityAction(player.GetComponent<PlayerAttack>().AttackClick);
            specialAttack = new UnityAction(player.GetComponent<PlayerAttack>().SpecialSkillClick);
            ultiAttack = new UnityAction(player.GetComponent<PlayerAttack>().UltiClick);

            _NormalButton.onClick.AddListener(normalAttack);
            _SpecialButton.onClick.AddListener(specialAttack);
            _UltiButton.onClick.AddListener(ultiAttack);

            //assign shop buttons based on teamID
            _ShopSpecialButton1 = FindMe.instance.P_ShopSpecial1.GetComponent<Button>();
            _ShopSpecialButton2 = FindMe.instance.P_ShopSpecial2.GetComponent<Button>();
            _ShopUltiButton1 = FindMe.instance.P_ShopUlti1.GetComponent<Button>();
            _ShopUltiButton2 = FindMe.instance.P_ShopUlti2.GetComponent<Button>();
            _ShopX = FindMe.instance.P_ShopUI_X.GetComponent<Button>();

            if(teamID == 1)
            {
                shop = FindMe.instance.P_BlueShop;
                buySpecial1 = new UnityAction(shop.GetComponent<BlueShop>().SelectFirstSpecial);
                buySpecial2 = new UnityAction(shop.GetComponent<BlueShop>().SelectSecondSpecial);
                buyUlti1 = new UnityAction(shop.GetComponent<BlueShop>().SelectFirstUlti);
                buyUlti2 = new UnityAction(shop.GetComponent<BlueShop>().SelectSecondUlti);
                closeShop = new UnityAction(shop.GetComponent<BlueShop>().CloseShopUI);
            }
            else if(teamID == 2)
            {
                shop = FindMe.instance.P_RedShop;
                buySpecial1 = new UnityAction(shop.GetComponent<RedShop>().SelectFirstSpecial);
                buySpecial2 = new UnityAction(shop.GetComponent<RedShop>().SelectSecondSpecial);
                buyUlti1 = new UnityAction(shop.GetComponent<RedShop>().SelectFirstUlti);
                buyUlti2 = new UnityAction(shop.GetComponent<RedShop>().SelectSecondUlti);
                closeShop = new UnityAction(shop.GetComponent<RedShop>().CloseShopUI);
            }

            _ShopSpecialButton1.onClick.AddListener(buySpecial1);
            _ShopSpecialButton2.onClick.AddListener(buySpecial2);
            _ShopUltiButton1.onClick.AddListener(buyUlti1);
            _ShopUltiButton2.onClick.AddListener(buyUlti2);
            _ShopX.onClick.AddListener(closeShop);
        }
    }
}