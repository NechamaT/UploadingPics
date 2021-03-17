using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace FileUploading.Data
{
    public class Image
    {
        public int Id { get; set; }
        public int View { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }

    public class Imagesdb
    {
        private readonly string _connectionString;

        public Imagesdb(string connectionString)
        {
            _connectionString = connectionString;

        }

        public void Add(Image img)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Images " +
                              " VALUES(@Views, @Password, @Name)" +
                              " SELECT SCOPE_IDENTITY()";
            conn.Open();
            cmd.Parameters.AddWithValue("@Views", img.View);
            cmd.Parameters.AddWithValue("@Password", img.Password);
            cmd.Parameters.AddWithValue("@Name", img.Name);
            img.Id = (int) (decimal) cmd.ExecuteScalar();

        }

        public Image GetImageById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images" +
                              " WHERE Id = @id";
            conn.Open();
            var image = new Image();
            cmd.Parameters.AddWithValue("@id", id);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                image.Id = (int) reader["id"];
                image.Name = (string) reader["Name"];
                image.Password = (string) reader["Password"];
                image.View = (int) reader["Views"];
            }

            return image;
        }

        public bool CheckPassword(int id, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images" +
                              " WHERE Id = @id" +
                              " AND Password = @password";
            conn.Open();
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@password", password);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }

            return false;
        }

        public void UpdateViews(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Images" +
                              " SET Views = Views +1" +
                              " WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

}