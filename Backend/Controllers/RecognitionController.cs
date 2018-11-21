using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Emgu.CV;
using Emgu.CV.Structure;
using WhosThat.Recognition;
using WhosThat.Recognition.Util;

namespace Backend.Controllers
{
    public class RecognitionController : ApiController
    {
		[HttpPost, Route("api/recognize")]
		public async Task<IHttpActionResult> RecognizeUser()
		{
			if (!Request.Content.IsMimeMultipartContent() || !Request.Content.Headers.ContentType.Equals("application/octet-stream"))
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
			var buffer = await file.ReadAsByteArrayAsync();
			Debug.WriteLine(buffer.Length);
			File.WriteAllBytes("D:/SomeDump/uploaded.bmp", buffer);
			if (buffer.Length != 76800)	//240*320	//Receiving only 240*320 gray image bytes - we will convert it to grayscale and remove any metadata in frontend
			{
				//throw new HttpResponseException(HttpStatusCode.BadRequest);
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, Request.Content));
			}

			var recognizedUserID = Statics.RecognizeUser(Statics.ByteArrayToImage(buffer, 240, 320));

			return Ok(Request.Content);
		}

	    [HttpPost, Route("api/train/{userID}/{imgCount}")]
	    public async Task<IHttpActionResult> TrainRecognizer(int userID, int imgCount)
	    {
		    if (!EmguSingleton.Instance.RecognizerIsTrained)
		    {
			    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError,
				    new StringContent("Recognition algorythm has not been trained yet - cannot recognize")));
		    }

		    if (imgCount < 1 || imgCount > 50)
			{
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, Request.Content));
			}

		    var currentPerson = Storage.FindPersonByID(userID);
		    if (currentPerson == null)
			{
				return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, Request.Content));
			}

			var provider = new MultipartMemoryStreamProvider();
		    await Request.Content.ReadAsMultipartAsync(provider);
		    if (provider.Contents.Count != imgCount)
		    {
			    //throw new HttpResponseException(HttpStatusCode.BadRequest);
			    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, Request.Content));
		    }

			var imagesToTrain = new List<Image<Gray, byte>>(imgCount);
		    foreach (var file in provider.Contents)
		    {
			    var fileBytes = await file.ReadAsByteArrayAsync();
			    if (fileBytes.Length != 76800) //240*320	//Receiving only 240*320 gray image bytes - we will convert it to grayscale and remove any metadata in frontend
			    {
				    //throw new HttpResponseException(HttpStatusCode.BadRequest);
				    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, Request.Content));
			    }
				imagesToTrain.Add(Statics.ByteArrayToImage(fileBytes, 240, 320));
			}

			Statics.TrainSinglePersonFaces(imagesToTrain, userID);
		    return Ok();
	    }
	}
}
