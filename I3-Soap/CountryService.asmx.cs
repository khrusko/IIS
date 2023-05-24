using System;
using System.IO;
using System.Web.Services;
using System.Xml;


namespace I3_Soap
{
	[WebService(Namespace = "http://localhost:60225/CountryService.asmx")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]

	public class CountryService : WebService
	{
		[WebMethod]
		public string GetCountryInfo(string countryName)
		{
			var solutionDirectoryPath = FindSolutionPath();
			string xmlFilePath = Path.Combine(solutionDirectoryPath, "countriesXML.xml");
			string countriesSearchList = Path.Combine(solutionDirectoryPath, "countriesSearchList.xml");

			//Copy the original xml into the new search list, if the search list already exists it is overwritten
			File.Copy(xmlFilePath, countriesSearchList, true);

			XmlDocument doc = new XmlDocument();
			doc.Load(countriesSearchList);

			//Xpath usage
			XmlNode countryNode = doc.SelectSingleNode($"/countries/country[Name='{countryName}']");
			if (countryNode != null)
			{
				string name = countryNode.SelectSingleNode("Name")?.InnerText;
				string capital = countryNode.SelectSingleNode("Capital")?.InnerText;
				string population = countryNode.SelectSingleNode("Population")?.InnerText;

				// handle null values
				name = name ?? "Unknown";
				capital = capital ?? "Unknown";
				population = population ?? "Unknown";

				return $"Country: {name} \nCapital: {capital} \nPopulation: {population}";
			}

			throw new Exception("Country not found.");
		}

		public string FindSolutionPath()
		{
			string solutionDirectoryPath = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
			for (int i = 0; i < 2; i++)
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