using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLab1
{
    public class Book
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string AuthorName { get; set; }

        public Book(int id, int authorId, string title, string genre, string authorName)
        {
            Id = id;
            AuthorId = authorId;
            Title = title;
            Genre = genre;
            AuthorName = authorName;
        }
    }
}
