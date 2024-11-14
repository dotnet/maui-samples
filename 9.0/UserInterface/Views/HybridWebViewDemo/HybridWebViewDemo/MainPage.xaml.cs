using System.Diagnostics;
using System.Text.Json.Serialization;

namespace HybridWebViewDemo;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        hybridWebView.SetInvokeJavaScriptTarget(this);
    }

    private void OnSendMessageButtonClicked(object sender, EventArgs e)
    {
        hybridWebView.SendRawMessage($"Hello from C#! #{count++}");
    }

    private async void OnInvokeJSMethodButtonClicked(object sender, EventArgs e)
    {
        string statusResult = string.Empty;
        double x = 123d;
        double y = 321d;

        ComputationResult? result = await hybridWebView.InvokeJavaScriptAsync<ComputationResult>(
            "AddNumbers", // JavaScript method name
            HybridSampleJSContext.Default.ComputationResult, // JSON serialization info for return type
            [x, y], // Parameter values
            [HybridSampleJSContext.Default.Double, HybridSampleJSContext.Default.Double]); // JSON serialization info for each parameter

        if (result is null)
            statusResult += Environment.NewLine + $"Received no result for operation with {x} and {y}.";
        else
            statusResult += Environment.NewLine + $"Called operation {result.operationName} with {x} and {y} to receive {result.result}.";

        Dispatcher.Dispatch(() => editor.Text += statusResult);
    }

    private async void OnInvokeAsyncJSMethodButtonClicked(object sender, EventArgs e)
    {
        string statusResult = string.Empty;

        Dictionary<string,string>? asyncResult = await hybridWebView.InvokeJavaScriptAsync<Dictionary<string, string>>(
            "EvaluateMeWithParamsAndAsyncReturn", // JavaScript method name
            HybridSampleJSContext.Default.DictionaryStringString, // JSON serialization info for return type
            ["new_key", "new_value"], // Parameter values
            [HybridSampleJSContext.Default.String, HybridSampleJSContext.Default.String]); // JSON serialization info for each parameter

        if (asyncResult is null)
            statusResult += Environment.NewLine + $"Received no result for operation.";
        else
            statusResult += Environment.NewLine + $"Received result from JavaScript method: {string.Join(",", asyncResult)}";

        Dispatcher.Dispatch(() => editor.Text += statusResult);
    }

    private void hybridWebView_RawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        Dispatcher.Dispatch(() => editor.Text += Environment.NewLine + e.Message);
    }

    public void DoSyncWork()
    {
        Debug.WriteLine("DoSyncWork");
    }

    public void DoSyncWorkParams(int i, string s)
    {
        Debug.WriteLine($"DoSyncWorkParams: {i}, {s}");
    }

    public string DoSyncWorkReturn()
    {
        Debug.WriteLine("DoSyncWorkReturn");
        return "Hello from C#!";
    }

    public SyncReturn DoSyncWorkParamsReturn(int i, string s)
    {
        Debug.WriteLine($"DoSyncWorkParamReturn: {i}, {s}");
        return new SyncReturn
        {
            Message = "Hello from C#!" + s,
            Value = i
        };
    }

    public class ComputationResult
    {
        public double result { get; set; }
        public string? operationName { get; set; }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ComputationResult))]
    [JsonSerializable(typeof(double))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    internal partial class HybridSampleJSContext : JsonSerializerContext
    {
        // This type's attributes specify JSON serialization info to preserve type structure
        // for trimmed builds.    
    }

    public class SyncReturn
    {
        public string? Message { get; set; }
        public int Value { get; set; }
    }
}
