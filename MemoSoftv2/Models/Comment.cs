namespace MemoSoftv2.Models
{
    using System;

    public class Comment
    {
        public Comment()
        {
            CreationDateTime = DateTime.Now;
        }

        public Comment(string text, DateTime creationDatetime)
        {
            Text = text;
            CreationDateTime = creationDatetime;
        }

        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTime CreationDateTime { get; set; }
    }
}
