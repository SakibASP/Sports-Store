using System.Text;
using System.Net;
using System.Net.Mail;
using SportsStore.Models;

namespace SportsStore.Common
{
    public class EmailSettings
    {
        public string MailToAddress = "sakibur.rahman.cse@gmail.com";
        public string MailFromAddress = "sendemail.cssbd@gmail.com";
        public bool UseSsl = true;
        public string Username = "sendemail.cssbd@gmail.com";
        public string Password = "laydleksjeddhzqn";
        public string ServerName = "smtp.gmail.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"D:\Sakib Git\Sports Store\Emails";
    }
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    smtpClient.EnableSsl = emailSettings.UseSsl;
                    smtpClient.Host = emailSettings.ServerName;
                    smtpClient.Port = emailSettings.ServerPort;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                    //if (emailSettings.WriteAsFile)
                    //{
                    //    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    //    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    //    smtpClient.EnableSsl = false;
                    //}
                    StringBuilder body = new StringBuilder()
                                            .AppendLine("A new order has been submitted")
                                            .AppendLine("---")
                                            .AppendLine("Items:");
                    foreach (var line in cart.Lines)
                    {
                        var subtotal = line.Product.Price * line.Quantity;
                        body.AppendFormat("{0} x {1} (subtotal: {2:c}",
                        line.Quantity,
                        line.Product.Name,
                        subtotal);
                    }
                    body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                                                                .AppendLine("---")
                                                                .AppendLine("Ship to:")
                                                                .AppendLine(shippingInfo.NAME)
                                                                .AppendLine(shippingInfo.LINE_1)
                                                                .AppendLine(shippingInfo.LINE_2 ?? "")
                                                                .AppendLine(shippingInfo.LINE_3 ?? "")
                                                                .AppendLine(shippingInfo.CITY)
                                                                .AppendLine(shippingInfo.STATE)
                                                                .AppendLine(shippingInfo.COUNTRY)
                                                                .AppendLine(shippingInfo.ZIP ?? "")
                                                                .AppendLine("---")
                                                                .AppendFormat("Gift wrap: {0}", shippingInfo.GIFTWRAP ? "Yes" : "No");
                    MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress, //From
                                                emailSettings.MailToAddress, //To
                                                "New order submitted!", //Subject
                                                body.ToString() // Body
                                                );
                    if (emailSettings.WriteAsFile)
                    {
                        mailMessage.BodyEncoding = Encoding.ASCII;
                    }
                    smtpClient.Send(mailMessage);
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
