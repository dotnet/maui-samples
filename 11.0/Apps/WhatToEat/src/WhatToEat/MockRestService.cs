namespace Recipes
{
    public class MockRestService
    {
        readonly List<Recipe> _allRecipes;

        public MockRestService()
        {
            _allRecipes = InitializeRecipes();
        }

        public async Task<RecipeData> GetRecipeDataAsync(string filter)
        {
            // Simulate network delay
            Random random = new Random();
            int randomWaitTime = random.Next(500, 1500);
            await Task.Delay(randomWaitTime);

            // Apply filtering if a filter is provided
            var filteredRecipes = string.IsNullOrWhiteSpace(filter)
                ? _allRecipes
                : FilterRecipes(_allRecipes, filter);

            var randomRecipes = filteredRecipes
                .OrderBy(x => random.Next())
                .ToList();

            return new RecipeData
            {
                Hits = randomRecipes.Select(recipe => new Hit
                {
                    Recipe = recipe
                }).ToArray()
            };
        }

        List<Recipe> FilterRecipes(List<Recipe> recipes, string filter)
        {
            // Handle multiple filters (e.g. "health=keto-friendly&mealType=dinner")
            if (filter.Contains("&"))
            {
                var filterParts = filter.Split('&');
                List<Recipe> filteredResults = new(recipes);

                // Apply each filter in sequence
                foreach (var part in filterParts)
                {
                    filteredResults = FilterRecipes(filteredResults, part.Trim());
                }

                return filteredResults;
            }

            // Dictionary mapping filter prefix to normalization function
            var filterHandlers = new Dictionary<string, Func<string, string>>
            {
                { "health=", NormalizeHealthFilter },
                { "mealType=", NormalizeMealTypeFilter },
                { "dishType=", NormalizeMealTypeFilter }  // dishType uses same normalization as mealType
            };

            // Check if this is a special filter type
            foreach (var handler in filterHandlers)
            {
                string prefix = handler.Key;
                if (filter.Contains(prefix))
                {
                    // Extract the value from the filter
                    string value = ExtractFilterValue(filter, prefix);

                    // Apply the appropriate normalization function
                    string tagToMatch = handler.Value(value);

                    return FilterRecipesByTag(recipes, tagToMatch);
                }
            }

            // If no special filter matched, apply standard filtering
            filter = filter.ToLowerInvariant();
            return recipes.Where(recipe =>
                recipe.RecipeName.ToLowerInvariant().Contains(filter) ||
                recipe.Ingredients.Any(ingredient => ingredient.ToLowerInvariant().Contains(filter)) ||
                recipe.Tags.Any(tag => tag.ToLowerInvariant().Contains(filter))
            ).ToList();
        }

        List<Recipe> FilterRecipesByTag(List<Recipe> recipes, string tagToMatch)
        {
            return recipes.Where(recipe =>
                recipe.Tags.Any(tag => tag.ToLowerInvariant().Contains(tagToMatch.ToLowerInvariant()))
            ).ToList();
        }

        string ExtractFilterValue(string filter, string prefix)
        {
            int startIndex = filter.IndexOf(prefix) + prefix.Length;
            int endIndex = filter.IndexOf('&', startIndex);

            if (endIndex == -1)
                endIndex = filter.Length;

            return filter.Substring(startIndex, endIndex - startIndex).Trim();
        }

        string NormalizeHealthFilter(string healthFilter)
        {
            return healthFilter.ToLowerInvariant() switch
            {
                "keto-friendly" => "keto",
                "vegan" => "vegan",
                "vegetarian" => "vegetarian",
                "gluten-free" => "gluten-free",
                "kosher" => "kosher",
                "balanced" => "balanced",
                _ => healthFilter.ToLowerInvariant()
            };
        }

        string NormalizeMealTypeFilter(string mealTypeFilter)
        {
            return mealTypeFilter.ToLowerInvariant() switch
            {
                "breakfast" => "breakfast",
                "brunch" => "breakfast",
                "lunch" => "lunch",
                "dinner" => "dinner",
                "snack" => "snack",
                "teatime" => "snack",
                "dessert" => "dessert",
                "desserts" => "dessert",
                "drinks" => "drinks",
                _ => mealTypeFilter.ToLowerInvariant()
            };
        }

        List<Recipe> InitializeRecipes()
        {
            // Generated with AI and enhanced with meal types and diet types
            return new List<Recipe>
            {
                new Recipe
                {
                    RecipeName = "Classic Greek Salad",
                    Ingredients = ["Tomatoes", "Cucumbers", "Red onion", "Feta cheese", "Olives", "Olive oil", "Oregano"],
                    Tags = ["Mediterranean", "Salad", "Lunch", "Dinner", "Vegetarian", "Gluten-free", "Balanced"],
                    ImageUrl = "https://www.italianbellavita.com/wp-content/uploads/2022/08/739C7136-CBA2-4DDC-8D56-ECA409F69AB9-3.jpeg",
                    RecipeUrl = "https://www.italianbellavita.com/2022/08/how-to-make-a-classic-greek-salad/"
                },
                new Recipe
                {
                    RecipeName = "Southern Fried Chicken",
                    Ingredients = ["Chicken pieces", "Buttermilk", "Flour", "Paprika", "Salt", "Pepper", "Oil for frying"],
                    Tags = ["American", "Dinner", "Lunch", "Balanced"],
                    ImageUrl = "https://www.tasteofhome.com/wp-content/uploads/2018/01/Crispy-Fried-Chicken_EXPS_TOHJJ22_6445_DR_02_03_11b.jpg",
                    RecipeUrl = "https://www.tasteofhome.com/recipes/real-southern-fried-chicken/"
                },
                new Recipe
                {
                    RecipeName = "Authentic Mexican Tacos",
                    Ingredients = ["Corn tortillas", "Grilled meat", "Cilantro", "Onions", "Lime", "Salsa"],
                    Tags = ["Mexican", "Dinner", "Lunch", "Gluten-free", "Balanced"],
                    ImageUrl = "https://thekitchencommunity.org/wp-content/uploads/2021/11/shutterstock_1690419967.jpg",
                    RecipeUrl = "https://thekitchencommunity.org/mexican-tacos/"
                },
                new Recipe
                {
                    RecipeName = "Easy Pumpkin Pie",
                    Ingredients = ["Pumpkin puree", "Eggs", "Sugar", "Cinnamon", "Nutmeg", "Pie crust"],
                    Tags = ["American", "Dessert", "Vegetarian", "Balanced"],
                    ImageUrl = "https://brooklynfarmgirl.com/wp-content/uploads/2019/10/Easy-Pumpkin-Pie-Recipe_3-650x975.jpg",
                    RecipeUrl = "https://brooklynfarmgirl.com/easy-pumpkin-pie-recipe/"
                },
                new Recipe
                {
                    RecipeName = "Spaghetti Carbonara",
                    Ingredients = ["Spaghetti", "Eggs", "Pancetta", "Parmesan cheese", "Salt", "Pepper"],
                    Tags = ["Italian", "Dinner", "Balanced"],
                    ImageUrl = "https://brooklynfarmgirl.com/wp-content/uploads/2022/06/Baked-Spaghetti-Casserole-_13-650x975.jpg",
                    RecipeUrl = "https://brooklynfarmgirl.com/baked-spaghetti-casserole/"
                },
                new Recipe
                {
                    RecipeName = "Caprese Salad",
                    Ingredients = ["Tomatoes", "Mozzarella cheese", "Basil leaves", "Olive oil", "Balsamic vinegar", "Salt"],
                    Tags = ["Italian", "Salad", "Lunch", "Appetizer", "Vegetarian", "Gluten-free", "Balanced", "Keto"],
                    ImageUrl = "https://www.acouplecooks.com/wp-content/uploads/2019/07/Mozzarella-Tomato-Basil-Salad-008.jpg",
                    RecipeUrl = "https://www.acouplecooks.com/caprese-salad/"
                },
                new Recipe
                {
                    RecipeName = "Meat Lover's Pizza",
                    Ingredients = ["Cheese", "Ground beef", "Tomatoes", "Olive oil", "Eggs"],
                    Tags = ["Italian", "Dinner", "Lunch", "Balanced"],
                    ImageUrl = "https://www.jessicagavin.com/wp-content/uploads/2022/02/meat-lovers-pizza-28.jpg",
                    RecipeUrl = "https://www.jessicagavin.com/meat-lovers-pizza/"
                },
                new Recipe
                {
                    RecipeName = "Vegetarian Buddha Bowl",
                    Ingredients = ["Quinoa", "Chickpeas", "Avocado", "Spinach", "Carrots", "Tahini dressing"],
                    Tags = ["International", "Lunch", "Dinner", "Vegetarian", "Vegan", "Balanced", "Gluten-free"],
                    ImageUrl = "https://cookieandkate.com/images/2017/10/best-buddha-bowl-recipe-4.jpg",
                    RecipeUrl = "https://cookieandkate.com/buddha-bowl-recipe/"
                },
                new Recipe
                {
                    RecipeName = "Honey Garlic Salmon",
                    Ingredients = ["Salmon fillets", "Honey", "Garlic", "Soy sauce", "Lemon juice", "Olive oil"],
                    Tags = ["Seafood", "Dinner", "Balanced", "Gluten-free", "Keto"],
                    ImageUrl = "https://www.wellplated.com/wp-content/uploads/2021/02/Easy-Honey-Garlic-Salmon.jpg",
                    RecipeUrl = "https://www.wellplated.com/honey-garlic-salmon/"
                },
                new Recipe
                {
                    RecipeName = "Cajun Shrimp Pasta",
                    Ingredients = ["Shrimp", "Pasta", "Heavy cream", "Garlic", "Parmesan cheese", "Cajun seasoning"],
                    Tags = ["Cajun", "Dinner", "Seafood", "Balanced"],
                    ImageUrl = "https://www.dinneratthezoo.com/wp-content/uploads/2019/05/cajun-shrimp-pasta-4.jpg",
                    RecipeUrl = "https://www.dinneratthezoo.com/cajun-shrimp-pasta/"
                },
                new Recipe
                {
                    RecipeName = "Creamy Mushroom Risotto",
                    Ingredients = ["Arborio rice", "Mushrooms", "Onion", "Garlic", "Parmesan cheese", "Chicken broth"],
                    Tags = ["Italian", "Dinner", "Vegetarian", "Balanced", "Gluten-free"],
                    ImageUrl = "https://www.recipetineats.com/tachyon/2019/10/Mushroom-Risotto_7.jpg?resize=900%2C1260&zoom=0.86",
                    RecipeUrl = "https://www.recipetineats.com/mushroom-risotto/"
                },
                new Recipe
                {
                    RecipeName = "Teriyaki Chicken",
                    Ingredients = ["Chicken thighs", "Soy sauce", "Brown sugar", "Garlic", "Ginger", "Cornstarch"],
                    Tags = ["Asian", "Japanese", "Dinner", "Lunch", "Balanced", "Gluten-free"],
                    ImageUrl = "https://www.wellplated.com/wp-content/uploads/2023/03/Teriyaki-Chicken-Marinade.jpg",
                    RecipeUrl = "https://www.wellplated.com/teriyaki-chicken/"
                },
                new Recipe
                {
                    RecipeName = "Classic Margherita Pizza",
                    Ingredients = ["Pizza dough", "Tomato sauce", "Fresh mozzarella", "Basil", "Olive oil"],
                    Tags = ["Italian", "Dinner", "Lunch", "Vegetarian", "Balanced"],
                    ImageUrl = "https://www.acouplecooks.com/wp-content/uploads/2022/10/Margherita-Pizza-082.jpg",
                    RecipeUrl = "https://www.acouplecooks.com/margherita-pizza/"
                },
                new Recipe
                {
                    RecipeName = "Homemade Sushi Rolls",
                    Ingredients = ["Sushi rice", "Nori sheets", "Salmon", "Avocado", "Cucumber", "Soy sauce"],
                    Tags = ["Japanese", "Asian", "Dinner", "Lunch", "Balanced", "Gluten-free"],
                    ImageUrl = "https://www.justonecookbook.com/wp-content/uploads/2020/01/Sushi-Rolls-Maki-Sushi-%E2%80%93-Hosomaki-1106-II.jpg",
                    RecipeUrl = "https://www.justonecookbook.com/sushi-rolls/"
                },
                new Recipe
                {
                    RecipeName = "Spicy Thai Basil Chicken",
                    Ingredients = ["Chicken breast", "Thai basil", "Garlic", "Soy sauce", "Oyster sauce", "Chili flakes"],
                    Tags = ["Thai", "Asian", "Dinner", "Spicy", "Balanced", "Gluten-free"],
                    ImageUrl = "https://www.recipetineats.com/tachyon/2017/03/Thai-Chilli-Basil-Chicken_6.jpg",
                    RecipeUrl = "https://www.recipetineats.com/thai-basil-chicken/"
                },
                new Recipe
                {
                    RecipeName = "Chocolate Lava Cake",
                    Ingredients = ["Dark chocolate", "Butter", "Sugar", "Eggs", "Flour", "Cocoa powder"],
                    Tags = ["Dessert", "Vegetarian", "Balanced"],
                    ImageUrl = "https://sallysbakingaddiction.com/wp-content/uploads/2017/02/lava-cake.jpg",
                    RecipeUrl = "https://www.sallysbakingaddiction.com/chocolate-lava-cakes/"
                },
                new Recipe
                {
                    RecipeName = "Teriyaki Beef Stir Fry",
                    Ingredients = ["Beef strips", "Soy sauce", "Ginger", "Garlic", "Bell peppers", "Broccoli"],
                    Tags = ["Asian", "Japanese", "Dinner", "Balanced", "Gluten-free"],
                    ImageUrl = "https://www.wellplated.com/wp-content/uploads/2020/05/Teriyaki-Beef-Stir-Fry.jpg",
                    RecipeUrl = "https://www.wellplated.com/teriyaki-beef-stir-fry/"
                },
                new Recipe
                {
                    RecipeName = "Cheesy Baked Ziti",
                    Ingredients = ["Ziti pasta", "Ricotta cheese", "Mozzarella cheese", "Marinara sauce", "Garlic", "Italian seasoning"],
                    Tags = ["Italian", "Dinner", "Vegetarian", "Balanced"],
                    ImageUrl = "https://www.spendwithpennies.com/wp-content/uploads/2023/03/Easy-Baked-Ziti-SpendWithPennies-5-800x1200.jpg",
                    RecipeUrl = "https://www.spendwithpennies.com/baked-ziti/"
                },
                new Recipe
                {
                    RecipeName = "Avocado Toast with Egg",
                    Ingredients = ["Bread", "Avocado", "Eggs", "Cherry tomatoes", "Salt", "Pepper", "Olive oil"],
                    Tags = ["Breakfast", "Vegetarian", "Balanced"],
                    ImageUrl = "https://cookieandkate.com/images/2012/04/avocado-toast-recipe-3.jpg",
                    RecipeUrl = "https://cookieandkate.com/avocado-toast-recipe/"
                },
                new Recipe
                {
                    RecipeName = "Overnight Oats with Berries",
                    Ingredients = ["Rolled oats", "Milk", "Greek yogurt", "Chia seeds", "Honey", "Mixed berries"],
                    Tags = ["Breakfast", "Vegetarian", "Balanced"],
                    ImageUrl = "https://www.acouplecooks.com/wp-content/uploads/2020/12/Overnight-Oats-011.jpg",
                    RecipeUrl = "https://www.acouplecooks.com/overnight-oats/"
                },
                new Recipe
                {
                    RecipeName = "Quinoa Stuffed Bell Peppers",
                    Ingredients = ["Bell peppers", "Quinoa", "Black beans", "Corn", "Tomatoes", "Cheese", "Cumin"],
                    Tags = ["Dinner", "Vegetarian", "Gluten-free", "Balanced"],
                    ImageUrl = "https://www.wellplated.com/wp-content/uploads/2019/10/Quinoa-Stuffed-Peppers.jpg",
                    RecipeUrl = "https://www.wellplated.com/quinoa-stuffed-peppers/"
                },
                new Recipe
                {
                    RecipeName = "Berry Smoothie Bowl",
                    Ingredients = ["Frozen mixed berries", "Banana", "Greek yogurt", "Almond milk", "Granola", "Honey"],
                    Tags = ["Breakfast", "Snack", "Vegetarian", "Balanced"],
                    ImageUrl = "https://cookieandkate.com/images/2016/04/acai-bowl-recipe-2.jpg",
                    RecipeUrl = "https://cookieandkate.com/acai-bowl-recipe/"
                },
                new Recipe
                {
                    RecipeName = "Keto Bacon and Egg Cups",
                    Ingredients = ["Eggs", "Bacon", "Cheese", "Bell peppers", "Spinach"],
                    Tags = ["Breakfast", "Keto", "Gluten-free", "Low-carb"],
                    ImageUrl = "https://www.wholesomeyum.com/wp-content/uploads/2021/04/wholesomeyum-Bacon-Egg-Muffins-Recipe-Breakfast-Egg-Muffins-with-Bacon-20.jpg",
                    RecipeUrl = "https://www.wholesomeyum.com/recipes/bacon-egg-muffins-recipe/"
                },
                new Recipe
                {
                    RecipeName = "Vegan Chickpea Curry",
                    Ingredients = ["Chickpeas", "Coconut milk", "Tomatoes", "Onion", "Garlic", "Curry powder", "Cilantro"],
                    Tags = ["Indian", "Dinner", "Vegan", "Vegetarian", "Gluten-free", "Balanced"],
                    ImageUrl = "https://rainbowplantlife.com/wp-content/uploads/2020/11/chickpea-curry-1.jpg",
                    RecipeUrl = "https://rainbowplantlife.com/vegan-chickpea-curry/"
                },
                new Recipe
                {
                    RecipeName = "Kosher Beef Brisket",
                    Ingredients = ["Beef brisket", "Onions", "Carrots", "Garlic", "Beef broth", "Tomato paste", "Kosher salt"],
                    Tags = ["Jewish", "Dinner", "Kosher", "Gluten-free"],
                    ImageUrl = "https://toriavey.com/images/2013/03/Passover-Brisket-2-1.jpg",
                    RecipeUrl = "https://toriavey.com/toris-kitchen/savory-slow-cooker-brisket/"
                },
                new Recipe
                {
                    RecipeName = "Homemade Lemonade",
                    Ingredients = ["Lemons", "Sugar", "Water", "Mint leaves"],
                    Tags = ["Drinks", "Vegan", "Vegetarian", "Gluten-free"],
                    ImageUrl = "https://www.acouplecooks.com/wp-content/uploads/2020/06/Lemonade-Recipe-051.jpg",
                    RecipeUrl = "https://www.acouplecooks.com/lemonade-recipe/"
                }
            };
        }
    }
}