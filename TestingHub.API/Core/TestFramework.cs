using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingHub.API.Core.Models;

namespace TestingHub.API.Core
{

    public class TestFramework<TStartup> where TStartup : class
    {
        public static TestApplication<TStartup> application { get; set; }
        public static HttpClient client { get; set; }
        public static Dictionary<string, TestCase> testCases { get; set; }

        static TestFramework()
        {
            application = new TestApplication<TStartup>();
            client = application.CreateClient();
            testCases = Common.GetTestConfig();

            var setUpConfig = Common.GetSetupConfig();

            foreach (var cookie in setUpConfig["Cookies"])
            {
                client.DefaultRequestHeaders.Add("Cookie", $"{cookie.Key}={cookie.Value}");
            }

            foreach (var header in setUpConfig["Headers"])
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        public void GenerateTestCase()
        {

            string inputFilePath = Path.Combine(Common.GetProjectPath(), "swagger.json");
            string outputFilePath = Path.Combine(Common.GetProjectPath(), $"testCases_{DateTime.Now.ToString("yyyy-MM-dd")}.json");

            if (!File.Exists(inputFilePath))
                throw new Exception($"Please configure swagger.json.");

            string inputJson = System.IO.File.ReadAllText(inputFilePath);
            JObject jsonObject = JObject.Parse(inputJson);

            Dictionary<string, Dictionary<string, object>> apiData = new Dictionary<string, Dictionary<string, object>>();

            var paths = jsonObject["paths"];

            foreach (var path in paths)
            {
                foreach (var method in path.First.Children<JProperty>())
                {
                    string apiUrl = path.Path.Replace("paths['", "").Replace("']", "");
                    Console.WriteLine(apiUrl);
                    string methodType = method.Name.ToUpper();

                    Dictionary<string, object> queryParams = new Dictionary<string, object>();

                    // Extract query params
                    if (method.Value["parameters"] != null)
                    {
                        foreach (var parameter in method.Value["parameters"])
                        {
                            string paramName = parameter["name"].ToString();
                            var schema = parameter["schema"];
                            if (schema != null)
                            {
                                string schemaType = schema["type"]?.ToString();

                                if (!string.IsNullOrEmpty(schemaType))
                                {
                                    queryParams[paramName] = GetDefaultValueForType(schemaType);
                                }
                            }

                        }
                    }

                    // Extracting body
                    Dictionary<string, object> bodyJson = new Dictionary<string, object>();
                    var requestBody = method.Value["requestBody"];
                    if (requestBody != null)
                    {
                        var schemaRef = requestBody["content"]["application/json"]["schema"]["$ref"].ToString();

                        var schemaParts = schemaRef.Split('/');
                        string schemaName = schemaParts[^1];

                        var schemas = jsonObject["components"]["schemas"];
                        var referencedSchema = schemas[schemaName]["properties"];

                        try
                        {

                            bodyJson = new Dictionary<string, object>();
                            foreach (JProperty prop in referencedSchema)
                            {
                                string propName = prop.Name;
                                if (prop.Value["type"] != null)
                                {
                                    string propType = prop.Value["type"].ToString();

                                    object defaultValue = GetDefaultValueForType(propType);
                                    bodyJson[propName] = defaultValue;
                                }

                                // For enums
                                else if (prop.Value["$ref"] != null)
                                {
                                    schemaParts = prop.Value["$ref"].ToString().Split('/');
                                    schemaName = schemaParts[^1];
                                    referencedSchema = schemas[schemaName];

                                    if (referencedSchema != null)
                                    {
                                        string propType = referencedSchema["type"]?.ToString();

                                        if (!string.IsNullOrEmpty(propType))
                                        {
                                            object defaultValue = GetDefaultValueForType(propType);
                                            bodyJson[propName] = defaultValue;
                                        }
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    Dictionary<string, object> apiInfo = new Dictionary<string, object>
                        {
                            { "bodyJson", bodyJson },
                            { "queryParams", queryParams },
                            { "method", methodType },
                            { "apiUrl", apiUrl }
                        };


                    var methodTestConditions = new List<string> {
                            "ShouldReturnSuccess_WhenValidInput",
                            "ShouldReturnFailed_WhenInvalidInput"
                        };

                    bool isInput = ((bodyJson.Count() != 0 || queryParams.Count() != 0) ? true : false);
                    string testName = null;

                    if (isInput)
                    {
                        foreach (string testCondition in methodTestConditions)
                        {
                            testName = apiUrl.Split('/').Last().ToString() + "_" + testCondition;
                            apiData[testName] = apiInfo;
                        }
                    }
                    else
                    {
                        testName = apiUrl.Split('/').Last().ToString();
                        apiData[testName] = apiInfo;
                    }

                }

            }

            string outputJson = Newtonsoft.Json.JsonConvert.SerializeObject(apiData, Newtonsoft.Json.Formatting.Indented);

            System.IO.File.WriteAllText(outputFilePath, outputJson);

            Console.WriteLine("Output saved to " + outputFilePath);

            Process.Start("notepad.exe", outputFilePath);

            Environment.Exit(0);

            static object GetDefaultValueForType(string type)
            {
                switch (type)
                {
                    case "integer":
                        return 0;
                    case "string":
                        return "string";
                    case "boolean":
                        return false;
                    case "array":
                        return new object[0];

                    // Add more cases for other types as needed
                    default:
                        return null;
                }
            }
        }



    }

}
