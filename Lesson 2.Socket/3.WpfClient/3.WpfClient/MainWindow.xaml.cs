using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3.WpfClient
{

    public class UploadImage
    {
        public string Image { get; set; } = String.Empty;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Зберігає повідомлення 
        private ChatMessage _message = new();
        //Сам клієнт
        private TcpClient _tcpClient = new();
        //Потік для відправки повідомлень
        private NetworkStream _ns;
        private Thread _thread; //робочий потік
        //якщо користувач обрав фото, то воно не буде пустим
        private string _imageUser = String.Empty;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(_imageUser))
            {
                MessageBox.Show("Оберіть фото користувача");
                return;
            }
            else if(string.IsNullOrEmpty(txtUserName.Text))
            {
                MessageBox.Show("Вкажіть ім'я користувача");
                return;
            }
            try
            {
                IPAddress ip = IPAddress.Parse("192.168.50.185");
                int port = 4789;
                _message.UserId = Guid.NewGuid().ToString(); // унікальний Id
                _message.Name = txtUserName.Text;
                _tcpClient.Connect(ip, port); //підключаємося до сервера
                _ns = _tcpClient.GetStream(); //отримуємо потік
                //в окремий потік кладемо відповіді від клієнта
                _thread = new Thread(obj => ResponseData((TcpClient)obj));
                _thread.Start(_tcpClient);
                btnSend.IsEnabled = true;
                btnConnect.IsEnabled = false;//Щоб другий раз не підлкючався
                txtUserName.IsEnabled = false; //Щоб користувача не міг змінювати
                _message.Text = "Приєднався до чату";
                var buffer = _message.Serialize(); //З повідомлення отримав байти
                _ns.Write(buffer);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Проблема підключення до серверу " + ex.Message);
            }

        }

        //imageUrl - посилання на фото
        private void ViewMessage(string text, string imageUrl)
        {
            var grid = new Grid();
            for (int i = 0; i < 2; i++)
            {
                var colDef = new ColumnDefinition();
                colDef.Width = GridLength.Auto;
                grid.ColumnDefinitions.Add(colDef);
            }
            BitmapImage bmp = new BitmapImage(new Uri($"{imageUrl}"));
            var image = new Image();
            image.Source = bmp;
            image.Width = 50;
            image.Height = 50;

            var textBlock = new TextBlock();
            Grid.SetColumn(textBlock, 1);
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(5, 0, 0, 0);
            textBlock.Text = text;
            grid.Children.Add(image);
            grid.Children.Add(textBlock);

            lbInfo.Items.Add(grid);
            lbInfo.Items.MoveCurrentToLast();
            lbInfo.ScrollIntoView(lbInfo.Items.CurrentItem);
        }

        private void ResponseData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] bytes = new byte[16054400];
            int byte_count;
            while((byte_count = ns.Read(bytes))>0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ChatMessage msg = ChatMessage.Deserialize(bytes);
                    string text = $"{msg.Name} -> {msg.Text}";
                    ViewMessage(text, "https://media.istockphoto.com/id/1759448630/photo/happy-caucasian-young-student-female-looking-at-camera-enjoying-with-a-perfect-white-teeth.jpg?s=612x612&w=0&k=20&c=KbfDI3FjAdGYK5QxTx3PJdxFyx9ZNgvOBd0P7E3Ah38=");
                }));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Повідомляє. що ми покидаємо чат
            _message.Text = "Покинув чат";
            var buffer = _message.Serialize();
            _ns.Write(buffer); //Насилаємо повідомлення

            //Тушимо нашого клієнта
            _tcpClient.Client.Shutdown(SocketShutdown.Both);
            _tcpClient.Close();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            _message.Text = txtText.Text;
            var buffer = _message.Serialize();
            _ns.Write(buffer);

            txtText.Text = "";
        }

        //користувач буде обирати фото, для цього ми фото
        //будемо завантажувати на глобальний сервер
        //cheburek.itstep.click
        private void btnPhotoSelect_Click(object sender, RoutedEventArgs e)
        { 
            OpenFileDialog dlg = new OpenFileDialog();
            if(dlg.ShowDialog().Value)
            {
                //MessageBox.Show("Select file", dlg.FileName);
                //Потрібно файл завантажити на сервер для зберігання.
                //Щоб він був доступний глобально
                //перетворюємо файл у байти
                var bytes = File.ReadAllBytes(dlg.FileName);
                var base64 = Convert.ToBase64String(bytes); //для передачі на сервер
                string json = JsonConvert.SerializeObject(new
                {
                    photo = base64,
                });
                bytes = Encoding.UTF8.GetBytes(json);
                string server = "https://cheburek.itstep.click";
                //За допомогою WebRequest - передама фото на сервер для зберігання
                WebRequest request = 
                    WebRequest.Create($"{server}/api/galleries/upload");
                request.Method = "POST";
                request.ContentType = "application/json";
                //Запишемо байти у потік
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                try
                {
                    var resp = request.GetResponse(); //відпраляємо фото на севрвер
                    using (var stream = new StreamReader(resp.GetResponseStream()))
                    {
                        string data = stream.ReadToEnd();
                        var objectServer = JsonConvert.DeserializeObject<UploadImage>(data);
                        //MessageBox.Show($"{server}{objectServer.Image}");
                        _imageUser = $"{server}{objectServer.Image}";
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}