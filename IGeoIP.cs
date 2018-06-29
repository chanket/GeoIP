using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Licc.GeoIP
{
    public interface IGeoIP
    {
        string Search(IPAddress ip);

        Task<string> SearchAsync(IPAddress ip);
    }
}
