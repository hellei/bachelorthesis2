using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Unity_Client : MonoBehaviour {

    //UDP Attributes
    UdpClient udpClient;
    IPEndPoint receiveIPGroup;
    string receivedString;
    int remotePort = 19784;
    AsyncCallback receiveCallback;
    bool connectedToServer = false;
    bool ipInitialized = false;
    bool udpClientActive = false;

    //Unity server attributes
    public static Unity_Client instance;
    string serverName;
    static string serverIP;
    static int serverPort;
    string gameName;
    public GameObject syncPrefab;

    void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        StartReceivingIP();
        Debug.Log("Started ip receiver");

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void Update()
    {
        if (ipInitialized && !connectedToServer)
        {
            ConnectToServer();
            connectedToServer = true;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (!ipInitialized && !udpClientActive)
        {
            StartReceivingIP();
        }
    }

    void OnApplicationQuit()
    {
        if (udpClientActive)
        {
            udpClient.SafeDispose();
            udpClientActive = false;
        }

        // Close Connection to Server
        if (Network.connections.Length > 0)
        {
            Network.CloseConnection(Network.connections[0], true);
        }
    }

    void OnApplicationPause() 
    {
        if (udpClientActive)
        {
            udpClient.SafeDispose();
            udpClientActive = false;
        }        

        // Close Connection to Server
        if (Network.connections.Length > 0)
        {
            Network.CloseConnection(Network.connections[0], true);
            Destroy(syncPrefab);
            connectedToServer = false;
            Application.LoadLevel(0);
        }
    }

    void InitializeBroadcastReceiver()
    {
        udpClient = new UdpClient(remotePort);
        receiveCallback = new AsyncCallback(ReceiveData);
        udpClient.BeginReceive(receiveCallback, null);
        udpClientActive = true;
        Debug.Log("Initialized receiver");
    }

    // Async UDP receiver
    public void StartReceivingIP()
    {
        try
        {
            if (udpClient == null)
            {
                InitializeBroadcastReceiver();
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e.Message);
        }
    }
    private void ReceiveData(IAsyncResult result)
    {
        
        receiveIPGroup = new IPEndPoint(IPAddress.Any, remotePort);
        byte[] received;
        if (udpClient != null)
        {
            received = udpClient.EndReceive(result, ref receiveIPGroup);
            Debug.Log("Try receiving");
        }
        else
        {
            Debug.Log("Returned: no receiver initialized");
            return;
        }
        udpClient.BeginReceive(new AsyncCallback(ReceiveData), null);
        receivedString = Encoding.ASCII.GetString(received);
        Debug.Log("Received :" + receivedString);
        ParseUDPString(receivedString);

        ipInitialized = true;
        udpClient.SafeDispose();
    }

    private void ParseUDPString(string receivedString)
    {
        string[] receivedData = new string[4];
        receivedData = receivedString.Split('%');

        serverName = receivedData[0];
        serverIP = receivedData[1];
        serverPort = (int) StringToFloat(receivedData[2]);
        gameName = receivedData[3];
        print(serverName + ", " + serverIP + ", " + serverPort + ", " + gameName);
        
    }

    float StringToFloat(string input)
    {
        System.Globalization.NumberFormatInfo nmbrstyle = new System.Globalization.NumberFormatInfo();
        nmbrstyle.NumberDecimalSeparator = ".";
        nmbrstyle.NumberGroupSeparator = ",";
        return float.Parse(input, nmbrstyle);
    }    

    void ConnectToServer()
    {
        Network.Connect(serverIP, serverPort);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Server Joined");
        connectedToServer = true;
        udpClient.SafeDispose();
    }

    void OnDisconnectedFromServer()
    {
        connectedToServer = false;
        Application.LoadLevel(0);
    }

   
    static string ip = "";

    void OnGUI()
    {
        if (!connectedToServer)
        {
            ip = GUI.TextField(new Rect(Screen.width / 2 - 300, Screen.height - 100, 600, 80), ip);
            if (GUI.Button(new Rect(Screen.width / 2 + 320, Screen.height - 100, 200, 80), "Connect"))
            {
                string[] tmp = new string[2];
                tmp = ip.Split(':');
                serverIP = tmp[0];
                serverPort = (int) StringToFloat(tmp[1]);
                ipInitialized = true;
            }                
        }
    }
}
