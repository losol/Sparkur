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
using Sparkur.Helpers;
using System.Threading.Tasks;

namespace Sparkur.Hubs
{

	public class InitializerHub : Hub
	{
		private int _progress = 0;

		private List<Resource> resources;

		private IFhirService fhirService;
		private ILocalhost localhost;
		private IFhirStoreAdministration fhirStoreAdministration;
		private IFhirIndex fhirIndex;
		private ExamplesSettings _examplesSettings;

		private int ResourceCount;

		public InitializerHub(IFhirService fhirService, ILocalhost localhost, IFhirStoreAdministration fhirStoreAdministration, IFhirIndex fhirIndex, ExamplesSettings examplesSettings)
		{
			this.localhost = localhost;
			this.fhirService = fhirService;
			this.fhirStoreAdministration = fhirStoreAdministration;
			this.fhirIndex = fhirIndex;
			this.resources = null;
			_examplesSettings = examplesSettings;
		}

		public List<Resource> GetExampleData()
		{
			var list = new List<Resource>();

			Bundle data;
			data = Examples.ImportEmbeddedZip(_examplesSettings.FilePath).ToBundle(localhost.DefaultBase);
			
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

		

		public async System.Threading.Tasks.Task Ping(string message)
        {
            await Clients.All.SendAsync("Pong", "Pong " + _examplesSettings.FilePath);
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
			SendProgressUpdate(message, _progress);
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
		public void LoadData()
		{
			var messages = new StringBuilder();
			messages.AppendLine("Import completed!");
			try
			{
				//cleans store and index
				SendProgressUpdate("Clearing the database...", 0);
				fhirStoreAdministration.Clean();
				fhirIndex.Clean();

				SendProgressUpdate("Loading examples data...", 5);
				this.resources = GetExampleData();

				var resarray = resources.ToArray();
				ResourceCount = resarray.Count();

				for (int x = 0; x <= ResourceCount - 1; x++)
				{
					var res = resarray[x];
					// Sending message:
					var msg = Message("Importing " + res.ResourceType.ToString() + " " + res.Id + "...", x);
					Clients.All.SendAsync("Importing", msg);

					try
					{
						//Thread.Sleep(1000);
						Key key = res.ExtractKey();

						if (res.Id != null && res.Id != "")
						{

							fhirService.Put(key, res);
						}
						else
						{
							fhirService.Create(key, res);
						}
					}
					catch (Exception e)
					{
						// Sending message:
						var msgError = Message("ERROR Importing " + res.ResourceType.ToString() + " " + res.Id + "... ", x);
						Clients.All.SendAsync("Error", msg);
						messages.AppendLine(msgError.Message + ": " + e.Message);
					}


				}

				SendProgressUpdate(messages.ToString(), 100);
			}
			catch (Exception e)
			{
				Progress("Error: " + e.Message);
			}
		}
		public class ImportProgressMessage
		{
			public int Progress;
			public string Message;
		}
	}

}