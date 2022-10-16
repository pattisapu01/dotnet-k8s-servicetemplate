using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Npgsql;
using Cloud.$ext_safeprojectname$.Domain.Interfaces;

namespace Cloud.$ext_safeprojectname$.Domain.Helpers
{
    /// <summary>
    /// Abstraction to help build PgSql connection strings
    /// </summary>
    public class ConnectionBuilder : IConnectionBuilder
{
	private readonly ISecretReader _secretStore;
	private readonly IConfiguration _configuration;
	private readonly ILogger<ConnectionBuilder> _logger;
	private readonly IWebHostEnvironment _env;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="secretStore"></param>
	/// <param name="configuration"></param>
	/// <param name="env"></param>
	/// <param name="logger"></param>
	/// <param name="tenantContext"></param>
	public ConnectionBuilder(ISecretReader secretStore,
		IConfiguration configuration,
		IWebHostEnvironment env,
		ILogger<ConnectionBuilder> logger)
	{
		_secretStore = secretStore;
		_configuration = configuration;
		_env = env;
		_logger = logger;
	}
	public ConnectionBuilder(ISecretReader secretStore,
		IConfiguration configuration,
		ILogger<ConnectionBuilder> logger)
	{
		_secretStore = secretStore;
		_configuration = configuration;
		_logger = logger;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="secretKeyPrefix"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public string GetDbConnectionString(string secretKeyPrefix = "")
	{
		var keyPrefix = secretKeyPrefix == string.Empty ? _configuration["SERVICE_NAME"] : secretKeyPrefix;
		var dbConnString = string.Empty;
		var key = $"{keyPrefix}-db-connection";
		dbConnString = _secretStore.GetSecret(key);


		// running in tye
		if (string.IsNullOrEmpty(dbConnString))
		{
			dbConnString = _configuration.GetConnectionString("db-store");
		}

		// handle breaking changes in Npgsql 6.0, sslmode=VerifyFull or VerifyCA instead of Require
		// TODO: remove once connection string secrets are updated when all services are at .net 6
		if (!string.IsNullOrEmpty(dbConnString) && dbConnString.Contains("sslmode=Require"))
		{
			dbConnString = dbConnString.Replace("sslmode=Require", "sslmode=VerifyFull");
		}

		return dbConnString;
	}
}
}