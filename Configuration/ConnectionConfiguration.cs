namespace MySQLEasyConnection.Configuration {
	using MySQLEasyConnection;
	using W3Tools;

	public abstract class ConnectionConfiguration {
		public static File GetConnectionConfigFile() {
			return new File("Configuration/ConnectionFiles/Connection.w3");
		}

		public static File GetDefaultConnectionConfigFile() {
			return new File("Configuration/ConnectionFiles/DefaultConnection.w3");
		}

		public static bool SaveConnection(Connection connection,
		                                  string path = "Configuration/Connection") {
			bool saved = false;
			if (connection != null && connection.AllSet()) {
				File saveFile = new File(path);
				saveFile.Delete();
				saveFile.WriteLine(connection.Server);
				saveFile.WriteLine(connection.User);
				saveFile.WriteLine(connection.Database);
				saveFile.WriteLine(connection.Password);
				saved = true;
			}
			return saved;
		}

		public static void LoadConnection(Connection connection, string path = null) {
			File savedConnection;
			if (path != null && File.Exists(path))
				savedConnection = new File(path);
			else if (GetConnectionConfigFile().Exists())
				savedConnection = GetConnectionConfigFile();
			else
				savedConnection = GetDefaultConnectionConfigFile();

			if (savedConnection.GetSizeInLines() == 4) {
				W3Parser connectionConfiguration = new W3Parser(savedConnection.Path);
				connection.Server = connectionConfiguration.Get("server")[0];
				connection.User = connectionConfiguration.Get("user")[0];
				connection.Database = connectionConfiguration.Get("database")[0];
				connection.Password = connectionConfiguration.Get("password")[0];
			}
		}

		public static bool LoadFromFile(Connection connection, string path) {
			bool loaded = false;
			if (path != null && File.Exists(path) && File.GetSizeInLines(path) == 4) {
				W3Parser connectionConfiguration = new W3Parser(path);
				connection.Server = connectionConfiguration.Get("server")[0];
				connection.User = connectionConfiguration.Get("user")[0];
				connection.Database = connectionConfiguration.Get("database")[0];
				connection.Password = connectionConfiguration.Get("password")[0];
				loaded = true;
			}
			return loaded;
		}

		public static void LoadPrimaryConnection(Connection connection) {
			LoadConnection(connection, GetConnectionConfigFile().Path);
		}

		public static void LoadDefaultConnection(Connection connection) {
			LoadConnection(connection, GetDefaultConnectionConfigFile().Path);
		}
	}
}