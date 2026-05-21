// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.Json;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Вкажіть шлях до файлу фото:");
Console.Write("->_");
string filePath = Console.ReadLine();

Console.WriteLine($"Шлях: {filePath}");

//Зберігаємо байти файлу зображення
var imageBytes = File.ReadAllBytes(filePath);
//Перетворити байти у Base64
string base64 = Convert.ToBase64String(imageBytes);
//Console.WriteLine("BASE 64 = "+ base64);

//json - формат - Тобто ми можемо перетворити любий обєкт у json
// json - рядок тексту
var uploadData = new { Photo = base64 };
//перетворюємо у json
var json = JsonSerializer.Serialize(uploadData);

//Console.WriteLine("json = " + json);
try
{
    //Це спеціальний клієнт, який може відправити запит на сервер
    //по суті він змінить мені Postman
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("https://cheburek.itstep.click/");

    var content = new StringContent(json, Encoding.UTF8, 
        "application/json");
    //Відправляємо запит на сервер
    HttpResponseMessage response =
        await client.PostAsync("api/Galleries/upload", content);
    if(response.IsSuccessStatusCode) // код усіпшної операції
    {
        var resp = await response.Content.ReadAsStringAsync();
        var imgUrl = JsonDocument.Parse(resp).RootElement
            .GetProperty("image").GetString();
        
        Console.WriteLine($"https://cheburek.itstep.click{imgUrl}");
    }
    else
    {
        throw new Exception($"Помилка передачі файлу {response.StatusCode}");
    }

}
catch(Exception ex)
{
    Console.WriteLine("Щось пішло не так: " + ex.Message);
}


