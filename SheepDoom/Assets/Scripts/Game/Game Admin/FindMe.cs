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
        [SerializeField] private GameObject BP1Star;
        [SerializeField] private GameObject BluePlayer1Image;
        [SerializeField] private GameObject BluePlayer1Name;
        [SerializeField] private GameObject BP2Star;
        [SerializeField] private GameObject BluePlayer2Image;
        [SerializeField] private GameObject BluePlayer2Name;
        [SerializeField] private GameObject BP3Star;
        [SerializeField] private GameObject BluePlayer3Image;
        [SerializeField] private GameObject BluePlayer3Name;
        [SerializeField] private GameObject RP1Star;
        [SerializeField] private GameObject RedPlayer1Image;
        [SerializeField] private GameObject RedPlayer1Name;
        [SerializeField] private GameObject RP2Star;
        [SerializeField] private GameObject RedPlayer2Image;
        [SerializeField] private GameObject RedPlayer2Name;
        [SerializeField] private GameObject RP3Star;
        [SerializeField] private GameObject RedPlayer3Image;
        [SerializeField] private GameObject RedPlayer3Name;

        public bool isInit { get; private set;}

        #endregion

        #region Properties
        public GameObject P_ShopSpecial1
        {
            get { return ShopSpecial1; }
            set { ShopSpecial1 = value; }
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

        public GameObject P_BluePlayerImage1
        {
            get { return BluePlayer1Image; }
            set { BluePlayer1Image = value; }
        }

        public GameObject P_BluePlayerName1
        {
            get { return BluePlayer1Name; }
            set { BluePlayer1Name = value; }
        }

        public GameObject P_BluePlayerImage2
        {
            get { return BluePlayer2Image; }
            set { BluePlayer2Image = value; }
        }

        public GameObject P_BluePlayerName2
        {
            get { return BluePlayer2Name; }
            set { BluePlayer2Name = value; }
        }

        public GameObject P_BluePlayerImage3
        {
            get { return BluePlayer3Image; }
            set { BluePlayer3Image = value; }
        }

        public GameObject P_BluePlayerName3
        {
            get { return BluePlayer3Name; }
            set { BluePlayer3Name = value; }
        }

        public GameObject P_RedPlayerImage1
        {
            get { return RedPlayer1Image; }
            set { RedPlayer1Image = value; }
        }

        public GameObject P_RedPlayerName1
        {
            get { return RedPlayer1Name; }
            set { RedPlayer1Name = value; }
        }

        public GameObject P_RedPlayerImage2
        {
            get { return RedPlayer2Image; }
            set { RedPlayer2Image = value; }
        }

        public GameObject P_RedPlayerName2
        {
            get { return RedPlayer2Name; }
            set { RedPlayer2Name = value; }
        }

        public GameObject P_RedPlayerImage3
        {
            get { return RedPlayer3Image; }
            set { RedPlayer3Image = value; }
        }

        public GameObject P_RedPlayerName3
        {
            get { return RedPlayer3Name; }
            set { RedPlayer3Name = value; }
        }

        public GameObject P_BP1Star
        {
            get { return BP1Star; }
            set { BP1Star = value; }
        }

        public GameObject P_BP2Star
        {
            get { return BP2Star; }
            set { BP2Star = value; }
        }

        public GameObject P_BP3Star
        {
            get { return BP3Star; }
            set { BP3Star = value; }
        }

        public GameObject P_RP1Star
        {
            get { return RP1Star; }
            set { RP1Star = value; }
        }

        public GameObject P_RP2Star
        {
            get { return RP2Star; }
            set { RP2Star = value; }
        }

        public GameObject P_RP3Star
        {
            get { return RP3Star; }
            set { RP3Star = value; }
        }



        #endregion

        private void Start()
        {
            instance = this;   
            isInit = true;
        }
    }
}
