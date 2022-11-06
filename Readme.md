
UI building from C# code.(WIP)

[counte app sample code](/Example/ExamplesCounterApp/)


* ready
  * controls add extension methods for decreative ui.  
* not ready (I don't even know if I need there.)
  * support [Binding to Commands](https://docs.avaloniaui.net/docs/data-binding/binding-to-commands)
  * support Elmish
  * support View Component

inspired by

* [fsprojects/Avalonia.FuncUI](https://github.com/fsprojects/Avalonia.FuncUI) 
* [AvaloniaUI/Avalonia.Markup.Declarative](https://github.com/AvaloniaUI/Avalonia.Markup.Declarative)

Reasons for doing something similar.

* FuncUI
  * so cool. this includes Elmish too.
  * however cannot support C#. [Avalonia MVU - a must, for C#](https://github.com/fsprojects/Avalonia.FuncUI/issues/171) 

* Markup.Declarative
  * friendly C#.
  * However, since the extended methods are created in the code generator, they must be defined separately in the custom control.
  * Adding `@` to the view model will bind it, but since it is processed by reference to the dotnet argument contents (string), I feel that unintended behavior may occur.
