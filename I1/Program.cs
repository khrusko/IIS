using I1.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;

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
								countryController.ProcessXmlFile(file);

								// Return the appropriate response
								context.Response.StatusCode = StatusCodes.Status200OK;
								await context.Response.WriteAsync("XML file is valid according to the provided XSD schema.");
							});
						});
					});
				});
	}
}
