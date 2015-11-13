using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using AmazonAws.Shared;

namespace AmazonAws.Ses
{
    public static class Repository
    {
        public static void Send(string from, string to, string bcc, string subject, string body)
        {
            using (var client = new AmazonSimpleEmailServiceClient(Settings.AccessKey, Settings.Secret, RegionEndpoint.USWest2))
            {
                var request = new SendRawEmailRequest
                {
                    Source = from,
                    Destinations = new List<string> { to, bcc },
                    RawMessage = CreateMessage(from, to, bcc, subject, body)
                };

                client.SendRawEmail(request);
            }
        }

        private static RawMessage CreateMessage(string from, string to, string bcc, string subject, string body)
        {
            var emailMessage = new EmailMessage();

            emailMessage.To.Add(to);
            emailMessage.Bcc.Add(bcc);
            emailMessage.From = from;
            emailMessage.Subject = subject;
            emailMessage.Body = body;

            return ToRawMessage(emailMessage.Message);
        }

        private static RawMessage ToRawMessage(MailMessage message)
        {
            var rawMessage = new RawMessage();

            using (var memoryStream = message.ToMemoryStream())
            {
                rawMessage.Data = memoryStream;
            }

            return rawMessage;
        }

        private static MemoryStream ToMemoryStream(this MailMessage message)
        {
            var assembly = typeof(SmtpClient).Assembly;
            var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");
            var fileStream = new MemoryStream();
            var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);
            var mailWriter = mailWriterContructor.Invoke(new object[] { fileStream });

            var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);
            sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true, true }, null);

            var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);
            closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);

            return fileStream;
        }
    }
}
