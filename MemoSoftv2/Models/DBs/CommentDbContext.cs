using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MemoSoftv2.Models.DBs
{
    public class CommentDbContext : DbContext
    {
        public CommentDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public Group CurrentGroup { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<Comment> Comments { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<Tag> Tags { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<TagMap> TagMaps { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<Group> Groups { get; set; }

        private int SearchLimitCount { get; set; } = 100;

        public void AddComment(Comment c)
        {
            Comments.Add(c);
            SaveChanges();
        }

        public void AddTag(Tag tag)
        {
            Tags.Add(tag);
            SaveChanges();
        }

        public void AddTagMap(TagMap tagMap)
        {
            if (TagMaps.Any(tm => tagMap.TagId == tm.TagId && tm.CommentId == tagMap.CommentId))
            {
                return;
            }

            TagMaps.Add(tagMap);
            SaveChanges();
        }

        public void AddGroup(Group group)
        {
            Groups.Add(group);
            SaveChanges();
        }

        public void AddDefaultGroup(Group group)
        {
            if (!Groups.Any())
            {
                AddGroup(group);
            }
        }

        public List<Comment> GetComments()
        {
            var currentGroup = CurrentGroup ?? new Group() { Id = 1 };

            var favoriteComments = Comments.Where(c => c.IsFavorite && c.GroupId == currentGroup.Id)
                .Where(c => !c.IsSubComment)
                .OrderByDescending(c => c.CreationDateTime)
                .Take(SearchLimitCount)
                .ToList();

            var notFavoriteComments = Comments.Where(c => !c.IsFavorite && c.GroupId == currentGroup.Id)
                .Where(c => !c.IsSubComment)
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
                var resultList = new List<Comment>();
                var allSubCommentTable = Comments.Where(c => c.IsSubComment);

                favoriteComments.ForEach(c =>
                {
                    resultList.Add(c);

                    allSubCommentTable.Where(sc => sc.ParentCommentId == c.Id)
                        .OrderByDescending(sc => sc.CreationDateTime)
                        .ToList()
                        .ForEach(sc => resultList.Add(sc));
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
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder
            {
                Port = 5433,
                Username = "postgres",
                Password = "password",
                Host = "localhost",
                Database = "testdb",
            };

            optionsBuilder.UseNpgsql(builder.ToString());
        }
    }
}