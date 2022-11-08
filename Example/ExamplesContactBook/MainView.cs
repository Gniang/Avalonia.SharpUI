using Avalonia.Controls;
using static Avalonia.SharpUI.ObservableState;
using Avalonia.SharpUI;
using Avalonia.Layout;
using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Animation;
using System.Diagnostics.CodeAnalysis;

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
                new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch
                }
                .DockTop()
                .SetBind(TextBox.TextProperty, filter, converter:ValueConverterSimple.OneWay(
                    (_)=> $"filterValues:{filter.Value}")
                )
                ,
            }
        );
    }


}

internal class ContactDetailsView : UserControl
{

    public ContactDetailsView(ObservableState<Contact?> contact)
    {
        var isEditing = UseState(false);

        this.Content = new DockPanel()
        {
            LastChildFill = true,
            ClipToBounds = true,
        }
        .DockTop()
        .Children(new Control[]
        {
            new TextBlock()
                .DockTop()
                .SetBind(TextBlock.TextProperty, contact)
                ,
            new TransitioningContentControl()
            {
                PageTransition = new CrossFade(TimeSpan.FromMilliseconds(300)),
            }
            .SetBind(ContentProperty, contact, converter: ValueConverterSimple.OneWay(
                x =>
                {
                    return contact.Value switch{
                        Contact ct when isEditing.Value => new ContactDetailsEditView(contact, isEditing),
                        Contact ct  => new ContactDetailsReadOnlyView(contact, isEditing),
                        _ => new TextBlock()
                                {
                                    Text = "-",
                                }
                                .DockTop(),
                    };
                })
            )
            ,
        });
    }
}

record EditContactResult(Contact? Contact)
{
    [MemberNotNullWhen(false, nameof(Contact))]
    public bool IsCancel => Contact == null;

    public static EditContactResult Update(Contact contact) => new(contact);
    public static EditContactResult Cancel => new(null as Contact);
}

internal class ContactDetailsEditView : UserControl
{

    public ContactDetailsEditView(ObservableState<Contact> contact, ObservableState<bool> isEditing)
    {
        void finishEdited(EditContactResult result)
        {
            if (!result.IsCancel)
            {
                contact.Value = result.Contact;
            }
            isEditing.Value = false;
        };
        this.Content = new StackPanel()
        {
            Margin = new Thickness(10),
            Width = 500,
            Spacing = 10,
        }
        .DockTop()
        .Children(new Control[]
        {
            new ContactEditor(contact),

            new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 5
            }
            .Children(new Control[]
            {
                new Button()
                {
                    Content = "Save",
                }
                .DockTop()
                .OnClick((s,e) =>
                    finishEdited(EditContactResult.Update(contact.Value)))
                ,
                new Button()
                {
                    Content = "Cancel",
                }
                .DockTop()
                .OnClick((s,e) =>
                    finishEdited(EditContactResult.Cancel))
            })
        });
    }
}

internal class ContactEditor : UserControl
{
    public ContactEditor(ObservableState<Contact> contact)
    {
        this.Content = new StackPanel()
        {
            Orientation = Orientation.Vertical,
            Spacing = 5,
        }
        .Children(new Control[]
        {
            new TextBox()
            {
                Watermark = "Full Name",
                UseFloatingWatermark = true,
                Text = contact.Value.FullName,
            }
            .DockTop()
            .SetSubscribe(TextBox.TextProperty,
                text => contact.Value = contact.Value with { FullName = text ?? "" }
            ),

            new TextBox()
            {
                Watermark = "Mail",
                UseFloatingWatermark = true,
                Text = contact.Value.Mail,
            }
            .DockTop()
            .SetSubscribe(TextBox.TextProperty,
                text => contact.Value = contact.Value with { Mail = text ?? "" }
            ),

            new TextBox()
            {
                Watermark = "Phone",
                UseFloatingWatermark = true,
                Text = contact.Value.Mail,
            }
            .DockTop()
            .SetSubscribe(TextBox.TextProperty,
                text => contact.Value = contact.Value with { Phone = text ?? "" }
            ),

        });
    }
}

internal class ContactDetailsReadOnlyView
{
    public ContactDetailsReadOnlyView(ObservableState<Contact> contact, ObservableState<bool> isEditing)
    {
    }
}

internal class ContactListView : UserControl
{
    public ContactListView(object value, ObservableState<Guid?> selectedId, ObservableState<string?> filter)
    {
        var filterDeferred = UseDeferred(filter, 1_000);

        this.Content = new DockPanel()
        {
            Width = 300,
            LastChildFill = true,
        }
        .Children(new Control[]
        {
            new TextBox()
            {
                Watermark = "search...",
                Text = filter.Value ?? "",
            }
            .SetSubscribe(TextBox.TextProperty, text => {
                if(text != filter.Value){
                    if (string.IsNullOrEmpty(text))
                        filter.Value = null;
                    else
                        filter.Value = text;
                }
            }),

            new TextBox()
            {
                Watermark = "search deferred..",
                Text = filterDeferred.Value ?? "",
            }
            .SetSubscribe(TextBox.TextProperty, text => {
                if(text != filterDeferred.Value){
                    if (string.IsNullOrEmpty(text))
                        filterDeferred.Value = null;
                    else
                        filterDeferred.Value = text;
                }
            }),
        });
    }

}