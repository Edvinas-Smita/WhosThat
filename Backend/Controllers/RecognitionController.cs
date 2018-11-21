using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

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
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			return Ok(Request.Content);
		}
	}
}
