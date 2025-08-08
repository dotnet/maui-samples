This cheat sheet should serve as a quick reminder of best practices when writing asynchronous code. Following these guidelines will help you avoid common pitfalls such as unobserved exceptions, deadlocks, and unexpected UI blocking.

---

### ✅ DOs

- **Always Await Your Tasks:**  
  Always use the `await` keyword on async operations to ensure exceptions are captured and to avoid blocking the calling thread.  

- **Use Async Task/Task<T> Methods:**  
  Prefer returning `Task` or `Task<T>` over `async void` so that exceptions can be observed, and your methods are easily composable and testable.  

- **Name Methods with the "Async" Suffix:**  
  Clearly differentiate asynchronous methods (e.g., `GetDataAsync()`) from synchronous ones.  

- **Pass Cancellation Tokens:**  
  Allow cancellation by accepting a `CancellationToken` in your async methods.  

- **Use ConfigureAwait(false) When Appropriate:**  
  In library code or server-side processes, use `ConfigureAwait(false)` to avoid unnecessary context switches and potential deadlocks.  

- **Keep Async Code “Async All the Way”:**  
  Propagate async all the way from the entry point (like event handlers or controller actions) rather than mixing sync and async code.  

- **Report Progress and Handle Exceptions Properly:**  
  Use tools like `IProgress<T>` to report progress and always catch exceptions at the appropriate level when awaiting tasks.  

---

### ❌ DON'Ts

- **Avoid async void Methods:**  
  Except for event handlers, never use `async void` because their exceptions are not observable and they’re difficult to test.  

- **Don’t Block on Async Code:**  
  Avoid using `.Wait()` or `.Result` as these can lead to deadlocks and wrap exceptions in `AggregateException`. If you must block, consider using `GetAwaiter().GetResult()`.  

- **Don’t Mix Blocking and Async Code:**  
  Blocking the calling thread in an otherwise async flow (e.g., by mixing synchronous calls with async ones) may cause deadlocks and performance issues.  

- **Avoid Wrapping Return Task in Try/Catch or Using Blocks:**  
  When a method returns a `Task`, wrapping it in a try/catch or using block may lead to unexpected behavior because the task completes outside those blocks.  

- **Don’t Overuse Fire-and-Forget Patterns:**  
  Unobserved tasks (fire-and-forget) can swallow exceptions and cause race conditions—if needed, use a “safe fire-and-forget” pattern with proper error handling.  
