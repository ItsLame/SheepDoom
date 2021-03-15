using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    //shop's UI
    public GameObject ShopMenuUI;

    //game's control UI
    public GameObject GameUI;

    //for pressing the shop with raycast
    private void Update()
    {
        //if more than one touch and at the beginning of the touch
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            //get a raycast to where you are touching
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            //store the info of hit object
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //if hit something
                if (hit.collider.gameObject.CompareTag("Shop"))
                {
                    Debug.Log("Shop Pressed!");
                    OpenShopUI();
                }
            }
        }

#if UNITY_EDITOR  //<-- only in unity editor
        //for PC 
        if (Input.GetMouseButtonDown(0))
        {
            //store the info of hit object
            RaycastHit hit;
            Debug.Log("Mouse 0 down");
            //get a raycast to where you are touching
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name + " was clicked");

                //if hit something
                if (hit.collider.gameObject.CompareTag("Shop"))
                {
                    Debug.Log("Shop Pressed!");
                    OpenShopUI();
                }
            }
        }
#endif

    }

    //opening shop
    public void OpenShopUI()
    {
        GameUI.GetComponent<Canvas>().enabled = false;
        ShopMenuUI.SetActive(true);
     }

    public void CloseShopUI()
    {
        GameUI.GetComponent<Canvas>().enabled = true;
        ShopMenuUI.SetActive(false);
    }
}
