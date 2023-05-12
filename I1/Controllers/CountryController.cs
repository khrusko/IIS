using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Xml.Schema;
using System.Xml;

namespace I1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountryController : ControllerBase
	{
		[HttpPost(Name = "SaveWithXSD")]
		public IActionResult SaveWithXSD(IFormFile file)
		{
			try
			{
				// Spremanje datoteke na privremeno mjesto
				var filePath = Path.GetTempFileName();
				using (var stream = System.IO.File.Create(filePath))
				{
					file.CopyTo(stream);
				}

				//Pronalazak XSD i XML datoteke
				string path = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
				string solutionDirectoryPath = Directory.GetParent(path).Parent.Parent.FullName; // Navigates up to the solution directory
				string xmlFilePath = Path.Combine(solutionDirectoryPath, "countriesXML.xml"); // Combines the solution directory path with the filename
				string xsdFilePath = Path.Combine(solutionDirectoryPath, "countriesXSD.xsd"); // Combines the solution directory path with the filename

				// Učitavanje XSD datoteke
				XmlSchemaSet schemas = new XmlSchemaSet();
				schemas.Add("", XmlReader.Create(xsdFilePath));

				// Učitavanje XML datoteke
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlFilePath);

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
					return Ok("XML file is valid according to the provided XSD schema.");
				}
				else
				{
					return BadRequest("XML file is not valid according to the provided XSD schema. Error: " + msg);
				}
			}
			catch (Exception ex)
			{
				return BadRequest("Error: " + ex.Message);
			}
		}
	}
}
