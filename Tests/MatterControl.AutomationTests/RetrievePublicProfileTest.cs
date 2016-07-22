﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MatterHackers.MatterControl.VersionManagement;
using System.IO;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.SettingsManagement;
using System.Threading;
using MatterHackers.MatterControl.Tests.Automation;
using MatterHackers.Agg.UI.Tests;
using MatterHackers.GuiAutomation;
using MatterHackers.Agg.UI;
using Newtonsoft.Json;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.MatterControl;

namespace MatterControl.Tests.MatterControl
{
	using OemProfileDictionary = Dictionary<string, Dictionary<string, string>>;

	[TestFixture]
	public class RetrievePublicProfileTest
	{
		private string deviceToken = null;

		[Test]
		public async void GetPublicProfileList()
		{
			StaticData.Instance = new MatterHackers.Agg.FileSystemStaticData(Path.Combine("..", "..", "..", "..", "StaticData"));

			string profilePath = Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "data", "temp", "cache", "profiles", "oemprofiles.json");

			//MatterControlUtilities.OverrideAppDataLocation();

			string resultsText = await CloudServices2.MHWebServices.Instance.Devices.GetPublicProfileList();

			var oemProfiles = JsonConvert.DeserializeObject<OemProfileDictionary>(resultsText);

			Assert.IsNotNull(oemProfiles);

			//Ensures we got success and a list of profiles
			Assert.IsTrue(oemProfiles.Keys.Count > 5);

			//Call Retrieve Profile next
			RetrievePrinterProfileWorking();
		}

		//[Test,Category("CloudProfiles")]
		public void RetrievePrinterProfileWorking()
		{
			string make = OemSettings.Instance.OemProfiles.First().Key;
			string model = OemSettings.Instance.OemProfiles[make].First().Key;
			deviceToken = OemSettings.Instance.OemProfiles[make][model];
			string expectedProfilePath = Path.Combine(ApplicationDataStorage.ApplicationUserDataPath, "Profiles", string.Format("{0}{1}", deviceToken, ProfileManager.ProfileExtension));
			if (File.Exists(expectedProfilePath))
			{
				File.Delete(expectedProfilePath);
			}
			RetrievePublicProfileRequest request = new RetrievePublicProfileRequest();

			string recievedPrinterProfile = RetrievePublicProfileRequest.DownloadPrinterProfile(deviceToken);

			Assert.IsNotNullOrEmpty(recievedPrinterProfile);
			//Assert.AreEqual(expectedProfilePath, recievedProfilePath,"Received Profile path does not match expected path.");
			//Assert.IsTrue(File.Exists(expectedProfilePath));
		}

		
	}
}
