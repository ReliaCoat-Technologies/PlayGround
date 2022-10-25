using MongoBackupDocker;

AppDomain.CurrentDomain.UnhandledException += ErrorHandlingFunctions.UnhandledExceptionTrapper;

await DashboardBackupFunctions.doDatabaseBackupAsync();

Console.ReadKey();