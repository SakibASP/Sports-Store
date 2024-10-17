using SportsStore.Common;
using SportsStore.Interfaces;
using SportsStore.Models;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace SportsStore.Repositories
{
    public class EmailOrderProcessor(EmailSettings settings) : IOrderProcessor
    {
        public async Task ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            using var smtpClient = new SmtpClient();
            try
            {
                smtpClient.EnableSsl = settings.UseSsl;
                smtpClient.Host = settings.ServerName;
                smtpClient.Port = settings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(settings.Username, settings.Password);

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
                MailMessage mailMessage = new(settings.MailFromAddress, //From
                                            shippingInfo.EMAIL!, //To
                                            "New order submitted!", //Subject
                                            body.ToString() // Body
                                            );
                if (settings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
