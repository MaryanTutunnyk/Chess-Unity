using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using Unity.Networking.Transport;
using UnityEngine;

public class NetworkController : Singleton<NetworkController>
{
    private string ip = "127.0.0.1";
    private ushort port = 8007;

    [SerializeField] private ServerBehaviour Server;
    [SerializeField] private ClientBehaviour Client;

    private int playerCount = -1;
    private int currentTeam = -1;

    public int CurrentTeam => currentTeam;

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    public void Connect()
    {
        if (IsPortUnavailable(port))
        {
            Debug.Log("Port is unavailable");
            Client.Init(ip, port);
        }
        else
        {
            Debug.Log("Port is available");
            Server.Init(port);
            Client.Init(ip, port);
        }

        RegisterEvents();
    }

    private bool IsPortUnavailable(ushort port)
    {
        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] ucpEndPoints = properties.GetActiveUdpListeners();
        List<int> usedPorts = ucpEndPoints.Select(p => p.Port).ToList<int>();
        return usedPorts.Contains(port);
    }

    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;

        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
    }

    private void UnregisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;

        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGameClient;
        NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;
    }

    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        NetWelcome nw = msg as NetWelcome;

        nw.AssignedTeam = ++playerCount;

        ServerBehaviour.Instance.SendToClient(cnn, nw);

        if (playerCount == 1)
        {
            ServerBehaviour.Instance.Broadcast(new NetStartGame());
        }
    }

    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
    {
        NetMakeMove mm = msg as NetMakeMove;

        ServerBehaviour.Instance.Broadcast(mm);
    }

    private void OnWelcomeClient(NetMessage msg)
    {
        NetWelcome nw = msg as NetWelcome;

        currentTeam = nw.AssignedTeam;

        Debug.Log("My assigned team is " + nw.AssignedTeam);
    }

    private void OnMakeMoveClient(NetMessage msg)
    {
        NetMakeMove mm = msg as NetMakeMove;

        Debug.Log("Make move team " + mm.TeamID + " from " + mm.OriginalX + " " + mm.OriginalY + " to " + mm.DestinationX + " " + mm.DestinationY);

        if (mm.TeamID != currentTeam)
        {
            Board.Instance.MovePiece(mm.OriginalX, mm.OriginalY, mm.DestinationX, mm.DestinationY);
            Board.Instance.ResetMoves();

            GameController.Instance.ChangeTurns();
            GameController.Instance.EvaluatePlayers();
        }
    }

    private void OnStartGameClient(NetMessage msg)
    {
        GameController.Instance.StartGame();
    }
}
