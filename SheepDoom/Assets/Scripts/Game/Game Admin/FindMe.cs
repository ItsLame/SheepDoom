using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{
    // let spawned objects (e.g. player objects) find game object inside scene

    public class FindMe : MonoBehaviour
    {
        public static FindMe instance;

        #region Variables

        [Header("--- Player Controls ---")]
        [Space(5)]
        [SerializeField] private GameObject AtkBtn;
        [SerializeField] private GameObject SpecialBtn;
        [SerializeField] private GameObject UltimateBtn;
        
        [Header("--- Player UI ---")]
        [Space(5)]
        [SerializeField] private GameObject DeathOverlay;
        [SerializeField] private GameObject DeadText;
        [SerializeField] private GameObject RespawningIn;
        [SerializeField] private GameObject RespawnTimer;
        
        [Header("--- Game UI ---")]
        [Space(5)]
        [SerializeField] private GameObject ShopUI;
        [SerializeField] private GameObject GameEvent;
        [SerializeField] private GameObject PlayerGold;
        [SerializeField] private GameObject ShopGold;
        
        [Header("--- Game Item ---")]
        [Space(5)]
        [SerializeField] private GameObject BlueShop;
        [SerializeField] private GameObject RedShop;

        [Space(5)]
        [SerializeField] private GameObject ShopSpecial1;
        [SerializeField] private GameObject ShopSpecial2;
        [SerializeField] private GameObject ShopUlti1;
        [SerializeField] private GameObject ShopUlti2;

        [Header("--- Kill, Death, Capture ---")]
        [Space(5)]
        [SerializeField] private GameObject K;
        [SerializeField] private GameObject D;
        [SerializeField] private GameObject C;
        [SerializeField] private GameObject ScoreboardBlue;
        [SerializeField] private GameObject ScoreboardRed;
        [SerializeField] private GameObject BlueWinLose;
        [SerializeField] private GameObject RedWinLose;

        public bool isInit { get; private set;}

        #endregion
        
        #region Properties
        
        public GameObject P_ShopSpecial1
        {
            get { return ShopSpecial1; }
            set { ShopSpecial1 = value;}
        }

        public GameObject P_ShopSpecial2
        {
            get { return ShopSpecial2; }
            set { ShopSpecial2 = value; }
        }

        public GameObject P_ShopUlti1
        {
            get { return ShopUlti1; }
            set { ShopUlti1 = value; }
        }

        public GameObject P_ShopUlti2
        {
            get { return ShopUlti2; }
            set { ShopUlti2 = value; }
        }


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

        public GameObject P_ShopUI
        {
            get{return ShopUI;}
            set{ShopUI = value;}
        }

        public GameObject P_GameEvent
        {
            get{return GameEvent;}
            set{GameEvent = value;}
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

        public GameObject P_ScoreboardBlue
        {
            get { return ScoreboardBlue; }
            set { ScoreboardBlue = value; }
        }

        public GameObject P_ScoreboardRed
        {
            get { return ScoreboardRed; }
            set { ScoreboardRed = value; }
        }

        public GameObject P_BlueWinLose
        {
            get { return BlueWinLose; }
            set { BlueWinLose = value; }
        }

        public GameObject P_RedWinLose
        {
            get { return RedWinLose; }
            set { RedWinLose = value; }
        }

        #endregion

        private void Start()
        {
            instance = this;   
            isInit = true;
        }
    }
}
