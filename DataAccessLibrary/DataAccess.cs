using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Windows.Storage;
using System.IO;

namespace DataAccessLibrary
{
    public static class DataAccess
    {
        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteDatabase.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteDatabase.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS TetrisResults (Primary_Key INTEGER PRIMARY KEY, " +
                    "UserName NVARCHAR(2048) NULL," +
                    "Result INT)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        public static void AddData(string inputText, int result)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO TetrisResults VALUES (NULL, @Entry, @Result);";
                insertCommand.Parameters.AddWithValue("@Entry", inputText);
                insertCommand.Parameters.AddWithValue("@Result", result);
                insertCommand.ExecuteReader();

                db.Close();
            }

        }
        public static List<String> GetData()
        {
            List<String> entries = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteDatabase.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT * from TetrisResults ORDER BY Result DESC", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(1) + ": " + query.GetString(2));
                }

                db.Close();
            }

            return entries;
        }
    }


}
