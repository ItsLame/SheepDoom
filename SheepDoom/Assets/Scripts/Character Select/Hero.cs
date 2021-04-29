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
        private bool isTaken; // default is false i guess?

        #region Properties

        public string P_heroName
        {
            get{return heroName;}
            set{heroName = value;}
        }

        public string P_heroDesc
        {
            get{return heroDesc;}
            set{heroDesc = value;}
        }

        public Sprite P_heroIcon
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
            // change hero info view (right side) if do this will change icon for other teammates as well
            CharacterSelectUIManager.instance.P_heroInfoImg.sprite = P_heroIcon;
            CharacterSelectUIManager.instance.P_heroInfoText.text = P_heroName + "\n-----\n" + P_heroDesc;

            // set hero info locally
            if (!P_isTaken)
            {
                // set lock in button to interactable after clicking on hero
                CharacterSelectUIManager.instance.P_lockInButton.GetComponent<Button>().interactable = true;
                // update view for other clients (of player's hero text under player's name)
                CharacterSelectUIManager.instance.ClientRequestUpdate(P_heroName, false);
            }
            else
            {
                // set lock in button to not interactable if another client has locked in to the hero
                CharacterSelectUIManager.instance.P_lockInButton.GetComponent<Button>().interactable = false;
            }
        }
    }
}
