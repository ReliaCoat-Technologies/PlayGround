using MongoBackupDocker;

AppDomain.CurrentDomain.UnhandledException += ErrorHandlingFunctions.UnhandledExceptionTrapper;

var session = new DashboardBackupSession();

await session.doDatabaseBackupAsync();