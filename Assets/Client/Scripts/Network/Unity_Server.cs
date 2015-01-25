using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Unity_Server : MonoBehaviour {

    // UDP broadcast attributes
    UdpClient udpClient;
    IPEndPoint receiveIPGroup;
    public int remotePort = 19784;
    public int remotePortServer = 19783;
    public float broadcastTimer = 0.25f;
    private float lastTimeBroadcasted;
    private bool clientInitialized = false;
    private bool udpClientActive = false;

    // Unity server attributes
    public int localPort = 25000;
    string myName = "Server";
    string myIP;
    string myGameName = "GameName";
    int playerCount = 0;
    public GameObject syncPrefab;


    void Start()
    {
        //Launch unity server
        myIP = Network.player.ipAddress;
        LaunchServer();

        InitializeBroadcast();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (playerCount == 0 && !udpClientActive)
        {
            InitializeBroadcast();
        }

        if (!clientInitialized && udpClientActive && Time.time - lastTimeBroadcasted > broadcastTimer)
        {
            SendData();
            lastTimeBroadcasted = Time.time;
        }

        if (clientInitialized && udpClientActive)
        {
            udpClient.SafeDispose();
            udpClientActive = false;
        }
    }

    void InitializeBroadcast()
    {
        // UDP broadcast clients        
        udpClient = new UdpClient(remotePortServer, AddressFamily.InterNetwork);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, remotePort);
        udpClient.Connect(groupEP);

        udpClientActive = true;
    }

    void SendData()
    {
        string customMessage = myName + "%" + myIP + "%" + localPort + "%" + myGameName;
        if (customMessage != "")
        {
            udpClient.Send(Encoding.ASCII.GetBytes(customMessage), customMessage.Length);
        }
    }

    void LaunchServer()
    {
        //Network.incomingPassword = "HolyMoly";
        bool useNat = !Network.HavePublicAddress();
        Network.InitializeServer(32, localPort, useNat);
        Debug.Log("Server launched");
    }

    void OnPlayerConnected(NetworkPlayer player)
    {        
        if (playerCount == 0)
        {
            Network.Instantiate(syncPrefab, Vector3.zero, Quaternion.identity, 0);
            clientInitialized = true;
        }

        Debug.Log("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player disconnected" + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);

        playerCount--;
        clientInitialized = false;

        Network.Disconnect();
        LaunchServer();
    }
}
