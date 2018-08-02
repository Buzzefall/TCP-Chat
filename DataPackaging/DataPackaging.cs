using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataPackaging
{

    [Serializable]
    public abstract class Data : IBinarySerializable
    {
        public void SerializeBytes(Stream stream)
        {
            new BinaryFormatter().Serialize(stream, this);
        }
    }

    [Serializable]
    public class UserData : Data
    {
        //[PrimaryKey, ]
        protected string _UserID;
        protected string _Name;
        protected string _Login;
        protected string _Password;

        public string UserID
        {
            get => _UserID;

            set => _UserID = value;
        }

        public string Name
        {
            get => _Name;

            set => _Name = value;
        }

        public string Login
        {
            get => _Login;

            set => _Login = value;
        }

        public string Password
        {
            get => _Password;

            set => _Password = value;
        }

    }

    [Serializable]
    public class OnlineList : Data
    {
        private readonly List<string> _userlist;

        public int Count => _userlist.Count;

        public string this[int n]
        {
            get => _userlist[n];
            set => _userlist[n] = value;
        }

        public OnlineList(List<string> list)
        {
            _userlist = list;
        }

    }
    
    [Serializable]
    public class TextMessage : Data
    {
        protected string _Text;
        protected string _From;
        protected string _TimeStamp = DateTime.Now.ToShortTimeString();

        public string TimeStamp
        {
            get => _TimeStamp;

            set => _TimeStamp = value;
        }

        public string From
        {
            get => _From;

            set => _From = value;
        }

        public string Text
        {
            get => _Text;

            set => _Text = value;
        }
    }
}
