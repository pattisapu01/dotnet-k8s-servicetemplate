namespace Cloud.$ext_safeprojectname$.Domain.Interfaces
{
    public interface IConnectionBuilder
    {
        string GetDbConnectionString(string secretKeyPrefix = "");
    }
}