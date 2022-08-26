using MySql.Data.MySqlClient;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;

namespace Hyperdrive.Core.Networking
{
    public class DBConnection
    {
		public static void TestConnection3()
        {
			var server = "server139.web-hosting.com";
			var sshPort = 21098;
			var sshUserName = "dispadna";
			var sshPassword = "MKLbpy7?!?!";
			var databaseUserName = "dispadna_hyperdrivepdf";
			var databasePassword = "MKLbpy7?!";

			try
			{
				using (var client = new SshClient(server, sshPort, sshUserName, sshPassword)) // establishing ssh connection to server where MySql is hosted
				{
					client.Connect();
					if (client.IsConnected)
					{
						var portForwarded = new ForwardedPortLocal("127.0.0.1", 3306, "127.0.0.1", 3306);
						client.AddForwardedPort(portForwarded);
						portForwarded.Start();
						using (MySqlConnection con = new MySqlConnection("SERVER=127.0.0.1;PORT=3306;UID=dispadna_hyperdrivepdf;PASSWORD=MKLbpy7?!;DATABASE=dispadna_hyperdrive_users"))
						{
							using (MySqlCommand com = new MySqlCommand("SELECT * FROM TestUsers1", con))
							{
								com.CommandType = CommandType.Text;
								DataSet ds = new DataSet();
								MySqlDataAdapter da = new MySqlDataAdapter(com);
								da.Fill(ds);
								foreach (DataRow drow in ds.Tables[0].Rows)
								{
									Console.WriteLine("From MySql: " + drow[1].ToString());
								}
							}
						}
						client.Disconnect();
					}
					else
					{
						Console.WriteLine("Client cannot be reached...");
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("CONNECTION ERROR: " + e.Message);
			}

		}


		public static void TestConnection2()
        {
			var server = "server139.web-hosting.com";
			var sshUserName = "dispadna";
			var sshPassword = "MKLbpy7?!?!";
			var databaseUserName = "dispadna_hyperdrivepdf";
			var databasePassword = "MKLbpy7?!";

			PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(server, sshUserName, sshPassword);
			connectionInfo.Timeout = TimeSpan.FromSeconds(30);
			var client = new SshClient(connectionInfo);
			client.Connect();
			ForwardedPortLocal portFwld = new ForwardedPortLocal("127.0.0.1", Convert.ToUInt32(21098), server, Convert.ToUInt32(3306)); client.AddForwardedPort(portFwld);
			portFwld.Start();

			var connection = new MySqlConnection("server = " + "127.0.0.1" + "; Database = database; password = PWD; UID = yourname; Port = 22");
			connection.Open();
		}

		public static void TestConnection()
        {
			var server = "server139.web-hosting.com";
			var sshUserName = "dispadna";
			var sshPassword = "MKLbpy7?!?!";
			var databaseUserName = "dispadna_hyperdrivepdf";
			var databasePassword = "MKLbpy7?!";

			var (sshClient, localPort) = ConnectSsh(server, sshUserName, sshPassword);
			using (sshClient)
			{
				MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder
				{
					Server = "127.0.0.1",
					Port = localPort,
					UserID = databaseUserName,
					Password = databasePassword,
				};

				var connection = new MySqlConnection(csb.ConnectionString);
				connection.Open();
			}
		}

        public static (SshClient SshClient, uint Port) ConnectSsh(string sshHostName, string sshUserName, string sshPassword = null, string sshKeyFile = null, string sshPassPhrase = null, int sshPort = 21098, string databaseServer = "localhost", int databasePort = 3306)
        {
            // check arguments
            if (string.IsNullOrEmpty(sshHostName))
                throw new ArgumentException($"{nameof(sshHostName)} must be specified.", nameof(sshHostName));
            if (string.IsNullOrEmpty(sshHostName))
                throw new ArgumentException($"{nameof(sshUserName)} must be specified.", nameof(sshUserName));
            if (string.IsNullOrEmpty(sshPassword) && string.IsNullOrEmpty(sshKeyFile))
                throw new ArgumentException($"One of {nameof(sshPassword)} and {nameof(sshKeyFile)} must be specified.");
            if (string.IsNullOrEmpty(databaseServer))
                throw new ArgumentException($"{nameof(databaseServer)} must be specified.", nameof(databaseServer));

            // define the authentication methods to use (in order)
            var authenticationMethods = new List<AuthenticationMethod>();
            if (!string.IsNullOrEmpty(sshKeyFile))
			{
				authenticationMethods.Add(new PrivateKeyAuthenticationMethod(sshUserName,
					new PrivateKeyFile(sshKeyFile, string.IsNullOrEmpty(sshPassPhrase) ? null : sshPassPhrase)));
			}
			if (!string.IsNullOrEmpty(sshPassword))
			{
				authenticationMethods.Add(new PasswordAuthenticationMethod(sshUserName, sshPassword));
			}
		
			// connect to the SSH server
			var sshClient = new SshClient(new ConnectionInfo(sshHostName, sshPort, sshUserName, authenticationMethods.ToArray()));
			sshClient.Connect();
		
			// forward a local port to the database server and port, using the SSH server
			var forwardedPort = new ForwardedPortLocal("127.0.0.1", databaseServer, (uint)databasePort);
			sshClient.AddForwardedPort(forwardedPort);
			forwardedPort.Start();
		
			return (sshClient, forwardedPort.BoundPort);
		}
	}
}