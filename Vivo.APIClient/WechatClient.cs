using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vivo.Model;

namespace Vivo.APIClient
{
    public  class WechatClient:BaseAPIClient
    {
        private static APIHellper apiHelper = new APIHellper();


        public WechatClient()
        {
            
        }


        /// <summary>
        /// 根据ID获取图库对象
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public  string GetAccessTonken()
        {
            string URL = "Wechat/GetAccessTokenByNone";

            URL = APIHellper.GetAPI(URL, APIAccessToken);
            string TaskJson =  apiHelper.GetHttpData(URL);

            string result = apiHelper.APIJsonDeserialize<string>(TaskJson);
            return result;
        }



    }
}
