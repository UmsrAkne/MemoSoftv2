using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Prism.Mvvm;

namespace MemoSoftv2.Models
{
    public class Group : BindableBase
    {
        private string name;
        private bool editMode;

        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get => name; set => SetProperty(ref name, value); }

        [NotMapped]
        public bool EditMode { get => editMode; set => SetProperty(ref editMode, value); }
    }
}