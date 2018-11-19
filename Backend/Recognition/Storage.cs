using Backend.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace WhosThat.Recognition
{
    public class Storage
	{
		public List<Face> Faces { get; set; } = new List<Face>();
		public Face this[int index]
	    {
		    get
		    {
			    return Faces[index];
		    }
		    set
		    {
			    Faces.Insert(index, value);
		    }
	    }


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
