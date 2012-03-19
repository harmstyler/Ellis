using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blend.Ellis.JobOutputs;
using System.Xml;
using System.Data.SqlClient;

namespace Blend.Ellis.JobOutputs
{
    class Sql : JobOutputBase
    {
        private Dictionary<string, string> mappings;

        public void Initialize(XmlNode settingsNode)
        {
            mappings = new Dictionary<string, string>();
            foreach (XmlNode mappingNode in settingsNode.SelectNodes("sqlMapping"))
            {
                mappings.Add(mappingNode.Attributes["fieldName"].Value, mappingNode.Attributes["partName"].Value);
            }
        }

        public override void Execute(Job thisJob)
        {
            foreach (Page thisPage in thisJob.Pages)
            {
                // For each, see if it exists as a database row
                SqlCommand command = new SqlCommand();
                command.Connection = new SqlConnection(Settings["connectionString"]);
                command.Connection.Open();
                command.CommandText = String.Format("SELECT COUNT(*) FROM {0} WHERE {1} = @Url", Settings["tableName"], Settings["urlFieldName"]);
                command.Parameters.AddWithValue("@Url", thisPage.Url.AbsoluteUri);

                StringBuilder sql = new StringBuilder();
                if ((int)command.ExecuteScalar() == 0)
                {
                    // It does NOT exist, so we're doing an INSERT
                    sql.Append("INSERT INTO ");
                    sql.Append(Settings["tableName"]);
                    sql.Append(" (");
                    sql.Append(String.Join(", ", mappings.Select(kvp => kvp.Key).ToArray()));
                    sql.Append(") VALUES (");
                    sql.Append(String.Join(", ", mappings.Select(kvp => "@" + kvp.Key).ToArray()));
                    sql.Append(")");
                }
                else
                {
                    // It DOES exist, so we're doing an update
                    sql.Append("UPDATE ");
                    sql.Append(Settings["tableName"]);
                    sql.Append(" SET ");
                    sql.Append(String.Join(", ", mappings.Select(kvp => String.Format("{0} = @{1}", kvp.Key, kvp.Key)).ToArray()));
                    sql.Append(" WHERE ");
                    sql.Append(Settings["urlFieldName"]);
                    sql.Append(" = @url");
                }

                command.CommandText = sql.ToString();
                command.Parameters.Clear();

                // Loop through each of the mappings and add a parameter for it.
                foreach (KeyValuePair<string, string> mapping in mappings)
                {
                    string value = String.Empty;
                    if (thisPage.GetPart(mapping.Value) != null)
                    {
                        value = thisPage.GetPart(mapping.Value).Content;
                    }

                    command.Parameters.AddWithValue(mapping.Key, value);
                }

                // Add the URL as a param too
                command.Parameters.AddWithValue("@" + Settings["urlFieldName"], thisPage.GetPart("_Url").Content);
                
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Log.Write("SQL ERROR on page ", thisPage.Url.AbsoluteUri.DoubleQuoted(), ": ", ex.Message);
                }
                

            }
        }
    }
}
