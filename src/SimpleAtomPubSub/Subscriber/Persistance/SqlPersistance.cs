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
    public class SqlPersistance : IDeadLetterPersistance
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
	            (id,body,createdAt,lastAttemptedProcessingAt,feedUrl,processingAttemptCount,readyToRetry,exception,exceptionStack,beingRetried])
	            values
	            (@Id,@Body,@CreatedAt,@LastAttemptedProcessingAt,@FeedUrl,1,0,@Exception,@ExceptionStack,0)
            end
            commit";

        private const string PullRetryMessages = @"begin tran
            select Id, CreatedAt, Body, FeedUrl from dbo.[DeadLetter] where ReadyToRetry = 1 and BeingRetried = 0
            update dbo.[DeadLetter] set BeingRetried = 1 where ReadyToRetry = 1 and BeingRetried = 0
            commit";

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
    }
}

