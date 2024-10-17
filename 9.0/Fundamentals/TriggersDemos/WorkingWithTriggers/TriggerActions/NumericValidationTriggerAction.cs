namespace WorkingWithTriggers
{
	public class NumericValidationTriggerAction : TriggerAction<Entry> 
	{
		protected override void Invoke (Entry entry)
		{
			double result;
			bool isValid = Double.TryParse (entry.Text, out result);
			entry.TextColor = isValid ? Colors.Black : Colors.Red;
		}
	}
}

