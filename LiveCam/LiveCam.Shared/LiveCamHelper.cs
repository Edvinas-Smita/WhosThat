
using Android.Graphics;
using Microsoft.ProjectOxford.Face;
using ServiceHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCam.Shared
{
    public static class LiveCamHelper
    {
        public static bool IsFaceRegistered { get; set; }

        public static bool IsInitialized { get; set; }

        public static string WorkspaceKey
        {
            get;
            set;
        }
        public static Action<string> GreetingsCallback { get => greetingsCallback; set => greetingsCallback = value; }

        private static Action<string> greetingsCallback;

        public static void Init(Action throttled = null)
        {
            FaceServiceHelper.ApiKey = "b1843365b41247538cffb304d36609b3";
            if(throttled!=null)
            FaceServiceHelper.Throttled += throttled;

            WorkspaceKey = Guid.NewGuid().ToString();
            ImageAnalyzer.PeopleGroupsUserDataFilter = WorkspaceKey;
            FaceListManager.FaceListsUserDataFilter = WorkspaceKey;

            IsInitialized = true;
        }

        public static async Task RegisterFaces()
        {

            try
            {
                var personGroupId = Guid.NewGuid().ToString();
                await FaceServiceHelper.CreatePersonGroupAsync(personGroupId,
                                                        "Xamarin",
                                                     WorkspaceKey);
                await FaceServiceHelper.CreatePersonAsync(personGroupId, "Albert Einstein");

                var personsInGroup = await FaceServiceHelper.GetPersonsAsync(personGroupId);

                await FaceServiceHelper.AddPersonFaceAsync(personGroupId, personsInGroup[0].PersonId,
                                                           "https://upload.wikimedia.org/wikipedia/commons/d/d3/Albert_Einstein_Head.jpg", null, null);

                await FaceServiceHelper.TrainPersonGroupAsync(personGroupId);


                IsFaceRegistered = true;


            }
            catch (FaceAPIException ex)

            {
                Console.WriteLine(ex.Message);
                IsFaceRegistered = false;

            }

        }

        public static async Task ProcessCameraCapture(ImageAnalyzer e)
        {

            DateTime start = DateTime.Now;

            await e.DetectFacesAsync();

            if (e.DetectedFaces.Any())
            {
                await e.IdentifyFacesAsync();
                string greetingsText = GetGreettingFromFaces(e);

                if (e.IdentifiedPersons.Any())
                {

                    if (greetingsCallback != null)
                    {
                        DisplayMessage(greetingsText);
                    }

                    Console.WriteLine(greetingsText);
                }
                else
                {
                    DisplayMessage("No Idea, who you're.. Register your face.");

                    Console.WriteLine("No Idea");

                }
            }
            else
            {
               // DisplayMessage("No face detected.");

                Console.WriteLine("No Face ");

            }

            TimeSpan latency = DateTime.Now - start;
            var latencyString = string.Format("Face API latency: {0}ms", (int)latency.TotalMilliseconds);
            Console.WriteLine(latencyString);
        }

        private static string GetGreettingFromFaces(ImageAnalyzer img)
        {
            if (img.IdentifiedPersons.Any())
            {
                string names = img.IdentifiedPersons.Count() > 1 ? string.Join(", ", img.IdentifiedPersons.Select(p => p.Person.Name)) : img.IdentifiedPersons.First().Person.Name;

                if (img.DetectedFaces.Count() > img.IdentifiedPersons.Count())
                {
                    return string.Format("Welcome back, {0} and company!", names);
                }
                else
                {
                    return string.Format("Welcome back, {0}!", names);
                }
            }
            else
            {
                if (img.DetectedFaces.Count() > 1)
                {
                    return "Hi everyone! If I knew any of you by name I would say it...";
                }
                else
                {
                    return "Hi there! If I knew you by name I would say it...";
                }
            }
        }

        static void DisplayMessage(string greetingsText)
        {
            greetingsCallback?.Invoke(greetingsText);
		}

	    public static byte[] BitmapToGrayscaleBytes(Bitmap bitmap)
	    {
		    var ints = new int[bitmap.ByteCount / 4];
		    var bytes = new byte[bitmap.ByteCount / 4];
		    bitmap.GetPixels(ints, 0, 0, 0, 0, bitmap.Width, bitmap.Height);

		    byte red, green, blue;
		    for (int i = 0; i < bitmap.ByteCount / 4; ++i)
		    {
			    red = (byte)(ints[i] & 0xFF);
			    green = (byte)((ints[i] >> 8) & 0xFF);
			    blue = (byte)((ints[i] >> 16) & 0xFF);
			    bytes[i] = (byte)((red + green + blue) / 3);
		    }

		    return bytes;
	    }
	}
}
