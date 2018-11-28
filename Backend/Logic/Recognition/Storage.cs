using Backend.Models;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Backend.Logic.Recognition
{
    public static class Storage
    {
		private delegate bool CheckFirstBySecondAndThird<T1, T2, T3>(T1 person, T2 loginName, T3 passwordHash);
		public static BindingList<Person> People = new BindingList<Person> { new Person("admin", "yep", "", "", new VeryDependentActions()) };  //c06fc555902938b41cdc7018de6a9250a8658064c34d8fb05dbe9087be3b5cd4

		public static Person FindPersonByID(int id)
	    {
			return People.FirstOrDefault(person => person.Id == id);

		    /*foreach (var person in People)
		    {
			    if (person.Id == id)
			    {
				    return person;
			    }
		    }
		    return null;*/
	    }

        public static Person FindPersonByCredentials(string name, string password)
        {
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
			{
				return null;
			}

			CheckFirstBySecondAndThird<Person, string, string> CorrectPersonCredentials = delegate (Person person, string loginName, string passwordHash)
			{
				if (loginName.Equals(person.Name))
				{
					using (SHA256 sha256Hash = SHA256.Create())
					{
						byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(person.Password));
						StringBuilder builder = new StringBuilder();
						for (int i = 0; i < bytes.Length; i++)
						{
							builder.Append(bytes[i].ToString("x2"));
						}
						string hashedStoredPass = builder.ToString();
						if (password.Equals(hashedStoredPass))
						{
							return true;
						}
					}

				}
				return false;
			};

			return People.FirstOrDefault(person => CorrectPersonCredentials(person, name, password));

            /*foreach (var person in People)
            {
                if (name == person.Name)
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
            return null;*/
        }
    }
}
