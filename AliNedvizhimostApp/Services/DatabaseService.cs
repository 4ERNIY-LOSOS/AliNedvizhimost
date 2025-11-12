using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Utils;

namespace AliNedvizhimostApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public User RegisterUser(string email, string password, string firstName, string lastName, string role)
        {
            var cleanEmail = email?.Trim();
            var cleanFirstName = firstName?.Trim();
            var cleanLastName = lastName?.Trim();

            if (string.IsNullOrEmpty(cleanEmail) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(cleanFirstName) || string.IsNullOrEmpty(cleanLastName))
            {
                throw new ArgumentException("Email, password, first name, and last name cannot be empty.");
            }

            string hashedPassword = PasswordHasher.HashPassword(password);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role) VALUES (@Email, @PasswordHash, @FirstName, @LastName, @Role); SELECT SCOPE_IDENTITY();";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", cleanEmail);
                    command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    command.Parameters.AddWithValue("@FirstName", cleanFirstName);
                    command.Parameters.AddWithValue("@LastName", cleanLastName);
                    command.Parameters.AddWithValue("@Role", role);

                    try
                    {
                        var result = command.ExecuteScalar();
                        var userId = Convert.ToInt32(result);
                        return new User
                        {
                            UserId = userId,
                            Email = cleanEmail,
                            FirstName = cleanFirstName,
                            LastName = cleanLastName,
                            Role = role
                        };
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            throw new InvalidOperationException($"Пользователь с таким Email '{cleanEmail}' уже существует.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        public User LoginUser(string email, string password)
        {
            email = email?.Trim();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT UserId, Email, PasswordHash, FirstName, LastName, Role FROM Users WHERE LOWER(Email) = LOWER(@Email);";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["PasswordHash"].ToString();
                            if (PasswordHasher.VerifyPassword(password, storedHash))
                            {
                                return new User
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    Role = reader.GetString(reader.GetOrdinal("Role"))
                                };
                            }
                        }
                    }
                }
            }
            return null;
        }

        public List<User> GetUsers()
        {
            Console.WriteLine("DatabaseService.GetUsers() called.");
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT UserId, Email, FirstName, LastName, Role FROM Users;";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Role = reader.GetString(reader.GetOrdinal("Role"))
                            });
                        }
                    }
                }
            }
            Console.WriteLine($"DatabaseService.GetUsers(): Returning {users.Count} users.");
            return users;
        }

        public void UpdateUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE Users SET Email = @Email, FirstName = @FirstName, LastName = @LastName, Role = @Role WHERE UserId = @UserId;";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }
        }

        public void DeleteUser(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string deleteUserSql = "DELETE FROM Users WHERE UserId = @UserId;";
                using (var command = new SqlCommand(deleteUserSql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }
        }

        public void DeleteProperty(int propertyId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Properties WHERE Id = @PropertyId;";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PropertyId", propertyId);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }
        }

        public List<Property> GetProperties()
        {
            Console.WriteLine("DatabaseService.GetProperties() called.");
            var properties = new List<Property>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, Title, Address, Price, Area, Rooms, Description, UserId, Status FROM Properties;";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader.GetString(reader.GetOrdinal("Title"));
                            properties.Add(new Property
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = title,
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Area = reader.GetDouble(reader.GetOrdinal("Area")),
                                Rooms = reader.GetInt32(reader.GetOrdinal("Rooms")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            });
                        }
                    }
                }
            }
            Console.WriteLine($"DatabaseService.GetProperties(): Returning {properties.Count} properties.");
            return properties;
        }

        public List<Property> GetPropertiesByUserId(int userId)
        {
            Console.WriteLine($"DatabaseService.GetPropertiesByUserId() called for UserId: {userId}.");
            var properties = new List<Property>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, Title, Address, Price, Area, Rooms, Description, UserId, Status FROM Properties WHERE UserId = @UserId;";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            properties.Add(new Property
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Area = reader.GetDouble(reader.GetOrdinal("Area")),
                                Rooms = reader.GetInt32(reader.GetOrdinal("Rooms")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            });
                        }
                    }
                }
            }
            Console.WriteLine($"DatabaseService.GetPropertiesByUserId(): Returning {properties.Count} properties.");
            return properties;
        }

        public void AddProperty(Property property)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Properties (Title, Address, Price, Area, Rooms, Description, UserId, Status) VALUES (@Title, @Address, @Price, @Area, @Rooms, @Description, @UserId, @Status);";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Title", property.Title);
                    command.Parameters.AddWithValue("@Address", property.Address);
                    command.Parameters.AddWithValue("@Price", property.Price);
                    command.Parameters.AddWithValue("@Area", property.Area);
                    command.Parameters.AddWithValue("@Rooms", property.Rooms);
                    command.Parameters.AddWithValue("@Description", property.Description);
                    command.Parameters.AddWithValue("@UserId", property.UserId);
                    command.Parameters.AddWithValue("@Status", property.Status);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }
        }

        public void UpdateProperty(Property property)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"UPDATE Properties SET 
                                Title = @Title, 
                                Address = @Address, 
                                Price = @Price, 
                                Area = @Area, 
                                Rooms = @Rooms, 
                                Description = @Description
                             WHERE Id = @Id;";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", property.Id);
                    command.Parameters.AddWithValue("@Title", property.Title);
                    command.Parameters.AddWithValue("@Address", property.Address);
                    command.Parameters.AddWithValue("@Price", property.Price);
                    command.Parameters.AddWithValue("@Area", property.Area);
                    command.Parameters.AddWithValue("@Rooms", property.Rooms);
                    command.Parameters.AddWithValue("@Description", property.Description);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }
        }

        public void UpdatePropertyStatus(int propertyId, string newStatus)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE Properties SET Status = @Status WHERE Id = @PropertyId;";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Status", newStatus);
                    command.Parameters.AddWithValue("@PropertyId", propertyId);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }
        }

        public void SendMessage(Message message)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Messages (SenderId, ReceiverId, PropertyId, Content, Timestamp) VALUES (@SenderId, @ReceiverId, @PropertyId, @Content, @Timestamp);";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@SenderId", message.SenderId);
                    command.Parameters.AddWithValue("@ReceiverId", message.ReceiverId);
                    command.Parameters.AddWithValue("@PropertyId", message.PropertyId);
                    command.Parameters.AddWithValue("@Content", message.Content);
                    command.Parameters.AddWithValue("@Timestamp", message.Timestamp);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }
        }

        public List<Message> GetMessages(int userId1, int userId2, int propertyId)
        {
            var messages = new List<Message>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"SELECT MessageId, SenderId, ReceiverId, PropertyId, Content, Timestamp 
                               FROM Messages 
                               WHERE PropertyId = @PropertyId 
                                 AND ((SenderId = @UserId1 AND ReceiverId = @UserId2) OR (SenderId = @UserId2 AND ReceiverId = @UserId1))
                               ORDER BY Timestamp ASC;";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PropertyId", propertyId);
                    command.Parameters.AddWithValue("@UserId1", userId1);
                    command.Parameters.AddWithValue("@UserId2", userId2);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new Message
                            {
                                MessageId = reader.GetInt32(reader.GetOrdinal("MessageId")),
                                SenderId = reader.GetInt32(reader.GetOrdinal("SenderId")),
                                ReceiverId = reader.GetInt32(reader.GetOrdinal("ReceiverId")),
                                PropertyId = reader.GetInt32(reader.GetOrdinal("PropertyId")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
                            });
                        }
                    }
                }
            }
            return messages;
        }

        public List<Conversation> GetConversations(int userId)
        {
            var conversations = new List<Conversation>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                // This query gets the latest message for each unique conversation (PropertyId, OtherUser)
                // A conversation is defined by a unique pair of (PropertyId, OtherUser) where OtherUser is
                // either the sender or receiver, but not the current userId.
                string sql = @"
                    SELECT 
                        m.PropertyId,
                        p.Title AS PropertyTitle,
                        p.Address AS PropertyAddress,
                        CASE 
                            WHEN m.SenderId = @UserId THEN m.ReceiverId 
                            ELSE m.SenderId 
                        END AS OtherUserId,
                        u.FirstName AS OtherUserFirstName,
                        u.LastName AS OtherUserLastName,
                        MAX(m.Timestamp) AS LastMessageTimestamp
                    FROM Messages m
                    JOIN Properties p ON m.PropertyId = p.Id
                    JOIN Users u ON u.UserId = CASE WHEN m.SenderId = @UserId THEN m.ReceiverId ELSE m.SenderId END
                    WHERE m.SenderId = @UserId OR m.ReceiverId = @UserId
                    GROUP BY m.PropertyId, p.Title, p.Address, 
                             CASE WHEN m.SenderId = @UserId THEN m.ReceiverId ELSE m.SenderId END,
                             u.FirstName, u.LastName
                    ORDER BY LastMessageTimestamp DESC;
                ";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            conversations.Add(new Conversation
                            {
                                PropertyId = reader.GetInt32(reader.GetOrdinal("PropertyId")),
                                PropertyTitle = reader.GetString(reader.GetOrdinal("PropertyTitle")),
                                PropertyAddress = reader.GetString(reader.GetOrdinal("PropertyAddress")),
                                OtherUserId = reader.GetInt32(reader.GetOrdinal("OtherUserId")),
                                OtherUserFirstName = reader.GetString(reader.GetOrdinal("OtherUserFirstName")),
                                OtherUserLastName = reader.GetString(reader.GetOrdinal("OtherUserLastName")),
                                LastMessageTimestamp = reader.GetDateTime(reader.GetOrdinal("LastMessageTimestamp"))
                            });
                        }
                    }
                }
            }
            return conversations;
        }

        public User GetUserById(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT UserId, Email, FirstName, LastName, Role FROM Users WHERE UserId = @UserId;";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Role = reader.GetString(reader.GetOrdinal("Role"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Property GetPropertyById(int propertyId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, Title, Address, Price, Area, Rooms, Description, UserId, Status FROM Properties WHERE Id = @PropertyId;";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PropertyId", propertyId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Property
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Area = reader.GetDouble(reader.GetOrdinal("Area")),
                                Rooms = reader.GetInt32(reader.GetOrdinal("Rooms")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}