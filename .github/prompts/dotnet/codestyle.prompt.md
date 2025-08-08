# Coding Style Guide
This guide provides a set of best practices and coding standards for writing C# code with GitHub Copilot. It covers various aspects of C# programming, including naming conventions, code structure, control flow, nullability, safe operations, asynchronous programming, and symbol references.

## XML document Comments:
Add missing XML Doc Comments and mention the purpose, intent, and 'the why' of the code, so developers unfamiliar with the project can better understand it. If comments already exist, update them to meet the before mentioned criteria if needed. Use the full syntax of XML Doc Comments to make them as awesome as possible including references to types. Don't add any documentation that is obvious for even novice developers by reading the code.

- **Good Practice**: Provide XML documentation for public members.
- **Bad Practice**: Not providing documentation or leaving it empty.

```csharp
// Good Practice
/// <summary>
/// Displays an alert with the specified message.
/// </summary>
/// <param name="message">The message to display.</param>
/// <param name="token">The cancellation token.</param>
public async Task ShowAlertAsync(string message, CancellationToken token = default)
{
	try
	{
		token.ThrowIfCancellationRequested();
		await _alertService.ShowAlertAsync(message, token);
	}
	catch (Exception ex)
	{
		Trace.WriteLine($"Error displaying alert: {ex.Message}");
	}
}

// Bad Practice
public async Task ShowAlertAsync(string message, CancellationToken token = default)
{
	try
	{
		token.ThrowIfCancellationRequested();
		await _alertService.ShowAlertAsync(message, token);
	}
	catch (Exception ex)
	{
		Trace.WriteLine($"Error displaying alert: {ex.Message}");
	}
}
```

## Type Definitions:
- Prefer records for data types:
	```csharp
	// Good: Immutable data type with value semantics
	public sealed record CustomerDto(string Name, Email Email);
	
	// Avoid: Class with mutable properties
	public class Customer
	{
		public string Name { get; set; }
		public string Email { get; set; }
	}
	```
- Make classes sealed by default:
	```csharp
	// Good: Sealed by default
	public sealed class OrderProcessor
	{
		// Implementation
	}
	
	// Only unsealed when inheritance is specifically designed for
	public abstract class Repository<T>
	{
		// Base implementation
	}
	```

## Control Flow:
- Prefer range indexers over LINQ:
	```csharp
	// Good: Using range indexers with clear comments
	var lastItem = items[^1];  // ^1 means "1 from the end"
	var firstThree = items[..3];  // ..3 means "take first 3 items"
	var slice = items[2..5];  // take items from index 2 to 4 (5 exclusive)
	
	// Avoid: Using LINQ when range indexers are clearer
	var lastItem = items.LastOrDefault();
	var firstThree = items.Take(3).ToList();
	var slice = items.Skip(2).Take(3).ToList();
	```
- Prefer collection initializers:
	```csharp
	// Good: Using collection initializers
	string[] fruits = ["Maui", "Community", "Toolkit"];
	
	// Avoid: Using explicit initialization when type is clear
	var fruits = new List<int>() {
		"Maui",
		"Community",
		"Toolkit"
	};
	```
- Use pattern matching effectively:
	```csharp
	// Good: Clear pattern matching
	static bool TryGetModalPageButton(Microsoft.Maui.ILayout layout, [NotNullWhen(true)] out Button? button)
	{
		button = null;

		foreach (var view in layout)
		{
			switch (view)
			{
				case Button { Text: tryStatusBarOnModalPageButtonText } modalPageButton:
					button = modalPageButton;
					return true;

				case Microsoft.Maui.ILayout nestedLayout:
					if (TryGetModalPageButton(nestedLayout, out var modalPageButtonInLayout))
					{
						button = modalPageButtonInLayout;
						return true;
					}
					break;
			}
		}

		return false;
	}
	
	// Avoid: Nested if statements
	static bool TryGetModalPageButton(Microsoft.Maui.ILayout layout, [NotNullWhen(true)] out Button? button)
	{
		button = null;

		foreach (var view in layout)
		{
			if (view is Button { Text: tryStatusBarOnModalPageButtonText } modalPageButton)
			{
				button = modalPageButton;
				return true;
			}
			else if (view is Microsoft.Maui.ILayout nestedLayout)
			{
				if (TryGetModalPageButton(nestedLayout, out var modalPageButtonInLayout))
				{
					button = modalPageButtonInLayout;
					return true;
				}
			}
		}

		return false;
	}
	```

## Nullability:
- Mark nullable fields explicitly:
	```csharp
	// Good: Explicit nullability
	public partial class MaskedBehavior : BaseBehavior<InputView>, IDisposable
	{
		IReadOnlyDictionary<int, char>? maskPositions;
 
		void SetMaskPositions(in string? mask)
		{
			if (string.IsNullOrEmpty(mask))
			{
				maskPositions = null;
				return;
			}

			var list = new Dictionary<int, char>();

			for (var i = 0; i < mask.Length; i++)
			{
				if (mask[i] != UnmaskedCharacter)
				{
					list.Add(i, mask[i]);
				}
			}

			maskPositions = list;
		}
	}

	// Avoid: Implicit nullability
	public partial class MaskedBehavior : BaseBehavior<InputView>, IDisposable
	{
		IReadOnlyDictionary<int, char> maskPositions; // Warning: Could be null
		private string lastPosition; // Warning: Could be null
	}
	```
- Use null checks only when necessary for reference types and public methods:
	```csharp
	// Good: Proper null checking
	void HandleButtonClicked(object? sender, EventArgs e)
	{
		ArgumentNullException.ThrowIfNull(sender);
		var button = (Button)sender;
		button.Behaviors.Add(new IconTintColorBehavior
		{
			TintColor = Colors.Green
		});
	}
	
	// Good: Using pattern matching for null checks
	void OnRegexPropertyChanged(string? regexPattern, RegexOptions regexOptions) => regex = regexPattern switch
	{
		null => null,
		_ => new Regex(regexPattern, regexOptions)
	};

	// Avoid: null checks for value types
	public void ProcessOrder(int orderId)
	{
		ArgumentNullException.ThrowIfNull(orderId);
	}

	// Avoid: null checks for non-public methods
	private void ProcessOrder(Order order)
	{
		ArgumentNullException.ThrowIfNull(order);
	}
	```
- Use null-forgiving operator only when appropriate:
	```csharp
	public class OrderValidator
	{
		private readonly IValidator<Order> _validator;
		
		public OrderValidator(IValidator<Order> validator)
		{
			_validator = validator ?? throw new ArgumentNullException(nameof(validator));
		}
		
		public ValidationResult Validate(Order order)
		{
			// We know _validator can't be null due to constructor check
			return _validator!.Validate(order);
		}
	}
	```
- Use nullability attributes:
	```csharp
	[AcceptEmptyServiceProvider]
	public partial class ImageResourceConverter : BaseConverterOneWay<string?, ImageSource?>
	{
		[return: NotNullIfNotNull(nameof(value))]
		public override ImageSource? ConvertFrom(string? value, CultureInfo? culture = null) => value switch
		{
			null => null,
			_ => ImageSource.FromResource(value, Application.Current?.GetType().Assembly)
		};
		
		// Method never returns null
		[return: NotNull]
		public static string EnsureNotNull(string? input) =>
			input ?? string.Empty;
		
		// Parameter must not be null when method returns true
		public static bool TryParse(string? input, [NotNullWhen(true)] out string? result)
		{
			result = null;
			if (string.IsNullOrEmpty(input))
				return false;
				
			result = input;
			return true;
		}
	}
	```
- Use init-only properties with non-null validation:
	```csharp
	// Good: Non-null validation in constructor
	public class AvatarModel
	{
		public Color BackgroundColor { get; init; } = Colors.Black;
		public Color BorderColor { get; init; } = Colors.White;
		
		public AvatarModel()
		{
			BackgroundColor = null!; // Will be set by required property
			BorderColor = null!; // Will be set by required property
		}
		
		private AvatarModel(Color backgroundColor, Color borderColor)
		{
			BackgroundColor = backgroundColor;
			BorderColor = borderColor;
		}
		
		public static AvatarModel Create(Color backgroundColor, Color borderColor) =>
			new(backgroundColor, borderColor);
	}
	```
- Document nullability in interfaces:
	```csharp
	public interface IOrderRepository
	{
		// Explicitly shows that null is a valid return value
		Task<Order?> FindByIdAsync(OrderId id, CancellationToken ct = default);
		
		// Method will never return null
		[return: NotNull]
		Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
		
		// Parameter cannot be null
		Task SaveAsync([NotNull] Order order, CancellationToken ct = default);
	}
	```

## Safe Operations:
- Use Try methods for safer operations:
	```csharp
	// Good: Using TryGetValue for dictionary access
	if (dictionary.TryGetValue(key, out var value))
	{
		// Use value safely here
	}
	else
	{
		// Handle missing key case
	}
	```
	```csharp
	// Avoid: Direct indexing which can throw
	var value = dictionary[key];  // Throws if key doesn't exist

	// Good: Using Uri.TryCreate for URL parsing
	if (Uri.TryCreate(urlString, UriKind.Absolute, out var uri))
	{
		// Use uri safely here
	}
	else
	{
		// Handle invalid URL case
	}
	```
	```csharp
	// Avoid: Direct Uri creation which can throw
	var uri = new Uri(urlString);  // Throws on invalid URL

	// Good: Using int.TryParse for number parsing
	if (int.TryParse(input, out var number))
	{
		// Use number safely here
	}
	else
	{
		// Handle invalid number case
	}
	```
	```csharp
	// Good: Combining Try methods with null coalescing
	var value = dictionary.TryGetValue(key, out var result)
		? result
		: defaultValue;

	// Good: Using Try methods in LINQ with pattern matching
	var validNumbers = strings
		.Select(s => (Success: int.TryParse(s, out var num), Value: num))
		.Where(x => x.Success)
		.Select(x => x.Value);
	```

- Prefer Try methods over exception handling:
	```csharp
	// Good: Using Try method
	if (decimal.TryParse(priceString, out var price))
	{
		// Process price
	}

	// Avoid: Exception handling for expected cases
	try
	{
		var price = decimal.Parse(priceString);
		// Process price
	}
	catch (FormatException)
	{
		// Handle invalid format
	}
	```

## Asynchronous Programming:
- Use Task.FromResult for pre-computed values:
	```csharp
	// Good: Return pre-computed value
	public Task<int> GetDefaultQuantityAsync() =>
		Task.FromResult(1);
	
	// Better: Use ValueTask for zero allocations
	public ValueTask<int> GetDefaultQuantityAsync() =>
		new ValueTask<int>(1);
	
	// Avoid: Unnecessary thread pool usage
	public Task<int> GetDefaultQuantityAsync() =>
		Task.Run(() => 1);
	```
- Always flow CancellationToken:
	```csharp
	// Good: Propagate cancellation
	public async Task<Order> ProcessOrderAsync(
		OrderRequest request,
		CancellationToken cancellationToken)
	{
		var order = await _repository.GetAsync(
			request.OrderId, 
			cancellationToken);
			
		await _processor.ProcessAsync(
			order, 
			cancellationToken);
			
		return order;
	}
	```
- Prefer await:
	```csharp
	// Good: Using await
	public async Task<Order> ProcessOrderAsync(OrderId id)
	{
		var order = await _repository.GetAsync(id);
		await _validator.ValidateAsync(order);
		return order;
	}
	```
- Never use Task.Result or Task.Wait:
	```csharp
	// Good: Async all the way
	public async Task<Order> GetOrderAsync(OrderId id)
	{
		return await _repository.GetAsync(id);
	}
	
	// Avoid: Blocking on async code
	public Order GetOrder(OrderId id)
	{
		return _repository.GetAsync(id).Result; // Can deadlock
	}
	```
- Use TaskCompletionSource correctly:
	```csharp
	// Good: Using RunContinuationsAsynchronously
	private readonly TaskCompletionSource<Order> _tcs = 
		new(TaskCreationOptions.RunContinuationsAsynchronously);
	
	// Avoid: Default TaskCompletionSource can cause deadlocks
	private readonly TaskCompletionSource<Order> _tcs = new();
	```
- Always dispose CancellationTokenSources:
	```csharp
	// Good: Proper disposal of CancellationTokenSource
	public async Task<Order> GetOrderWithTimeout(OrderId id)
	{
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
		return await _repository.GetAsync(id, cts.Token);
	}
	```
- Prefer async/await over direct Task return:
	```csharp
	// Good: Using async/await
	public async Task<Order> ProcessOrderAsync(OrderRequest request)
	{
		await _validator.ValidateAsync(request);
		var order = await _factory.CreateAsync(request);
		return order;
	}
	
	// Avoid: Manual task composition
	public Task<Order> ProcessOrderAsync(OrderRequest request)
	{
		return _validator.ValidateAsync(request)
			.ContinueWith(t => _factory.CreateAsync(request))
			.Unwrap();
	}
	```

## Symbol References:
- Always use nameof operator:
	```csharp
	// Good: Using nameof in attributes
	public class OrderProcessor
	{
		[Required(ErrorMessage = "The {0} field is required")]
		[Display(Name = nameof(OrderId))]
		public string OrderId { get; init; }
		
		[MemberNotNull(nameof(_repository))]
		private void InitializeRepository()
		{
			_repository = new OrderRepository();
		}
		
		[NotifyPropertyChangedFor(nameof(FullName))]
		public string FirstName
		{
			get => _firstName;
			set => SetProperty(ref _firstName, value);
		}
	}
	```
- Use nameof with exceptions:
	```csharp
	public class OrderService
	{
		public async Task<Order> GetOrderAsync(OrderId id, CancellationToken ct)
		{
			var order = await _repository.FindAsync(id, ct);
			
			if (order is null)
				throw new OrderNotFoundException(
					$"Order with {nameof(id)} '{id}' not found");
					
			if (!order.Lines.Any())
				throw new InvalidOperationException(
					$"{nameof(order.Lines)} cannot be empty");
					
			return order;
		}
		
		public void ValidateOrder(Order order)
		{
			if (order.Lines.Count == 0)
				throw new ArgumentException(
					"Order must have at least one line",
					nameof(order));
		}
	}
	```
- Use nameof in logging:
	```csharp
	public class OrderProcessor
	{
		private readonly ILogger<OrderProcessor> _logger;
		
		public async Task ProcessAsync(Order order)
		{
			_logger.LogInformation(
				"Starting {Method} for order {OrderId}",
				nameof(ProcessAsync),
				order.Id);
				
			try
			{
				await ProcessInternalAsync(order);
			}
			catch (Exception ex)
			{
				_logger.LogError(
					ex,
					"Error in {Method} for {Property} {Value}",
					nameof(ProcessAsync),
					nameof(order.Id),
					order.Id);
				throw;
			}
		}
	}
	```

### Usings and Namespaces:
- Use implicit usings:
	```csharp
	// Good: Implicit
	namespace MyNamespace
	{
		public class MyClass
		{
			// Implementation
		}
	}
	// Avoid:
	using System; // DON'T USE
	using System.Collections.Generic; // DON'T USE
	using System.IO; // DON'T USE
	using System.Linq; // DON'T USE
	using System.Net.Http; // DON'T USE
	using System.Threading; // DON'T USE
	using System.Threading.Tasks;// DON'T USE
	using System.Net.Http.Json; // DON'T USE
	using Microsoft.AspNetCore.Builder; // DON'T USE
	using Microsoft.AspNetCore.Hosting; // DON'T USE
	using Microsoft.AspNetCore.Http; // DON'T USE
	using Microsoft.AspNetCore.Routing; // DON'T USE
	using Microsoft.Extensions.Configuration; // DON'T USE
	using Microsoft.Extensions.DependencyInjection; // DON'T USE
	using Microsoft.Extensions.Hosting; // DON'T USE
	using Microsoft.Extensions.Logging; // DON'T USE
	using Good: Explicit usings; // DON'T USE
	
	namespace MyNamespace
	{
		public class MyClass
		{
			// Implementation
		}
	}
	```
- Use file-scoped namespaces:
	```csharp
	// Good: File-scoped namespace
	namespace MyNamespace;
	
	public class MyClass
	{
		// Implementation
	}
	
	// Avoid: Block-scoped namespace
	namespace MyNamespace
	{
		public class MyClass
		{
			// Implementation
		}
	}
	```

### Methods Returning Task and ValueTask
- **Good Practice**: Include a `CancellationToken` as a parameter for methods returning `Task` or `ValueTask`.
- **Bad Practice**: Not including a `CancellationToken`.

```csharp
// Good Practice
public async Task LoadDataAsync(CancellationToken token = default)
{
	token.ThrowIfCancellationRequested();
	// Method implementation
}

// Bad Practice
public async Task LoadDataAsync()
{
	// Method implementation
}
```

### Pattern Matching
- **Good Practice**: Use `is` for null checking and type checking.
- **Bad Practice**: Using `==` for null checking or casting for type checking.

```csharp
// Good Practice
if (something is null)
{
	// Handle null
}

if (something is Bucket bucket)
{
	bucket.Empty();
}

// Bad Practice
if (something == null)
{
	// Handle null
}

var bucket = something as Bucket;
if (bucket != null)
{
	bucket.Empty();
}
```