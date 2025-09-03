﻿using ConceptDevelopment.Net.Cryptography;

namespace SolitaireEncryption.Tests;

/// <summary>
/// Tests are taken from the author's official website
/// https://www.schneier.com/code/sol-test.txt
/// </summary>
public class DecryptionTests
{
    static string TestKeyText(string key, string text)
    {
        var ps = new PontifexSolitaire(key);
        var output = ps.Decrypt(text).Pad5();
        return output;
    }

    [Fact]
    public void TestKey_f()
    {
        var output = TestKeyText("f", "XYIUQ BMHKK JBEGY");
        Assert.Equal("AAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_fo()
    {
        var output = TestKeyText("fo", "TUJYM BERLG XNDIW");
        Assert.Equal("AAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_foo()
    {
        var output = TestKeyText("foo", "ITHZU JIWGR FARMW");
        Assert.Equal("AAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_a()
    {
        var output = TestKeyText("a", "XODAL GSCUL IQNSC");
        Assert.Equal("AAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_aa()
    {
        var output = TestKeyText("aa", "OHGWM XXCAI MCIQP");
        Assert.Equal("AAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_aaa()
    {
        var output = TestKeyText("aaa", "DCSQY HBQZN GDRUT");
        Assert.Equal("AAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_b()
    {
        var output = TestKeyText("b", "XQEEM OITLZ VDSQS");
        Assert.Equal("AAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_bc()
    {
        var output = TestKeyText("bc", "QNGRK QIHCL GWSCE");
        Assert.Equal("AAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_bcd()
    {
        var output = TestKeyText("bcd", "FMUBY BMAXH NQXCJ");
        Assert.Equal("AAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_cryptonomicon_a()
    {
        var output = TestKeyText("cryptonomicon", "SUGSR SXSWQ RMXOH IPBFP XARYQ");
        Assert.Equal("AAAAAAAAAAAAAAAAAAAAAAAAA".Pad5(), output);
    }

    [Fact]
    public void TestKey_cryptonomicon_solitaire()
    {
        var output = TestKeyText("cryptonomicon", "KIRAK SFJAN");
        Assert.Equal("SOLITAIRE".Pad5(), output);
    }
}
