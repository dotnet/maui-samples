namespace ShellRouteTemplates.Models;

public static class RouteCatalog
{
	public const string NotSupplied = "(not supplied)";

	public static IReadOnlyList<RouteExample> Examples { get; } =
	[
		new("required", "Required parameter", "trip/{tripId}", "//routes/trip/SEA-204?caseId=required", "QueryProperty", "tripId", "SEA-204", "A trip identifier is required."),
		new("optional-present", "Optional parameter (present)", "traveler/{name?}", "//routes/traveler/Ada?caseId=optional-present", "IQueryAttributable", "name", "Ada", "The final segment can be supplied."),
		new("optional-absent", "Optional parameter (absent)", "traveler/{name?}", "//routes/traveler?caseId=optional-absent", "IQueryAttributable", "name", NotSupplied, "The final segment can be omitted."),
		new("default", "Default value", "rating/{stars=5}", "//routes/rating?caseId=default", "IQueryAttributable", "stars", "5", "An omitted final segment delivers its default."),
		new("constraint-int", "Constraint: int", "reservation/{reservationId:int}", "//routes/reservation/42?caseId=constraint-int", "IQueryAttributable", "reservationId", "42", "Accepts a 32-bit integer."),
		new("constraint-long", "Constraint: long", "loyalty/{points:long}", "//routes/loyalty/9000000000?caseId=constraint-long", "IQueryAttributable", "points", "9000000000", "Accepts a 64-bit integer."),
		new("constraint-double", "Constraint: double", "budget/{amount:double}", "//routes/budget/1299.50?caseId=constraint-double", "IQueryAttributable", "amount", "1299.50", "Accepts an invariant-culture number."),
		new("constraint-bool", "Constraint: bool", "toggle/{enabled:bool}", "//routes/toggle/true?caseId=constraint-bool", "IQueryAttributable", "enabled", "true", "Accepts true or false."),
		new("constraint-guid", "Constraint: guid", "booking/{reference:guid}", "//routes/booking/550e8400-e29b-41d4-a716-446655440000?caseId=constraint-guid", "IQueryAttributable", "reference", "550e8400-e29b-41d4-a716-446655440000", "Accepts a GUID."),
		new("constraint-alpha", "Constraint: alpha", "region/{name:alpha}", "//routes/region/Pacific?caseId=constraint-alpha", "IQueryAttributable", "name", "Pacific", "Accepts letters only."),
		new("catch-all", "Catch-all", "files/{*path}", "//routes/files/trips/SEA-204/receipt.pdf?caseId=catch-all", "IQueryAttributable", "path", "trips/SEA-204/receipt.pdf", "Captures all remaining path segments."),
		new("mixed", "Mixed literal and parameter", "trip-{tripId}-summary", "//routes/trip-SEA-204-summary?caseId=mixed", "QueryProperty", "tripId", "SEA-204", "Matches a parameter between literal text.")
	];

	public static RouteExample? Find(string? id) =>
		Examples.FirstOrDefault(example => example.Id == id);
}
