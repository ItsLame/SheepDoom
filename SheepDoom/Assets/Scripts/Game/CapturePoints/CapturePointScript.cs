using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapturePointScript : MonoBehaviour
{
    //attach the score gameobject to count the score
    public GameObject scoreGameObject;

    //tower hp counters
    [Space(20)]
    [SerializeField]
    //base hp
    [Tooltip("How much HP the tower has, edit this")]
    private float TowerHP;
    [SerializeField]
    private float TowerInGameHP; //to be used in game, gonna be the one fluctuating basically

    //rate of capture
    [SerializeField]
    private float TowerCaptureRate;

    //regeneration rate if not under capture
    [SerializeField]
    private float TowerRegenRate;

    //captured bools
    [Space(20)]
    [SerializeField]
    private bool CapturedByBlue;
    [SerializeField]
    private bool CapturedByRed;
    [SerializeField]
    private int numOfCapturers; //logging number to check if tower is under capture or not

    // Start is called before the first frame update
    void Start()
    {
        //set the tower's hp based on the settings
        TowerInGameHP = TowerHP;

        //single player mode, red team ownership at start
        CapturedByRed = true;

        //no one is capturing it at start so put at 0
        numOfCapturers = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //regen hp if tower is not under capture
        if ((numOfCapturers == 0) && (TowerInGameHP < TowerHP))
        {
            TowerInGameHP += TowerRegenRate * Time.deltaTime;
        }

        //debug showing tower HP
        Debug.Log("Tower HP:" + TowerInGameHP);

        //once HP = 0, notify the scoring and convert the tower
        //for now since single player mode, only use blue team's settings
        if (TowerInGameHP <= 0 && !CapturedByBlue)
        {
            //show which point is captured, change point authority and max out towerHP
            Debug.Log(this.name + "Point Captured By Blue Team");
            CapturedByBlue = true;
            CapturedByRed = false;
            TowerInGameHP = TowerHP;

            //reference the score script to increase score function
            scoreGameObject.GetComponent<Score>().blueScoreUp();
        }

        //change color when captured by blue
        if (CapturedByBlue)
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

    //check for player enter
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player In Zone");
            numOfCapturers += 1;
        }
    }

    //for capture hp reduction when staying in area
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //single player mode, so only blue team
            if (!CapturedByBlue)
            {
                Debug.Log(other.name + "capturing Tower");
                TowerInGameHP -= TowerCaptureRate * Time.deltaTime;
            }
        }
    }

    //check for player exit
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player Left Zone");
            numOfCapturers -= 1;
        }
    }

}
