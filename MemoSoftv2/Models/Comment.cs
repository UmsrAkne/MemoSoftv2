namespace MemoSoftv2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Prism.Mvvm;

    public class Comment : BindableBase
    {
        private bool isEditing;

        public Comment()
        {
        }

        public Comment(string text, DateTime creationDatetime)
        {
            Text = text;
            CreationDateTime = creationDatetime;
        }

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public DateTime CreationDateTime { get; set; } = DateTime.Now;

        [NotMapped]
        public bool IsEditing { get => isEditing; set => SetProperty(ref isEditing, value); }
    }
}
