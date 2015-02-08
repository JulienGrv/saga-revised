public struct ServerInfo
{
    public ServerInfo(byte Index, string Name, byte playercount, byte ping)
    {
        this.Index = Index;
        this.Name = Name;
        this.playercount = playercount;
        this.ping = ping;
    }

    public byte Index;
    public string Name;
    public byte ping;
    public byte playercount;
}