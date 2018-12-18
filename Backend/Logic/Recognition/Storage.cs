using Backend.Models;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Backend.Logic.Recognition
{
    public static class Storage
    {
		private delegate bool CheckFirstBySecondAndThird<T1, T2, T3>(T1 person, T2 loginName, T3 passwordHash);
		//public static BindingList<Person> People = new BindingList<Person> { new Person("admin", "yep", "", "", new VeryDependentActions()) };
		private static BB_Entities entities = new BB_Entities();

		#region user/person CRUD
		public static List<Person> GetAllPeople()
		{
			return (List<Person>)entities.Users.Select(user => Person.PersonFromUser(user));
		}

		public static List<User> GetAllUsers()
		{
			return (List<User>)entities.Users.ToList();
		}

		public static Person FindPersonByID(long id)
	    {
			return Person.PersonFromUser(entities.Users.FirstOrDefault(user => user.UserID == id));
			//return People.FirstOrDefault(person => person.Id == id);

			/*foreach (var person in People)
		    {
			    if (person.Id == id)
			    {
				    return person;
			    }
		    }
		    return null;*/
		}

		public static User FindUserByID(long id)
		{
			return entities.Users.FirstOrDefault(user => user.UserID == id);
		}

		public static void AddPerson(Person person)
		{
			AddUser(person.ToUser());
		}

		public static void AddUser(User user)
		{
			using (SHA256 sha256Hash = SHA256.Create())
			{
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				user.Password = builder.ToString();	//hashing (w/o salt) user password before storing it
			}
			entities.Users.Add(user);
			entities.SaveChanges();
		}

		public static void UpdateUser(User user)
		{
			var current = FindUserByID(user.UserID);
			current.Name = user.Name;
			current.Surname = user.Surname;
			current.Email = user.Email;
			current.Password = user.Password;
			current.Bio = user.Bio;
			current.Likes = user.Likes;
			entities.SaveChanges();
		}

		public static void UpdatePerson(Person person, long id)
		{
			var userWithoutID = person.ToUser();
			userWithoutID.UserID = id;
			UpdateUser(userWithoutID);
		}

		public static void DeleteUser(User user)
		{
			entities.Users.Remove(user);
			entities.SaveChanges();
		}

		public static void DeletePerson(long id)
		{
			DeleteUser(FindUserByID(id));
		}

		public static void DeleteUserByID(long id)
		{
			DeleteUser(FindUserByID(id));
		}
		#endregion


		#region picture CRUD minus update because why
		public static void AddPictureLink(long userID, string link)
		{
			entities.Photos.Add(new Photo()
			{
				UserID = userID,
				PhotoLink = link
			});
			entities.SaveChanges();
		}

		public static List<Photo> GetALLPics()
		{
			return entities.Photos.ToList();
		}

		public static Photo GetPictureByID(long pictureID)
		{
			return entities.Photos.FirstOrDefault(photo => photo.PhotoID == pictureID);
		}

		public static List<Photo> GetAllUserPictures(long userID)
		{
			return entities.Photos.ToList();
		}

		public static Photo GetNthPictureOfUser(long userID, int nth)
		{
			var userPics = GetAllUserPictures(userID);
			if (userPics.Count < nth - 1)
			{
				return null;
			}

			return userPics[nth];
		}

		public static void DeletePictureByID(long pictureID)
		{
			DeletePicture(GetPictureByID(pictureID));
		}

		public static void DeletePicture(Photo pic)
		{
			entities.Photos.Remove(pic);
			entities.SaveChanges();
		}
		#endregion


		public static User FindPersonByCredentials(string email, string passwordHash)
        {
			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passwordHash))
			{
				return null;
			}

			/*CheckFirstBySecondAndThird<User, string, string> CorrectUserCredentials = delegate (User user, string Email, string PasswordHash)
			{
				if (Email.Equals(user.Email))
				{
					using (SHA256 sha256Hash = SHA256.Create())
					{
						byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
						StringBuilder builder = new StringBuilder();
						for (int i = 0; i < bytes.Length; i++)
						{
							builder.Append(bytes[i].ToString("x2"));
						}
						var hashedStoredPass = builder.ToString();
						Debug.WriteLine("stored: {0}\nTest: {1}", hashedStoredPass, PasswordHash);
						if (PasswordHash.Equals(hashedStoredPass))
						{
							return true;
						}
					}

				}
				return false;
			};*/

			return entities.Users.ToList().FirstOrDefault(user => user.Email.Equals(email) && user.Password.Equals(passwordHash));

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
