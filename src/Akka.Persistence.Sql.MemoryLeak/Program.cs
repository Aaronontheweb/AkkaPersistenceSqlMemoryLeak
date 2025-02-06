using Akka;
using Akka.Hosting;
using Akka.Persistence.Query;
using Akka.Persistence.Sql.Hosting;
using Akka.Persistence.Sql.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Debug = System.Diagnostics.Debug;

var hostBuilder = new HostBuilder();

hostBuilder.ConfigureAppConfiguration((context, builder) =>
{
    builder.AddJsonFile("appsettings.json", optional: false);
    builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
    builder.AddEnvironmentVariables();
});

hostBuilder.ConfigureServices((context, services) =>
{
    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
    Debug.Assert(connectionString != null, "connectionString != null");
    
    services.AddAkka("ReproductionSystem", (builder, sp) =>
    {
        builder
            .WithSqlPersistence(options =>
            {
                options.ConnectionString = connectionString;
                options.ProviderName = ProviderName.SqlServer;
            })
            .AddStartup(async (system, registry) =>
            {
                var readJournal = PersistenceQuery.Get(system)
                    .ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);

                Source<EventEnvelope, NotUsed> source = readJournal
                    .AllEvents(Offset.NoOffset());

                var mat = ActorMaterializer.Create(system);
                await source.RunForeach(_ => { Console.WriteLine($"Event has been read from stream at {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.fff tt}"); }, mat);
            });
    });
});

var host = hostBuilder.Build();

await host.RunAsync();