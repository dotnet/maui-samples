#if IOS
using Foundation;
using ObjCRuntime;
using System.Diagnostics;
using System.Runtime.InteropServices;
#endif

namespace procrastinate.Services;

/// <summary>
/// On-device AI excuse generator using Apple's Foundation Models framework (iOS 26+)
/// Uses the FMWrapper native library to bridge Swift Foundation Models to C#
/// </summary>
public class OnDeviceAIExcuseGenerator : IExcuseGenerator
{
    public string Name => "On-Device AI (Apple)";
    
    public bool IsAvailable
    {
        get
        {
#if IOS
            if (!OperatingSystem.IsIOSVersionAtLeast(26))
                return false;
            
            try
            {
                return FMWrapperBridge.IsAvailable();
            }
            catch
            {
                return false;
            }
#else
            return false;
#endif
        }
    }

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
#if IOS
        var stopwatch = Stopwatch.StartNew();
        
        if (!OperatingSystem.IsIOSVersionAtLeast(26))
        {
            stopwatch.Stop();
            return new ExcuseResult("On-device AI requires iOS 26+ with Apple Intelligence enabled.", Name, stopwatch.Elapsed);
        }

        try
        {
            if (!FMWrapperBridge.IsAvailable())
            {
                stopwatch.Stop();
                return new ExcuseResult(FMWrapperBridge.GetUnavailabilityReason(), Name, stopwatch.Elapsed);
            }

            var languageName = language switch
            {
                "fr" => "French",
                "es" => "Spanish",
                "pt" => "Portuguese",
                "nl" => "Dutch",
                "cs" => "Czech",
                _ => "English"
            };

            var prompt = $"Write a single short humorous fictional excuse that starts with 'I' - something silly and absurd a person might say when they're late or can't do something. Make it sound like a real excuse someone would say out loud. Write it in {languageName}. Just the excuse itself, nothing else.";
            
            var instructions = "You are writing funny excuses in first person. Start naturally like 'I can't because...' or 'I would but...'. Keep it to one or two sentences. Be creative and absurd but make it sound like something a person would actually say.";

            var result = await FMWrapperBridge.GenerateTextAsync(prompt, instructions);
            stopwatch.Stop();
            
            return new ExcuseResult(
                result ?? "The on-device AI is also procrastinating...", 
                Name, 
                stopwatch.Elapsed,
                Model: "Apple Intelligence"
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ExcuseResult($"On-device AI error: {ex.Message}", Name, stopwatch.Elapsed);
        }
#else
        await Task.CompletedTask;
        return new ExcuseResult("On-device AI is only available on iOS 26+ with Apple Intelligence.", Name, TimeSpan.Zero);
#endif
    }
}

#if IOS
/// <summary>
/// Bridge to the native FMWrapper Swift library
/// </summary>
internal static class FMWrapperBridge
{
    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_getClass")]
    private static extern IntPtr objc_getClass(string name);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "sel_registerName")]
    private static extern IntPtr sel_registerName(string name);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern bool objc_msgSend_bool(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern void objc_msgSend_3ptr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern void objc_msgSend_2ptr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

    [DllImport("/usr/lib/libdl.dylib")]
    private static extern IntPtr dlopen(string path, int mode);

    private const int RTLD_NOW = 2;

    private static IntPtr? _wrapperClass;
    private static IntPtr? _wrapperInstance;
    private static bool _frameworkLoaded;

    private static void EnsureFrameworkLoaded()
    {
        if (_frameworkLoaded) return;
        
        // Load the FMWrapper framework - it's embedded in the app bundle
        var bundlePath = Foundation.NSBundle.MainBundle.BundlePath;
        var frameworkPath = System.IO.Path.Combine(bundlePath, "Frameworks", "FMWrapper.framework", "FMWrapper");
        
        var handle = dlopen(frameworkPath, RTLD_NOW);
        _frameworkLoaded = handle != IntPtr.Zero;
    }

    private static IntPtr GetWrapperClass()
    {
        if (_wrapperClass.HasValue) return _wrapperClass.Value;
        
        EnsureFrameworkLoaded();
        _wrapperClass = objc_getClass("FMWrapper");
        return _wrapperClass.Value;
    }

    private static IntPtr GetWrapperInstance()
    {
        if (_wrapperInstance.HasValue && _wrapperInstance.Value != IntPtr.Zero)
            return _wrapperInstance.Value;

        var cls = GetWrapperClass();
        if (cls == IntPtr.Zero) return IntPtr.Zero;

        var alloc = objc_msgSend(cls, sel_registerName("alloc"));
        if (alloc == IntPtr.Zero) return IntPtr.Zero;

        _wrapperInstance = objc_msgSend(alloc, sel_registerName("init"));
        return _wrapperInstance.Value;
    }

    public static bool IsAvailable()
    {
        try
        {
            var cls = GetWrapperClass();
            if (cls == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("FMWrapper: Class not found");
                return false;
            }

            var result = objc_msgSend_bool(cls, sel_registerName("isAvailable"));
            System.Diagnostics.Debug.WriteLine($"FMWrapper: isAvailable returned {result}");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FMWrapper: IsAvailable exception: {ex.Message}");
            return false;
        }
    }

    public static string GetUnavailabilityReason()
    {
        try
        {
            var cls = GetWrapperClass();
            if (cls == IntPtr.Zero)
                return "FMWrapper framework not loaded - class not found";

            var resultHandle = objc_msgSend(cls, sel_registerName("getUnavailabilityReason"));
            var reason = NSString.FromHandle(resultHandle) ?? "Unknown reason";
            System.Diagnostics.Debug.WriteLine($"FMWrapper: Unavailability reason: {reason}");
            return reason;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public static Task<string?> GenerateTextAsync(string prompt, string instructions)
    {
        return Task.Run(async () =>
        {
            try
            {
                var cls = GetWrapperClass();
                if (cls == IntPtr.Zero)
                {
                    return "FMWrapper not initialized";
                }

                using var promptNS = new NSString(prompt);
                using var instructionsNS = new NSString(instructions);

                // Start generation
                objc_msgSend_2ptr(cls, sel_registerName("startGenerationWithPrompt:instructions:"), 
                    promptNS.Handle, instructionsNS.Handle);

                // Poll for completion (max 60 seconds)
                for (int i = 0; i < 600; i++)
                {
                    await Task.Delay(100);

                    // Check if still generating
                    var isGenerating = objc_msgSend_bool(cls, sel_registerName("isGenerating"));
                    if (!isGenerating)
                    {
                        // Get result or error
                        var errorHandle = objc_msgSend(cls, sel_registerName("getLastError"));
                        if (errorHandle != IntPtr.Zero)
                        {
                            var error = NSString.FromHandle(errorHandle);
                            if (!string.IsNullOrEmpty(error))
                            {
                                return $"AI Error: {error}";
                            }
                        }

                        var resultHandle = objc_msgSend(cls, sel_registerName("getLastResult"));
                        if (resultHandle != IntPtr.Zero)
                        {
                            var result = NSString.FromHandle(resultHandle);
                            if (!string.IsNullOrEmpty(result))
                            {
                                return result;
                            }
                        }

                        return "No response from AI";
                    }
                }

                return "Request timed out - AI took too long to respond";
            }
            catch (Exception ex)
            {
                return $"Bridge error: {ex.Message}";
            }
        });
    }
}
#endif
