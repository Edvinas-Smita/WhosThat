using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Backend.Models;
using Emgu.CV;
using Emgu.CV.Structure;
using Backend.Logic.Recognition;
using Backend.Logic.Recognition.Util;

namespace Backend.Controllers
{
    public class RecognitionController : ApiController
    {
		private static int IMAGE_BYTE_COUNT = 76800;    //240*320	//Receiving only 240*320 gray image bytes - we will convert it to grayscale and remove any metadata in frontend

		[HttpPost, Route("api/recognize")]
		public async Task<IHttpActionResult> RecognizeUser()
		{
			/*if (!Request.Content.IsMimeMultipartContent() || !Request.Content.Headers.ContentType.Equals("application/octet-stream"))
			{
				//throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, Request.Content));
			}
			
			var provider = new MultipartMemoryStreamProvider();
			await Request.Content.ReadAsMultipartAsync(provider);
			if (provider.Contents.Count != 1)
			{
				//throw new HttpResponseException(HttpStatusCode.BadRequest);
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, Request.Content));
			}

			var file = provider.Contents[0];
			var buffer = await file.ReadAsByteArrayAsync();*/

			var buffer = await Request.Content.ReadAsByteArrayAsync();
			Debug.WriteLine(buffer.Length);
			File.WriteAllBytes("E:/SomeDump/uploaded.bmp", buffer);
			if (buffer.Length != IMAGE_BYTE_COUNT)
			{
				//throw new HttpResponseException(HttpStatusCode.BadRequest);
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, Request.Content));
			}

			if (!EmguSingleton.Instance.RecognizerIsTrained)
			{
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError,
					new StringContent("Recognition algorythm has not been trained yet - cannot recognize")));
			}

			var recognizedUserID = Statics.RecognizeUser(Statics.ByteArrayToImage(buffer, 240, 320));
            Person recognized = Storage.FindPersonByID(recognizedUserID);
            if (recognized != null)
                return Ok(recognized);
            else
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError));
		}

	    [HttpPost, Route("api/train/{userID}/{imgCount}")]
	    public async Task<IHttpActionResult> TrainRecognizer(int userID, int imgCount)
		{
			Debug.WriteLine("Incoming POST for api/train/{0}/{1}", userID, imgCount);
			if (imgCount < 1 || imgCount > 50)
			{
				Debug.WriteLine("Image count must be between 1 and 50 - Yours was " + imgCount);
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, new StringContent("Image count must be between 1 and 50 - Yours was " + imgCount)));
			}

			Debug.WriteLine("Counts are gud");
			if (Storage.FindUserByID(userID) == null)   //check if that user e x i s t s
			{
				Debug.WriteLine("No user with given id (" + userID + ") was found");
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, new StringContent("No user with given id (" + userID + ") was found")));
			}

			Debug.WriteLine("User e x i s t s");
			var buffer = await Request.Content.ReadAsByteArrayAsync();
			if (buffer.Length != IMAGE_BYTE_COUNT * imgCount)
			{
				Debug.WriteLine("Buffer size (" + buffer.Length + ") did not match expected size - " + (IMAGE_BYTE_COUNT * imgCount));
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, new StringContent("Buffer size (" + buffer.Length + ") did not match expected size - " + (IMAGE_BYTE_COUNT * imgCount))));
			}

			Debug.WriteLine("buffer size matches image count");
			var imagesToTrain = new List<Image<Gray, byte>>(imgCount);
		    for (int i = 0; i < imgCount; ++i)
		    {
				imagesToTrain.Add(Statics.ByteArrayToImage(Statics.SubArray(buffer, i * IMAGE_BYTE_COUNT, IMAGE_BYTE_COUNT), 240, 320));
			}

			Debug.WriteLine("Training...");
			Statics.TrainSinglePersonFaces(imagesToTrain, userID);
			Debug.WriteLine("Trained");
			return Ok();
	    }

		[HttpDelete, Route("api/train/{userID}")]
	    public IHttpActionResult DeleteUserRecognitionData(int userID)	//all of it
		{
			return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotImplemented));
			//write recognizer training data to file (or memory stream),
			//regexp replace all lines that contain data with userID label with nothing,
			//???
			//profit
		}
	}
}
