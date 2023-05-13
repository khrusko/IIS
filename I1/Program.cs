using CookComputing.XmlRpc;
using I1.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Xml.Linq;

namespace I1
{
	public class Program
	{

		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			// Run the application
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.Configure(app =>
					{
						app.UseRouting();

						app.UseEndpoints(endpoints =>
						{
							endpoints.MapPost("/api/Country/SaveWithXSD", async context =>
							{
								var file = context.Request.Form.Files.GetFile("file");

								// Call the ProcessXmlFile method in CountryController
								var countryController = new CountryController();
								if(file == null)
								{
									await context.Response.WriteAsync("Can't open the file, try again.");
									return;
								}
								countryController.ProcessXmlFileWithXSD(file);

								// Return the appropriate response
								context.Response.StatusCode = StatusCodes.Status200OK;
								await context.Response.WriteAsync("XML file is valid according to the provided XSD schema.");
							});

							endpoints.MapPost("/api/Country/SaveWithRNG", async context =>
							{
								var file = context.Request.Form.Files.GetFile("file");

								if (file == null)
								{
									await context.Response.WriteAsync("Can't open the file, try again.");
									return;
								}

								// Call the ProcessXmlFile method in CountryController
								var countryController = new CountryController();
								countryController.ProcessXmlFileWithRNG(file);

								// Return the appropriate response
								context.Response.StatusCode = StatusCodes.Status200OK;
								await context.Response.WriteAsync("XML file is valid according to the provided RNG schema.");
							});

							endpoints.MapGet("/api/Country/Temperature/{cityName}", async context =>
							{
								// Extract city name from the route
								var cityName = context.Request.RouteValues["cityName"].ToString();

								// Call the GetCurrentTemperature method in CountryController
								var countryController = new CountryController();
								var temperature = await countryController.GetCurrentTemperature(cityName);

								// Return the appropriate response
								context.Response.StatusCode = StatusCodes.Status200OK;
								await context.Response.WriteAsync($"{temperature}");
							});
						});
					});
				});
	}
}
