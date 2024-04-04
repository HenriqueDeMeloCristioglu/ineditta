using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.BuildingBlocks.Core.Tokens
{
    public interface ITokenService
    {
        public string Create(TimeSpan expireTimeFromNow);
        public bool IsExpired(string token);
    }
}
