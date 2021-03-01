using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    //public variables
    public NetworkManager myNM;
    public GameObject enterBtn;
    public GameObject hostBtn;
    public GameObject leaveBtn;
    public GameObject joinBtn;
    public GameObject exitBtn;
    public Text playerNameText;
    public Text roomNameText;
    public GameObject playerList;
    public GameObject roomList;
    public GameObject roomPrefab;
    public GameObject titleCanvas;
    public GameObject lobbyCanvas;

    //private variables
    private GameObject clone;
    private string myPlayerName;
    private string myRoomName;

    //server -> client variables
    int roomCount = 0;
    [SyncVar] int roomCountTemp = 0;

    private void Start()
    {
        titleCanvas.SetActive(true);
        lobbyCanvas.SetActive(false);
        myNM.StartServer();
    }

    private void Update()
    {
        if(roomCountTemp != roomCount)
        {
            if(roomCountTemp > roomCount)
            {
                AddToRoomList();
            }
            else
            {
                RemoveFromRoomList();
            }
        }
    }

    public void SetPlayerName(string _playerNameField)
    {
        myPlayerName = _playerNameField;
        playerNameText.text = "Player Name: " + myPlayerName;
    }

    public void SetRoomName(string _roomNameField)
    {
        myRoomName = _roomNameField;
        roomNameText.text = "Room Name: " + myRoomName;
    }

    public void EnterLobby()
    {
        if(myPlayerName == null || myPlayerName =="")
        {
            Debug.Log("Player name invalid! Try again...");
        }
        else
        {
            Debug.Log("Welcome " + myPlayerName + "!");
            myNM.StartClient();

            titleCanvas.SetActive(false);
            lobbyCanvas.SetActive(true);
        }
    }

    public void ExitLobby()
    {
        myNM.StopClient();

        titleCanvas.SetActive(true);
        lobbyCanvas.SetActive(false);    }

    public void MyNewHost()
    {
        if(myRoomName == null || myRoomName =="")
        {
            Debug.Log("Room name invalid! Try again...");
        }
        else
        {
            Debug.Log("Joining " + myRoomName + "...");
            hostBtn.SetActive(false);
            leaveBtn.SetActive(true);
            joinBtn.SetActive(false);
            exitBtn.SetActive(false);

            roomCountTemp += 1;
        }
    }

    public void LeaveHost()
    {
        Debug.Log("Leaving " + myRoomName + "...");
        hostBtn.SetActive(true);
        leaveBtn.SetActive(false);
        joinBtn.SetActive(true);
        exitBtn.SetActive(true);

        roomCountTemp -= 1;
    }

    private void AddToRoomList()
    {
        roomCount += 1;
        clone = (GameObject)Instantiate (roomPrefab, transform.position, Quaternion.identity);
        clone.transform.GetChild(0).GetComponentInChildren<Text>().text = myRoomName;
        clone.transform.GetChild(1).GetComponentInChildren<Text>().text = "1/6";
        clone.transform.parent=roomList.transform;
        
        NetworkServer.Spawn(clone);
    }

    private void RemoveFromRoomList()
    {
        roomCount -= 1;
        //Destroy (clone);

        NetworkServer.Destroy(clone);
   }
}
