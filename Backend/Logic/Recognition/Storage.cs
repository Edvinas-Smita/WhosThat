using Backend.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace WhosThat.Recognition
{
    public static class Storage
    {
		public static BindingList<Person> People = new BindingList<Person> { new Person("admin", "yep", "", "") };

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
        public static Person FindPersonByCredentials(string name, string password)
        {
            foreach(var person in People)
            {
                if(name == person.Name)
                {
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(person.Password));
                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            builder.Append(bytes[i].ToString("x2"));
                        }
                        if(builder.ToString() == password)
                        {
                            return person;
                        }
                    }

                }
            }
            return null;
        }
    }
}
