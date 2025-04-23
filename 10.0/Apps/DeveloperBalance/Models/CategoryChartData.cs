namespace DeveloperBalance.Models;

public class CategoryChartData
{
	public string Title { get; set; } = string.Empty;
	public int Count { get; set; }

	public CategoryChartData(string title, int count)
	{
		Title = title;
		Count = count;
	}
}