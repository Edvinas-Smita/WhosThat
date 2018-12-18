using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using Backend.Models;
using Backend.Logic.Recognition.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Backend;

namespace UnitTests4Backend
{
	[TestClass]
	public class UnitTest1
	{
		private Person createdPerson;

		[TestMethod]
		public void TestCreatePerson()
		{
			var client = new HttpClient();
			client.BaseAddress = new Uri("http://88.119.27.98:55555");
			var testUser = new { Name = "adminadmin", Email = "admin", Password = "8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918" };
			var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(testUser));
			content.Headers.ContentType =
				new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json");
			var result = client.PostAsync("api/people", content).Result;
			var response = result.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("RESPONSE: " + response);
			createdPerson = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(response);
			Assert.IsNotNull(createdPerson);
			//Assert.IsTrue(createdPerson.Name.Equals(testUser.name) && createdPerson.Bio.Equals(testUser.bio) && createdPerson.Likes.Equals(testUser.likes));
		}

		[TestMethod]
		public void TestAddImageToTestPerson()
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("http://88.119.27.98:55555");
				var serializedResult = client.GetAsync("api/people").Result.Content.ReadAsStringAsync().Result;
				var existingPeople = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(serializedResult);
				if (existingPeople.Count == 0)
				{
					TestCreatePerson();
					return;
				}

				var zeeFotozUno = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Photo>>(client.GetAsync("api/pictures/0").Result.Content.ReadAsStringAsync().Result);
				client.PostAsync("api/pictures/0", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new { Value = "U N I T" })));
				var zeeFotozDos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Photo>>(client.GetAsync("api/pictures/0").Result.Content.ReadAsStringAsync().Result);

				Assert.AreEqual(zeeFotozUno.Count + 1, zeeFotozDos);
			}
		}

		[TestMethod]
		public void TestEmguImageCreation()
		{
			byte[] bytes = File.ReadAllBytes(@"D:\ToTest");
			Assert.AreEqual(240 * 320, bytes.Length);
			var emguImg = Statics.ByteArrayToImage(bytes, 240, 320);
			Assert.IsNotNull(emguImg);
			var bitmap = emguImg.ToBitmap(240, 320);
			for (int y = 0; y < 320; y++)
			{
				for (int x = 0; x < 240; x++)
				{
					Assert.AreEqual(bytes[240 * y + x], bitmap.GetPixel(x, y).R);
					Assert.AreEqual(bytes[240 * y + x], bitmap.GetPixel(x, y).G);
					Assert.AreEqual(bytes[240 * y + x], bitmap.GetPixel(x, y).B);
				}
			}
			Assert.AreEqual(bytes[0], bitmap.GetPixel(0, 0).B);
			bitmap.Save(@"D:\FromTest.bmp");
		}
	}
}
