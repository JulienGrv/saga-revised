using Saga.Data;
using Saga.Map;
using Saga.PrimaryTypes;
using System.Data;

namespace Saga.Scripting.Dummy_Templates
{
    internal static class CreditProvider
    {
        public static int LoadCredits(Character character)
        {
            IQueryProvider provider = Singleton.Database.GetQueryProvider();
            provider.CmdText = "SELECT Credits FROM list_credits WHERE CharId=@CharId";
            provider.Parameters.AddWithValue("CharId", character.ModelId);

            int credits = 0;
            using (IDataReader reader = Singleton.Database.ExecuteDataReader(provider, CommandBehavior.SingleResult))
            {
                while (reader.Read())
                {
                    credits = reader.GetInt32(0);
                }
            }

            return credits;
        }

        public static bool SaveCredits(Character character, int CreditValue)
        {
            IQueryProvider provider = Singleton.Database.GetQueryProvider();
            provider.CmdText = "UPDATE list_credits SET Credits=@Credits CharId=@CharId";
            provider.Parameters.AddWithValue("CharId", character.ModelId);
            provider.Parameters.AddWithValue("Credits", CreditValue);
            return Singleton.Database.ExecuteNonQuery(provider) > 0;
        }
    }
}