// Decompiled with JetBrains decompiler
// Type: Yurowm.Contact.MailAgent
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Text;
using System.Threading;

namespace Yurowm.Contact
{
  public class MailAgent
  {
    private static List<string> errors = new List<string>();
    private static string to = "yurowm+u3d@gmail.com";
    private static string from = "jellymobileclient@gmail.com";
    private static string server = "smtp.gmail.com";
    private static int port = 587;
    private static MailAgent.ContactDelegate onSent = (MailAgent.ContactDelegate) (() => { });
    private static Thread sendingThread = (Thread) null;
    private static MailMessage mail;

    public static void Send(
      string name,
      string subject,
      string body,
      string replyto,
      MailAgent.ContactDelegate _onSent,
      List<string> attachments = null)
    {
      if (MailAgent.IsSending())
        return;
      MailAgent.onSent += _onSent;
      MailAgent.mail = new MailMessage();
      MailAgent.mail.From = new MailAddress(MailAgent.from, name);
      MailAgent.mail.ReplyTo = new MailAddress(replyto, name);
      MailAgent.mail.To.Add(MailAgent.to);
      MailAgent.mail.Subject = subject;
      MailAgent.mail.Body = body;
      if (attachments != null)
      {
        foreach (string attachment in attachments)
          MailAgent.mail.Attachments.Add(new Attachment(attachment));
      }
      MailAgent.sendingThread = new Thread(new ThreadStart(MailAgent.Sending));
      MailAgent.sendingThread.Start();
    }

    private static void Sending()
    {
      MailAgent.errors.Clear();
      byte[] bytes = new byte[16]
      {
        (byte) 118,
        (byte) 122,
        (byte) 122,
        (byte) 121,
        (byte) 115,
        (byte) 122,
        (byte) 102,
        (byte) 114,
        (byte) 110,
        (byte) 118,
        (byte) 97,
        (byte) 99,
        (byte) 105,
        (byte) 108,
        (byte) 109,
        (byte) 108
      };
      SmtpClient smtpClient = new SmtpClient(MailAgent.server);
      smtpClient.Port = MailAgent.port;
      smtpClient.Credentials = (ICredentialsByHost) new NetworkCredential(MailAgent.from, Encoding.ASCII.GetString(bytes));
      smtpClient.EnableSsl = true;
      smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
      smtpClient.UseDefaultCredentials = false;
      ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback) ((s, certificate, chain, sslPolicyErrors) => true);
      try
      {
        smtpClient.Send(MailAgent.mail);
      }
      catch (Exception ex)
      {
        MailAgent.errors.Add(ex.Message);
        MailAgent.onSent = (MailAgent.ContactDelegate) (() => { });
        return;
      }
      MailAgent.onSent();
      MailAgent.onSent = (MailAgent.ContactDelegate) (() => { });
    }

    public static bool IsSending() => MailAgent.sendingThread != null && MailAgent.sendingThread.IsAlive;

    public static void Break()
    {
      if (MailAgent.sendingThread == null)
        return;
      MailAgent.sendingThread.Abort();
      MailAgent.sendingThread = (Thread) null;
    }

    public static List<string> GetErrors() => MailAgent.errors;

    public delegate void ContactDelegate();
  }
}
