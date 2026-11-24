namespace SolitaireEncryption.Tests;

public class DisplayTests
{
	[Fact]
	public void TestPad5 ()
	{
		Assert.Equal("XYIUQ BMHKK JBEGY", "XYIUQBMHKKJBEGY".Pad5());
	}

	[Fact]
	public void TestPad5_short ()
	{
		// throws because not a multiple of 5 characters
		Assert.Equal("HELOX", "HELO".Pad5()); 
	}

	[Fact]
	public void TestPad5_medium ()
	{
		// throws because not a multiple of 5 characters
		Assert.Equal("SOLIT AIREX", "SOLITAIRE".Pad5()); 
	}
}

