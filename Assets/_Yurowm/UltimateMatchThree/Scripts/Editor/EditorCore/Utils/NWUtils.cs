// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.NWUtils
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Yurowm.EditorCore.Utils
{
  internal static class NWUtils
  {
    private const int maxLength = 1048576;
    private const int maxDelaySec = 5;
    public static bool log;
    public static FileInfo logFile;
    public static DateTime logPeriod;
    private const int logCrop = 500;

    public static IPAddress GetServerIP() => new IPAddress(((IEnumerable<string>) "188.164.250.119".Split('.')).Select<string, byte>((Func<string, byte>) (x => byte.Parse(x))).ToArray<byte>());

    public static void SendMessage(this Socket sender, string message)
    {
      int length = message.Length;
      sender.Send(BitConverter.GetBytes(length));
      if (length <= 0)
        return;
      SocketError errorCode = SocketError.Success;
      int offset = 0;
      byte[] bytes = Encoding.ASCII.GetBytes(message);
      while (offset < length)
        offset += sender.Send(bytes, offset, length - offset, SocketFlags.None, out errorCode);
      if (errorCode != SocketError.Success)
        throw new NWUtils.JMNWException(errorCode);
      NWUtils.Log("Message sent: " + message + "\n Result: " + (object) errorCode, NWUtils.LogType.Sent);
    }

    public static void SendMessage(this Socket sender, byte[] raw)
    {
      sender.SendTimeout = 30000;
      int length = raw.Length;
      sender.Send(BitConverter.GetBytes(length));
      if (length <= 0)
        return;
      SocketError errorCode = SocketError.Success;
      int offset = 0;
      while (offset < length)
        offset += sender.Send(raw, offset, length - offset, SocketFlags.None, out errorCode);
      if (errorCode != SocketError.Success)
        throw new NWUtils.JMNWException(errorCode);
      NWUtils.Log("Raw message sent. Size: " + (object) raw.Length + ", Result: " + (object) errorCode, NWUtils.LogType.Sent);
    }

    public static string ReceiveMessage(this Socket receiver)
    {
      try
      {
        receiver.ReceiveTimeout = 30000;
        string message = "";
        byte[] buffer = new byte[4];
        receiver.Receive(buffer);
        int int32 = BitConverter.ToInt32(buffer, 0);
        DateTime now = DateTime.Now;
        if (int32 > 1048576)
          throw new Exception("Message is too long");
        if (int32 > 0)
        {
          SocketError errorCode = SocketError.Success;
          byte[] numArray = new byte[int32];
          int offset = 0;
          while (offset < int32 && errorCode == SocketError.Success)
          {
            int num = receiver.Receive(numArray, offset, int32 - offset, SocketFlags.None, out errorCode);
            if (num > 0)
            {
              offset += num;
              now = DateTime.Now;
            }
            else if ((DateTime.Now - now).TotalSeconds > 5.0)
              throw new Exception("Message recieving is stoped");
          }
          message = Encoding.ASCII.GetString(numArray, 0, int32);
          NWUtils.Log("Message received: " + message + "\n Result: " + (object) errorCode, NWUtils.LogType.Received);
          if (errorCode != SocketError.Success)
            throw new NWUtils.JMNWException(errorCode);
        }
        return message;
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    public static byte[] ReceiveRawMessage(this Socket receiver)
    {
      byte[] buffer = new byte[4];
      receiver.Receive(buffer);
      int int32 = BitConverter.ToInt32(buffer, 0);
      DateTime now = DateTime.Now;
      if (int32 > 1048576)
        throw new Exception("Message is too long");
      if (int32 > 0)
      {
        SocketError errorCode = SocketError.Success;
        buffer = new byte[int32];
        int offset = 0;
        while (offset < int32 && errorCode == SocketError.Success)
        {
          int num = receiver.Receive(buffer, offset, int32 - offset, SocketFlags.None, out errorCode);
          if (num > 0)
          {
            offset += num;
            now = DateTime.Now;
          }
          else if ((DateTime.Now - now).TotalSeconds > 5.0)
            throw new Exception("Message recieving is stoped");
        }
        if (errorCode != SocketError.Success)
          throw new NWUtils.JMNWException(errorCode);
        NWUtils.Log("Raw message received. Size: " + (object) buffer.Length + "; Result: " + (object) errorCode, NWUtils.LogType.Received);
      }
      return buffer;
    }

    public static void Disconnect(this Socket socket)
    {
      try
      {
        if (socket == null || !socket.Connected)
          return;
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
      }
      catch (Exception ex)
      {
      }
    }

    public static string ToMessage(this Dictionary<string, string> dictionary) => string.Join("`", dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key + "~" + x.Value)).ToArray<string>());

    public static Dictionary<string, string> ToDictionary(this string line) => ((IEnumerable<string>) line.Split('`')).ToList<string>().Select<string, string[]>((Func<string, string[]>) (x => x.Split('~'))).ToDictionary<string[], string, string>((Func<string[], string>) (x => x[0]), (Func<string[], string>) (x => x[1]));

    public static void Log(string text, NWUtils.LogType type = NWUtils.LogType.Plain)
    {
      if (type != NWUtils.LogType.Error)
      {
        if (type != NWUtils.LogType.Plain)
          return;
        Debug.Log((object) text);
      }
      else
        Debug.LogError((object) text);
    }

    public enum LogType
    {
      Sent,
      Received,
      Error,
      Plain,
      Hidden,
    }

    internal class JMNWException : Exception
    {
      public readonly string error;

      public JMNWException(SocketError error) => this.error = error.ToString();
    }
  }
}
