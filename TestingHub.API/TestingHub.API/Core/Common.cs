using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestingHub.API.Core.Models;

namespace TestingHub.API.Core
{
    public class Common
    {
        public static async Task<ApiResponse> ParseHttpResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (IsJsonValid(content))
            {
                var result = JsonConvert.DeserializeObject<HTTPResponse>(content);
                return new ApiResponse
                {
                    Data = (JObject)result.Data,
                    Meta = result.Meta
                };
            }
            else
            {
                return new ApiResponse
                {
                    Data = new JObject(
                        new JProperty("text", content)
                    )
                };
            }

        }

        public static bool IsJsonValid(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;

            try
            {
                using var jsonDoc = JsonDocument.Parse(json);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public static Application GetAppSettings()
        //{
        //    string sourceAppSettingsPath = Path.Combine(GetProjectPath(), GetSetupConfig()["Config"]["AppSettings"]);

        //    if (File.Exists(sourceAppSettingsPath))
        //    {
        //        string appSettingsContent = File.ReadAllText(sourceAppSettingsPath);

        //        File.WriteAllText("appsettings.json", appSettingsContent);

        //        var configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //        .Build();

        //        return configuration.GetSection("Application").Get<Application>();
        //    }

        //    return null;

        //}

        internal static Dictionary<string, Dictionary<string, string>> GetSetupConfig()
        {
            string setUpPath = Path.Combine(GetProjectPath(), "setup.json");
            string jsonContents = File.ReadAllText(setUpPath);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContents);
        }

        public static string GetProjectPath()
        {
            //return AppDomain.CurrentDomain.BaseDirectory;

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Search for TestingHub.API.csproj files in the current directory and its parent directories
            string[] solutionFiles = Directory.GetFiles(currentDirectory, "TestingHub.csproj");

            while (solutionFiles.Length == 0)
            {
                string parentDirectory = Directory.GetParent(currentDirectory)?.FullName;

                if (parentDirectory == null || parentDirectory == currentDirectory)
                {
                    throw new InvalidOperationException("Solution file not found.");
                }

                currentDirectory = parentDirectory;
                solutionFiles = Directory.GetFiles(currentDirectory, "TestingHub.csproj");
            }

            // Return the directory containing the first TestingHub.API.csproj file found
            return Path.GetDirectoryName(solutionFiles.First());
        }

        public static Dictionary<string, TestCase> GetTestConfig()
        {
            string testConfigPath = Path.Combine(GetProjectPath(), "testCases.json");
            string jsonContents = File.ReadAllText(testConfigPath);
            return JsonConvert.DeserializeObject<Dictionary<string, TestCase>>(jsonContents);

        }
    }

}
