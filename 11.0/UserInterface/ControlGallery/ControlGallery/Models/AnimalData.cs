namespace ControlGallery.Models;

// Sample data for the CarouselView, CollectionView, IndicatorView, and SwipeView demos.
// The images are 3D Fluent Emoji by Microsoft (MIT license), bundled in Resources/Images/Animals.
public static class AnimalData
{
    public static IReadOnlyList<Animal> All { get; } =
    [
        new() { Name = "Monkey", Location = "Africa, Asia & the Americas", Image = "monkey.png",
            Details = "There are more than 260 species of monkeys. Old World monkeys live in Africa and Asia, and New World monkeys from southern Mexico to South America." },
        new() { Name = "Gorilla", Location = "Central Africa", Image = "gorilla.png",
            Details = "Gorillas are the largest living primates. They live in family groups led by a dominant adult male, known as a silverback." },
        new() { Name = "Orangutan", Location = "Borneo & Sumatra", Image = "orangutan.png",
            Details = "Orangutans spend most of their lives in trees, more than any other great ape. Their name comes from Malay words meaning \"person of the forest\"." },
        new() { Name = "Lion", Location = "Africa & India", Image = "lion.png",
            Details = "Lions live in social groups called prides. A lion's roar can be heard up to 8 kilometers away." },
        new() { Name = "Tiger", Location = "Asia", Image = "tiger.png",
            Details = "Tigers are the largest living cat species. Like fingerprints, no two tigers have the same pattern of stripes." },
        new() { Name = "Elephant", Location = "Africa & Asia", Image = "elephant.png",
            Details = "Elephants are the largest living land animals. They use their trunks to breathe, smell, drink, and pick things up." },
        new() { Name = "Giraffe", Location = "Africa", Image = "giraffe.png",
            Details = "Giraffes are the tallest living land animals. Despite their long necks, they have seven neck vertebrae, the same number as humans." },
        new() { Name = "Zebra", Location = "Africa", Image = "zebra.png",
            Details = "Every zebra has its own pattern of black and white stripes. Zebras live in herds and often graze alongside wildebeest." },
        new() { Name = "Giant panda", Location = "China", Image = "panda.png",
            Details = "Giant pandas eat almost nothing but bamboo, and can spend more than 12 hours a day eating it." },
        new() { Name = "Koala", Location = "Australia", Image = "koala.png",
            Details = "Koalas feed almost entirely on eucalyptus leaves, and sleep for up to 20 hours a day." },
        new() { Name = "Polar bear", Location = "The Arctic", Image = "polar_bear.png",
            Details = "Polar bears have black skin under their thick, translucent fur. They mainly hunt seals from the sea ice." },
        new() { Name = "Penguin", Location = "Southern Hemisphere", Image = "penguin.png",
            Details = "Penguins can't fly, but their flippers make them excellent swimmers. Almost all species live in the Southern Hemisphere." },
        new() { Name = "Kangaroo", Location = "Australia", Image = "kangaroo.png",
            Details = "Kangaroos are marsupials that carry their young, called joeys, in a pouch. They move by hopping on their powerful hind legs." },
        new() { Name = "Sloth", Location = "Central & South America", Image = "sloth.png",
            Details = "Sloths spend most of their lives hanging upside down in trees. They move so slowly that algae can grow in their fur." },
        new() { Name = "Flamingo", Location = "Africa, Asia, Europe & the Americas", Image = "flamingo.png",
            Details = "Flamingos get their pink color from pigments in the algae and shrimp they eat. They often rest standing on one leg." },
        new() { Name = "Red fox", Location = "Northern Hemisphere & Australia", Image = "fox.png",
            Details = "The red fox has the widest range of any wild carnivore. It lives across Europe, Asia, North America, and North Africa, and was introduced to Australia." },
    ];
}
