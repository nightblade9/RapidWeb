namespace WebApp.DataAccess.HealthCheck;

public interface IConnectionChecker
{
    Task<bool> CanConnectToDatabase();
}
