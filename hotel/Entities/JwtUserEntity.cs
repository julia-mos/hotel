using System;
using System.Collections.Generic;

namespace hotel.Entities
{
    public class JwtUserEntity
    {
        public string UserId;
        public List<string> roles;

        public JwtUserEntity(string UserId, List<string> roles)
        {
            this.UserId = UserId;
            this.roles = roles;
        }
    }
}
