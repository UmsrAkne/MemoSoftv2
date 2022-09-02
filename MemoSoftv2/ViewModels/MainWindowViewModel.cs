namespace MemoSoftv2.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Media;
    using MemoSoftv2.Models;
    using MemoSoftv2.Models.DBs;
    using Prism.Commands;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private ObservableCollection<Comment> comments;
        private string inputText;
        private string systemMessage;
        private Comment editingComment;
        private Comment selectionComment;
        private bool isTextBoxFocused;

        private CommentDbContext commentDbContext = new CommentDbContext();

        public MainWindowViewModel()
        {
            try
            {
                commentDbContext.Database.EnsureCreated();
            }
            catch (Npgsql.NpgsqlException)
            {
                commentDbContext = null;
                SystemMessage = "PostgreSQL データベースへの接続に失敗しました";
            }

            if (commentDbContext != null)
            {
                ReloadCommentCommand.Execute();
            }
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ObservableCollection<Comment> Comments { get => comments; set => SetProperty(ref comments, value); }

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public string SystemMessage { get => systemMessage; set => SetProperty(ref systemMessage, value); }

        public bool IsTextBoxFocused { get => isTextBoxFocused; set => SetProperty(ref isTextBoxFocused, value); }

        public Comment SelectionComment { get => selectionComment; set => SetProperty(ref selectionComment, value); }

        public DelegateCommand PostCommentCommand => new DelegateCommand(() =>
        {
            if (editingComment == null)
            {
                commentDbContext.AddComment(new Comment(InputText, DateTime.Now));
                InputText = string.Empty;
            }
            else
            { // コメント編集中の場合
                editingComment.Text = InputText;
                editingComment.IsEditing = false;
                editingComment = null;
                commentDbContext.SaveChanges();
            }

            ReloadCommentCommand.Execute();
        });

        public DelegateCommand<Comment> EditCommentCommand => new DelegateCommand<Comment>((comment) =>
        {
            InputText = comment.Text;
            comment.IsEditing = true;
            editingComment = comment;
        });

        public DelegateCommand<Comment> AddFavoriteCommentCommand => new DelegateCommand<Comment>((comment) =>
        {
            comment.IsFavorite = !comment.IsFavorite;
            commentDbContext.SaveChanges();
            ReloadCommentCommand.Execute();
        });

        public DelegateCommand CancelEditCommentCommand => new DelegateCommand(() =>
        {
            if (editingComment != null)
            {
                editingComment.IsEditing = false;
                editingComment = null;
                InputText = string.Empty;
            }
        });

        public DelegateCommand ReloadCommentCommand => new DelegateCommand(() =>
        {
            Comments = new ObservableCollection<Comment>(commentDbContext.GetComments());
        });

        public DelegateCommand FocusToTextBoxCommand => new DelegateCommand(() =>
        {
            // 元から true である場合、フォーカスが移動しないので、一度 false にセットする。
            IsTextBoxFocused = false;
            IsTextBoxFocused = true;
        });

        public DelegateCommand<object> ChangeBackgroundColorCommand => new DelegateCommand<object>((olorBrush) =>
        {
            var mediaColor = ((SolidColorBrush)olorBrush).Color;
            SelectionComment.BackgroundColorArgb = System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B).ToArgb();
            commentDbContext.SaveChanges();
        });

        public DelegateCommand ExitCommand => new DelegateCommand(() =>
        {
            System.Windows.Application.Current.Shutdown();
        });
    }
}
