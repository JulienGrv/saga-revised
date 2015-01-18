using System;
using System.Collections.Generic;

[Obsolete("Do not use this anymore")]
public interface IZoneManager
{

    bool Subscribe(string uri, int port);
    bool QueryZoneHosted();



}

[Obsolete("Do not use this anymore")]
public interface IPlayerManager
{

    bool GetCharData(uint CharId, out CharDetails details);
    byte[] GetCharList();
    bool DeleteCharacter(uint charId);
    bool VerifyNameExists(string name);
    bool CreateCharacter(out uint charId, CharCreationArgument arg);    
    CharInfo[] FindCharacters(uint UserId);


}

[Obsolete("Do not use this anymore")]
[Serializable()]	



[Obsolete("Do not use this anymore")]
[Serializable()]	
public struct CharCreationArgument
{
    public uint UserId;
    public string CharName;
    public byte[] FaceDetails;
    public string WeaponName;
    public ushort WeaponAffix;
}

[Obsolete("Do not use this anymore")]
[Serializable()]
public struct CharDetails
{
    public uint Jexp;
    public byte[] FaceDetails;
    public byte Map;
}