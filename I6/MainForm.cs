using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace I6
{
	public partial class MainForm : Form
	{
		private int buttonSize = 150;
		private readonly HttpClient _client;

		public MainForm()
		{

			InitializeComponent();
			this.Text = "IIS-khrusko";
			this.Width = 900;
			this.Height = 485;

			_client = new HttpClient();

			for (int i = 1; i <= 6; i++)
			{
				int buttonSpacing = 74;
				Button button = new Button();
				button.Name = "ButtonI" + i;
				button.Width = buttonSize;
				button.Height = 75;
				button.Left = 0;
				button.Top = buttonSpacing * (i - 1);
				button.Font = new System.Drawing.Font("Arial", 10);

				switch (i)
				{
					case 1:
						button.Text = "I1 - Create Country XSD";
						button.Click += ButtonI1_Click;
						break;
					case 2:
						button.Text = "I2 - Create Country RNG";
						button.Click += ButtonI2_Click;
						break;
					case 3:
						button.Text = "I3 - Search countries";
						button.Click += ButtonI3_Click;
						break;
					case 4:
						button.Text = "I4 - Validation with JAXB and XSD";
						button.Click += ButtonI4_Click;
						break;
					case 5:
						button.Text = "I5 - Get current temperature by city";
						button.Click += ButtonI5_Click;
						break;
					case 6:
						button.Text = "I6 - Get Countries";
						button.Click += ButtonI6_Click;
						break;

				}
				this.Controls.Add(button); // Add the button to the form's controls
			}
		}

		private void ButtonI1_Click(object sender, EventArgs e)
		{
			ClearContent();
			GetI1DataAsync();
		}
		private void ButtonI2_Click(object sender, EventArgs e)
		{
			ClearContent();
			GetI2DataAsync();
		}
		private void ButtonI3_Click(object sender, EventArgs e)
		{
			ClearContent();
			GetI3DataAsync();
		}
		private void ButtonI4_Click(object sender, EventArgs e)
		{
			ClearContent();
			GetI4DataAsync();
		}
		private void ButtonI5_Click(object sender, EventArgs e)
		{
			ClearContent();
			GetI5DataAsync();
		}
		private void ButtonI6_Click(object sender, EventArgs e)
		{
			ClearContent();
			GetI6DataAsync();
		}

		private async void GetI1DataAsync()
		{
			//Pronalazak XSD i XML datoteke
			string path = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
			string solutionDirectoryPath = Directory.GetParent(path).Parent.Parent.Parent.FullName; // Navigates up to the solution directory
			string fixedFilePath = Path.Combine(solutionDirectoryPath, "countriesXML.xml"); // Combines the solution directory path with the filename
			string newFilePath = Path.GetTempFileName(); // Generate a unique temporary file path

			//Copy the original file into a temp one
			File.Copy(fixedFilePath, newFilePath, true);

			// Open the editor for the temp file
			Process.Start("notepad++.exe", newFilePath)?.WaitForExit();

			// Read the contents of the fixed file
			string fileContent = File.ReadAllText(newFilePath);

			// Save the modified content to the new file
			File.WriteAllText(newFilePath, fileContent);

			using (var client = new HttpClient())
			{
				var formContent = new MultipartFormDataContent();

				var fileContentTemp = new ByteArrayContent(File.ReadAllBytes(newFilePath));
				fileContentTemp.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

				// Add the new file to the POST request
				formContent.Add(fileContentTemp, "file", Path.GetFileName(newFilePath));

				// Send the POST request to the server
				var response = await client.PostAsync("http://localhost:5000/api/Country/SaveWithXSD", formContent);

				if (response.IsSuccessStatusCode)
				{
					MessageBox.Show("File successfully validated and uploaded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
				}
				else
				{
					MessageBox.Show("There was an error validating and uploading your file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
				}
			}
		}

		private async void GetI2DataAsync()
		{
			//Pronalazak XSD i XML datoteke
			string path = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
			string solutionDirectoryPath = Directory.GetParent(path).Parent.Parent.Parent.FullName; // Navigates up to the solution directory
			string fixedFilePath = Path.Combine(solutionDirectoryPath, "countriesXML.xml"); // Combines the solution directory path with the filename
			string newFilePath = Path.GetTempFileName(); // Generate a unique temporary file path

			//Copy the original file into a temp one
			File.Copy(fixedFilePath, newFilePath, true);

			// Open the editor for the temp file
			Process.Start("notepad++.exe", newFilePath)?.WaitForExit();

			// Read the contents of the fixed file
			string fileContent = File.ReadAllText(newFilePath);

			// Save the modified content to the new file
			File.WriteAllText(newFilePath, fileContent);

			using (var client = new HttpClient())
			{
				var formContent = new MultipartFormDataContent();

				var fileContentTemp = new ByteArrayContent(File.ReadAllBytes(newFilePath));
				fileContentTemp.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

				// Add the new file to the POST request
				formContent.Add(fileContentTemp, "file", Path.GetFileName(newFilePath));

				// Send the POST request to the server
				var response = await client.PostAsync("http://localhost:5000/api/Country/SaveWithRNG", formContent);

				if (response.IsSuccessStatusCode)
				{
					MessageBox.Show("File successfully validated and uploaded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
				}
				else
				{
					MessageBox.Show("There was an error validating and uploading your file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
				}
			}
		}
		private async void GetI3DataAsync()
		{
			ClearContent();
			TextBox tbCountryName = new TextBox();
			tbCountryName.Left = 360;
			tbCountryName.Top = 50;
			tbCountryName.Width = 200;
			tbCountryName.Height = 100;
			tbCountryName.Font = new System.Drawing.Font("Arial", 15);
			tbCountryName.Text = "Country name";
			tbCountryName.ForeColor = System.Drawing.Color.LightGray;
			tbCountryName.Click += (sender, e) =>
			{
				tbCountryName.Text = String.Empty;
				tbCountryName.ForeColor = System.Drawing.Color.Black;
			};
			Button btnSubmit = new Button();
			btnSubmit.Name = "SubmitButton";
			btnSubmit.Text = "Search";
			btnSubmit.Left = 600;
			btnSubmit.Top = 50;
			btnSubmit.Height = 30;
			btnSubmit.Width = 100;
			btnSubmit.Font = new System.Drawing.Font("Arial", 15);
			this.Controls.Add(tbCountryName);
			this.Controls.Add(btnSubmit);
			btnSubmit.Click += (sender, e) =>
			{
				string countryName = tbCountryName.Text;
				BtnSearch_Click(sender, e, countryName);
			};
		}

		private async void BtnSearch_Click(object sender, EventArgs e, string countryName)
		{
			ClearLabels();
			using (HttpClient client = new HttpClient())
			{
				string url = "http://localhost:60225/CountryService.asmx/GetCountryInfo";
				string requestData = $"countryName={HttpUtility.UrlEncode(countryName)}";

				try
				{
					var content = new StringContent(requestData, Encoding.UTF8, "application/x-www-form-urlencoded");
					HttpResponseMessage response = await client.PostAsync(url, content);

					if (response.IsSuccessStatusCode)
					{
						string responseContent = await response.Content.ReadAsStringAsync();

						// Extract the country information from the response XML
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(responseContent);
						string countryData = xmlDoc.InnerText;

						System.Windows.Forms.Label label = new System.Windows.Forms.Label();
						label.Width = 600;
						label.Height = 300;
						label.Top = 200;
						label.Left = 400;
						label.Font = new System.Drawing.Font("Arial", 15);
						label.Text = $"-Country information- \n{countryData}";
						label.Name = "countryLabel";
						this.Controls.Add(label);
					}
					else
					{
						System.Windows.Forms.Label label = new System.Windows.Forms.Label();
						label.Width = 600;
						label.Top = 200;
						label.Left = 280;
						label.Font = new System.Drawing.Font("Arial", 15);
						label.Text = $"The country {countryName} does not exist. Please enter a valid country!";
						label.ForeColor = System.Drawing.Color.Red;
						label.Name = "countryLabel";
						this.Controls.Add(label);
					}
				}
				catch (HttpRequestException ex)
				{
					// Handle the exception, display an error message, or perform any necessary actions
					Console.WriteLine("An error occurred while sending the request: " + ex.Message);
				}
			}
		}


		//private string ExtractCountryInfoFromXml(string xmlData)
		//{
		//	XmlDocument doc = new XmlDocument();
		//	doc.LoadXml(xmlData);

		//	XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
		//	namespaceManager.AddNamespace("ns", "http://localhost:50000/api/Country/Info");

		//	XmlNode countryNode = doc.SelectSingleNode("//ns:string", namespaceManager);
		//	if (countryNode != null)
		//	{
		//		return countryNode.InnerText;
		//	}

		//	return string.Empty;
		//}




		private async void GetI4DataAsync()
		{
			ClearLabels();
			//Pronalazak XSD i XML datoteke
			string path = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
			string solutionDirectoryPath = Directory.GetParent(path).Parent.Parent.Parent.FullName;
			string xsdPath = Path.Combine(solutionDirectoryPath, "countriesXSD.xsd");
			string countriesSearchList = Path.Combine(solutionDirectoryPath, "countriesSearchList.xml");

			if (xsdPath != null&&countriesSearchList!=null)
			{
				//Handle JAXB Validation => validate searchCountriesList.xml by XSD
				await RunJavaValidation(countriesSearchList, xsdPath);
			}
			else
			{
				System.Windows.Forms.Label label = new System.Windows.Forms.Label();
				label.Width = 600;
				label.Top = 200;
				label.Left = 280;
				label.Font = new System.Drawing.Font("Arial", 15);
				label.Text = $"The list with all the countries or the XSD file does not exist, please run I3 first!";
				label.ForeColor = System.Drawing.Color.Red;
				label.Name = "countryLabel";
				this.Controls.Add(label);
			}
		}

		private async Task RunJavaValidation(string xmlPath, string xsdPath)
		{
			string path = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
			string solutionDirectoryPath = Directory.GetParent(path).Parent.Parent.Parent.FullName; // Navigates up to the solution directory
			string jarPath = Path.Combine(solutionDirectoryPath, "XmlValidator.jar");

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = "java"; // java should be in PATH
			psi.Arguments = $"-jar \"{jarPath}\" \"{xmlPath}\" \"{xsdPath}\"";
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.UseShellExecute = false;

			Process p = Process.Start(psi);
			string stdout = await p.StandardOutput.ReadToEndAsync();
			string stderr = await p.StandardError.ReadToEndAsync();
			p.WaitForExit();

			System.Windows.Forms.Label label = new System.Windows.Forms.Label();
			label.Width = 600;
			label.Height = 300;
			label.Top = 200;
			label.Left = 280;
			label.Font = new System.Drawing.Font("Arial", 15);
			if (stderr =="")
			{
				label.Text = $"Output: {stdout}";
			}
			else
			{
				label.Text = $"Error: {stderr}";
				label.ForeColor = System.Drawing.Color.Red;
			}
			label.Name = "countryLabel";
			this.Controls.Add(label);
		}



		private async void GetI5DataAsync()
		{
			ClearContent();
			TextBox tbCityName = new TextBox();
			tbCityName.Left = 360;
			tbCityName.Top = 50;
			tbCityName.Width = 200;
			tbCityName.Height = 100;
			tbCityName.Font = new System.Drawing.Font("Arial", 15);
			tbCityName.Text = "City name";
			tbCityName.ForeColor = System.Drawing.Color.LightGray;
			tbCityName.Click += (sender, e) =>
			{
				tbCityName.ForeColor = System.Drawing.Color.Black;
				tbCityName.Text = String.Empty;
			};
			Button btnSubmit = new Button();
			btnSubmit.Name = "SubmitButton";
			btnSubmit.Text = "Submit";
			btnSubmit.Left = 600;
			btnSubmit.Top = 50;
			btnSubmit.Height = 30;
			btnSubmit.Width = 100;
			btnSubmit.Font = new System.Drawing.Font("Arial", 15);
			this.Controls.Add(tbCityName);
			this.Controls.Add(btnSubmit);
			btnSubmit.Click += (sender, e) =>
			{
				string cityName = tbCityName.Text; // Capture the current value of tbCityName.Text
				BtnSubmit_Click(sender, e, cityName);
			};
		}

		private async void BtnSubmit_Click(object sender, EventArgs e, string cityName)
		{
			ClearLabels();

			using (HttpClient client = new HttpClient())
			{
				HttpResponseMessage response = await client.GetAsync($"http://localhost:5000/api/Country/Temperature/{cityName}");

				if (response.IsSuccessStatusCode)
				{
					// Read the response content directly as a string
					string temperature = await response.Content.ReadAsStringAsync();

					System.Windows.Forms.Label label = new System.Windows.Forms.Label();
					label.Width = 600;
					label.Top = 200;
					label.Left = 280;
					label.Font = new System.Drawing.Font("Arial", 15);
					label.Text = $"The current temperature in {cityName} is {temperature}° Celsius";
					label.Name = "temperatureLabel";
					this.Controls.Add(label);
				}
				else
				{
					System.Windows.Forms.Label label = new System.Windows.Forms.Label();
					label.Width = 600;
					label.Top = 200;
					label.Left = 280;
					label.Font = new System.Drawing.Font("Arial", 15);
					label.Text = $"The city {cityName} does not exist. Please enter a valid city!";
					label.ForeColor = System.Drawing.Color.Red;
					label.Name = "temperatureLabel";
					this.Controls.Add(label);

				}
			}
		}




		private async void GetI6DataAsync()
		{
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("https://referential.p.rapidapi.com/v1/country?fields=currency%2Ccurrency_num_code%2Ccurrency_code%2Ccontinent_code%2Ccurrency%2Ciso_a3%2Cdial_code&limit=250"),
				Headers =
				{
					{ "X-RapidAPI-Key", "f12cd15c99mshdf119a7d610b7cfp10fa1fjsn2d759c4d092f" },
					{ "X-RapidAPI-Host", "referential.p.rapidapi.com" },
				},
			};

			using (var response = await _client.SendAsync(request))
			{
				response.EnsureSuccessStatusCode();
				var body = await response.Content.ReadAsStringAsync();

				// Deserialize JSON response to list of Country objects
				List<CountryRapid> countries = JsonConvert.DeserializeObject<List<CountryRapid>>(body);

				DataGridView dataGridView = new DataGridView
				{
					DataSource = countries,
					Dock = DockStyle.None,
					Top = 0,
					Left = buttonSize,
					Width = this.ClientSize.Width - buttonSize,
					Height = this.ClientSize.Height
				};

				this.Controls.Add(dataGridView);
			}
		}

		private void ClearLabels()
		{
			foreach (Control control in this.Controls)
			{
				if (control is System.Windows.Forms.Label)
				{
					control.Dispose();
					continue;
				}
			}
		}

		private void ClearContent()
		{

			Button button1 = this.Controls.OfType<Button>().FirstOrDefault(btn => btn.Name == "SubmitButton");
			System.Windows.Forms.Label label1 = this.Controls.OfType<System.Windows.Forms.Label>().FirstOrDefault(lbl => lbl.Name == "temperatureLabel");

			//Clear the Label
			foreach (Control control in this.Controls)
			{
				if (control is System.Windows.Forms.Label)
				{
					control.Dispose();
					continue;
				}
			}

			if (button1 != null)
			{
				this.Controls.Remove(button1);
				button1.Dispose();
			}

			if (label1 != null)
			{
				this.Controls.Remove(label1);
				label1.Dispose();
			}

			foreach (Control control in this.Controls)
			{
				if (control is Button)
				{
					continue;
				}
				control.Dispose();
			}
		}
	}
}
