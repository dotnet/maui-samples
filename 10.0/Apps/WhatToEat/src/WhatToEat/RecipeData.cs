namespace Recipes
{
    public class RecipeData
    {
        public Hit[] Hits { get; set; }
    }

    public class Hit
    {
        public Recipe Recipe { get; set; }
        public int Id { get; set; }
    }

    public class Recipe
    {
        public string RecipeName { get; set; }

        public string[] Ingredients { get; set; }

        public string[] Tags { get; set; }

        public string ImageUrl { get; set; }

        public string RecipeUrl { get; set; }
    }
}