using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_RPG.DTOs.User
{
    public class UserLoginDTO
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }
}