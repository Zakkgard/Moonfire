using Moonfire.Core.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.Core.Networking.Interfaces
{
    public interface IAuthClient : IClient
    {
        Authenticator Authenticator { get; set; }
    }
}
