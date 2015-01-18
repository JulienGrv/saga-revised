public struct CharInfo
{
    public string name;
    public byte gender;
    public uint cexp;
    public uint charId;
    public byte map;
    public byte job;
}

public struct AclEntry
{
    public uint RuleId;
    public string IP;
    public string Mask;
    public byte Operation;
}