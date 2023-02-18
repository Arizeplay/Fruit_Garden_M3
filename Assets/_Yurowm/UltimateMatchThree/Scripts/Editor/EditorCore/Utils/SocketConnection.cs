// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.SocketConnection
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Yurowm.EditorCore.Utils
{
  internal class SocketConnection : IDisposable
  {
    public Socket socket;
    public IPEndPoint end_point;
    public static bool busy;
    public static bool waitDisconnect;
    internal static int numberOfConnection;
    internal static int numberOfDisconnection;
    private const int bufferSize = 8192;

    internal static int numberOfActiveConnection => SocketConnection.numberOfConnection - SocketConnection.numberOfDisconnection;

    public SocketConnection(bool connect = true)
      : this(NWUtils.GetServerIP(), connect)
    {
    }

    public SocketConnection(IPAddress ip, bool connect = true)
    {
      if (SocketConnection.waitDisconnect)
        SocketConnection.WaitDisconnect();
      SocketConnection.busy = true;
      this.end_point = new IPEndPoint(ip, 11001);
      try
      {
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.socket.SendBufferSize = 8192;
        this.socket.ReceiveBufferSize = 8192;
        if (connect)
          this.socket.Connect((EndPoint) this.end_point);
        ++SocketConnection.numberOfConnection;
      }
      catch (SocketException ex)
      {
        SocketConnection.busy = false;
        throw new NWUtils.JMNWException(ex.SocketErrorCode);
      }
      catch (Exception ex)
      {
        SocketConnection.busy = false;
        Console.WriteLine((object) ex);
      }
    }

    public SocketConnection(Socket _socket)
    {
      this.socket = _socket;
      ++SocketConnection.numberOfConnection;
      NWUtils.Log(string.Format("Connected: {0} ({1})", (object) _socket.RemoteEndPoint, (object) Thread.CurrentThread.Name));
    }

    public void Dispose()
    {
      ++SocketConnection.numberOfDisconnection;
      SocketConnection.busy = false;
      this.socket.Disconnect();
      this.socket = (Socket) null;
    }

    public void Send(string message) => this.socket.SendMessage(message);

    public void Send(byte[] raw) => this.socket.SendMessage(raw);

    public void Send(NWMessage message) => this.socket.SendMessage(message.ToJSON());

    public void SendFile(FileInfo file)
    {
      Console.WriteLine(file.FullName);
      FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
      byte[] numArray = new byte[fileStream.Length];
      fileStream.Read(numArray, 0, (int) fileStream.Length);
      fileStream.Close();
      this.socket.SendMessage(numArray);
    }

    public string Receive() => this.socket.ReceiveMessage();

    public byte[] ReceiveRaw() => this.socket.ReceiveRawMessage();

    public NWMessage ReceiveMessage() => NWMessage.FromJSON(this.socket.ReceiveMessage());

    public void ReceiveFile(FileInfo file)
    {
      byte[] rawMessage = this.socket.ReceiveRawMessage();
      if (file.Exists)
        file.Delete();
      Console.WriteLine(file.FullName);
      FileStream fileStream = new FileStream(file.FullName, FileMode.CreateNew, FileAccess.Write);
      fileStream.Write(rawMessage, 0, rawMessage.Length);
      fileStream.Close();
    }

    public static void WaitDisconnect()
    {
      while (SocketConnection.busy)
        Thread.Sleep(50);
    }
  }
}
