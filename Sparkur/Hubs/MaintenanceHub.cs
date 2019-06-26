using Hl7.Fhir.Model;
using Spark.Core;
using Spark.Engine.Core;
using Spark.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Engine.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Sparkur.Config;
using Sparkur.Utilities;
using System.Threading.Tasks;

namespace Sparkur.Hubs
{

	public class MaintenanceHub : Hub
	{
		private int _progress = 0;

		private List<Resource> _resources = null;

		private IFhirService _fhirService;
		private ILocalhost _localhost;
		private IFhirStoreAdministration _fhirStoreAdministration;
		private IFhirIndex _fhirIndex;
		private ExamplesSettings _examplesSettings;

		private int ResourceCount;

		public MaintenanceHub(IFhirService fhirService, ILocalhost localhost, IFhirStoreAdministration fhirStoreAdministration, IFhirIndex fhirIndex, ExamplesSettings examplesSettings)
		{
			_localhost = localhost;
			_fhirService = fhirService;
			_fhirStoreAdministration = fhirStoreAdministration;
			_fhirIndex = fhirIndex;
			_examplesSettings = examplesSettings;
		}

		public List<Resource> GetExampleData()
		{
			var list = new List<Resource>();

			Bundle data;
			data = FhirFileImport.ImportEmbeddedZip(_examplesSettings.FilePath).ToBundle(_localhost.DefaultBase);
			
			if (data.Entry != null && data.Entry.Count() != 0)
			{
				foreach (var entry in data.Entry)
				{
					if (entry.Resource != null)
					{
						list.Add((Resource)entry.Resource);
					}
				}
			}
			return list;
		}

		public async System.Threading.Tasks.Task SendProgressUpdate(string message, int progress)
		{
			_progress = progress;

			var msg = new ImportProgressMessage
			{
				Message = message,
				Progress = progress
			};

			await Clients.All.SendAsync("UpdateProgress", msg);
		}

		private async System.Threading.Tasks.Task Progress(string message)
		{
			await SendProgressUpdate(message, _progress);
		}

		private ImportProgressMessage Message(string message, int idx)
		{
			var msg = new ImportProgressMessage
			{
				Message = message,
				Progress = (int)10 + (idx + 1) * 90 / ResourceCount
			};
			return msg;
		}

		public async void ClearStore() {
			try {
				await SendProgressUpdate("Clearing the database...", 0);
				_fhirStoreAdministration.Clean();
				await SendProgressUpdate("Database cleared", 100);
			} 
			catch (Exception e) {
				await SendProgressUpdate("ERROR CLEARING :(", 100);
			}

		}
		public async void LoadData()
		{
			var messages = new StringBuilder();
			messages.AppendLine("Import completed!");
			try
			{
				//cleans store and index
				await SendProgressUpdate("Clearing the database...", 0);
				_fhirStoreAdministration.Clean();
				_fhirIndex.Clean();

				await SendProgressUpdate("Loading examples data...", 5);
				this._resources = GetExampleData();

				var resarray = _resources.ToArray();
				ResourceCount = resarray.Count();

				for (int x = 0; x <= ResourceCount - 1; x++)
				{
					var res = resarray[x];
					// Sending message:
					var msg = Message("Importing " + res.ResourceType.ToString() + " " + res.Id + "...", x);
					await Clients.All.SendAsync("Importing", msg);

					try
					{
						//Thread.Sleep(1000);
						Key key = res.ExtractKey();

						if (res.Id != null && res.Id != "")
						{

							_fhirService.Put(key, res);
						}
						else
						{
							_fhirService.Create(key, res);
						}
					}
					catch (Exception e)
					{
						// Sending message:
						var msgError = Message("ERROR Importing " + res.ResourceType.ToString() + " " + res.Id + "... ", x);
						await Clients.All.SendAsync("Error", msg);
						messages.AppendLine(msgError.Message + ": " + e.Message);
					}


				}

				await SendProgressUpdate(messages.ToString(), 100);
			}
			catch (Exception e)
			{
				await Progress("Error: " + e.Message);
			}
		}
		public class ImportProgressMessage
		{
			public int Progress;
			public string Message;
		}
	}

}