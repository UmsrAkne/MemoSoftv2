namespace MemoSoftv2.Models
{
    public class SubComment : Comment
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ParentCommentId { get; set; }
    }
}