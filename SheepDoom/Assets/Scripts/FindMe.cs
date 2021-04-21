using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{
    public class FindMe : MonoBehaviour
    {
        public static FindMe instance;

        [Header("Player Controls")]
        [SerializeField] private GameObject Joystick;
        [SerializeField] private GameObject AtkBtn;
        [SerializeField] private GameObject SpecialBtn;
        [SerializeField] private GameObject UltimateBtn;
        
        [Header("Player UI")]
        [SerializeField] private GameObject DeathOverlay;
        [SerializeField] private GameObject DeadText;
        [SerializeField] private GameObject RespawningIn;
        [SerializeField] private GameObject RespawnTimer;
        
        [Header("Game UI")]
        [SerializeField] private GameObject ShopUI;
        [SerializeField] private GameObject AnnouncerText;
        [SerializeField] private GameObject GameEventText;
        [SerializeField] private GameObject PlayerGold;
        [SerializeField] private GameObject ShopGold;
        
        [Header("Game Item")]
        [SerializeField] private GameObject BlueShop;
        [SerializeField] private GameObject RedShop;
        
        [Header("KDC")]
        [SerializeField] private GameObject K;
        [SerializeField] private GameObject D;
        [SerializeField] private GameObject C;

        public bool isInit { get; private set;}
        
        #region Properties
        
        public GameObject P_AtkBtn
        {
            get{return AtkBtn;}
            set{AtkBtn = value;}
        }
        public GameObject P_SpecialBtn
        {
            get{return SpecialBtn;}
            set{SpecialBtn = value;}
        }
        public GameObject P_UltimateBtn
        {
            get{return UltimateBtn;}
            set{UltimateBtn = value;}
        }
        public GameObject P_DeathOverlay
        {
            get{return DeathOverlay;}
            set{DeathOverlay = value;}
        }
        public GameObject P_ShopUI
        {
            get{return ShopUI;}
            set{ShopUI = value;}
        }

        public GameObject P_K
        {
            get{return K;}
            set{K = value;}
        }
        public GameObject P_D
        {
            get{return D;}
            set{D = value;}
        }
        public GameObject P_C
        {
            get{return C;}
            set{C = value;}
        }

        public GameObject P_BlueShop
        {
            get{return BlueShop;}
            set{BlueShop = value;}
        }

        public GameObject P_RedShop
        {
            get{return RedShop;}
            set{RedShop = value;}
        }

        public GameObject P_DeadText
        {
            get{return DeadText;}
            set{DeadText = value;}
        }
        public GameObject P_RespawningIn
        {
            get{return RespawningIn;}
            set{RespawningIn = value;}
        }
        public GameObject P_RespawnTimer
        {
            get{return RespawnTimer;}
            set{RespawnTimer = value;}
        }

        public GameObject P_AnnouncerText
        {
            get{return AnnouncerText;}
            set{AnnouncerText = value;}
        }

        public GameObject P_GameEventText
        {
            get{return GameEventText;}
            set{GameEventText = value;}
        }

        public GameObject P_PlayerGold
        {
            get{return PlayerGold;}
            set{PlayerGold = value;}
        }
        public GameObject P_ShopGold
        {
            get{return ShopGold;}
            set{ShopGold = value;}
        }

        #endregion

        private void Start()
        {
            instance = this;   
            isInit = true;
        }
    }
}
