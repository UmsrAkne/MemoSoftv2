using System.Diagnostics;

namespace MemoSoftv2.Models.DBs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    public class CommentDbContext : DbContext
    {
        public CommentDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        private DbSet<Comment> Comments { get; set; }

        private DbSet<Tag> Tags { get; set; }

        private DbSet<TagMap> TagMaps { get; set; }

        private DbSet<Group> Groups { get; set; }

        private DbSet<SubComment> SubComments { get; set; }

        public Group CurrentGroup { get; set; }

        private int SearchLimitCount { get; set; } = 100;

        public void AddComment(Comment c)
        {
            Comments.Add(c);
            SaveChanges();
        }

        public void AddSubComment(Comment parent, SubComment child)
        {
            child.ParentCommentId = parent.Id;
            SubComments.Add(child);
            SaveChanges();
        }

        public void AddTag(Tag tag)
        {
            Tags.Add(tag);
            SaveChanges();
        }

        public void AddTagMap(TagMap tagmap)
        {
            if (TagMaps.Any(tm => tagmap.TagId == tm.TagId && tm.CommentId == tagmap.CommentId))
            {
                return;
            }

            TagMaps.Add(tagmap);
            SaveChanges();
        }

        public void AddGroup(Group group)
        {
            Groups.Add(group);
            SaveChanges();
        }

        public void AddDefaultGroup(Group group)
        {
            if (Groups.Count() == 0)
            {
                AddGroup(group);
            }
        }

        public List<Comment> GetComments()
        {
            var currentGroup = CurrentGroup == null ? new Group() { Id = 1 } : CurrentGroup;

            var favoriteComments = Comments.Where(c => c.IsFavorite && c.GroupId == currentGroup.Id)
                .OrderByDescending(c => c.CreationDateTime)
                .Take(SearchLimitCount)
                .ToList();

            var notFavoriteComments = Comments.Where(c => !c.IsFavorite && c.GroupId == currentGroup.Id)
                .OrderByDescending(c => c.CreationDateTime)
                .Take(SearchLimitCount)
                .ToList();

            favoriteComments.AddRange(notFavoriteComments);

            // タグマップテーブルとタグテーブルを結合する。戻ってくるリストは匿名型
            var tags = TagMaps.Join(
                Tags,
                tm => tm.TagId,
                t => t.Id,
                (tm, t) => new { tm.Id, tm.CommentId, t.Name, });

            // GroupBy で結合したタグテーブルを CommentId でグルーピング
            var groupingTags = tags.GroupBy(t => t.CommentId);

            // CommentId 一つに対して付いているタグを結合して一つの文字列にする
            var tagNamesTable = groupingTags.Select(
                x => new
                {
                    commentId = x.Key,
                    name = string.Join(", ", x.OrderBy(a => a.Name).Select(a => a.Name)),
                });

            // 内部結合でタグがついているコメントのリストを作成して ForEach
            favoriteComments.Join(
                tagNamesTable,
                c => c.Id,
                t => t.commentId,
                (c, t) => new { comment = c, tag = t })
                .ToList()
                .ForEach(c => c.comment.Tag = c.tag.name);

            if (favoriteComments.Count != 0)
            {
                var maxId = favoriteComments.Max(c => c.Id);
                var minId = favoriteComments.Min(c => c.Id);

                var subComments = SubComments.Where(c => c.ParentCommentId <= maxId && c.ParentCommentId >= minId).ToList();
                var resultList = new List<Comment>();

                favoriteComments.ForEach(c =>
                {
                    resultList.Add(c);
                    resultList.AddRange(subComments
                        .Where(sc => sc.ParentCommentId == c.Id)
                        .OrderByDescending(sc => sc.CreationDateTime));
                });

                return resultList;
            }

            return favoriteComments;
        }

        public List<Tag> GetTags()
        {
            return Tags.Where(t => true).ToList();
        }

        public List<Group> GetGroup()
        {
            return Groups.Where(g => true).OrderBy(g => g.Id).ToList();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

            builder.Port = 5433;
            builder.Username = "postgres";
            builder.Password = "password";
            builder.Host = "localhost";
            builder.Database = "testdb";

            optionsBuilder.UseNpgsql(builder.ToString());
        }
    }
}