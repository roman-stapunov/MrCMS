using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg.Db;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    [Description("Use PostgreSql 9.5+ Database")]
    public class PostgreSqlProvider : IDatabaseProvider
    {
        private DatabaseSettings _databaseSettings;
        public PostgreSqlProvider(DatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public string Type { get { return GetType().FullName; } }
        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            var persistenceConfigurer = PostgreSQLConfiguration.Standard.ConnectionString(x => x.Is(_databaseSettings.ConnectionString));

            persistenceConfigurer.DefaultSchema("public");

            return persistenceConfigurer;
        }

        public void AddProviderSpecificConfiguration(NHibernate.Cfg.Configuration config)
        {
        }
    }
}
