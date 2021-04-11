using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounter : MonoBehaviour
{
    public float PlayerCount;
    // Start is called before the first frame update
    void Start()
    {
        PlayerCount = 0;
    }
    
    public void addPlayer()
    {
        PlayerCount += 1;
    }

    public void deductPlayer()
    {
        PlayerCount -= 1;
    }
}
