namespace Recipes
{
    public class MockRestService
    {
        public async Task<RecipeData> GetRecipeDataAsync(string uri)
        {
            Random random = new Random();
            int randomWaitTime = random.Next(500, 1500);
            await Task.Delay(randomWaitTime);

            // Generated with AI
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    RecipeName = "Classic Greek Salad",
                    Ingredients = ["Tomatoes", "Cucumbers", "Red onion", "Feta cheese", "Olives", "Olive oil", "Oregano"],
                    ImageUrl = "https://www.italianbellavita.com/wp-content/uploads/2022/08/739C7136-CBA2-4DDC-8D56-ECA409F69AB9-3.jpeg",
                    RecipeUrl = "https://www.italianbellavita.com/2022/08/how-to-make-a-classic-greek-salad/"
                },
                new Recipe
                {
                    RecipeName = "Southern Fried Chicken",
                    Ingredients = ["Chicken pieces", "Buttermilk", "Flour", "Paprika", "Salt", "Pepper", "Oil for frying"],
                    ImageUrl = "https://www.tasteofhome.com/wp-content/uploads/2018/01/Crispy-Fried-Chicken_EXPS_TOHJJ22_6445_DR_02_03_11b.jpg",
                    RecipeUrl = "https://www.tasteofhome.com/recipes/real-southern-fried-chicken/"
                },
                new Recipe
                {
                    RecipeName = "Authentic Mexican Tacos",
                    Ingredients = ["Corn tortillas", "Grilled meat", "Cilantro", "Onions", "Lime", "Salsa"],
                    ImageUrl = "https://thekitchencommunity.org/wp-content/uploads/2021/11/shutterstock_1690419967.jpg",
                    RecipeUrl = "https://thekitchencommunity.org/mexican-tacos/"
                },
                new Recipe
                {
                    RecipeName = "Easy Pumpkin Pie",
                    Ingredients = ["Pumpkin puree", "Eggs", "Sugar", "Cinnamon", "Nutmeg", "Pie crust"],
                    ImageUrl = "https://brooklynfarmgirl.com/wp-content/uploads/2019/10/Easy-Pumpkin-Pie-Recipe_3-650x975.jpg",
                    RecipeUrl = "https://brooklynfarmgirl.com/easy-pumpkin-pie-recipe/"
                },
                new Recipe
                {
                    RecipeName = "Spaghetti Casserole",
                    Ingredients = ["Spaghetti", "Eggs", "Pancetta", "Parmesan cheese", "Salt", "Pepper"],
                    ImageUrl = "https://brooklynfarmgirl.com/wp-content/uploads/2022/06/Baked-Spaghetti-Casserole-_13-650x975.jpg",
                    RecipeUrl = "https://brooklynfarmgirl.com/baked-spaghetti-casserole/"
                },
                new Recipe
                {
                    RecipeName = "Caprese Salad",
                    Ingredients = ["Tomatoes", "Mozzarella cheese", "Basil leaves", "Olive oil", "Balsamic vinegar", "Salt"],
                    ImageUrl = "https://www.acouplecooks.com/wp-content/uploads/2019/07/Mozzarella-Tomato-Basil-Salad-008.jpg",
                    RecipeUrl = "https://www.acouplecooks.com/caprese-salad/"
                },
                new Recipe
                {
                    RecipeName = "Meat Lover’s Pizza",
                    Ingredients = ["Cheese", "Ground beef", "Tomatoes", "Olive oil", "Eggs"],
                    ImageUrl = "https://www.jessicagavin.com/wp-content/uploads/2022/02/meat-lovers-pizza-28.jpg",
                    RecipeUrl = "https://www.jessicagavin.com/meat-lovers-pizza/"
                },
                new Recipe
                {
                    RecipeName = "Vegetarian Buddha Bowl",
                    Ingredients = ["Quinoa", "Chickpeas", "Avocado", "Spinach", "Carrots", "Tahini dressing"],
                    ImageUrl = "https://cookieandkate.com/images/2017/10/best-buddha-bowl-recipe-4.jpg",
                    RecipeUrl = "https://cookieandkate.com/buddha-bowl-recipe/"
                },
            };

            var randomRecipes = recipes.OrderBy(x => random.Next()).Take(6).ToList();

            return new RecipeData
            {
                Hits = randomRecipes.Select(recipe => new Hit
                {
                    Recipe = recipe
                }).ToArray()
            };
        }
    }
}