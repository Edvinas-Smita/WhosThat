﻿using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Diagnostics;
using Backend.Logic.Recognition;

namespace Backend.Controllers
{
    public class PictureController : ApiController
    {
        const int BMP_SIGNATURE = 0x424D;
        const int JPEG_SIGNATURE = 0xFFD8FF;

	    [HttpPost, Route("api/pictures/{personID}")]
	    public async Task<IHttpActionResult> PostPersonPicture(int personID)
		{
			Debug.WriteLine("Incoming POST for api/pictures/id");
			if (!Request.Content.IsMimeMultipartContent())
		    {
			    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
		    }

		    var currentPerson = Storage.FindPersonByID(personID);
		    if (currentPerson == null)
		    {
			    throw new HttpResponseException(HttpStatusCode.NotFound);
		    }

			var provider = new MultipartMemoryStreamProvider();
		    await Request.Content.ReadAsMultipartAsync(provider);
			string[] allowedImageExts = new string[]{ ".jpg", ".jpeg", ".gif", ".bmp", ".png" };
		    foreach (var file in provider.Contents)
		    {
			    var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
			    if (!allowedImageExts.Any(ext => filename.EndsWith(ext)))
			    {
				    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			    }
			    var buffer = await file.ReadAsByteArrayAsync();

                if(
                    ( (((int)buffer[0] << 8) + (int)buffer[1]) != BMP_SIGNATURE) &&
                    ( (((int)buffer[0] << 16) + ((int)buffer[1] << 8) + (int)buffer[2]) != JPEG_SIGNATURE)
                )
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                currentPerson.Images.Add(buffer);
		    }

		    return Ok();
		}

	    [HttpGet, Route("api/pictures/{personID}/{pictureNr}")]
        public HttpResponseMessage GetPersonPicture(int personID, int pictureNr)
		{
			Debug.WriteLine("Incoming GET for api/pictures/id/nr");
			if (pictureNr < 0)
		    {
			    throw new HttpResponseException(HttpStatusCode.BadRequest);
		    }

		    var currentPerson = Storage.FindPersonByID(personID);
		    if (currentPerson == null || currentPerson.Images.Count <= pictureNr)
		    {
			    throw new HttpResponseException(HttpStatusCode.NotFound);
		    }
			
		    var result = new HttpResponseMessage(HttpStatusCode.OK)
		    {
			    Content = new ByteArrayContent(currentPerson.Images[pictureNr])
		    };
		    result.Content.Headers.ContentDisposition =
			    new ContentDispositionHeaderValue("attachment")
			    {
				    FileName = personID + "_" + pictureNr
			    };
		    result.Content.Headers.ContentType =
			    new MediaTypeHeaderValue("application/octet-stream");

		    return result;
	    }

		/*[HttpGet, Route("api/pictures/{personID}")]
	    public HttpResponseMessage GetPersonPictures(int personID)
		{
			var currentPerson = Storage.FindPersonByID(personID);
			if (currentPerson == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			var stream = new MemoryStream();
			// processing the stream.
			var result = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new ByteArrayContent(stream.ToArray())
			};
			result.Content.Headers.ContentDisposition =
				new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
				{
					FileName = "CertificationCard.pdf"
				};
			result.Content.Headers.ContentType =
				new MediaTypeHeaderValue("application/octet-stream");

			return result;
		}*/

		[HttpDelete, Route("api/pictures/{personID}/{pictureNr}")]
		public HttpResponseMessage DeletePersonPicture(int personID, int pictureNr)
		{
			Debug.WriteLine("Incoming DELETE for api/pictures/id/nr");
			if (pictureNr < 0)
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			var currentPerson = Storage.FindPersonByID(personID);
			if (currentPerson == null || currentPerson.Images.Count <= pictureNr)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
			
			currentPerson.Images.RemoveAt(pictureNr);

			return Request.CreateResponse(HttpStatusCode.OK);
		}
	}
}
