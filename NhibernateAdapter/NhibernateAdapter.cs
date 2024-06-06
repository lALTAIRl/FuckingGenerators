using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Extensions.NpgSql;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using GeneratedClasses;
using GeneratedMappings;

namespace NhibernateAdapter
{
    public static class NhibernateAdapter
    {
        public static Configuration _NHConfiguration;
        public static ISessionFactory _sessionFactory;
        
        public static void Test()
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();

            var records = session.Query<Animal>().ToList();

            foreach (var record in records)
            {
                Console.WriteLine(record.Id);
            }
        }

        public static Configuration ConfigureNHibernate()
        {
            var config = new Configuration();
            config.SessionFactoryName("test");

            config.DataBaseIntegration(db =>
            {
                db.Dialect<PostgreSQLDialect>();
                db.Driver<NpgSqlDriver>();
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = System.Data.IsolationLevel.ReadCommitted;
                db.ConnectionString = "Host=localhost;Port=5432;Database=pgtestdata;Username=postgres;Password=password";
                db.Timeout = 10;
            });

            return config;
        }

        public static HbmMapping GetMappings()
        {
            ModelMapper mapper = new ModelMapper();

            mapper.AddMapping<AnimalMap>();
            mapper.AddMapping<ApplicationMap>();
            mapper.AddMapping<BuildingMap>();
            mapper.AddMapping<ImageMap>();

            HbmMapping mapping = mapper.CompileMappingFor(new[]
            {
                typeof(Animal),
                typeof(Application),
                typeof(Building),
                typeof(Image),
            });
            return mapping;
        }

        public static void SetupNhibernate()
        {
            _NHConfiguration = ConfigureNHibernate();
            var mapping = GetMappings();

            _NHConfiguration.AddDeserializedMapping(mapping, "NHSchemaTest");

            SchemaMetadataUpdater.QuoteTableAndColumns(_NHConfiguration);
            _sessionFactory = _NHConfiguration.BuildSessionFactory();
        }

        public static void CreateDatabaseSchema()
        {
            new SchemaExport(_NHConfiguration).Drop(false, true);
            new SchemaExport(_NHConfiguration).Create(false, true);
        }

        public static bool ValidateSchema()
        {
            try
            {
                var validator = new SchemaValidator(_NHConfiguration);
                validator.Validate();
                Console.WriteLine("SCHEMA VALID!");
                return true;
            }
            catch (HibernateException e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public static void PopulateTestData()
        {
            var animals = new List<Animal>()
            {
                new Animal { Id=1 },
                new Animal { Id=2 },
                new Animal { Id=3 },
                new Animal { Id=4 },
            };

            var applications = new List<Application>()
            {
                new Application { Id=1 },
                new Application { Id=2 },
            };

            var buildings = new List<Building>()
            {
                new Building { Id=1 },
            };

            var images = new List<Image>()
            {
                new Image { Id=1 },
                new Image { Id=2 },
            };

            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();

            foreach (var animal in animals)
            {
                session.SaveOrUpdate(animal);
            }

            foreach (var application in applications)
            {
                session.SaveOrUpdate(application);
            }

            foreach (var building in buildings)
            {
                session.SaveOrUpdate(building);
            }

            foreach (var image in images)
            {
                session.SaveOrUpdate(image);
            }

            transaction.Commit();
        }
    }
}