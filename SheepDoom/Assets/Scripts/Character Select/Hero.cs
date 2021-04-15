using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SheepDoom
{
    public abstract class Hero : MonoBehaviour, IHeroBehaviour
    {
        private string heroName;
        private string heroDesc;
        private Sprite heroIcon;
        private bool isTaken;

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

        protected bool P_isTaken
        {
            get{return isTaken;}
            set{isTaken = value;}
        }

        #endregion

        private void Start()
        {
            InitHeroInfo(); // initialize hero info (name and description)
        }

        protected abstract void InitHeroInfo();

        public abstract void SetTaken(bool _isTaken);

        public virtual void OnClickHero()
        {
            // set hero info locally
            CharacterSelectUIManager.instance.P_heroInfoImg.sprite = P_heroIcon;
            CharacterSelectUIManager.instance.P_heroInfoText.text = P_heroName + "\n-----\n" + P_heroDesc;

            if(P_isTaken == false)
                // set lock in button to interactable after clicking on hero
                CharacterSelectUIManager.instance.P_lockInButton.GetComponent<Button>().interactable = true;
            else
                // set lock in button to not interactable if another client has locked in to the hero
                CharacterSelectUIManager.instance.P_lockInButton.GetComponent<Button>().interactable = false;

            // update view for other clients (of player's hero text under player's name)
            CharacterSelectUIManager.instance.ClientRequestUpdate(P_heroName, false);
        }
    }
}
