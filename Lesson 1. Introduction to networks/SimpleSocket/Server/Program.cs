
//Установка кодування
using System.Net;
using System.Net.Sockets;
using System.Text;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Запуск сервера і програми");

var hostName = Dns.GetHostName();
Console.WriteLine($"Мій хост: {hostName}");

//Ми можемо визначити адреси нашого ПК
var localhost = await Dns.GetHostEntryAsync(hostName);
int i = 0;
Console.WriteLine("Оберіть IP адрес на якому " +
    "буде працювати сервер:");
foreach(var item in localhost.AddressList)
{
    Console.WriteLine($"{++i}.{item}");
}
Console.Write("->_");
int numberIP = int.Parse(Console.ReadLine());

var serverIp = localhost.AddressList[numberIP - 1];
///Кінцева точка - MS SQL Server
///На якому працює MS SQL Server - 1433
///PostgreSQL - 5432
/// У Вас є Ip адреса + порт на якому запущена програма
int serverPort = 1087; // це порт, який буде для нашої програми

Console.Title = serverIp + ":" + serverPort;

//Сокет - Це спеціальний інструмент для відпрвки
//або отримання повідомлень у вигляді байт
//1- Створюємо кінцеву точнку, до якої може звертатися
IPEndPoint iPEndPoint = new IPEndPoint(serverIp, serverPort);
//2 - створюємо сокет, який може не себе приймати повідомлення
Socket server = new Socket(AddressFamily.InterNetwork,
    SocketType.Stream, ProtocolType.Tcp);
try
{
    //Робимо прив'язку до нашої кінцевої точки
    server.Bind(iPEndPoint);
    //Розмір черги на отримання запитів від клієнтів
    server.Listen(10);
    while(true)
    {
        Console.WriteLine("-----Сервер очієкує клієнтів----");
        //У цьому коді він очікує інформації від клієнтів
        Socket client = server.Accept(); //Тут зупинка, поки хось не постуєкає
        Console.WriteLine($"До нас постукав {client.RemoteEndPoint}");
        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }
}
catch(Exception ex)
{
    Console.WriteLine("Сталася халепа Хюстон - " + ex.ToString());
}

Console.ReadKey(); // Пауза