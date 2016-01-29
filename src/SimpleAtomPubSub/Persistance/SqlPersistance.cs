using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SimpleAtomPubSub.Persistance
{
    public class SqlPersistance : IEventPersistance
    {
        private const string InsertEvent =
            @"insert into dbo.[Events] (id, createdAt, body, feedId) select @id, @createdAt, @body, f.id from dbo.[Feeds] f where Uri = @feedUri";

        private const string GetEventsByFeedUri =
            @"select e.id, e.createdAt, e.body, f.uri from dbo.[Events] e join dbo.[Feeds] f on e.FeedId = f.Id where f.uri = @uri order by e.CreatedAt desc
                                                    select top 1 f2.Id, f2.Uri
                                                    from dbo.feeds f1 
                                                    join dbo.feeds f2 on f1.Uri = @uri and isnull(f1.CreatedAt, '9999-12-31') >= f2.CreatedAt and f1.Id <> f2.Id
                                                    order by f2.CreatedAt desc
                                                    select top 1 f2.Id, f2.Uri
                                                    from dbo.feeds f1 
                                                    join dbo.feeds f2 on f1.Uri = @uri and isnull(f1.CreatedAt, '9999-12-31') <= isnull(f2.CreatedAt, '9999-12-31') and f1.Id <> f2.Id
                                                    order by isnull(f2.CreatedAt, '9999-12-31') asc";

        private const string MoveEventsToNewFeed = @"declare @sourceFeedId int
                                    declare @newFeedId int
                                    select @sourceFeedId = Id from dbo.[Feeds] where Uri = @sourceUri
                                    if exists(select * from dbo.[Events] where feedId = @sourceFeedId)
                                    begin 
                                        insert into dbo.Feeds (Uri, CreatedAt) values (@newUri, @createdAt)
                                        select @newFeedId = @@IDENTITY
                                        update dbo.[Events] set feedId = @newFeedId where feedId = @sourceFeedId
                                    end";
        private readonly string _connectionString;

        public SqlPersistance(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public void AddToWorkingFeed(Message e)
        {
            using (var c = new SqlConnection(_connectionString))
            {
                var cmd = c.CreateCommand();
                cmd.CommandText = InsertEvent;
                cmd.Parameters.Add(new SqlParameter("@id", e.Id));
                cmd.Parameters.Add(new SqlParameter("@createdAt", e.CreatedAt));
                cmd.Parameters.Add(new SqlParameter("@body", e.Body));
                cmd.Parameters.Add(new SqlParameter("@feedUri", e.FeedUri));

                c.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public FeedData GetMessages(string feedUri)
        {
            var messages = new List<Message>();
            string prevUri = null;
            string nextUri = null;

            using (var c = new SqlConnection(_connectionString))
            {
                var cmd = c.CreateCommand();
                cmd.CommandText = GetEventsByFeedUri;
                cmd.Parameters.Add(new SqlParameter("@uri", feedUri));

                c.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(new Message
                        {
                            Body = (string) reader["body"],
                            CreatedAt = (DateTime) reader["createdAt"],
                            Id = (Guid) reader["id"],
                            FeedUri = (string) reader["uri"]
                        });
                    }

                    reader.NextResult();
                    while (reader.Read())
                    {
                        prevUri = (string) reader["Uri"];
                    }

                    reader.NextResult();
                    while (reader.Read())
                    {
                        nextUri = (string) reader["Uri"];
                    }
                }
            }

            return new FeedData
            {
                Messages = messages,
                NextUri = nextUri,
                PreviousUri = prevUri
            };
        }

        public void MoveToNewFeed(string sourceFeedUri, string archiveFeedUri, DateTime createdAt)
        {
            using (var c = new SqlConnection(_connectionString))
            {
                var cmd = c.CreateCommand();
                cmd.CommandText = MoveEventsToNewFeed;
                cmd.Parameters.Add(new SqlParameter("@sourceUri", sourceFeedUri));
                cmd.Parameters.Add(new SqlParameter("@newUri", archiveFeedUri));
                cmd.Parameters.Add(new SqlParameter("@createdAt", createdAt));

                c.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}