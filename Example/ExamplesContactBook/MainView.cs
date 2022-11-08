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


internal class MainView : UserControl
{
    public MainView()
    {
        var selectedId = UseState(null as Guid?);
        var filter = UseState(null as string);
        Contact? selectedContactFunc() => ContactStore.shared.Contacts.FirstOrDefault(x => x.Id == selectedId.Value);
        UseEffect(() =>
        {
            if (Application.Current?.ApplicationLifetime is
                IClassicDesktopStyleApplicationLifetime lifetime)
            {
                Contact? selectedContact = selectedContactFunc();
                if (selectedContact is Contact c)
                {
                    lifetime.MainWindow.Title = $"ContactBook - {c.FullName}";
                }
                else
                {
                    lifetime.MainWindow.Title = "ContactBook";
                };
            }
        }
        ,
selectedId);


        Task.Run(async () =>
        {
            await Task.Delay(2000);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                selectedId.Value = ContactStore.shared.Contacts[5].Id;
            });
        });

        this.Content = new DockPanel()
            .DockTop()
            .Children(new Control[]
            {
                new ContactListView(null, selectedId, filter),
                new ContactDetailsView(null),
                new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch
                }
                .DockTop()
                .SetBind(TextBox.TextProperty, selectedId, converter:ValueConverterSimple.OneWay(
                    (_)=> $"{selectedId.Value} testmsg{null}")
                )
                ,
            }
        );
    }


}

internal class ContactDetailsView : UserControl
{
    private object value;

    public ContactDetailsView(object value)
    {
        this.value = value;
    }
}

internal class ContactListView : UserControl
{
    public ContactListView(object value, ObservableState<Guid?> selectedId, ObservableState<string?> filter)
    {
    }

}