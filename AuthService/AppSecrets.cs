using System;
namespace AuthService
{
    public class AppSecrets
    {
        public Connections ConnectionStrings { get; set; }

        public class Connections
        {
            public string DbConnectionString { get; set; }
        }
    }
}
