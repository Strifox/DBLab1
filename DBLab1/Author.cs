using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLab1
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; } 
        public string Gender { get; set; }

        public Author(int id,string name, int age, DateTime birthDate, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            BirthDate = birthDate;
            Gender = gender;
        }

 
    }
}
