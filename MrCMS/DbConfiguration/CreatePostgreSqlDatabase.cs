using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Installation;
using MrCMS.Settings;
using Npgsql;

namespace MrCMS.DbConfiguration
{
    public class CreatePostgreSqlDatabase: ICreateDatabase<PostgreSqlProvider>
    {
        public void CreateDatabase(InstallModel model)
        {
            var connectionString = this.GetConnectionString(model);

            if (model.SqlServerCreateDatabase)
            {
                var errorCreatingDatabase = CreatePostgresqlDatabase(connectionString);
                if(!string.IsNullOrWhiteSpace(errorCreatingDatabase))
                    throw new Exception(errorCreatingDatabase);

                int tries = 0;
                while (tries < 50)
                {
                    if (NewDbIsReady(connectionString))
                        break;
                    tries++;
                    Thread.Sleep(100);
                }
            }
            else
            {
                if (!this.PostgresqlDatabaseExists(connectionString))
                    throw new Exception("Database does not exist or you don't have permissions to connect to it");
            }
        }

        public string GetConnectionString(InstallModel model)
        {
            return model.DatabaseConnectionString;
        }

        private string CreatePostgresqlDatabase(string connectionString)
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                string databaseName = builder.Database;
                builder.Database = "postgres";
                using (var conn = new NpgsqlConnection(builder.ToString()))
                {
                    conn.Open();
                    using (var command = new NpgsqlCommand(string.Format("CREATE DATABASE {0}", databaseName), conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("An error occured when creating database: {0}", ex.Message);
            }
        }

        private bool NewDbIsReady(string connectionString)
        {
            try
            {
                var provider = GetProvider(connectionString);
                new NHibernateConfigurator(provider).CreateSessionFactory();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private IDatabaseProvider GetProvider(string connectionString)
        {
            return new PostgreSqlProvider(new DatabaseSettings
            {
                ConnectionString = connectionString
            });
        }

        private bool PostgresqlDatabaseExists(string connectionString)
        {
            try
            {
                //just try to connect
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
