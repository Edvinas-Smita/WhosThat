using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Logic;

namespace Backend.Models
{
    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate (object sender, PropertyChangedEventArgs args) { };

		private string firstName;
		private string lastName;
        private string email;
        private string passHash;
        private string bio;
        private string likes;
        private VeryDependentActions actions;
        public Lazy<LazyClass> lazy = new Lazy<LazyClass>(() =>
        {
            return new LazyClass(1337);
        });
		public string FirstName
		{
			get { return firstName; }
			set
			{
				firstName = value;
				PropertyChanged(this, new PropertyChangedEventArgs("FirstName"));
			}
		}
		public string LastName
		{
			get { return lastName; }
			set
			{
				lastName = value;
				PropertyChanged(this, new PropertyChangedEventArgs("LastName"));
			}
		}

		public string Email
        {
            get { return email; }
            set
            {
                actions.Stuff();
                email = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Email"));
            }
        }
        public string PasswordHash
        {
            get { return passHash; }
            set
            {
                passHash = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PasswordHash"));
            }
        }
        public string Bio
        {
            get { return bio; }
            set
            {
                bio = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Bio"));
            }
        }
        public string Likes
        {
            get { return likes; }
            set
            {
                likes = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Likes"));
            }
        }
        //public ObservableCollection<byte[]> Images = new ObservableCollection<byte[]>();

        public Person(string firstName, string lastName, string email, string passHash, string bio, string likes)
		{
			this.actions = new VeryDependentActions();

			this.firstName = firstName;
			this.lastName = lastName;
			this.email = email;
            this.passHash = passHash;
            this.bio = bio;
            this.likes = likes;
        }

		public static Person PersonFromUser(User user)
		{
			return new Person(user.Name, user.Surname, user.Email, user.Password, user.Bio, user.Likes);
		}

		public User ToUser()
		{
			return new User()
			{
				Name = this.FirstName,
				Surname = this.LastName,
				Email = this.Email,
				Password = this.PasswordHash,
				Bio = this.Bio,
				Likes = this.Likes
			};
		}
    }
}
