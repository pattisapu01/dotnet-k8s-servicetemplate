namespace Cloud.$ext_safeprojectname$.Domain.Interfaces
{
	public interface ISecretReader
	{
		public string GetSecret(string key);
		public string GetSecret(string key, string location);
	}
}