using Avalonia.Controls;
using static Avalonia.SharpUI.ObservableState;
using Avalonia.SharpUI;
using Avalonia.Layout;
using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Bogus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using System.IO;
using System.Net.Http;
using Avalonia.Media.Imaging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExamplesContactBook;

record Contact(Guid Id, string FullName, string Mail, string Phone)
{
    public static Contact Create(string fullName, string mail, string phone)
        => new Contact(Guid.NewGuid(), fullName, mail, phone);
    public static Contact Init()
        => new Contact(Guid.Empty, "", "", "");
    public static Contact Random()
    {
        var faker = new Faker(locale: "de").Person;
        return new Contact(Guid.NewGuid(), faker.FullName, faker.Email, faker.Phone);
    }
};

internal class ContactStore
{
    public static readonly ContactStore shared = new ContactStore();

    private readonly List<Contact> value
            = Enumerable.Range(0, 100)
                    .Select(_ => Contact.Random())
                    .ToList();
    public List<Contact> Contacts => value;
}


record FaceUrlJson([property: JsonPropertyName("url")] string Url);

public class Api
{
    private static readonly string randomImageUri = "https://100k-faces.glitch.me";
    private static readonly string randomImagePath = "/random-image-url";
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task<Bitmap> RandomImage()
    {
        HttpRequestMessage request = new(HttpMethod.Get, randomImageUri + randomImagePath);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        var r = await httpClient.SendAsync(request);
        var js = await r.Content!.ReadAsStringAsync();
        var d = JsonSerializer.Deserialize<FaceUrlJson>(js);
        await Task.Delay(500);
        var imgBytes = await httpClient.GetByteArrayAsync(d!.Url);
        using var s = new MemoryStream(imgBytes);
        return new Bitmap(s);
    }
}
