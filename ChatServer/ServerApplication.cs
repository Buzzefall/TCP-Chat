namespace ChatServer
{
    public class ServerApplication
    {
        public static void Main(string[] args)
        {
            var server = new Server();
            server.Start();

            //while (true)
            //{
            //    Server.UsersOnline();
            //    Thread.Sleep(5000);
            //}
        }

    }
}
