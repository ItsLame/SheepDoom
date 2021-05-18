using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public class RedShop : Shop
    {
        [SerializeField] private GameObject ShopPlayer = null;
        [SerializeField] private float PlayerGold;
        [SerializeField] private int ShopTeamIndex = 2;

        [Space(15)]
        [SerializeField] private float SpecialCost;
        [SerializeField] private float UltiCost;

        [Space(15)]
        //shop's UI
        [SerializeField] public GameObject ShopMenuUI;

        [Space(15)]
        //shops buttons
        [SerializeField] private GameObject SpecialButton1;
        [SerializeField] private GameObject SpecialButton2;
        [SerializeField] private GameObject UltiButton1;
        [SerializeField] private GameObject UltiButton2;

        [Space(15)]
        //game's control UI
        [SerializeField] private GameObject GameUI;
        //player special & ulti button
        [SerializeField] private GameObject PlayerSpecialButton;
        [SerializeField] private GameObject PlayerUltiButton;

        public override GameObject P_shopPlayer { get => ShopPlayer; set => ShopPlayer = value; }
        //public override float P_playerGold { get => PlayerGold; set => PlayerGold = value; }

        protected override void InitShop()
        {
            P_playerGold = PlayerGold;
            P_shopTeamIndex = ShopTeamIndex;
            P_specialCost = SpecialCost;
            P_ultiCost = UltiCost;
            P_shopMenuUI = ShopMenuUI;
            P_specialButton1 = SpecialButton1;
            P_specialButton2 = SpecialButton2;
            P_ultiButton1 = UltiButton1;
            P_ultiButton2 = UltiButton2;
            P_gameUI = GameUI;
            P_playerSpecialButton = PlayerSpecialButton;
            P_playerUltiButton = PlayerUltiButton;
        }
    }
}

#region archive
/*
//[Space(15)]
//control checks for if player purchased special / ulti
//[SerializeField] private bool HasPurchasedSpecial = false;
//[SerializeField] private bool HasPurchasedUlti = false;

protected override void InitShop()
{
    P_shopTeamIndex = ShopTeamIndex;
    P_specialCost = SpecialCost;
    P_ultiCost = UltiCost;
    P_shopMenuUI = ShopMenuUI;
    P_specialButton1 = SpecialButton1;
    P_specialButton2 = SpecialButton2;
    P_ultiButton1 = UltiButton1;
    P_ultiButton2 = UltiButton2;
    P_gameUI = GameUI;
    P_playerSpecialButton = PlayerSpecialButton;
    P_playerUltiButton = PlayerUltiButton;
}

    public override int P_shopTeamIndex { get => ShopTeamIndex; set => P_shopTeamIndex = value; }
    public override float P_specialCost { get => SpecialCost; set => SpecialCost = value; }
    public override float P_ultiCost { get => UltiCost; set => UltiCost = value; }
    public override GameObject P_shopMenuUI { get => ShopMenuUI; set => ShopMenuUI = value; }
    public override GameObject P_specialButton1 { get => SpecialButton1; set => SpecialButton1 = value; }
    public override GameObject P_specialButton2 { get => SpecialButton2; set => SpecialButton2 = value; }
    public override GameObject P_ultiButton1 { get => UltiButton1; set => UltiButton1 = value; }
    public override GameObject P_ultiButton2 { get => UltiButton2; set => UltiButton2 = value; }
    public override GameObject P_gameUI { get => GameUI; set => GameUI = value; }
    public override GameObject P_playerSpecialButton { get => PlayerSpecialButton; set => PlayerSpecialButton = value; }
    public override GameObject P_playerUltiButton { get => PlayerUltiButton; set => PlayerUltiButton = value; }
*/
#endregion
