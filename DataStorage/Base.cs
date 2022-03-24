using System;
//using System.Data.SQLite;
using System.IO;

namespace DataStorage
{
    public class DataBase : IBase, IDisposable
    {
        //[PrimaryKey]
        private readonly string _dbFileLocation;
        private readonly string _dbFileName;
        private readonly string _dbFilePath;

        //private SQLiteConnection Connection { get; set; }
        //private SQLiteCommand Command { get; set; }

        ~DataBase()
        {
            Dispose();
        }

        public DataBase()
        {
            //dbFileLocation = @"Data\";
            //dbFileName = "LocalStorage.sqlite";
            //dbFilePath = dbFileLocation + dbFileName;
            //Connection = new SQLiteConnection();
            //Command = new SQLiteCommand();
        }


        public bool Connect()
        {
            var connected = false;

            //if (!File.Exists(dbFilePath)) SQLiteConnection.CreateFile(dbFilePath);
            
            //try
            //{
            //    Connection = new SQLiteConnection("Data Source=" + dbFilePath + ";Version=3;");
            //}

            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}


            return connected;
        }

        public void Dispose()
        {

        }
    }
}