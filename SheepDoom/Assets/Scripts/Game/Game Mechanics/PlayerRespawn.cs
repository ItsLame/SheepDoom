using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour
{
    //target respawn location
    public GameObject respawnLocation;
    [SerializeField]
    private float respawnTimer;
    [SerializeField]
    private float respawnTimerInGame;
    public bool isDead = false;
    [Space(15)]
    public Text respawnText;
    public GameObject deathOverlay;

    // Start is called before the first frame update
    void Start()
    {
        //setting default respawn time, will be manipulated in future
        respawnTimerInGame = respawnTimer;
        //not dead when player starts game
    }

    // Update is called once per frame
    void Update()
    {
        //if dead, respawnTimer counts down
        if (isDead)
        {
            deathOverlay.SetActive(true);
            //splat body
            Rigidbody myRigidBody = GetComponent<Rigidbody>();
            Vector3 moveMe = new Vector3(0, 1, 0);
            myRigidBody.rotation = Quaternion.LookRotation(moveMe);

            //deactivate movement
            this.gameObject.GetComponent<CharacterMovement>().isDead = true;
            this.gameObject.GetComponent<PlayerAttack>().isDead = true;

            respawnTimerInGame -= Time.deltaTime;
            respawnText.text = respawnTimerInGame.ToString();

            //respawn once timer == 0
            if (respawnTimerInGame <= 0)
            {
                deathOverlay.SetActive(false);
                RespawnPlayer();
            }
        }
    }

    public void RespawnPlayer()
    {
        respawnTimerInGame = respawnTimer;
        isDead = false;

        this.gameObject.transform.position = respawnLocation.transform.position;
        Rigidbody myRigidBody = GetComponent<Rigidbody>();
        Vector3 moveMe = new Vector3(0, 0, 0);
        myRigidBody.rotation = Quaternion.LookRotation(moveMe);
        //activate all buttons
        this.gameObject.GetComponent<CharacterMovement>().isDead = false;
        this.gameObject.GetComponent<PlayerAttack>().isDead = false;
        this.gameObject.GetComponent<PlayerHealth>().RefillHealth();
    }
}
