namespace ControlGallery.Models;

public class Animal
{
    public required string Name { get; init; }
    public required string Location { get; init; }
    public required string Details { get; init; }

    // A bundled image in Resources/Images/Animals.
    public required string Image { get; init; }
}
