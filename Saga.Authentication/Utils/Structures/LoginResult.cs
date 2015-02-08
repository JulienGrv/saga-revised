using System;

public struct LoginResult
{
    public uint userid;
    public string lg_username;
    public string lg_password;
    public DateTime lg_entry;
    public byte lg_gender;
    public long last_server;
    public uint last_session;
    public uint ative_session;
    public bool is_testaccount;
    public bool is_activated;
    public bool is_banned;
    public bool has_agreed;
    public byte gmlevel;
    public DateTime DateOfBirth;
}