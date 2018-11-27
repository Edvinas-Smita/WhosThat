using Backend.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Backend.Logic.Recognition
{
    public static class Storage
    {
		public static BindingList<Person> People = new BindingList<Person>();

	    public static Person FindPersonByID(int id)
	    {
		    foreach (var person in People)
		    {
			    if (person.Id == id)
			    {
				    return person;
			    }
		    }

		    return null;
	    }
    }
}
