using System;


[Serializable()]
public class ServerInfo
{
    public ServerInfo(string name, long ip, int port)
    {
        this.name = name;
        this.ip = ip;
        this.port = port;
    }

    public string name;
    public long ip;
    public int port;
}

public interface IServerManager
{
    bool Subscribe(ServerInfo info);
    bool Unsubscribe(ServerInfo info);
}

