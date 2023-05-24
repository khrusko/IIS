using CookComputing.XmlRpc;
using I1.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace I1
{
	public class Program
	{

		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

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

								var countryController = new CountryController();
								if(file == null)
								{
									await context.Response.WriteAsync("Can't open the file, try again.");
									return;
								}
								countryController.ProcessXmlFileWithXSD(file);


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

								var countryController = new CountryController();
								countryController.ProcessXmlFileWithRNG(file);

								context.Response.StatusCode = StatusCodes.Status200OK;
								await context.Response.WriteAsync("XML file is valid according to the provided RNG schema.");
							});

							endpoints.MapGet("/api/Country/Temperature/{cityName}", async context =>
							{
								var cityName = context.Request.RouteValues["cityName"].ToString();

								var countryController = new CountryController();
								var temperature = await countryController.GetCurrentTemperature(cityName);

								context.Response.StatusCode = StatusCodes.Status200OK;
								await context.Response.WriteAsync($"{temperature}");
							});

							//endpoints.MapGet("/api/Country/Info/{countryName}", async context =>
							//{
							//	var countryName = context.Request.RouteValues["countryName"].ToString();

							//	var countryController = new CountryController();
							//	var information = await countryController.Info(countryName);

							//	context.Response.StatusCode = StatusCodes.Status200OK;
							//	await context.Response.WriteAsync($"{information}");
							//});
							
						});
					});
				});
	}
}
