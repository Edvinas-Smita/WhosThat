using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhosThat.Recognition;

namespace Backend.Models
{
    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate (object sender, PropertyChangedEventArgs args) { };

        public int Id;

        private string name;
        private string password;
        private string bio;
        private string likes;
        private VeryDependentActions actions;
        private Lazy<LazyClass> lazy = new Lazy<LazyClass>(() =>
        {
            return new LazyClass(1337);
        });

        public string Name
        {
            get { return name; }
            set
            {
                actions.Stuff();
                name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
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
        public ObservableCollection<byte[]> Images = new ObservableCollection<byte[]>();

        public Person(string name, string password, string bio, string likes, VeryDependentActions act)
        {
            Name = name;
            Password = password;
            Bio = bio;
            Likes = likes;
            actions = act;

            Id = IdFactory.GetNextId();
        }

	    public static Person PersonWithValidID(Person person)
	    {
			return new Person(person.Name, person.Password, person.Bio, person.likes, person.actions);
	    }
    }
}
