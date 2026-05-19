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
//тут робимо повідомлення для сервера, яке хочемо йому послати
Console.WriteLine("Напишіть текст для сервера: ");
string text = Console.ReadLine();
try
{
    var endPoint = new IPEndPoint(ip, port);
    Socket clientConnect = new Socket(AddressFamily.InterNetwork,
        SocketType.Stream, ProtocolType.Tcp);
    clientConnect.Connect(endPoint);
    var data = Encoding.UTF8.GetBytes(text); // текст буде у байтах
    clientConnect.Send(data); //Відправляємо текст серверу
    //Очищаємо дату і будемо отримувати відповідь від сервера
    data = new byte[1024];
    int bytes = 0; // Кількість байт, що прислє нам сервер
    do
    {
        bytes = clientConnect.Receive(data); // читаємо дані дві сервера
        string str = Encoding.UTF8.GetString(data); // декодую відповідь
        Console.WriteLine($"Сервер сказа: {str}");//виводу дану відповідь
    } while (clientConnect.Available > 0); //доки не прочитаємо усю відповідь
}
catch(Exception ex)
{
    Console.WriteLine("Щось пішло не так: "+ ex.Message);
}

