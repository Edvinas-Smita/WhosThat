using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Runtime.Serialization;
using System.IO;
using System;
using Backend.Models;
using Newtonsoft.Json;
using WhosThat.Recognition;

namespace bigbackend
{
    public class ValuesController : ApiController
    {
        [Route("api/values/{id}")]
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            Person found = Storage.FindPersonByID(id);
            if (found != null)
                return Request.CreateResponse<Person>(HttpStatusCode.OK, found);
            else return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "This person has not been found.");
        }

        [Route("api/values/post")]
        [HttpPost]
        public HttpResponseMessage Post(Person newPerson)
        {
            if (ModelState.IsValid && newPerson != null)
            {
                //var id = Guid.NewGuid();
                //newPerson.id = id;
                //Person newPerson = JsonConvert.DeserializeObject<Person>(personjson);
                Storage.People.Add(newPerson);

                foreach(Person p in Storage.People)
                {
                    Console.WriteLine(p.Name + "  " + p.Bio + " " + p.Id.ToString());
                }

                var response = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(newPerson.Name + " " + newPerson.Bio + " has been added to the server.")
                };
                response.Headers.Location =
                    new Uri(Url.Link("DefaultApi", new { action = "status" }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
