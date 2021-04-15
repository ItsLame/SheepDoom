using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public abstract class Hero : NetworkBehaviour, IHeroBehaviour
    {
        private string heroName;
        private string heroDesc;
        private Sprite heroIcon;

        #region Properties

        protected string P_heroName
        {
            get{return heroName;}
            set{heroName = value;}
        }

        protected string P_heroDesc
        {
            get{return heroDesc;}
            set{heroDesc = value;}
        }

        protected Sprite P_heroIcon
        {
            get{return heroIcon;}
            set{heroIcon = value;}
        }

        #endregion

        private void Start()
        {
            InitHeroInfo();

            Debug.Log("HERO ABSTRACT CLASS \n -----");
            Debug.Log("Hero name: "+P_heroName);
            Debug.Log("Hero desc: "+P_heroDesc);
            Debug.Log("Hero icon: "+P_heroIcon);
        }

        protected abstract void InitHeroInfo();

        public virtual void OnClickHero()
        {
            // set hero info locally
            CharacterSelectUIManager.instance.P_heroInfoImg.sprite = P_heroIcon;
            CharacterSelectUIManager.instance.P_heroInfoText.text = P_heroName + "\n-----\n" + P_heroDesc;

            // update view for other clients (of player's hero text under player's name)
            CharacterSelectUIManager.instance.ClientRequestUpdate(P_heroName);
        }
    }
}
