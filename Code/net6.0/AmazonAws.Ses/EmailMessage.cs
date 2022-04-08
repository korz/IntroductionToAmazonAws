using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace AmazonAws.Ses
{
    public class EmailMessage
    {
        public string From { get; set; }
        public IList<string> To { get; set; }
        public IList<string> Bcc { get; set; }

        public string Subject { get; set; }
        public Encoding SubjectEncoding { get; set; }

        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }

        public MailMessage Message { get { return this.CreateMessage(); } }

        public EmailMessage()
        {
            this.SubjectEncoding = Encoding.UTF8;
            this.IsBodyHtml = true;
            this.To = new List<string>();
            this.Bcc = new List<string>();
        }

        private MailMessage CreateMessage()
        {
            var message = new MailMessage
            {
                Subject = this.Subject,
                SubjectEncoding = this.SubjectEncoding,
                Body = this.Body,
                IsBodyHtml = this.IsBodyHtml,
                From = new MailAddress(this.From)
            };

            foreach (var to in this.To)
            {
                message.To.Add(new MailAddress(to));
            }

            //foreach (var bcc in this.Bcc)
            //{
            //    message.Bcc.Add(new MailAddress(bcc));
            //}

            return message;
        }
    }
}