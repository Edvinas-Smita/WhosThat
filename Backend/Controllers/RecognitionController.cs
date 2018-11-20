using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WhosThat.Recognition;

namespace Backend.Controllers
{
    public class RecognitionController : ApiController
    {
		[HttpPost, Route("api/recognize")]
		public async Task<IHttpActionResult> RecognizeUser()
		{
			if (!Request.Content.IsMimeMultipartContent() || !Request.Content.Headers.ContentType.Equals("application/octet-stream"))
			{
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}
			
			var provider = new MultipartMemoryStreamProvider();
			await Request.Content.ReadAsMultipartAsync(provider);
			if (provider.Contents.Count != 1)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			var file = provider.Contents[0];
			var buffer = await file.ReadAsByteArrayAsync();

			return Ok();
		}
	}
}
