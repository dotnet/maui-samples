# Unit Testing
The default unit testing framework for .NET projects is xUnit. This document provides guidelines for writing unit tests using xUnit.

Tests should be reliable, maintainable, provide meaningful coverage, and provide proper isolation and clear patterns for test organization and execution.

## Requirements
- Use xUnit as the testing framework
- Ensure test isolation
- Follow consistent patterns
- Maintain high code coverage

## Test Class Structure:

- Use ITestOutputHelper for logging:
	```csharp
	public class OnControlBindingContextChanged(ITestOutputHelper output)
	{
		
		[Fact]
		public async Task BindingContext_Changed()
		{
			output.WriteLine("Starting test with default binding context");
			// Test implementation
		}
	}
	```
- Use fixtures for shared state:
	```csharp
	public class DatabaseFixture : IAsyncLifetime
	{
		public DbConnection Connection { get; private set; }
		
		public async Task InitializeAsync()
		{
			Connection = new SqlConnection("connection-string");
			await Connection.OpenAsync();
		}
		
		public async Task DisposeAsync()
		{
			await Connection.DisposeAsync();
		}
	}
	
	public class OrderTests : IClassFixture<DatabaseFixture>
	{
		private readonly DatabaseFixture _fixture;
		private readonly ITestOutputHelper _output;
		
		public OrderTests(DatabaseFixture fixture, ITestOutputHelper output)
		{
			_fixture = fixture;
			_output = output;
		}
	}
	```

## Test Methods:

- Prefer Theory over multiple Facts:
	```csharp
	public class DiscountCalculatorTests
	{
		public static TheoryData<IComparable, IComparable, IComparable, object, object, object> TestData { get; } = new()
		{
			// System.Byte
			{
				Convert.ToByte('C'), Convert.ToByte('B'), Convert.ToByte('D'), TrueTestObject, FalseTestObject, TrueTestObject
			},
			{
				Convert.ToByte('B'), Convert.ToByte('B'), Convert.ToByte('D'), TrueTestObject, FalseTestObject, TrueTestObject
			},
			{
				Convert.ToByte('D'), Convert.ToByte('B'), Convert.ToByte('D'), TrueTestObject, FalseTestObject, TrueTestObject
			},
		};
		
		[Theory]
		[MemberData(nameof(TestData))]
		public void IsInRangeConverterConvertFrom(IComparable value, IComparable comparingMinValue, IComparable comparingMaxValue, object trueObject, object falseObject, object expectedResult)
		{
			// Arrange
			IsInRangeConverter isInRangeConverter = new()
			{
				MinValue = comparingMinValue,
				MaxValue = comparingMaxValue,
				FalseObject = falseObject,
				TrueObject = trueObject,
			};

			// Act
			object? convertResult = ((ICommunityToolkitValueConverter)isInRangeConverter).Convert(value, typeof(object), null, CultureInfo.CurrentCulture);
			object convertFromResult = isInRangeConverter.ConvertFrom(value, CultureInfo.CurrentCulture);

			// Assert
			Assert.Equal(expectedResult, convertResult);
			Assert.Equal(expectedResult, convertFromResult);
		}
	}
	```
- Follow Arrange-Act-Assert pattern:
	```csharp
	[Fact]
	public void ViewStructure_CorrectNumberOfChildren()
	{
		// Arrange
		const int maximumRating = 3;
		RatingView ratingView = new()
		{
			MaximumRating = maximumRating
		};

		// Act
		var controlTemplate = ratingView.ControlTemplate;
		var visualTreeDescendantsCount = ratingView.RatingLayout.GetVisualTreeDescendants().Count;
		var childrenCount = ratingView.RatingLayout.Children.Count;

		// Assert
		Assert.NotNull(controlTemplate);
		Assert.Equal((maximumRating * 2) + 1, visualTreeDescendantsCount);
		Assert.Equal(maximumRating, childrenCount);
	}
	```

## Test Isolation:

- Use fresh data for each test:
	```csharp
	public class OrderTests
	{
		private static Order CreateTestOrder() =>
			new(OrderId.New(), TestData.CreateOrderLines());
			
		[Fact]
		public async Task ProcessOrder_Success()
		{
			var order = CreateTestOrder();
			// Test implementation
		}
	}
	```
- Clean up resources:
	```csharp
	[Fact]
	public void IsDisposedDisposeTokenSource()
	{
		// Arrange
		Image testControl = new()
		{
			Source = new GravatarImageSource()
		};

		// Act
		testControl.Layout(new Rect(0, 0, 37, 73));
		var gravatarImageSource = (GravatarImageSource)testControl.Source;
		bool isDisposedBefore = gravatarImageSource.IsDisposed;
		gravatarImageSource.Dispose();
		bool isDisposedAfter = gravatarImageSource.IsDisposed;

		// Assert
		Assert.True(testControl.Source is GravatarImageSource);
		Assert.False(isDisposedBefore);
		Assert.True(isDisposedAfter);
	}
	```

## Best Practices:

- Name tests clearly:
	```csharp
	// Good: Clear test names
	[Fact]
	public void ChangingEmailWithNoSizeDoesNotUpdateUri()
	
	// Avoid: Unclear names
	[Fact]
	public void ChangeEmail()
	```
- Use meaningful assertions:
	```csharp
	// Good: Clear assertions
	Assert.Equal(expected, actual);
	Assert.Contains(expectedItem, collection);
	Assert.Throws<ArgumentOutOfRangeException>(() => new RatingView().MaximumRating = 0);
	
	// Avoid: Multiple assertions without context
	Assert.NotNull(result);
	Assert.True(result.Success);
	Assert.Equal(0, result.Errors.Count);
	```
- Handle async operations properly:
	```csharp
	// Good: Async test method
	[Fact]
	public async Task ProcessOrder_ValidOrder_Succeeds()
	{
		await using var processor = new OrderProcessor();
		var result = await processor.ProcessAsync(order);
		Assert.True(result.IsSuccess);
	}
	
	// Avoid: Sync over async
	[Fact]
	public void ProcessOrder_ValidOrder_Succeeds()
	{
		using var processor = new OrderProcessor();
		var result = processor.ProcessAsync(order).Result;  // Can deadlock
		Assert.True(result.IsSuccess);
	}
	```
- Use `TestContext.Current.CancellationToken` for cancellation:
	```csharp
	// Good:
	[Fact]
	public async Task ProcessOrder_CancellationRequested()
	{
		await using var processor = new OrderProcessor();
		var result = await processor.ProcessAsync(order, TestContext.Current.CancellationToken);
		Assert.True(result.IsSuccess);
	}
	// Avoid:
	[Fact]
	public async Task ProcessOrder_CancellationRequested()
	{
		await using var processor = new OrderProcessor();
		var result = await processor.ProcessAsync(order, CancellationToken.None);
		Assert.False(result.IsSuccess);
	}
	```

## Assertions:

- Use xUnit's built-in assertions:
	```csharp
	// Good: Using xUnit's built-in assertions
	public class OrderTests
	{
		[Fact]
		public void CalculateTotal_WithValidLines_ReturnsCorrectSum()
		{
			// Arrange
			var order = new Order(
				OrderId.New(),
				new[]
				{
					new OrderLine("SKU1", 2, 10.0m),
					new OrderLine("SKU2", 1, 20.0m)
				});
			
			// Act
			var total = order.CalculateTotal();
			
			// Assert
			Assert.Equal(40.0m, total);
		}
		
		[Fact]
		public void Order_WithInvalidLines_ThrowsException()
		{
			// Arrange
			var invalidLines = new OrderLine[] { };
			
			// Act & Assert
			var ex = Assert.Throws<ArgumentException>(() =>
				new Order(OrderId.New(), invalidLines));
			Assert.Equal("Order must have at least one line", ex.Message);
		}
		
		[Fact]
		public void Order_WithValidData_HasExpectedProperties()
		{
			// Arrange
			var id = OrderId.New();
			var lines = new[] { new OrderLine("SKU1", 1, 10.0m) };
			
			// Act
			var order = new Order(id, lines);
			
			// Assert
			Assert.NotNull(order);
			Assert.Equal(id, order.Id);
			Assert.Single(order.Lines);
			Assert.Collection(order.Lines,
				line =>
				{
					Assert.Equal("SKU1", line.Sku);
					Assert.Equal(1, line.Quantity);
					Assert.Equal(10.0m, line.Price);
				});
		}
	}
	```
	
- Avoid third-party assertion libraries:
	```csharp
	// Avoid: Using FluentAssertions or similar libraries
	public class OrderTests
	{
		[Fact]
		public void CalculateTotal_WithValidLines_ReturnsCorrectSum()
		{
			var order = new Order(
				OrderId.New(),
				new[]
				{
					new OrderLine("SKU1", 2, 10.0m),
					new OrderLine("SKU2", 1, 20.0m)
				});
			
			// Avoid: Using FluentAssertions
			order.CalculateTotal().Should().Be(40.0m);
			order.Lines.Should().HaveCount(2);
			order.Should().NotBeNull();
		}
	}
	```
	
- Use proper assertion types:
	```csharp
	public class CustomerTests
	{
		[Fact]
		public void Customer_WithValidEmail_IsCreated()
		{
			// Boolean assertions
			Assert.True(customer.IsActive);
			Assert.False(customer.IsDeleted);
			
			// Equality assertions
			Assert.Equal("john@example.com", customer.Email);
			Assert.NotEqual(Guid.Empty, customer.Id);
			
			// Collection assertions
			Assert.Empty(customer.Orders);
			Assert.Contains("Admin", customer.Roles);
			Assert.DoesNotContain("Guest", customer.Roles);
			Assert.All(customer.Orders, o => Assert.NotNull(o.Id));
			
			// Type assertions
			Assert.IsType<PremiumCustomer>(customer);
			Assert.IsAssignableFrom<ICustomer>(customer);
			
			// String assertions
			Assert.StartsWith("CUST", customer.Reference);
			Assert.Contains("Premium", customer.Description);
			Assert.Matches("^CUST\\d{6}$", customer.Reference);
			
			// Range assertions
			Assert.InRange(customer.Age, 18, 100);
			
			// Reference assertions
			Assert.Same(expectedCustomer, actualCustomer);
			Assert.NotSame(differentCustomer, actualCustomer);
		}
	}
	```
	
- Use Assert.Collection for complex collections:
	```csharp
	[Fact]
	public void ProcessOrder_CreatesExpectedEvents()
	{
		// Arrange
		var processor = new OrderProcessor();
		var order = CreateTestOrder();
		
		// Act
		var events = processor.Process(order);
		
		// Assert
		Assert.Collection(events,
			evt =>
			{
				Assert.IsType<OrderReceivedEvent>(evt);
				var received = Assert.IsType<OrderReceivedEvent>(evt);
				Assert.Equal(order.Id, received.OrderId);
			},
			evt =>
			{
				Assert.IsType<InventoryReservedEvent>(evt);
				var reserved = Assert.IsType<InventoryReservedEvent>(evt);
				Assert.Equal(order.Id, reserved.OrderId);
				Assert.NotEmpty(reserved.ReservedItems);
			},
			evt =>
			{
				Assert.IsType<OrderConfirmedEvent>(evt);
				var confirmed = Assert.IsType<OrderConfirmedEvent>(evt);
				Assert.Equal(order.Id, confirmed.OrderId);
				Assert.True(confirmed.IsSuccess);
			});
	}
	```