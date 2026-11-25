using ConceptDevelopment.Net.Cryptography;

namespace SolitaireEncryption;

public partial class SolitairePage : ContentPage
{
    public SolitairePage()
    {
        InitializeComponent();
    }

    public async void EncryptClicked(object sender, EventArgs e)
    {
        if (!ValidInputs())
        {
            await DisplayAlertAsync("Missing input", "You must enter plaintext/ciphertext and the key", "OK");
            return;
        }
        var ps = new PontifexSolitaire(key.Text);
        ciphertext.Text = ps.Encrypt(plaintext.Text).Pad5();
    }

    public async void DecryptClicked(object sender, EventArgs e)
    {
        if (!ValidInputs())
        {
            await DisplayAlertAsync("Missing input", "You must enter plaintext/ciphertext and the key", "OK");
            return;
        }
        var ps = new PontifexSolitaire(key.Text);
        ciphertext.Text = ps.Decrypt(plaintext.Text).Pad5();
    }

    // input validation
    bool ValidInputs()
    {
        return !(string.IsNullOrWhiteSpace(key.Text) || string.IsNullOrWhiteSpace(plaintext.Text));
    }
}

