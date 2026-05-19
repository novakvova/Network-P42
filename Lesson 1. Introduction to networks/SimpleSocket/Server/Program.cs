
//Установка кодування
using System.Net;
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


