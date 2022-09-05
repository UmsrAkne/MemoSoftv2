﻿namespace MemoSoftv2.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Media;
    using MemoSoftv2.Models;
    using MemoSoftv2.Models.DBs;
    using Prism.Commands;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "MemoSoft v2";
        private ObservableCollection<Comment> comments;
        private List<Tag> tags;
        private ObservableCollection<Group> groups;
        private string inputText;
        private string systemMessage;
        private Comment editingComment;
        private Comment selectionComment;
        private Group selectionGroup;
        private bool isTextBoxFocused;

        private CommentDbContext commentDbContext = new CommentDbContext();

        public MainWindowViewModel()
        {
            try
            {
                commentDbContext.Database.EnsureCreated();
                commentDbContext.AddDefaultGroup(new Group() { Name = "Default Group", Id = 1 });
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

        public List<Tag> Tags { get => tags; set => SetProperty(ref tags, value); }

        public ObservableCollection<Group> Groups { get => groups; set => SetProperty(ref groups, value); }

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public string SystemMessage { get => systemMessage; set => SetProperty(ref systemMessage, value); }

        public bool IsTextBoxFocused { get => isTextBoxFocused; set => SetProperty(ref isTextBoxFocused, value); }

        public Comment SelectionComment { get => selectionComment; set => SetProperty(ref selectionComment, value); }

        public Group SelectionGroup { get => selectionGroup; set => SetProperty(ref selectionGroup, value); }

        public DelegateCommand PostCommentCommand => new DelegateCommand(() =>
        {
            if (string.IsNullOrWhiteSpace(InputText))
            {
                return;
            }

            if (Mode == Mode.Post)
            {
                commentDbContext.AddComment(new Comment(InputText, DateTime.Now) { GroupId = SelectionGroup.Id });
            }
            else if (Mode == Mode.Edit)
            {
                // コメント編集中の場合
                editingComment.Text = InputText;
                editingComment.IsEditing = false;
                editingComment = null;
                commentDbContext.SaveChanges();
            }
            else if (Mode == Mode.TagAddition)
            {
                commentDbContext.AddTag(new Tag() { Name = InputText });
            }

            InputText = string.Empty;
            SystemMessage = string.Empty;
            Mode = Mode.Post;
            ReloadCommentCommand.Execute();
        });

        public DelegateCommand<Comment> EditCommentCommand => new DelegateCommand<Comment>((comment) =>
        {
            InputText = comment.Text;
            comment.IsEditing = true;
            editingComment = comment;
            Mode = Mode.Edit;
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
            Groups = new ObservableCollection<Group>(commentDbContext.GetGroup());
            Tags = commentDbContext.GetTags();

            if (SelectionGroup == null && Groups.Count != 0)
            {
                SelectionGroup = Groups.FirstOrDefault();
            }
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

        public DelegateCommand StartTagAdditionModeCommand => new DelegateCommand(() =>
        {
            SystemMessage = "タグをテキストボックスに入力してください";
            Mode = Mode.TagAddition;
        });

        public DelegateCommand ExitCommand => new DelegateCommand(() =>
        {
            System.Windows.Application.Current.Shutdown();
        });

        public DelegateCommand<Tag> AttachTagCommand => new DelegateCommand<Tag>((param) =>
        {
            var tagMap = new TagMap()
            {
                CommentId = SelectionComment.Id,
                TagId = param.Id,
            };

            commentDbContext.AddTagMap(tagMap);
            ReloadCommentCommand.Execute();
        });

        public DelegateCommand AddGroupCommand => new DelegateCommand(() =>
        {
            commentDbContext.AddGroup(new Group() { Name = "New group" });
            ReloadCommentCommand.Execute();
        });

        public DelegateCommand<Group> ChangeGroupNameCommand => new DelegateCommand<Group>((group) =>
        {
            group.EditMode = true;
        });

        public DelegateCommand<Group> ConfirmGroupNameCommand => new DelegateCommand<Group>((group) =>
        {
            group.EditMode = false;
            commentDbContext.SaveChanges();
        });

        private Mode Mode { get; set; } = Mode.Post;
    }
}
