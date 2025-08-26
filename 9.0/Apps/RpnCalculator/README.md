---
name: .NET MAUI - RPN Calculator
description: An RPN (Reverse Polish Notation) calculator allows numbers and operations to be entered without parentheses or an equal key. RPN (also called...
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: rpncalculator
---
# RPN Calculator

An RPN (Reverse Polish Notation) calculator allows numbers and operations to be entered without parentheses or an equal key. RPN (also called postfix notation) is described in the Wikipedia article [**Reverse Polish notation**](https://en.wikipedia.org/wiki/Reverse_Polish_notation). This sample demonstrates how to build an RPN calculator using .NET MAUI.

RPN is based on a stack. Numbers are pushed on the stack by pressing the ENTER key. Unary operations (such as **log** and **sin**) pop a number from the stack, apply the operation, and push the result back on the stack. Binary operations (such as **+** and **/**) pop two numbers from the stack, perform the operation, and push the result on the stack.

To perform the calculation

5 &#x00D7; (3 + 4) &#x2013; 2

press the following keys:

5 ENTER 3 ENTER 4 ENTER + &#x00D7; 2 ENTER &#x2013;

The + operation adds 3 and 4, the times operation multiplies 5 and that result, and the minus operation subtracts 2 from that result.

The layout of the keys appears twice in the **MainPage.xaml** file, separately for portrait mode and landscape mode. The **MainPage.xaml.cs** code-behind file switches between these two layouts based on the relative width and height of the page.

The calculator logic is encapsulated in the **RpnCalculatorViewModel.cs** file. The XAML file and the ViewModel are linked through XAML-based data bindings, which are described in detail in the series of articles on [**Data Binding**](https://docs.microsoft.com/dotnet/maui/fundamentals/data-binding/), and particularly the [**The Command Interface**](https://docs.microsoft.com/dotnet/maui/fundamentals/data-binding/commanding) article.

![RPN Calculator application screenshot](Screenshots/rpn-calculator-vertical.png "RPN Calculator application screenshot")
