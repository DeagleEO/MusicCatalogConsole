using Microsoft.Data.Sqlite;
using MusicCatalogConsole.Models;
using System;
using System.Collections.Generic;

namespace MusicCatalogConsole
{
    public class DatabaseHelper
    {
        private readonly string connectionString = "Data Source=music.db";

        public DatabaseHelper()
        {
            CreateTables();
        }

        private void CreateTables()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Artists (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS Releases (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ArtistId INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Year INTEGER,
                MediaType TEXT NOT NULL,
                Description TEXT,  -- ДОБАВЬТЕ ЭТУ СТРОКУ
                FOREIGN KEY (ArtistId) REFERENCES Artists(Id)
            );
        ";
                command.ExecuteNonQuery();
            }
        }

        public void AddArtist(Artist artist)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Artists (Name) VALUES ($name)";
            command.Parameters.AddWithValue("$name", artist.Name);
            command.ExecuteNonQuery();
        }

        public List<Artist> GetAllArtists()
        {
            var artists = new List<Artist>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Artists ORDER BY Name";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                artists.Add(new Artist
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return artists;
        }

        public void AddRelease(Release release)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO Releases (ArtistId, Title, Year, MediaType, Description)
            VALUES ($artistId, $title, $year, $mediaType, $description)";
                command.Parameters.AddWithValue("$artistId", release.ArtistId);
                command.Parameters.AddWithValue("$title", release.Title);
                command.Parameters.AddWithValue("$year", release.Year);
                command.Parameters.AddWithValue("$mediaType", release.MediaType);
                command.Parameters.AddWithValue("$description", release.Description ?? "");  // ДОБАВЬТЕ
                command.ExecuteNonQuery();
            }
        }

        public List<Release> GetAllReleases()
        {
            var releases = new List<Release>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT r.Id, r.ArtistId, a.Name, r.Title, r.Year, r.MediaType
                FROM Releases r
                INNER JOIN Artists a ON r.ArtistId = a.Id
                ORDER BY a.Name, r.Year";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                releases.Add(new Release
                {
                    Id = reader.GetInt32(0),
                    ArtistId = reader.GetInt32(1),
                    ArtistName = reader.GetString(2),
                    Title = reader.GetString(3),
                    Year = reader.GetInt32(4),
                    MediaType = reader.GetString(5)
                });
            }

            return releases;
        }

        public void DeleteRelease(int releaseId)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Releases WHERE Id = $id";
            command.Parameters.AddWithValue("$id", releaseId);
            command.ExecuteNonQuery();
        }
    }
}