using Commons.Xml.Relaxng;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;


namespace I1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountryController : ControllerBase
	{

		public bool ProcessXmlFileWithXSD(IFormFile file)
		{
			// Spremanje datoteke na privremeno mjesto
			var filePath = Path.GetTempFileName();
			using (var stream = System.IO.File.Create(filePath))
			{
				file.CopyTo(stream);
			}

			var solutionDirectoryPath = FindSolutionPath();
			string xmlFilePath = Path.Combine(solutionDirectoryPath, "countriesXML.xml");
			string xsdFilePath = Path.Combine(solutionDirectoryPath, "countriesXSD.xsd");

			// Učitavanje XSD datoteke
			XmlSchemaSet schemas = new XmlSchemaSet();
			schemas.Add("", XmlReader.Create(xsdFilePath));

			// Učitavanje XML datoteke
			XmlDocument doc = new XmlDocument();
			doc.Load(file.OpenReadStream());

			// Validacija
			string msg = "";
			doc.Schemas = schemas;
			doc.Validate((sender, args) =>
			{
				msg = args.Message;
			});

			if (string.IsNullOrEmpty(msg))
			{
				doc.Save(xmlFilePath);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ProcessXmlFileWithRNG(IFormFile file)
		{
			// Saving the file to a temporary location
			var filePath = Path.GetTempFileName();
			using (var stream = System.IO.File.Create(filePath))
			{
				file.CopyTo(stream);
			}

			var solutionDirectoryPath = FindSolutionPath();
			string xmlFilePath = Path.Combine(solutionDirectoryPath, "countriesXML.xml");
			string rngFilePath = Path.Combine(solutionDirectoryPath, "countriesRNG.rng");

			// Loading the RNG file
			RelaxngPattern rng;
			using (var rngReader = XmlReader.Create(rngFilePath))
			{
				rng = RelaxngPattern.Read(rngReader);
			}

			// Loading the XML file
			XmlDocument doc = new XmlDocument();
			doc.Load(file.OpenReadStream());

			// Validation
			string msg = "";
			using (var xmlReader = new RelaxngValidatingReader(new XmlTextReader(xmlFilePath), rng))
			{
				try
				{
					while (xmlReader.Read()) { } // Exception if invalid
				}
				catch (Exception ex)
				{
					msg = ex.Message;
				}
			}

			if (string.IsNullOrEmpty(msg))
			{
				doc.Save(xmlFilePath);
				return true;
			}
			else
			{
				return false;
			}
		}

		[HttpPost(Name = "SaveWithXSD")]
		public IActionResult SaveWithXSD(IFormFile file)
		{
			try
			{
				bool isValid = ProcessXmlFileWithXSD(file);

				if (isValid)
				{
					return Ok("XML file is valid according to the provided XSD schema.");
				}
				else
				{
					return BadRequest("XML file is not valid according to the provided XSD schema.");
				}
			}
			catch (Exception ex)
			{
				return BadRequest("Error: " + ex.Message);
			}
		}

		[HttpPost(Name = "SaveWithRNG")]
		public IActionResult SaveWithRNG(IFormFile file)
		{
			try
			{
				bool isValid = ProcessXmlFileWithRNG(file);

				if (isValid)
				{
					return Ok("XML file is valid according to the provided RNG schema.");
				}
				else
				{
					return BadRequest("XML file is not valid according to the provided RNG schema.");
				}
			}
			catch (Exception ex)
			{
				return BadRequest("Error: " + ex.Message);
			}
		}

		[HttpGet("Temperature/{cityName}")]
		public async Task<string> GetCurrentTemperature(string cityName)
		{
			try
			{
				using (HttpClient client = new HttpClient())
				{
					string xmlContent = await client.GetStringAsync("https://vrijeme.hr/hrvatska_n.xml");
					XDocument xmlDoc = XDocument.Parse(xmlContent);

					var cityTemperature = xmlDoc.Descendants("Grad")
						.FirstOrDefault(g => (string)g.Element("GradIme") == cityName)?
						.Element("Podatci")?
						.Element("Temp")?.Value;

					if (cityTemperature != null)
					{
						return cityTemperature.ToString();
					}
					else
					{
						throw new Exception($"Temperature for city {cityName} not found");
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error: " + ex.Message);
			}
		}

		//I3-Rest
		//[HttpGet("Info/{countryName}")]
		//public async Task<string> Info(string countryName)
		//{
		//	var solutionDirectoryPath = FindSolutionPath();
		//	string xmlFilePath = Path.Combine(solutionDirectoryPath, "countriesXML.xml");
		//	string countriesSearchList = Path.Combine(solutionDirectoryPath, "countriesSearchList.xml");

		//	//Copy the original xml into the new search list, if the search list already exists it is overwritten
		//	System.IO.File.Copy(xmlFilePath, countriesSearchList, true);

		//	XmlDocument doc = new XmlDocument();
		//	await Task.Run(() => doc.Load(countriesSearchList));

		//	//Xpath usage
		//	XmlNode countryNode = doc.SelectSingleNode($"/countries/country[Name='{countryName}']");
		//	if (countryNode != null)
		//	{
		//		string name = countryNode.SelectSingleNode("Name")?.InnerText;
		//		string capital = countryNode.SelectSingleNode("Capital")?.InnerText;
		//		string population = countryNode.SelectSingleNode("Population")?.InnerText;

		//		// handle null values
		//		name = name ?? "Unknown";
		//		capital = capital ?? "Unknown";
		//		population = population ?? "Unknown";

		//		return $"Country: {name} \nCapital: {capital} \nPopulation: {population}";
		//	}

		//	throw new Exception("Country not found.");
		//}

		public string FindSolutionPath()
		{
			string solutionDirectoryPath = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
			for (int i = 0; i < 5; i++)
			{
				var directoryInfo = Directory.GetParent(solutionDirectoryPath);
				if (directoryInfo != null)
				{
					solutionDirectoryPath = directoryInfo.FullName;
				}
				else
				{
					throw new Exception($"Could not find parent directory at level {i}");
				}
			}
			return solutionDirectoryPath;
		}

	}
}
