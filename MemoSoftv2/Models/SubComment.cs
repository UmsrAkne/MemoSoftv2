namespace MemoSoftv2.Models
{
    public class SubComment : Comment
    {
        public int ParentCommentId { get; set; }
    }
}