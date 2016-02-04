using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAtomPubSub.Publisher.Persistance;

namespace SimpleAtomPubSub.Subscriber.Persistance
{
    public class SqlPersistance : IDeadLetterPersistance, ISubscriptionPersistance
    {
        private readonly string _connectionString;

        private const string InsertOrUpdateIntoDeadLetter = @"begin tran
            if (exists(select * from dbo.DeadLetter where Id = @Id))
            begin
	            update dbo.DeadLetter
                set 
                LastAttemptedProcessingAt = @LastAttemptedProcessingAt,
                ProcessingAttemptCount = ProcessingAttemptCount + 1,
                Exception = @Exception,
                ExceptionStack = @ExceptionStack,
                ReadyToRetry = 0,
                BeingRetried = 0
                where Id = @Id
            end
            else
            begin
	            insert into dbo.DeadLetter
	            (id,body,createdAt,lastAttemptedProcessingAt,feedUrl,processingAttemptCount,readyToRetry,exception,exceptionStack,beingRetried)
	            values
	            (@Id,@Body,@CreatedAt,@LastAttemptedProcessingAt,@FeedUrl,1,0,@Exception,@ExceptionStack,0)
            end
            commit";

        private const string PullRetryMessages = @"begin tran
            select Id, CreatedAt, Body, FeedUrl from dbo.[DeadLetter] where ReadyToRetry = 1 and BeingRetried = 0
            update dbo.[DeadLetter] set BeingRetried = 1 where ReadyToRetry = 1 and BeingRetried = 0
            commit";

        private const string RegisterSubscrition = @"if (not exists(select * from dbo.Subscriptions where Url = @Url))
            begin
	            insert into dbo.Subscriptions
	            (Url, LastObservedEventId)
	            values
	            (@Url, null)	
            end
            select Id from dbo.Subscriptions where url = @Url";

        private const string UpdateLastSeenEvent = @"update dbo.Subscriptions set LastObservedEventId = @LastObservedEventId where Id = @Id";

        private const string GetLastObservedEvent = @"select LastObservedEventId from dbo.Subscriptions where Id = @Id";

        public SqlPersistance(string connectionStringName)
        {
            var cs = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (cs == null)
                throw new ConfigurationErrorsException($"App/Web.config file is missing connection string with name '{connectionStringName}'.");

            _connectionString = cs.ConnectionString;
        }

        public void Deadletter(Message message, Exception exception)
        {
            using (var c = new SqlConnection(_connectionString))
            {
                var cmd = c.CreateCommand();
                cmd.CommandText = InsertOrUpdateIntoDeadLetter;

                cmd.Parameters.Add(new SqlParameter("@Id", message.Id));
                cmd.Parameters.Add(new SqlParameter("@Body", message.Body));
                cmd.Parameters.Add(new SqlParameter("@CreatedAt", message.CreatedAt));
                cmd.Parameters.Add(new SqlParameter("@LastAttemptedProcessingAt", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@FeedUrl", message.FeedUri));
                cmd.Parameters.Add(new SqlParameter("@Exception", exception.Message));
                cmd.Parameters.Add(new SqlParameter("@ExceptionStack", exception.StackTrace));

                c.Open();
                cmd.ExecuteNonQuery();
            }
        }
        
        public IEnumerable<Message> PullRetries()
        {
            using (var c = new SqlConnection(_connectionString))
            {
                var cmd = c.CreateCommand();
                cmd.CommandText = PullRetryMessages;
                
                c.Open();
                var reader = cmd.ExecuteReader();
                var result = new List<Message>();
                while (reader.Read())
                {
                    result.Add(new Message()
                    {
                        Id = (Guid)reader["Id"],
                        Body = (string)reader["Body"],
                        CreatedAt = (DateTime)reader["CreatedAt"],
                        FeedUri = (string)reader["FeedUrl"]
                    });
                }

                return result;
            }
        }

        public int Register(string url)
        {
            using (var c = new SqlConnection(_connectionString))
            {
                var cmd = c.CreateCommand();
                cmd.CommandText = RegisterSubscrition;

                cmd.Parameters.Add(new SqlParameter("@Url", url));

                c.Open();
                return (int) cmd.ExecuteScalar();
            }
        }

        public void SetLastObservedEvent(int id, Guid eventId)
        {
            using (var c = new SqlConnection(_connectionString))
            {
                var cmd = c.CreateCommand();
                cmd.CommandText = UpdateLastSeenEvent;

                cmd.Parameters.Add(new SqlParameter("@Id", id));
                cmd.Parameters.Add(new SqlParameter("@LastObservedEventId", eventId));

                c.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Guid? GetLastObservedEventId(int id)
        {
            using (var c = new SqlConnection(_connectionString))
            {
                var cmd = c.CreateCommand();
                cmd.CommandText = GetLastObservedEvent;

                cmd.Parameters.Add(new SqlParameter("@Id", id));

                c.Open();
                return (Guid?)cmd.ExecuteScalar();
            }
        }
    }
}

