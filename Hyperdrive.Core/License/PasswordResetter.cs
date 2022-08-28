using MimeKit;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
// using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hyperdrive.Core.License
{
    public class PasswordResetter
    {
        public void SendPasswordResetEmail(string email)
        {
            /*
            string Body = System.IO.File.ReadAllText("License/EmailTemplates/PasswordResetEmail.htm");
            Body = Body.Replace("#code#", "ij6e4a");

            MailMessage message = new MailMessage();
            message.From = new MailAddress("support@displace.international", "HyperdrivePDF Support");
            message.To.Add(email);
            message.Subject = "Password Reset - HyperdrivePDF";
            message.IsBodyHtml = true;
            message.Body = Body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = true;

            smtpClient.Host = "mail.privateemail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential("support@displace.international", "MKLbpy7?!");
            smtpClient.Send(message);
            */
        }

        public void SendEmail()
        {
            /*
            var message = System.IO.File.ReadAllText("License/EmailTemplates/PasswordResetEmail.htm");
            var headersToSign = new[] { HeaderId.From, HeaderId.To, HeaderId.Subject, HeaderId.Date };
            var signer = new DkimSigner("C:/Users/Doug/Desktop/Emails/privatekey.pem", "displace.international", ".")
            {
                AgentOrUserIdentifier = "@eng.example.net",
                Domain = "example.net",
                Selector = "brisbane",
            };

            message.Sign(signer, headersToSign,
                DkimCanonicalizationAlgorithm.Relaxed,
                DkimCanonicalizationAlgorithm.Simple);
            

            string Body = System.IO.File.ReadAllText("License/EmailTemplates/PasswordResetEmail.htm");
            Body = Body.Replace("#code#", "ij6e4a");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("support@displace.international", "HyperdrivePDF Support");
            mailMessage.To.Add("dmvanzee@gmail.com");
            mailMessage.Subject = "Password Reset - HyperdrivePDF";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = Body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = true;

            MimeMessage message = MimeMessage.CreateFromMailMessage(mailMessage);
            HeaderId[] headersToSign = new HeaderId[] { HeaderId.From, HeaderId.Subject, HeaderId.Date };

            string domain = "displace.international";
            string selector = "brisbane";

            DkimSigner signer = new DkimSigner("C:/Users/Doug/Desktop/Emails/privatekey.pem", domain, selector)
            {
                SignatureAlgorithm = DkimSignatureAlgorithm.RsaSha1,
                AgentOrUserIdentifier = "@eng.example.com",
                QueryMethod = "dns/txt",
            };

            // Prepare the message body to be sent over a 7bit transport (such as 
            // older versions of SMTP). This is VERY important because the message
            // cannot be modified once we DKIM-sign our message!
            //
            // Note: If the SMTP server you will be sending the message over 
            // supports the 8BITMIME extension, then you can use
            // `EncodingConstraint.EightBit` instead.
            message.Prepare(EncodingConstraint.SevenBit);

            message.Sign(signer, headersToSign,
                DkimCanonicalizationAlgorithm.Relaxed,
                DkimCanonicalizationAlgorithm.Simple);
            */
        }

        public void SendEmail2()
        {
            /*
            string Body = System.IO.File.ReadAllText("License/EmailTemplates/PasswordResetEmail.htm");
            Body = Body.Replace("#code#", "ij6e4a");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("support@displace.international", "HyperdrivePDF Support");
            mailMessage.To.Add("dmvanzee@gmail.com");
            mailMessage.Subject = "Password Reset - HyperdrivePDF";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = Body;

            string domain = "displace.international";

            var message = MimeMessage.CreateFromMailMessage(mailMessage);

            
            HeaderId[] headers = new HeaderId[] { HeaderId.From, HeaderId.Subject, HeaderId.Date };
            DkimCanonicalizationAlgorithm headerAlgorithm = DkimCanonicalizationAlgorithm.Relaxed;
            DkimCanonicalizationAlgorithm bodyAlgorithm = DkimCanonicalizationAlgorithm.Relaxed;

            string dkimPath = Path.Combine("C:/Users/Doug/Desktop/Emails/privatekey.pem", "DKIM");
            string privateKey = Path.Combine(dkimPath, "kup-nemovitost.cz.private.rsa");

            DkimSigner signer = new DkimSigner(privateKey, domain, "mail")
            {
                SignatureAlgorithm = DkimSignatureAlgorithm.RsaSha1,
                AgentOrUserIdentifier = "@" + domain,
                QueryMethod = "dns/txt",
            };

            message.Prepare(EncodingConstraint.SevenBit);
            //message.Sign(signer, headers, headerAlgorithm, bodyAlgorithm);
            // message.Sign(signer, headerAlgorithm);

            using (var ctx = new MySecureMimeContext())
            {
                message.Body = MultipartSigned.Create(ctx, signer, Body);
            }
            

            var headers = new HeaderId[] { HeaderId.From, HeaderId.Subject, HeaderId.Date };
            var signer = new DkimSigner("C:/Users/Doug/Desktop/Emails/privatekey.pem", "displace.international", "mail", DkimSignatureAlgorithm.RsaSha256)
            {
                HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                AgentOrUserIdentifier = "@displace.international",
                QueryMethod = "dns/txt",
            };

            // Prepare the message body to be sent over a 7bit transport (such as older versions of SMTP).
            // Note: If the SMTP server you will be sending the message over supports the 8BITMIME extension,
            // then you can use `EncodingConstraint.EightBit` instead.
            message.Prepare(EncodingConstraint.SevenBit);

            signer.Sign(message, headers);

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("privateemail.com", 587, true);
                client.Send(message);
                client.Disconnect(true);
            }
            */
        }

        public void SendEmail3()
        {
            MimeMessage mimeMessage = new MimeMessage();


            string Body = System.IO.File.ReadAllText("License/EmailTemplates/PasswordResetEmail.htm");
            Body = Body.Replace("#code#", "ij6e4a");

            var builder = new BodyBuilder();
            //builder.TextBody = body;
            builder.HtmlBody = Body;

            mimeMessage.Body = builder.ToMessageBody();

            mimeMessage.From.Add(new MailboxAddress("HyperdrivePDF Support", "senderEmail@gmail.com"));
            mimeMessage.To.Add(new MailboxAddress("", "dmvanzee@gmail.com"));
            mimeMessage.Subject = "Password Reset - HyperdrivePDF";
            HeaderId[] headersToSign = new HeaderId[] { HeaderId.From, HeaderId.Subject, HeaderId.Date };
            mimeMessage.Prepare(EncodingConstraint.SevenBit);

            var signer = new DkimSigner("C:/Users/Doug/Desktop/Emails/privatekey.pem", "displace.international", "mail", DkimSignatureAlgorithm.RsaSha256)
            {
                HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                AgentOrUserIdentifier = "@displace.international",
                QueryMethod = "dns/txt",
            };

            signer.Sign(mimeMessage, headersToSign);

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("privateemail.com", 587, true);
                client.Authenticate("support@displace.international", "MKLbpy7?!");
                client.Send(mimeMessage);
                client.Disconnect(true);
            }
            
        }

        // "App password" (not working?): chpw-dnsh-tdeb-fjsk
    }
}
