

namespace KryX2.Settings
{

    internal enum GameClient
    {
        Starcraft,
        Broodwar,
        Random
    }

    static class UserSettings
    {
        internal static string TargetChannel { get; set; }
        internal static string IdleMessage { get; set; }
        internal static string MasterName { get; set; }
        internal static string ServerName { get; set; }
        internal static string ClientName { get; set; }
        internal static string ClientPassword { get; set; }
        internal static bool RandomNames { get; set; }
        internal static GameClient GameClient { get; set; }
        internal static int ClientsPerProxy { get; set; }
        internal static int MaxClients { get; set; }

        internal static void GenerateSettings() //r/ temp, should be updating setttings through config
        {
            TargetChannel = "op bnetwebs".ToLower();
            MasterName = "Distul".ToLower();
            ServerName = "asia.battle.net";
            IdleMessage = "Test idle message. Please disregard.";
            ClientName = "w3akl0adingU";
            ClientPassword = "mypassaa";
            RandomNames = false;
            GameClient = GameClient.Random;
            ClientsPerProxy = 4;
            MaxClients = 20;
        }
    }
}
