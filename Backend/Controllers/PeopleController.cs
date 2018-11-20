using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Runtime.Serialization;
using System.IO;
using System;
using System.Diagnostics;
using System.Linq;
using Backend.Models;
using Newtonsoft.Json;
using WhosThat.Recognition;

namespace bigbackend
{
    public class ValuesController : ApiController
	{
		[Route("api/people")]
		[HttpPost]
		public HttpResponseMessage PostPerson(Person newPerson)
		{
			if (ModelState.IsValid && newPerson != null)
			{
				var actualPerson = Person.PersonWithValidID(newPerson);
				Storage.People.Add(actualPerson);

				/*foreach (Person p in Storage.People)
				{
					Console.WriteLine(p.Name + "  " + p.Bio + " " + p.Id.ToString());
				}*/

				/*var response = new HttpResponseMessage(HttpStatusCode.Created)
				{
					Content = new StringContent(newPerson.Name + " " + newPerson.Bio + " has been added to the server.")
				};
				response.Headers.Location =
					new Uri(Url.Link("DefaultApi", new { action = "status" }));
				return response;*/
				return Request.CreateResponse(HttpStatusCode.Created, actualPerson);
			}
			else
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

		[Route("api/people/{id}")]
        [HttpGet]
        public HttpResponseMessage GetPerson(int id)
        {
            Person found = Storage.FindPersonByID(id);
	        return found == null
		        ? Request.CreateErrorResponse(HttpStatusCode.NotFound, new HttpError("No person with that ID exists"))
		        : Request.CreateResponse(HttpStatusCode.OK, found);
        }

	    [Route("api/people")]
	    [HttpGet]
	    public HttpResponseMessage GetPeople()
	    {
			return Request.CreateResponse(HttpStatusCode.OK, Storage.People.ToList());
	    }

		[Route("api/people/{id}")]
		[HttpPut]
		public HttpResponseMessage UpdatePerson(int id, Person person)
		{
			var currentPerson = Storage.FindPersonByID(id);
			if (currentPerson == null)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}

			if (person.Name != null)
			{
				currentPerson.Name = person.Name;
			}

			if (person.Bio != null)
			{
				currentPerson.Bio = person.Bio;
			}

			if (person.Likes != null)
			{
				currentPerson.Likes = person.Likes;
			}

			return Request.CreateResponse(HttpStatusCode.OK);
		}

		[Route("api/people/{id}")]
		[HttpDelete]
		public HttpResponseMessage DeletePerson(int id)
		{
			var currentPerson = Storage.FindPersonByID(id);
			if (currentPerson == null)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}

			Storage.People.Remove(currentPerson);
			return Request.CreateResponse(HttpStatusCode.OK);
		}
    }
}
