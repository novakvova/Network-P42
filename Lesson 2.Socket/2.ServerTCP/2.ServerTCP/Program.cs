// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _2.ServerTCP;
class Program
{
    //для блокування потоку
    //з безпекою виконання додатку
    //він буде парцювати між потоково - це означає що не важно у якому потоці було запущено,
    //він буде блокований гломально - це означає, що якщо на сервер стукає два клієнта
    //однчасно вони будуть оброблаяти у різних потоках
    static readonly object _lock = new object();
    //списко клієнтів, які будуть в чаті
    static readonly Dictionary<int, TcpClient> list_clients = new();
    static async Task Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        int countClients = 1; //кількість клієнтів - по суті його id

        Console.WriteLine("----Сервер TCP----");
        var hostName = Dns.GetHostName();
        Console.WriteLine($"Мій хости {hostName}" );
        //визначаємо ip адрес
        var localhost = await Dns.GetHostEntryAsync(hostName);
        //отримав список IP адрес
        var count = localhost.AddressList.Length;
        var ip = localhost.AddressList[count-1];// по суті - це мій гломальний ip
        int i = 0;
        Console.WriteLine("Вкажіть IP адресу:");
        foreach(var addr in localhost.AddressList)
            Console.WriteLine($"{++i}.{addr}");
        Console.Write($"({ip})->_");
        var temp = Console.ReadLine();
        if(!string.IsNullOrEmpty(temp)) //якщо користувач вкаже не пусту адресу, то ми її обираємо
            ip = IPAddress.Parse(temp);
        int port = 4789;
        Console.Title = $"Ваш IP {ip}:{port}";
        TcpListener socketServer = new TcpListener(ip, port);
        socketServer.Start();
        Console.WriteLine($"Run server: {ip}:{port}");
        while(true)
        {
            TcpClient client = socketServer.AcceptTcpClient(); //очікуємо запитів
            lock(_lock)
            {
                list_clients.Add(countClients, client); //Зберігаю інформацію про клієнта
            }
            Console.WriteLine($"На сервер підключився новий клієнт {client.Client.RemoteEndPoint}");
            Thread t = new Thread(handle_clients); // вказує на метод handle_clients
            t.Start(countClients); //передаю ідентифікатор клієнта
            countClients++; //Збільшуємо лічисльник клієнтів
        }
    }
    //оброник для клієнтів, які заходять у чат
    public static void handle_clients(object c)
    {
        int id = (int)c;//отримали номер клієнта
        TcpClient client;
        lock(_lock)
        {
            client = list_clients[id]; //Отримуємо клієнта, який потрапив у чат
        }
        //кожне клієнт зможе відпраляти повідомленя по своє зєднанню на север,
        //тому ми у циклі буде очікувати повідомлень від кожного клієнта
        try
        {
            while(true)
            {
                //Отримуємо потік від клієнта
                NetworkStream strm = client.GetStream();
                var buffer = new byte[10240]; //Розмір буферу для повідомлень
                int byte_count = strm.Read(buffer); //читаємо дані у буфер
                if (byte_count == 0)
                    break;
                //Читаємо дані від клієнта
                string data = Encoding.UTF8.GetString(buffer,0,byte_count);
                Console.WriteLine($"Client message {data}");
                brodcast(data); // це метод, який повімлення від 1 клієнта розсилає усім іншим у чаті
            }
        }
        catch { }
        lock (_lock)
        {
            Console.WriteLine($"Чат покинув клієнт {client.Client.RemoteEndPoint}");
            list_clients.Remove(id); // видаляємо клієнта із списку клієнтів, щоб не відправляти йому повідмлення
        }
        client.Client.Shutdown(SocketShutdown.Both);
        client.Close();
    }
    public static void brodcast(string data) //Розсилає усім клієнта
    {
        //Розсилаємо усім козакам повідомлення хто є у чаті
        byte[] buffer = Encoding.UTF8.GetBytes(data);
        lock(_lock) //Блокуємо потік, щоб росилка відбулася стабільно
        {
            try
            {
                foreach(var c in list_clients.Values) //Отримали список клієнтів
                {
                    NetworkStream stream = c.GetStream(); //Отримали потік клієнта
                    stream.Write(buffer); //відпраляємо клієнту повідомлення
                }
            }
            catch { }
        }
    }
}

