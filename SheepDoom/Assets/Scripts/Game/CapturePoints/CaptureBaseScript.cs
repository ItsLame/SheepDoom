using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CaptureBaseScript : MonoBehaviour
{
    //attach the score gameobject to count the score
    public GameObject scoreGameObject;

    //Base hp counters
    [Space(20)]
    [SerializeField]
    //base hp
    [Tooltip("How much HP the Base has, edit this")]
    private float BaseHP;
    [SerializeField]
    private float BaseInGameHP; //to be used in game, gonna be the one fluctuating basically

    //rate of capture
    [SerializeField]
    private float BaseCaptureRate;

    //regeneration rate if not under capture
    [SerializeField]
    private float BaseRegenRate;

    //captured bools
    [Space(20)]
    [SerializeField]
    private bool CapturedByBlue2;
    [SerializeField]
    private bool CapturedByRed2;
    [SerializeField]
    private int numOfCapturersBase; //logging number to check if Base is under capture or not

    public event Action<float> OnHealthPctChangedTower = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        //set the Base's hp based on the settings
        BaseInGameHP = BaseHP;

        //single player mode, red team ownership at start
        CapturedByRed2 = true;

        //no one is capturing it at start so put at 0
        numOfCapturersBase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //regen hp if tower is not under capture
        if ((numOfCapturersBase == 0) && (BaseInGameHP < BaseHP))
        {
            //BaseInGameHP += BaseRegenRate * Time.deltaTime;
            modifyinghealth(BaseRegenRate * Time.deltaTime);
            //debug showing base hp
            Debug.Log(this.name + " HP: " + BaseInGameHP);
        }

        //once HP = 0, notify the scoring and convert the Base
        //for now since single player mode, only use blue team's settings
        if (BaseInGameHP <= 0 && !CapturedByBlue2)
        {
            //show which point is captured, change point authority and max out BaseHP
            Debug.Log(this.name + " Captured By Blue Team");
            CapturedByBlue2 = true;
            CapturedByRed2 = false;
            //BaseInGameHP = BaseHP;
            modifyinghealth(BaseHP);
            //reference the score script to END THE GAME IN BLUE VICTORY   <------------------------------------------------- GAME END CALL
            scoreGameObject.GetComponent<GameScore>().GameEnd(1);
        }

        //change color when captured by blue
        if (CapturedByBlue2)
        {
            var captureRenderer = this.GetComponent<Renderer>();
            captureRenderer.material.SetColor("_Color", Color.blue);
        }

        //else its red
        else
        {
            var captureRenderer = this.GetComponent<Renderer>();
            captureRenderer.material.SetColor("_Color", Color.red);
        }
    }

    public void modifyinghealth(float amount)
    {
        BaseInGameHP += amount;

        float currenthealthPct = BaseInGameHP / BaseHP;
        OnHealthPctChangedTower(currenthealthPct);
    }

    //check for player enter
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
        //    Debug.Log("Player In Zone");
            numOfCapturersBase += 1;
        }
    }

    //for capture hp reduction when staying in area
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //single player mode, so only blue team
            if (!CapturedByBlue2)
            {
          //      Debug.Log(other.name + "capturing Base");
                //BaseInGameHP -= BaseCaptureRate * Time.deltaTime;
                modifyinghealth(-(BaseCaptureRate * Time.deltaTime));
                //debug showing base hp
                Debug.Log(this.name + " HP: " + BaseInGameHP);
            }
        }
    }

    //check for player exit
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
        //    Debug.Log("Player Left Zone");
            numOfCapturersBase -= 1;
        }
    }

}
