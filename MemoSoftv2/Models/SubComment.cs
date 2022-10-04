using System.ComponentModel.DataAnnotations.Schema;

namespace MemoSoftv2.Models
{
    public class SubComment : Comment
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ParentCommentId { get; set; }

        [NotMapped]
        public new bool IsSubComment => true;
    }
}