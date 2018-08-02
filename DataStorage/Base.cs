using System;
using System.Data.SQLite;
using System.IO;

namespace DataStorage
{
    public class DataBase : IBase, IDisposable
    {
        //[PrimaryKey]
        private readonly string dbFileLocation;
        private readonly string dbFileName;
        private readonly string dbFilePath;

        private SQLiteConnection Connection { get; set; }
        private SQLiteCommand Command { get; set; }

        public DataBase()
        {
            dbFileLocation = @"Data\";
            dbFileName = "LocalStorage.sqlite";
            dbFilePath = dbFileLocation + dbFileName;
            Connection = new SQLiteConnection();
            Command = new SQLiteCommand();
        }


        public bool Connect()
        {
            var connected = false;

            if (!File.Exists(dbFilePath)) SQLiteConnection.CreateFile(dbFilePath);
            
            try
            {
                Connection = new SQLiteConnection("Data Source=" + dbFilePath + ";Version=3;");
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return connected;
        }

        public void Dispose()
        {

        }
    }
}