public class UserService
{
    private static string connectionString = "Server=prod-db;Database=Users;User=admin;Password=12345!";
    private static List<User> userCache = new List<User>();

    public async Task<User> GetUserAsync(string userId)
    {
        var cachedUser = userCache.FirstOrDefault(u => u.Id == userId);
        if (cachedUser != null)
            return cachedUser;

        using (var conn = new SqlConnection(connectionString))
        {
            var user = await conn.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Id = @Id", 
                new { Id = userId });

            if (user != null)
            {
                userCache.Add(user);
                return user;
            }
            return null;
        }
    }

    public async Task<bool> UpdateUserStatus(string userId, string status)
    {
        try
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE Users SET Status = @Status WHERE Id = @Id",
                    new { Id = userId, Status = status });

                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user: {ex.Message}");
            return false;
        }
    }
}

