using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.Model
{
    [SerializableAttribute]
    public class EmailInfo
    {
        public EmailInfo()
        {
            ReplayTo = null;
        }



        public string Subject { get; set; }
        public string Body { get; set; }
        public List<MailAddress> MailAddress { get; set; }
        public List<MailAddress> CC { get; set; }
        public List<Attachment> Attachment { get; set; }
        /// <summary>
        /// 邮件回复地址
        /// </summary>
        public MailAddress ReplayTo { get; set; }


        public int FromPort { get; set; }
        public string FromEmailAddress { get; set; }
        public string FromEmailPwd { get; set; }
        public string FromEmailHost { get; set; }
        public string FromEmailDisplayName { get; set; }

    }
}
