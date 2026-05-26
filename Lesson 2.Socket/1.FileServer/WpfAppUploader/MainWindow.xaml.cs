using System.Net.Http;
using System.Security.Policy;
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

namespace WpfAppUploader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Буде клієнт глобальний
        HttpClient client;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task connectBT_Click(object sender, RoutedEventArgs e)
        {
            //Потрібно написати перевірку чи доступна адреса, яка вказана
            //у полі portTB - 
            //Основна задача перевірити чи можна завнатажувати фото на дану адресу.
            //MessageBox.Show();
            //Це буде наші клієнт
            try
            {
                client = new HttpClient()
                {
                    BaseAddress = new Uri(portTB.Text)
                };
                //відправляю пустий запит для перевірки чи доступно
                var resp = await client.PostAsync("api/galleries/upload", null);
                if(resp.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Зєднання усішно!");
                }
                else
                {
                    throw new Exception("Сервер не доступний");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Щось пішло не так");
            }
        }

        private void connectBT_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new HttpClient()
                {
                    BaseAddress = new Uri(portTB.Text)
                };
                //відправляю пустий запит для перевірки чи доступно
                var resp = client.PostAsync("api/galleries/upload", null).Result;
                if (resp.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Зєднання усішно!");
                }
                else
                {
                    throw new Exception("Сервер не доступний");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Щось пішло не так");
            }
        }
    }
}