using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using Backend.Models;
using WhosThat.Recognition;
using System.Diagnostics;

namespace bigbackend
{
    public class ValuesController : ApiController
	{
        [Route("api/login")]
        [HttpPost]
        public HttpResponseMessage GetPersonByLogin(string name, string password)
        {
            Person person = Storage.FindPersonByCredentials(name, password);
            if (person != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, person);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new HttpError("Person with these credentials was not found."));
            }

        }

		[Route("api/people")]
		[HttpPost]
		public HttpResponseMessage PostPerson(Person newPerson)
		{
			Debug.WriteLine("Incoming POST for api/people");
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
			Debug.WriteLine("Incoming GET for api/people/id");
			Person found = Storage.FindPersonByID(id);
	        return found == null
		        ? Request.CreateErrorResponse(HttpStatusCode.NotFound, new HttpError("No person with that ID exists"))
		        : Request.CreateResponse(HttpStatusCode.OK, found);
        }

	    [Route("api/people")]
	    [HttpGet]
	    public HttpResponseMessage GetPeople()
		{
			Debug.WriteLine("Incoming GET for api/people");
			return Request.CreateResponse(HttpStatusCode.OK, Storage.People.ToList());
	    }

		[Route("api/people/{id}")]
		[HttpPut]
		public HttpResponseMessage UpdatePerson(int id, Person person)
		{
			Debug.WriteLine("Incoming PUT for api/people/id");
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
			Debug.WriteLine("Incoming DELETE for api/people/id");
			var currentPerson = Storage.FindPersonByID(id);
            currentPerson.lazy.Value.Lazy(); // dummy lazy init
			if (currentPerson == null)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}

			Storage.People.Remove(currentPerson);
			return Request.CreateResponse(HttpStatusCode.OK);
		}
    }
}
