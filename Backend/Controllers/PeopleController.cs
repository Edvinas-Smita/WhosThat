using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Backend.Models;

namespace Backend.Controllers
{
    public class PeopleController : ApiController
    {
		private Person[] people = new Person[]
		{
			new Person{id = 0, firstName = "Some", lastName = "One"},
			new Person{id = 1, firstName = "Some", lastName = "Two"},
			new Person{id = 2, firstName = "Some", lastName = "Three"},
			new Person{id = 3, firstName = "Some", lastName = "Four"},
			new Person{id = 4, firstName = "Some", lastName = "Five"}
		};

	    public List<Person> GetPeople()
	    {
		    return people.ToList();
	    }

	    public IHttpActionResult GetPerson(int id)
	    {
		    Person person = people.FirstOrDefault(pers => pers.id == id);
		    if (person == null)
		    {
			    return NotFound();
		    }

		    return Ok(person);
	    }
    }
}
