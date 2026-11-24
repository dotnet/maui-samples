using ConceptDevelopment.Net.Cryptography;

namespace SolitaireEncryption.Tests;

/// <summary>
/// Tests are taken from the author's official website
/// https://www.schneier.com/code/sol-test.txt
/// </summary>
public class EncryptionTests
{
	static string TestKeyText (string key, string text)
	{
		var ps = new PontifexSolitaire (key);
		var output = ps.Encrypt (text).Pad5 ();
		return output;
	}

	[Fact]
	public void TestKey_nullkey ()
	{
		var output = TestKeyText ("", "AAAAAAAAAAAAAAA");
		Assert.Equal("EXKYI ZSGEH UNTIQ", output);
	}

	[Fact]
	public void TestKey_f ()
	{
		var output = TestKeyText ("f", "AAAAAAAAAAAAAAA");
		Assert.Equal("XYIUQ BMHKK JBEGY", output);
	}

	[Fact]
	public void TestKey_fo ()
	{
		var output = TestKeyText ("fo", "AAAAAAAAAAAAAAA");
		Assert.Equal("TUJYM BERLG XNDIW", output);
	}

	[Fact]
	public void TestKey_foo ()
	{
		var output = TestKeyText ("foo", "AAAAAAAAAAAAAAA");
		Assert.Equal("ITHZU JIWGR FARMW", output);
	}

	[Fact]
	public void TestKey_a ()
	{
		var output = TestKeyText ("a", "AAAAAAAAAAAAAAA");
		Assert.Equal("XODAL GSCUL IQNSC", output);
	}

	[Fact]
	public void TestKey_aa ()
	{
		var output = TestKeyText ("aa", "AAAAAAAAAAAAAAA");
		Assert.Equal("OHGWM XXCAI MCIQP", output);
	}

	[Fact]
	public void TestKey_aaa ()
	{
		var output = TestKeyText ("aaa", "AAAAAAAAAAAAAAA");
		Assert.Equal("DCSQY HBQZN GDRUT", output);
	}

	[Fact]
	public void TestKey_b ()
	{
		var output = TestKeyText ("b", "AAAAAAAAAAAAAAA");
		Assert.Equal("XQEEM OITLZ VDSQS", output);
	}

	[Fact]
	public void TestKey_bc ()
	{
		var output = TestKeyText ("bc", "AAAAAAAAAAAAAAA");
		Assert.Equal("QNGRK QIHCL GWSCE", output);
	}

	[Fact]
	public void TestKey_bcd ()
	{
		var output = TestKeyText ("bcd", "AAAAAAAAAAAAAAA");
		Assert.Equal("FMUBY BMAXH NQXCJ", output);
	}

	[Fact]
	public void TestKey_cryptonomicon_a ()
	{
		var output = TestKeyText ("cryptonomicon", "AAAAAAAAAAAAAAAAAAAAAAAAA");
		Assert.Equal("SUGSR SXSWQ RMXOH IPBFP XARYQ", output);
	}

	[Fact]
	public void TestKey_cryptonomicon_solitaire ()
	{
		var output = TestKeyText ("cryptonomicon", "SOLITAIRE");
		Assert.Equal("KIRAK SFJAN", output);
	}
}

