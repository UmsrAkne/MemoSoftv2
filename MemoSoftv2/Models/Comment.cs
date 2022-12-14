using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using Prism.Mvvm;

namespace MemoSoftv2.Models
{
    public class Comment : BindableBase
    {
        private bool isEditing;
        private bool isFavorite;
        private int backgroundColorArgb = Color.White.ToArgb();
        private int numIndent;
        private int groupId;
        private string tag;

        public Comment()
        {
        }

        public Comment(string text, DateTime creationDatetime)
        {
            Text = text;
            CreationDateTime = creationDatetime;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public DateTime CreationDateTime { get; set; } = DateTime.Now;

        [Required]
        public bool IsFavorite { get => isFavorite; set => SetProperty(ref isFavorite, value); }

        [Required]
        public int BackgroundColorArgb { get => backgroundColorArgb; set => SetProperty(ref backgroundColorArgb, value); }

        [Required]
        public int NumIndent { get => numIndent; set => SetProperty(ref numIndent, value); }

        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        [Required]
        public int GroupId { get => groupId; set => SetProperty(ref groupId, value); }

        [Required]
        public int ParentCommentId { get; set; }

        [Required]
        public bool IsSubComment { get; set; }

        [NotMapped]
        public bool IsEditing { get => isEditing; set => SetProperty(ref isEditing, value); }

        [NotMapped]
        public string Tag { get => tag; set => SetProperty(ref tag, value); }
    }
}