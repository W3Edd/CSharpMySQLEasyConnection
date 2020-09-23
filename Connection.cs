namespace PruebaMySQL.MySQLConnection {
	using System;
	using System.Collections.Generic;
	using System.Data;
	using MySql.Data.MySqlClient;
	using PruebaMySQL.Configuration;
	using W3Tools;

	public class Connection {
		private static readonly Logger Logger = new Logger();

		public Connection() {
			ConnectionConfiguration.LoadConnection(this);
		}

		public Connection(string server, string user, string database, string password) {
			this.Server = server;
			this.User = user;
			this.Database = database;
			this.Password = password;
		}

		public string Server { get; set; }
		public string User { get; set; }
		public string Database { get; set; }
		public string Password { get; set; }
		public MySqlConnection SqlConnection { get; set; }

		public void LoadConnection() {
			ConnectionConfiguration.LoadConnection(this);
		}

		public bool LoadFromFile(string path) {
			return ConnectionConfiguration.LoadFromFile(this, path);
		}

		public bool SaveToFile(string path) {
			return ConnectionConfiguration.SaveConnection(this, path);
		}

		public bool AllSet() {
			return this.Server != null &&
			       this.User != null &&
			       this.Database != null &&
			       this.Password != null;
		}

		public bool IsReady() {
			bool isReady;
			if (this.OpenConnection()) {
				isReady = true;
			} else {
				ConnectionConfiguration.LoadDefaultConnection(this);
				isReady = this.OpenConnection();
			}
			this.CloseConnection();
			return isReady;
		}

		public bool OpenConnection() {
			bool open = false;
			if (this.AllSet())
				try {
					string connectionParameters =
						$"server={this.Server};user={this.User};database={this.Database};" +
						$"port=3306;password={this.Password}";
					this.SqlConnection = new MySqlConnection(connectionParameters);
					this.SqlConnection.Open();
					open = true;
				}
				catch (Exception e) {
					Logger.Log(e);
				}
			return open;
		}

		public void CloseConnection() {
			try {
				while (this.SqlConnection.State == ConnectionState.Open) this.SqlConnection.Close();
			}
			catch (Exception e) {
				Logger.Log(e);
			}
			finally {
				this.SqlConnection.Close();
			}
		}

		public bool SendQuery(string query, string[] values) {
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (values == null) throw new ArgumentNullException(nameof(values));

			bool querySent = false;

			try {
				this.OpenConnection();
				MySqlCommand command = new MySqlCommand(query, this.SqlConnection);
				for (int i = 0; i < values.Length; i++)
					command.Parameters.AddWithValue($"@{i + 1}", values[i]);
				int status = command.ExecuteNonQuery();
				this.CloseConnection();
				querySent = status != -1;
			}
			catch (Exception e) {
				Logger.Log(e);
			}
			return querySent;
		}

		public string[][] Select(string query, string[] values) {
			List<string[]> responses = new List<string[]>();

			try {
				string[] currentRow;
				this.OpenConnection();

				MySqlCommand command = new MySqlCommand(query, this.SqlConnection);
				for (int i = 0; i < values.Length; i++)
					command.Parameters.AddWithValue($@"{i + 1}", values[i]);
				MySqlDataReader reader = command.ExecuteReader();

				int fieldCount;
				while (reader.Read()) {
					fieldCount = reader.FieldCount;
					currentRow = new string[fieldCount];
					for (int i = 0; i < fieldCount; i++) currentRow[i] = (string) reader[i];
					responses.Add(currentRow);
				}
				this.CloseConnection();
				reader.Close();
			}
			catch (Exception e) {
				Logger.Log(e);
			}
			return responses.ToArray();
		}
	}
}