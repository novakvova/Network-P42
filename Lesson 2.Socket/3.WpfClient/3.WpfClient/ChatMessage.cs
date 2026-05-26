using System.IO;

namespace _3.WpfClient;

//Повідомлення, якими будуть обмінюватися клієнти
public class ChatMessage
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    //Можна Serialize та Desserialize
    public byte[] Serialize()
    {
        using var m = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(m);
        writer.Write(UserId);
        writer.Write(Name);
        writer.Write(Text);
        return m.ToArray(); //повертаємо масив byte
    }
    public static ChatMessage Deserialize(byte[] data)
    {
        ChatMessage msg = new ChatMessage();
        using var ms = new MemoryStream(data);
        using BinaryReader reader = new (ms);
        msg.UserId = reader.ReadString();
        msg.Name = reader.ReadString();
        msg.Text = reader.ReadString();
        return msg;
    }

}
