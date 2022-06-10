using System;
namespace hotel
{
    public class AppSecrets
    {
        public Connections ConnectionStrings { get; set; }

        public JWTConfig JWT { get; set; }


        public class Connections
        {
            public string DbConnectionString { get; set; }
        }

        public class JWTConfig
        {
            public string Secret { get; set; }
        }
    }
}
