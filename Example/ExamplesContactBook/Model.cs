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