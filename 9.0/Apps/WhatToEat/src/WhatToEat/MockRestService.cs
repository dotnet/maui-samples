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
                new Recipe
                {
                    RecipeName = "Honey Garlic Salmon",
                    Ingredients = ["Salmon fillets", "Honey", "Garlic", "Soy sauce", "Lemon juice", "Olive oil"],
                    ImageUrl = "https://www.wellplated.com/wp-content/uploads/2021/02/Easy-Honey-Garlic-Salmon.jpg",
                    RecipeUrl = "https://www.wellplated.com/honey-garlic-salmon/"
                },
                new Recipe
                {
                    RecipeName = "Cajun Shrimp Pasta",
                    Ingredients = ["Shrimp", "Pasta", "Heavy cream", "Garlic", "Parmesan cheese", "Cajun seasoning"],
                    ImageUrl = "https://www.dinneratthezoo.com/wp-content/uploads/2019/05/cajun-shrimp-pasta-4.jpg",
                    RecipeUrl = "https://www.dinneratthezoo.com/cajun-shrimp-pasta/"
                },
                new Recipe
                {
                    RecipeName = "Creamy Mushroom Risotto",
                    Ingredients = ["Arborio rice", "Mushrooms", "Onion", "Garlic", "Parmesan cheese", "Chicken broth"],
                    ImageUrl = "https://www.recipetineats.com/tachyon/2019/10/Mushroom-Risotto_7.jpg?resize=900%2C1260&zoom=0.86",
                    RecipeUrl = "https://www.recipetineats.com/mushroom-risotto/"
                },
                new Recipe
                {
                    RecipeName = "Teriyaki Chicken",
                    Ingredients = ["Chicken thighs", "Soy sauce", "Brown sugar", "Garlic", "Ginger", "Cornstarch"],
                    ImageUrl = "https://www.wellplated.com/wp-content/uploads/2023/03/Teriyaki-Chicken-Marinade.jpg",
                    RecipeUrl = "https://www.wellplated.com/teriyaki-chicken/"
                },
                new Recipe
                {
                    RecipeName = "Classic Margherita Pizza",
                    Ingredients = ["Pizza dough", "Tomato sauce", "Fresh mozzarella", "Basil", "Olive oil"],
                    ImageUrl = "https://www.acouplecooks.com/wp-content/uploads/2022/10/Margherita-Pizza-082.jpg",
                    RecipeUrl = "https://www.acouplecooks.com/margherita-pizza/"
                },
                new Recipe
                {
                    RecipeName = "Homemade Sushi Rolls",
                    Ingredients = ["Sushi rice", "Nori sheets", "Salmon", "Avocado", "Cucumber", "Soy sauce"],
                    ImageUrl = "https://www.justonecookbook.com/wp-content/uploads/2020/01/Sushi-Rolls-Maki-Sushi-%E2%80%93-Hosomaki-1106-II.jpg",
                    RecipeUrl = "https://www.justonecookbook.com/sushi-rolls/"
                },
                new Recipe
                {
                    RecipeName = "Spicy Thai Basil Chicken",
                    Ingredients = ["Chicken breast", "Thai basil", "Garlic", "Soy sauce", "Oyster sauce", "Chili flakes"],
                    ImageUrl = "https://www.recipetineats.com/tachyon/2017/03/Thai-Chilli-Basil-Chicken_6.jpg",
                    RecipeUrl = "https://www.recipetineats.com/thai-basil-chicken/"
                },
                new Recipe
                {
                    RecipeName = "Chocolate Lava Cake",
                    Ingredients = ["Dark chocolate", "Butter", "Sugar", "Eggs", "Flour", "Cocoa powder"],
                    ImageUrl = "https://sallysbakingaddiction.com/wp-content/uploads/2017/02/lava-cake.jpg",
                    RecipeUrl = "https://www.sallysbakingaddiction.com/chocolate-lava-cakes/"
                },
                new Recipe
                {
                    RecipeName = "Teriyaki Beef Stir Fry",
                    Ingredients = ["Beef strips", "Soy sauce", "Ginger", "Garlic", "Bell peppers", "Broccoli"],
                    ImageUrl = "https://www.wellplated.com/wp-content/uploads/2020/05/Teriyaki-Beef-Stir-Fry.jpg",
                    RecipeUrl = "https://www.wellplated.com/teriyaki-beef-stir-fry/"
                },
                new Recipe
                {
                    RecipeName = "Cheesy Baked Ziti",
                    Ingredients = ["Ziti pasta", "Ricotta cheese", "Mozzarella cheese", "Marinara sauce", "Garlic", "Italian seasoning"],
                    ImageUrl = "https://www.spendwithpennies.com/wp-content/uploads/2023/03/Easy-Baked-Ziti-SpendWithPennies-5-800x1200.jpg",
                    RecipeUrl = "https://www.spendwithpennies.com/baked-ziti/"
                },
                new Recipe
                {
                    RecipeName = "Avocado Toast with Egg",
                    Ingredients = ["Bread", "Avocado", "Eggs", "Cherry tomatoes", "Salt", "Pepper", "Olive oil"],
                    ImageUrl = "https://cookieandkate.com/images/2012/04/avocado-toast-recipe-3.jpg",
                    RecipeUrl = "https://cookieandkate.com/avocado-toast-recipe/"
                }
            };

            int randomRecipesCount = random.Next(10, 20);
            var randomRecipes = recipes.OrderBy(x => random.Next()).Take(randomRecipesCount).ToList();

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