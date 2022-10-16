using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using Cloud.$ext_safeprojectname$.Domain.Interfaces;

namespace Cloud.$ext_safeprojectname$.Domain.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class SecretReader : ISecretReader
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger = Log.ForContext<SecretReader>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public SecretReader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public string GetSecret(string key, string location)
        {
            return GetFromEnv(key) ?? GetSecretFromFile(key, location);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetSecret(string key)
        {
            return GetSecret(key, null);
        }

        private static string GetFromEnv(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="storeLocation"></param>
        /// <returns></returns>
        private string GetSecretFromFile(string key, string storeLocation)
        {
            if (string.IsNullOrEmpty(storeLocation))
            {
                storeLocation = _configuration["secret-store-location"] ?? _configuration["SECRET_STORE_LOCATION"] ?? string.Empty;
            }

            var filename = storeLocation.Contains("\\")
                ? $"{storeLocation}\\{key}"
                : $"{storeLocation}/{key}";

            if (File.Exists(filename))
            {
                return File.ReadAllText(filename);
            }

            _logger.Debug("The File {Filename} is NOT found! App may not function correctly", filename);
            return string.Empty;
        }
    }

}