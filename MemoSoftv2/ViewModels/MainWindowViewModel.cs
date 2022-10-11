using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using MemoSoftv2.Models;
using MemoSoftv2.Models.DBs;
using MemoSoftv2.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MemoSoftv2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly DbContextWrapper dbContextWrapper;

        private string title = "MemoSoft v2";
        private ObservableCollection<Comment> comments;
        private List<Tag> tags;
        private ObservableCollection<Group> groups;
        private string inputText;
        private string systemMessage;
        private Comment editingComment;
        private Comment selectionComment;
        private Comment parentComment;
        private Group selectionGroup;
        private bool isTextBoxFocused;
        private Mode mode = Mode.Post;

        public MainWindowViewModel(IDialogService dialogService, DbContextWrapper dbContextWrapper)
        {
            this.dialogService = dialogService;
            this.dbContextWrapper = dbContextWrapper;
            ShowConnectionPageCommand.Execute();
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ObservableCollection<Comment> Comments
        {
            get => comments; private set => SetProperty(ref comments, value);
        }

        public ObservableCollection<Group> Groups
        {
            get => groups; private set => SetProperty(ref groups, value);
        }

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public string SystemMessage
        {
            get => systemMessage; private set => SetProperty(ref systemMessage, value);
        }

        public bool IsTextBoxFocused { get => isTextBoxFocused; set => SetProperty(ref isTextBoxFocused, value); }

        public Comment SelectionComment { get => selectionComment; set => SetProperty(ref selectionComment, value); }

        public Comment ParentComment
        {
            get => parentComment;
            private
            set
            {
                if (!value.IsSubComment)
                {
                    SetProperty(ref parentComment, value);
                }
            }
        }

        public Group SelectionGroup
        {
            get => selectionGroup;
            set
            {
                SetProperty(ref selectionGroup, value);
                dbContextWrapper.CommentDbContext.CurrentGroup = value;
            }
        }

        public DelegateCommand PostCommentCommand => new DelegateCommand(() =>
        {
            if (string.IsNullOrWhiteSpace(InputText))
            {
                return;
            }

            if (Mode == Mode.Post)
            {
                dbContextWrapper.CommentDbContext.AddComment(new Comment(InputText, DateTime.Now) { GroupId = SelectionGroup.Id });
            }
            else if (Mode == Mode.Edit)
            {
                // コメント編集中の場合
                editingComment.Text = InputText;
                editingComment.IsEditing = false;
                editingComment = null;
                dbContextWrapper.CommentDbContext.SaveChanges();
            }
            else if (Mode == Mode.TagAddition)
            {
                dbContextWrapper.CommentDbContext.AddTag(new Tag() { Name = InputText });
            }
            else if (Mode == Mode.SubComment)
            {
                dbContextWrapper.CommentDbContext.AddComment(new Comment()
                {
                    IsSubComment = true,
                    Text = InputText,
                    GroupId = SelectionGroup.Id,
                    ParentCommentId = ParentComment.Id,
                });
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
            dbContextWrapper.CommentDbContext.SaveChanges();
            ReloadCommentCommand.Execute();
        });

        public DelegateCommand CancelEditCommentCommand => new DelegateCommand(() =>
        {
            if (editingComment != null)
            {
                editingComment.IsEditing = false;
                editingComment = null;
            }

            ParentComment = null;
            InputText = string.Empty;
            Mode = Mode.Post;
        });

        public DelegateCommand ReloadCommentCommand => new DelegateCommand(() =>
        {
            Comments = new ObservableCollection<Comment>(dbContextWrapper.CommentDbContext.GetComments());
            Groups = new ObservableCollection<Group>(dbContextWrapper.CommentDbContext.GetGroup());
            Tags = dbContextWrapper.CommentDbContext.GetTags();

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

        public DelegateCommand<object> ChangeBackgroundColorCommand => new DelegateCommand<object>((colorBrush) =>
        {
            var mediaColor = ((SolidColorBrush)colorBrush).Color;
            SelectionComment.BackgroundColorArgb = System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B).ToArgb();
            dbContextWrapper.CommentDbContext.SaveChanges();
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

            dbContextWrapper.CommentDbContext.AddTagMap(tagMap);
            ReloadCommentCommand.Execute();
        });

        public DelegateCommand AddGroupCommand => new DelegateCommand(() =>
        {
            dbContextWrapper.CommentDbContext.AddGroup(new Group() { Name = "New group" });
            ReloadCommentCommand.Execute();
        });

        public DelegateCommand<Group> ChangeGroupNameCommand => new DelegateCommand<Group>((group) =>
        {
            group.EditMode = true;
        });

        public DelegateCommand<Group> ConfirmGroupNameCommand => new DelegateCommand<Group>((group) =>
        {
            group.EditMode = false;
            dbContextWrapper.CommentDbContext.SaveChanges();
        });

        public DelegateCommand SubCommentModeCommand => new DelegateCommand(() =>
        {
            if (SelectionComment.IsSubComment)
            {
                return;
            }

            ParentComment = SelectionComment;
            Mode = Mode.SubComment;
        });

        public DelegateCommand ShowConnectionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(ConnectionPage), default, _ => { });
            Connect(dbContextWrapper);
        });


        public Mode Mode { get => mode; set => SetProperty(ref mode, value); }

        // ReSharper disable once MemberCanBePrivate.Global
        public List<Tag> Tags { get => tags; set => SetProperty(ref tags, value); }

        private void Connect(DbContextWrapper wrapper)
        {
            bool connectionSuccess = true;

            try
            {
                wrapper.RecreateDbContext();
                wrapper.CommentDbContext.AddDefaultGroup(new Group() { Name = "Default Group", Id = 1 });
            }
            catch (Exception e)
            {
                if (e is Npgsql.NpgsqlException or System.Net.Sockets.SocketException)
                {
                    SystemMessage = "PostgreSQL データベースへの接続に失敗しました";
                    connectionSuccess = false;
                }
                else
                {
                    throw;
                }
            }

            if (connectionSuccess)
            {
                SystemMessage = "PostgreSQL データベースへ接続しました";
                ReloadCommentCommand.Execute();
            }
        }
    }
}