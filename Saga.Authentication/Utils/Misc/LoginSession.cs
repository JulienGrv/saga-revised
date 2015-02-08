using Saga.Authentication.Network;
using System.Collections.Generic;
using System.Net;

namespace Saga.Authentication.Utils
{
    internal static class LoginSessionHandler
    {
        public static Dictionary<uint, LoginSession> sessions = new Dictionary<uint, LoginSession>();
    }

    internal class LoginSession
    {
        public static int RequiredAge = 30;
        public static bool INTESTMODE = false;
        public bool IsLoggedIn = false;
        public int NMaxCharacters = 3;
        public int NIncorrectPasswords = 0;
        public int NLoginAttempts = 0;
        public int NCharacterCount = 2;
        public uint UserId = 0;
        public uint playerid = 0;
        public uint characterid = 0;
        public uint Gender = 1;
        public bool IsWaiting = false;
        public byte World = 0;
        public byte Age = 0;
        public uint PedingCharDeletion;
        public uint ActiveSession;
        public byte LastPlayedWorld;
        public byte GmLevel;
        public IPAddress Adress;
        public CharInfo PendingCharInfo;
        public LogonClient client = null;
        public List<CharInfo> list = new List<CharInfo>();
        public Dictionary<uint, byte[]> CachedCharacterStates = new Dictionary<uint, byte[]>();
    }
}