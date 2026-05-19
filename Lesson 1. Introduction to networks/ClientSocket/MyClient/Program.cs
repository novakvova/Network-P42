using System.Net;
using System.Net.Sockets;
using System.Text;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Вкажіть IP сервера:");
Console.Write("->_");
var ip = IPAddress.Parse(Console.ReadLine());
Console.WriteLine("Вкажіть PORT сервера:");
Console.Write("->_");
var port = int.Parse(Console.ReadLine());
try
{
    var endPoint = new IPEndPoint(ip, port);
    Socket clientConnect = new Socket(AddressFamily.InterNetwork,
        SocketType.Stream, ProtocolType.Tcp);
    clientConnect.Connect(endPoint);
}
catch(Exception ex)
{
    Console.WriteLine("Щось пішло не так: "+ ex.Message);
}

