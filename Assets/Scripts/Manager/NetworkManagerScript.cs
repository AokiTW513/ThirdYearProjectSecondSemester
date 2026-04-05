using Mirror;
using UnityEngine;

public class NetworkManagerScript : NetworkManager
{
    public static NetworkManagerScript Instance { get; private set; }

    public override void Awake()
    {
        base.Awake();

        if(Instance != null)
        {
            Destroy(gameObject);   
        }
        else
        {
            Instance = this;   
        }
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("開始架設伺服器");
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        Debug.Log("停止架設伺服器");
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"已連接．連線ID:{conn.connectionId}，IP:{conn.address}");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log($"已斷開連接．連線ID:{conn.connectionId}，IP:{conn.address}");
       }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"已新增玩家，連線ID:{conn.connectionId}，IP:{conn.address}");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("開始伺服器");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("停止架設伺服器");
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        Debug.Log($"伺服器已轉換場景{newSceneName}");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("客戶端連接");
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("客戶端斷開連接");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("開始客戶端");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("關閉客戶端");
    }
}