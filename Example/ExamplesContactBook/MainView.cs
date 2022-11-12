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
using Avalonia.Controls.Documents;
using System.Collections.Generic;
using Avalonia.Controls.Templates;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System.Net.Http;
using System.IO;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;

namespace ExamplesContactBook;


internal class MainView : UserControl
{
    public MainView()
    {
        var contacts = ContactStore.shared.Contacts;
        var selectedId = UseState(null as Guid?);
        var filter = UseState(null as string);
        var selectedContact = UseState(null as Contact);
        UseEffect(() =>
        {
            if (Application.Current?.ApplicationLifetime is
                IClassicDesktopStyleApplicationLifetime lifetime &&
                lifetime.MainWindow is Window window)
            {
                if (selectedContact.Value is Contact c)
                {
                    window.Title = $"ContactBook - {c.FullName}";
                }
                else
                {
                    window.Title = "ContactBook";
                };
            }
        }
        ,
        selectedContact);

        UseEffect(() =>
        {
            selectedContact.Value = contacts.FirstOrDefault(x => x.Id == selectedId.Value);
        },
        selectedId);

        var filteredContacts = UseState(contacts.AsReadOnly());
        UseEffect(() =>
        {
            filteredContacts.Value = contacts
                .Where(x => filter.Value switch
                {
                    string f => x.FullName.Contains(f) ||
                                  x.Mail.Contains(f) ||
                                  x.Phone.Contains(f),
                    _ => true,
                })
                .ToList()
                .AsReadOnly();
        },
        filter);

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
                new ContactListView(filteredContacts, selectedId, filter),
                new ContactDetailsView(selectedContact),
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

        IBinding BindMulti(IEnumerable<IObservableState> states, BindingMode mode, IMultiValueConverter converter)
        {
            return new MultiBinding()
            {
                Bindings = states.Select(x => new Binding("Value") { Source = x }).ToArray(),
                Mode = mode,
                Converter = converter,
            };
        }

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
                [!ContentProperty] = BindMulti(new IObservableState [] {contact, isEditing } ,BindingMode.OneWay , converter: ValueConverterSimple.Multi(
                    x =>
                    {
                        return contact switch{
                            _ when contact.Value != null && isEditing.Value => new ContactDetailsEditView(contact, isEditing),
                            _ when contact.Value != null => new ContactDetailsReadOnlyView(contact, isEditing),
                            _ => new TextBlock()
                                    {
                                        Text = "-",
                                    }
                                    .DockTop(),
                        };
                    })
                 )
            },
            //.SetBind(ContentProperty, isEditing, converter: ValueConverterSimple.OneWay(
            //    x =>
            //    {
            //        return contact switch{
            //            _ when contact.Value != null && isEditing.Value => new ContactDetailsEditView(contact, isEditing),
            //            _ when contact.Value != null => new ContactDetailsReadOnlyView(contact, isEditing),
            //            _ => new TextBlock()
            //                    {
            //                        Text = "-",
            //                    }
            //                    .DockTop(),
            //        };
            //    })
            //)
            //,
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

        var editingContact = UseState(contact.Value);

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
            new ContactEditor(editingContact),

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
                    finishEdited(EditContactResult.Update(editingContact.Value)))
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
    public ContactEditor(ObservableState<Contact> editingContact)
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
                Text = editingContact.Value.FullName,
            }
            .DockTop()
            .On(nameof(TextBox.TextChanged), (object? s, TextChangedEventArgs e) =>
                editingContact.Value = editingContact.Value with { FullName = (s as TextBox)?.Text ?? "" }
            ),

            new TextBox()
            {
                Watermark = "Mail",
                UseFloatingWatermark = true,
                Text = editingContact.Value.Mail,
            }
            .DockTop()
            .SetSubscribe(TextBox.TextProperty,
                text => editingContact.Value = editingContact.Value with { Mail = text ?? "" }
            )
            ,
            new TextBox()
            {
                Watermark = "Phone",
                UseFloatingWatermark = true,
                Text = editingContact.Value.Mail,
            }
            .DockTop()
            .SetSubscribe(TextBox.TextProperty,
                text => editingContact.Value = editingContact.Value with { Phone = text ?? "" }
            )
            ,
        });
    }
}

internal class ContactDetailsReadOnlyView : UserControl
{
    public ContactDetailsReadOnlyView(ObservableState<Contact> contact, ObservableState<bool> isEditing)
    {
        this.Content = new StackPanel()
        {
            Margin = new Thickness(10),
            Width = 500,
            Spacing = 10,
        }
        .DockTop()
        .Children(new Control[]
        {
            new ContactView(contact),

            new StackPanel()
            {
                Spacing = 5,
                HorizontalAlignment = HorizontalAlignment.Right,
                Orientation = Orientation.Horizontal,
            }
            .Children(new Control[]
            {
                new Button()
                {
                    Content = "edit",
                }
                .DockBottom()
                .OnClick((s,e) => isEditing.Value = true)
                ,
                new Button()
                {
                    Content = "delete",
                    Background = Brush.Parse("#c0392b")
                }
                .DockBottom()
                .OnClick((s,e) => isEditing.Value = false)
            })
        }); ;
    }
}

internal class ContactView : UserControl
{
    public ContactView(ObservableState<Contact> contact)
    {
        //var image = UseState(null as Bitmap);

        //Api.RandomImage().ContinueWith(async (x) =>
        //{
        //    await Dispatcher.UIThread.InvokeAsync(async () => { image.Value = await x; });
        //});

        IObservableState<Deferred<T>> UseAsync<T>(Task<T> t)
        {
            var state = UseState(Deferred.NotStartedYet<T>());
            Action a = async () =>
            {
                state.Value = Deferred.Pending<T>();
                try
                {
                    state.Value = Deferred.Resolved(await t);
                }
                catch (Exception e)
                {
                    state.Value = Deferred.Failed<T>(e);
                }
            };
            a.Invoke();
            return state;
        };

        var image = UseAsync(Api.RandomImage());

        this.Content = new TemplatedControl()
        {
            [!TemplatedControl.TemplateProperty] = image.ToBinding(BindingMode.OneWay, ValueConverterSimple.OneWay((_) =>
            {
                return new FuncControlTemplate<TemplatedControl>((_, name) =>
                {
                    var img = image.Value;
                    return new StackPanel()
                    {
                        Orientation = Orientation.Vertical,
                        Spacing = 5,
                    }
                    .Children(new Control[]
                    {
                        img.State switch
                        {
                            DeferredState.NotStartedYet | DeferredState.Pending
                                => new ProgressBar()
                                {
                                    IsEnabled = true,
                                    IsIndeterminate = true,
                                },
                            DeferredState.Resolved
                                => new Image()
                                {
                                    Source = img.Value!,
                                    Height = 200,
                                },
                            DeferredState.Failed
                                => new TextBlock()
                                {
                                    Text = img.Exception!.ToString(),
                                },
                            _ => throw new NotImplementedException(),
                        },

                        new TextBlock()
                        {
                            Foreground = Brush.Parse("#3498db"),
                            Text = "Full Name",
                        }
                        .DockTop(),
                        new TextBlock()
                        {
                            Text = contact.Value.FullName,
                        }
                        .DockTop(),

                         new TextBlock()
                        {
                            Foreground = Brush.Parse("#3498db"),
                            Text = "Mail",
                        }
                        .DockTop(),
                        new TextBlock()
                        {
                            Text = contact.Value.Mail,
                        }
                        .DockTop(),

                        new TextBlock()
                        {
                            Foreground = Brush.Parse("#3498db"),
                            Text = "Phone",
                        }
                        .DockTop(),
                        new TextBlock()
                        {
                            Text = contact.Value.Phone,
                        }
                        .DockTop(),
                    });
                });
                //}),
                //Template = 
            })
           ),
        };
    }
}
public enum DeferredState
{
    NotStartedYet,
    Pending,
    Resolved,
    Failed
}
public record Deferred<T>
{
    public T? Value { get; init; }
    public DeferredState State { get; init; }
    public Exception? Exception { get; init; }
}
public class Deferred
{
    public static Deferred<T> NotStartedYet<T>() => new Deferred<T>() { State = DeferredState.NotStartedYet };
    public static Deferred<T> Pending<T>() => new Deferred<T>() { State = DeferredState.Pending };
    public static Deferred<T> Resolved<T>(T value) => new Deferred<T>() { State = DeferredState.Resolved, Value = value };
    public static Deferred<T> Failed<T>(Exception ex) => new Deferred<T>()
    {
        State = DeferredState.Failed,
        Exception = ex
    };
}

public class Api
{
    private static readonly string randomImageUri = "https://thispersondoesnotexist.com/image";
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task<Bitmap> RandomImage()
    {
        var bytes = await httpClient.GetByteArrayAsync(randomImageUri);
        using var s = new MemoryStream(bytes);
        return new Bitmap(s);
    }
}

internal class ContactListView : UserControl
{
    public ContactListView(IObservableState<ReadOnlyCollection<Contact>> filteredCntacts, ObservableState<Guid?> selectedId, ObservableState<string?> filter)
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
            .DockTop()
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
            .DockTop()
            .SetSubscribe(TextBox.TextProperty, text => {
                if(text != filterDeferred.Value){
                    if (string.IsNullOrEmpty(text))
                        filterDeferred.Value = null;
                    else
                        filterDeferred.Value = text;
                }
            }),

            new ListBox()
            {
                [!ListBox.ItemsProperty] = filteredCntacts.ToBinding(),
                [!ListBox.SelectedItemProperty] = selectedId.ToBinding(BindingMode.OneWay,
                    converter:ValueConverterSimple.OneWay(
                    _ => filteredCntacts.Value
                        .FirstOrDefault(x => x.Id == selectedId.Value)
                    )
                ),
                ItemTemplate = new FuncDataTemplate<Contact>((value, namescope) =>
                {
                    return new TextBlock(){Text = value.FullName};
                })
            }
            .DockTop()
            .SetSubscribe(ListBox.SelectedIndexProperty, idx =>
                {
                    selectedId.Value = filteredCntacts.Value
                        .ElementAtOrDefault(idx)?.Id;
                }
            )
        }); ;
    }

}
