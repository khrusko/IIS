using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Forms;


namespace I6
{
	public partial class MainForm : Form
	{
		private int buttonSize = 150;
		private readonly HttpClient _client;

		public MainForm()
		{
			
			InitializeComponent();
			this.Width= 900;
			this.Height = 500;

			_client = new HttpClient();

			for(int i = 1; i <= 6; i++)
			{
				Button button = new Button();
				button.Name = "ButtonI" + i;
				button.Width = buttonSize;
				button.Height = 50;
				button.Left = 0;

				button.Font = new System.Drawing.Font("Arial", 10);

				switch (i)
				{
					case 1:
						button.Top = 0;
						button.Text = "I1 - Create Country XSD";
						button.Click += ButtonI1_Click;
						break;
					case 2:
						button.Top = 50;
						button.Text = "I2 - Create Country RNG";
						button.Click += ButtonI2_Click;
						break;
					case 3:
						button.Top = 100;
						button.Text = "I3 - Search countries";
						button.Click += ButtonI3_Click;
						break;
					case 4:
						button.Top = 150;
						button.Text = "I4 - Validation with JAXB and XSD";
						button.Click += ButtonI4_Click;
						break;
					case 5:
						button.Top = 200;
						button.Text = "I5 - Get current temperature by city";
						button.Click += ButtonI5_Click;
						break;
					case 6:
						button.Top = 250;
						button.Text = "I6 - Get Countries";
						button.Click += ButtonI6_Click;
						break;

				}

				this.Controls.Add(button); // Add the button to the form's controls

			}

		}

		private void ButtonI1_Click(object sender, EventArgs e)
		{
			GetI1DataAsync();
		}
		private void ButtonI2_Click(object sender, EventArgs e)
		{
			GetI2DataAsync();
		}
		private void ButtonI3_Click(object sender, EventArgs e)
		{
			GetI3DataAsync();
		}
		private void ButtonI4_Click(object sender, EventArgs e)
		{
			GetI4DataAsync();
		}
		private void ButtonI5_Click(object sender, EventArgs e)
		{
			GetI5DataAsync();
		}
		private void ButtonI6_Click(object sender, EventArgs e)
		{
			GetI6DataAsync();
		}

		private async void GetI1DataAsync()
		{
			
		}
		private async void GetI2DataAsync()
		{

		}
		private async void GetI3DataAsync()
		{

		}
		private async void GetI4DataAsync()
		{

		}
		private async void GetI5DataAsync()
		{

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

				// Create DataGridView and add it to the form
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
	}
}
