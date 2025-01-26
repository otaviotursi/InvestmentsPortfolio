using System.Reflection;
using Customers.Repository;
using Customers.Repository.Interface;
using FluentValidation;
using Infrastructure.Cache;
using Infrastructure.Email;
using Infrastructure.Email.Interface;
using Infrastructure.QuartzJobs;
using Infrastructure.Services;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Portfolio.Command.Handler;
using Portfolio.Repository;
using Portfolio.Repository.Interface;
using Portfolio.Service.Kafka;
using Products.Command;
using Products.Command.Handler;
using Products.Query.Handler;
using Products.Repository;
using Products.Repository.Interface;
using Products.Service.Kafka;
using Quartz;
using Statement.Command;
using Statement.Command.Handler;
using Statement.Repository;
using Statement.Repository.Interface;
using Statement.Service.Kafka;
using Users.Repository;
using Users.Repository.Interface;
using KafkaConfig = Infrastructure.Kafka.KafkaConfig;

namespace Investments
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddDependencies(services, configuration);
            AddCronJob(services, configuration);
            AddRedisCache(services, configuration);
            AddMongoDB(services, configuration);
            AddMediatR(services, configuration);
            AddRepositories(services, configuration);
            AddServices(services, configuration);



        }

        private static void AddCronJob(IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
            });
            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });
            services.ConfigureOptions<ProductExpirationBackgroundJobSetup>();

        }

        private static void AddDependencies(IServiceCollection services, IConfiguration configuration)
        {

            //services.AddTransient<IRequestHandler<InsertProductCommand, string>, InsertProductCommandHandler>();

            //services.AddTransient<INotificationHandler<CreateProductEvent>, CreateProductEventHandler>();

            services.AddScoped<GetPortfolioStatementByCustomerQueryHandler>();
            services.AddScoped<GetAllProductQueryHandler>();
            services.AddScoped<GetProductByQueryHandler>();
            services.AddScoped<GetStatementByProductQueryHandler>();
            services.AddScoped<GetPortfolioAllCustomersQueryHandler>();
            services.AddScoped<GetPortfolioByCustomerQueryHandler>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        }

        private static void AddServices(IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<KafkaConfig>(configuration.GetSection("Kafka"));
            services.AddHostedService<StatementKafkaConsumerService>();
            services.AddHostedService<ProductKafkaConsumerService>();
            services.AddHostedService<PortfolioKafkaConsumerService>();
            services.AddScoped<IKafkaProducerService, KafkaPublisherService>();
            services.Configure<EmailConfig>(configuration.GetSection("EmailConfig"));

        }

        private static void AddRepositories(IServiceCollection services, IConfiguration configuration)
        {
            // Registrar o MongoClient
            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(configuration.GetConnectionString("DefaultConnection")));



            // Registrar o ProductReadRepository com os valores diretamente
            services.AddSingleton<IProductRepository>(sp =>
                new ProductRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("ProductsCollectionName")  // Nome correto da coleção
                ));

            // Registrar o ProductWriteRepository com os valores diretamente
            services.AddScoped<IProductStatementRepository>(sp =>
                new ProductStatementRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("ProductsStatementCollectionName")  // Nome correto da coleção
                ));


            // Registrar o CustomerRepository com os valores diretamente
            services.AddScoped<ICustomerRepository>(sp =>
                new CustomerRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("CustomerCollectionName")  // Nome correto da coleção
                ));

            // Registrar o UserRepository com os valores diretamente
            services.AddScoped<IUserRepository>(sp =>
                new UserRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("UserCollectionName")  // Nome correto da coleção
                ));


            // Registrar o PortfolioRepository com os valores diretamente
            services.AddScoped<IPortfolioRepository>(sp =>
                new PortfolioRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("PortfolioCollectionName")  // Nome correto da coleção
                ));

            // Registrar o PortfolioRepository com os valores diretamente
            services.AddScoped<IPortfolioStatementRepository>(sp =>
                new PortfolioStatementRepository(
                    sp.GetRequiredService<IMongoClient>(),
                    configuration.GetConnectionString("DefaultDatabase"),  // Nome correto do banco de dados
                    configuration.GetConnectionString("PortfolioStatementCollectionName")  // Nome correto da coleção
                ));
            
        }

        private static void AddMediatR(IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services
                .AddValidatorsFromAssembly(assembly)
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));


        }
        private static void AddMongoDB(IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetConnectionString("MongoDb");
            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
        }

        private static void AddRedisCache(IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.InstanceName = configuration.GetSection("Regis:InstanceName")?.Value;
                opt.Configuration = configuration.GetSection("Regis:Ip")?.Value;
            });
            services.AddScoped<ICacheHelper, CacheHelper>();
        }
    }
}
