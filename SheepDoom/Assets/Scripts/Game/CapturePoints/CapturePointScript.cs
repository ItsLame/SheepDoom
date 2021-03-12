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

    //captured bools
    [Space(20)]
    [SerializeField]
    private bool CapturedByBlue;
    [SerializeField]
    private bool CapturedByRed;

    // Start is called before the first frame update
    void Start()
    {
        //set the tower's hp based on the settings
        TowerInGameHP = TowerHP;

        //single player mode, red team ownership at start
        CapturedByRed = true;
    }

    // Update is called once per frame
    void Update()
    {
        //debug showing tower HP
        Debug.Log("Tower HP:" + TowerInGameHP);
        //once HP = 0, notify the scoring and convert the tower
        //for now since single player mode, only use blue team's settings
        if (TowerInGameHP <= 0 && !CapturedByBlue)
        {
            //show which point is captured
            Debug.Log(this.name + "Point Captured By Blue Team");
            CapturedByBlue = true;
            CapturedByRed = false;

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

    //debug for player enter
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player In Zone");
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
}
