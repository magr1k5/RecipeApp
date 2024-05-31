using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace RecipeApp.Classes
{
    public class DatabaseManager : IDisposable
    {
        private NpgsqlConnection connection;
        private readonly string connectionString;

        public DatabaseManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void OpenConnection()
        {
            connection = new NpgsqlConnection(connectionString);
            connection.Open();
        }

        public void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public NpgsqlConnection GetConnection()
        {
            return connection;
        }

        public void Dispose()
        {
            CloseConnection();
        }

        public Users GetUserByUsernameAndPassword(string username, string password)
        {
            Users user = null;

            using (var cmd = new NpgsqlCommand("SELECT u.userid , u.username, u.name, u.surname, ur.rolename " +
                                              "FROM users u " +
                                              "JOIN userroles ur ON u.roleid = ur.roleid " +
                                              "WHERE u.username = @username AND u.password = @password", GetConnection()))
            {
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new Users
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Name = reader.GetString(2),
                            Surname = reader.GetString(3),
                            RoleName = reader.GetString(4)
                        };
                    }
                }
            }

            return user;
        }

        public List<Recipes> GetRecipes()
        {
            OpenConnection();
            List<Recipes> recipes = new List<Recipes>();

            using (var cmd = new NpgsqlCommand("SELECT * FROM recipes WHERE pending = false", GetConnection()))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Recipes recipe = new Recipes
                    {
                        recipeid = reader.GetInt32(0),
                        recipename = reader.GetString(1),
                        recipedescription = reader.GetString(2),
                        recipegroup = reader.GetString(3),
                        userid = reader.GetInt32(4),
                        img = reader.GetString(5),
                        pending = reader.GetBoolean(6)
                    };
                    recipes.Add(recipe);
                }
            }
            CloseConnection();
            return recipes;
        }

        public List<Ingredient> GetIngredients()
        {

            OpenConnection();
            List<Ingredient> ingredients = new List<Ingredient>();

            using (var cmd = new NpgsqlCommand("SELECT * FROM ingredients", GetConnection()))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Создаем объект Ingredient и заполняем его данными из базы
                        Ingredient ingredient = new Ingredient
                        {
                            IngredientID = reader.GetInt32(0),
                            IngredientName = reader.GetString(1),
                            IngredientGroup = reader.GetString(2),
                            Img = reader.GetString(3)
                        };
                        ingredients.Add(ingredient);
                    }
                }
            }
            CloseConnection();
            return ingredients;
        }

        


        public List<Ingredient> GetIngredientsForRecipe(int recipeId)
        {
            OpenConnection();
            List<Ingredient> recipeIngredients = new List<Ingredient>();


            using (var cmd = new NpgsqlCommand("SELECT i.*, ri.quantity FROM ingredients i " +
                "JOIN recipeingredients ri ON i.ingredientid = ri.ingredientid " +
                "WHERE ri.recipeid = @recipeId", GetConnection()))
            {
                cmd.Parameters.AddWithValue("recipeId", recipeId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ingredient recipeIngredient = new Ingredient
                        {
                            IngredientID = reader.GetInt32(0),
                            IngredientName = reader.GetString(1),
                            IngredientGroup = reader.GetString(2),
                            Img = reader.GetString(3),
                            Quantity = reader.GetString(4)
                        };
                        recipeIngredients.Add(recipeIngredient);
                    }
                }
            }
            CloseConnection();
            return recipeIngredients;
        }

        public List<RecipesSteps> GetRecipeSteps(int recipeId)
        {
            OpenConnection();
            List<RecipesSteps> recipeSteps = new List<RecipesSteps>();

            using (var cmd = new NpgsqlCommand("SELECT stepnumber, stepdescription " +
                                              "FROM recipesteps " +
                                              "WHERE recipeid = @recipeId " +
                                              "ORDER BY stepnumber", GetConnection()))
            {
                cmd.Parameters.AddWithValue("@recipeId", recipeId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecipesSteps step = new RecipesSteps
                        {
                            stepnumber = reader.GetInt32(0),
                            stepdescription = reader.GetString(1)
                        };
                        recipeSteps.Add(step);
                    }
                }
            }

            return recipeSteps;
        }

        public bool AddRecipeToFavorites(int userId, int recipeId)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO userfavorites (userid, recipeid) VALUES (@userId, @recipeId)", GetConnection()))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("recipeId", recipeId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }


        public bool RemoveRecipeFromFavorites(int userId, int recipeId)
        {
            using (var cmd = new NpgsqlCommand("DELETE FROM userfavorites WHERE userid = @userId AND recipeid = @recipeId", GetConnection()))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("recipeId", recipeId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<int> GetFavoriteRecipes(int userId)
        {
            List<int> favoriteRecipeIds = new List<int>();

            using (var cmd = new NpgsqlCommand("SELECT recipeid FROM userfavorites WHERE userid = @userId", GetConnection()))
            {
                cmd.Parameters.AddWithValue("userId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        favoriteRecipeIds.Add(reader.GetInt32(0));
                    }
                }
            }

            return favoriteRecipeIds;
        }

        


        public bool IsRecipeInFavorites(int userId, int recipeId)
        {
            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM userfavorites WHERE userid = @userId AND recipeid = @recipeId", GetConnection()))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("recipeId", recipeId);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
        }

        public Recipes GetRecipeById(int recipeId)
        {
            using (var cmd = new NpgsqlCommand("SELECT * FROM recipes WHERE recipeid = @recipeId", GetConnection()))
            {
                cmd.Parameters.AddWithValue("recipeId", recipeId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Recipes recipe = new Recipes
                        {
                            recipeid = reader.GetInt32(0),
                            recipename = reader.GetString(1),
                            recipedescription = reader.GetString(2),
                            recipegroup = reader.GetString(3),
                            userid = reader.GetInt32(4),
                            img = reader.GetString(5),
                            pending = reader.GetBoolean(6)
                        };
                        return recipe;
                    }
                }
            }

            return null;
        }

        public int GetIngredientIdByName(string ingredientName)
        {
            OpenConnection();
            using (var cmd = new NpgsqlCommand("SELECT ingredientid FROM ingredients WHERE ingredientname = @ingredientName", GetConnection()))
            {
                cmd.Parameters.AddWithValue("@ingredientName", ingredientName);

                var result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    // Или верните значение по умолчанию или бросьте исключение, в зависимости от вашей логики
                    return -1; // Пример значения по умолчанию
                }
            }

        }

        public int AddRecipe(string recipeName, string recipeDescription, string recipeGroup, int userId, string img, bool pending)
        {
            OpenConnection();
            using (var cmd = new NpgsqlCommand("INSERT INTO recipes (recipename, recipedescription, recipegroup, userid , img , pending) VALUES (@recipeName, @recipeDescription, @recipeGroup, @userId , @img , @pending) RETURNING recipeid", GetConnection()))
            {
                cmd.Parameters.AddWithValue("@recipeName", recipeName);
                cmd.Parameters.AddWithValue("@recipeDescription", recipeDescription);
                cmd.Parameters.AddWithValue("@recipeGroup", recipeGroup);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@img", img);
                cmd.Parameters.AddWithValue("@pending", pending);

                int recipeId = (int)cmd.ExecuteScalar();
                CloseConnection();

                return recipeId;
            }
        }

        public bool AddRecipeIngredient(int recipeId, int ingredientId, string quantity)
        {
            // int ingredientId = GetIngredientIdByName(ingredientName);

            if (ingredientId == -1)
            {
                // Ингредиент с таким именем не найден, добавьте его в базу данных или обработайте ошибку по вашему усмотрению
                return false;
            }

            using (var cmd = new NpgsqlCommand("INSERT INTO recipeingredients (recipeid, ingredientid, quantity) VALUES (@recipeId, @ingredientId, @quantity)", GetConnection()))
            {
                cmd.Parameters.AddWithValue("@recipeId", recipeId);
                cmd.Parameters.AddWithValue("@ingredientId", ingredientId);
                cmd.Parameters.AddWithValue("@quantity", quantity);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }


        public bool AddRecipeStep(int recipeId, int stepNumber, string stepDescription)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO recipesteps (recipeid, stepnumber, stepdescription) VALUES (@recipeId, @stepNumber, @stepDescription)", GetConnection()))
            {
                cmd.Parameters.AddWithValue("@recipeId", recipeId);
                cmd.Parameters.AddWithValue("@stepNumber", stepNumber);
                cmd.Parameters.AddWithValue("@stepDescription", stepDescription);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<string> GetRecipeGroups()
        {
            OpenConnection();
            List<string> recipeGroups = new List<string>();

            using (var cmd = new NpgsqlCommand("SELECT DISTINCT recipegroup FROM recipes", GetConnection()))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    recipeGroups.Add(reader.GetString(0));
                }
            }
            CloseConnection();
            return recipeGroups;
        }

        public bool UpdateRecipeImage(int recipeId, string img)
        {
            OpenConnection();
            using (NpgsqlConnection connection = GetConnection())
            {

                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE recipes SET img = @img WHERE recipeid = @recipeId", connection))
                {
                    cmd.Parameters.AddWithValue("@img", img);
                    cmd.Parameters.AddWithValue("@recipeId", recipeId);
                    Console.WriteLine("Debug: img before update:" + img);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public List<Recipes> GetPendingRecipes()
        {
            List<Recipes> pendingRecipes = new List<Recipes>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM recipes WHERE pending = true", connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Recipes recipe = new Recipes
                            {
                                recipeid = (int)reader["recipeid"],
                                recipename = reader["recipename"].ToString(),
                                recipedescription = reader["recipedescription"].ToString(),
                                recipegroup = reader["recipegroup"].ToString(),
                                img = reader["img"].ToString()
                            };
                            pendingRecipes.Add(recipe);
                        }
                    }
                }
            }
            return pendingRecipes;

        }

        public bool UpdateRecipePendingStatus(int recipeId, bool pendingStatus)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("UPDATE recipes SET pending = @pending WHERE recipeid = @recipeid", connection))
                {
                    command.Parameters.AddWithValue("@pending", pendingStatus);
                    command.Parameters.AddWithValue("@recipeid", recipeId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool DeleteRecipe(int recipeId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Удалите связанные записи из таблицы recipesteps
                        using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM recipesteps WHERE recipeid = @recipeid", connection, transaction))
                        {
                            command.Parameters.AddWithValue("recipeid", recipeId);
                            command.ExecuteNonQuery();
                        }

                        // 2. Удалите связанные записи из таблицы recipeingredients
                        using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM recipeingredients WHERE recipeid = @recipeid", connection, transaction))
                        {
                            command.Parameters.AddWithValue("recipeid", recipeId);
                            command.ExecuteNonQuery();
                        }

                        // 3. Удалите запись из таблицы recipes
                        using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM recipes WHERE recipeid = @recipeid", connection, transaction))
                        {
                            command.Parameters.AddWithValue("recipeid", recipeId);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
            }
        }

        public List<Recipes> GetPendingRecipesByUser(int userId) ///////////////////////////////////////////////////////////
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM recipes WHERE userid = @userId AND pending = true";

                var parameters = new NpgsqlParameter[]
                {
            new NpgsqlParameter("@userId", userId)
                };

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);
                    using (var reader = command.ExecuteReader())
                    {
                        List<Recipes> recipes = new List<Recipes>();
                        while (reader.Read())
                        {
                            recipes.Add(new Recipes
                            {
                                recipeid = reader.GetInt32(reader.GetOrdinal("recipeid")),
                                recipename = reader.GetString(reader.GetOrdinal("recipename")),
                                recipedescription = reader.GetString(reader.GetOrdinal("recipedescription")),
                                recipegroup = reader.GetString(reader.GetOrdinal("recipegroup")),
                                img = reader.GetString(reader.GetOrdinal("img")),
                                pending = reader.GetBoolean(reader.GetOrdinal("pending"))
                            });
                        }
                        return recipes;
                    }
                }
            }
        }

        public List<Recipes> GetPendingRecipesByUser2(int userId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM recipes WHERE userid = @userId AND pending = false";

                var parameters = new NpgsqlParameter[]
                {
            new NpgsqlParameter("@userId", userId)
                };

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);
                    using (var reader = command.ExecuteReader())
                    {
                        List<Recipes> recipes = new List<Recipes>();
                        while (reader.Read())
                        {
                            recipes.Add(new Recipes
                            {
                                recipeid = reader.GetInt32(reader.GetOrdinal("recipeid")),
                                recipename = reader.GetString(reader.GetOrdinal("recipename")),
                                recipedescription = reader.GetString(reader.GetOrdinal("recipedescription")),
                                recipegroup = reader.GetString(reader.GetOrdinal("recipegroup")),
                                img = reader.GetString(reader.GetOrdinal("img")),
                                pending = reader.GetBoolean(reader.GetOrdinal("pending"))
                            });
                        }
                        return recipes;
                    }
                }
            }
        }

        public List<Recipes> SearchRecipesInDatabase(string searchQuery)
        {
            List<Recipes> searchResults = new List<Recipes>();

            try
            {
                OpenConnection();
                string query = "SELECT DISTINCT r.recipeid, r.recipename, r.recipedescription, r.recipegroup, r.img, r.pending " +
                               "FROM recipes r " +
                               "LEFT JOIN recipeingredients ri ON r.recipeid = ri.recipeid " +
                               "LEFT JOIN ingredients i ON ri.ingredientid = i.ingredientid " +
                               "WHERE r.recipename LIKE @SearchQuery " +
                               "OR r.recipedescription LIKE @SearchQuery " +
                               "OR i.ingredientname LIKE @SearchQuery " +
                               "AND r.pending = false";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Recipes recipe = new Recipes
                            {
                                recipeid = reader.GetInt32(0),
                                recipename = reader.GetString(1),
                                recipedescription = reader.GetString(2),
                                recipegroup = reader.GetString(3),
                                img = reader.GetString(4),
                                pending = reader.GetBoolean(5)
                            };
                            searchResults.Add(recipe);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                CloseConnection();
            }

            return searchResults;
        }

    }
}

