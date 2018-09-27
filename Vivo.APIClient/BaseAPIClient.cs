using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.APIClient
{
    public class BaseAPIClient
    {
        public BaseAPIClient()
        {
            APIAccessToken = string.Empty;
        }

        public string APIAccessToken { get; set; }
    }
}
