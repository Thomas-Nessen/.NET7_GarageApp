using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace DBHelpers
{
    public class DBCommander
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly List<Parameter> _parameters = new();

        private string _connection = "";
        private bool _isstp;
        private string _qry = "";

        private readonly bool _isLogged;

        private const int _maxlengthlogoutput = 500;
        private bool _hasoutputparameter = false;

        // public DBCommander() { }

        public DBCommander(string connection, bool isStp, string query, bool isLogged = true)
        {
            _connection = connection;
            _isstp = isStp;
            _qry = query;
            _isLogged = isLogged;

            // every stp returns a value
            AddParam("@returnvalue", null);
        }

        public string DbConnection
        {
            set
            {
                _connection = value;
            }
        }

        public string StoredProcedure
        {
            set
            {
                _qry = value;
            }
        }

        public bool QueryType
        {
            set
            {
                _isstp = value;
            }
        }

        public void AddParam(string? n, object? v)
        {
            // if object v is null --> than this is an SQL output parameter
            _parameters.Add(new Parameter { ParName = n, ParValue = v });
        }

#region Async methodes

        public async Task<int> ExecuteStpNonQueryAsync()
        {
            int retval;

            // use this for all stp's that handle Update, Create and Delete
            try
            {
                using SqlConnection conn = new(_connection);

                SqlCommand cmd = new()
                {
                    Connection = conn,
                };

                cmd.CommandText = _qry;
                cmd.CommandType = (_isstp) ?  CommandType.StoredProcedure : CommandType.Text;

                cmd.Parameters.AddRange(GetParametersFromAddParam());

                await conn.OpenAsync();

                await cmd.ExecuteNonQueryAsync();

                if (_isstp)
                {
                    // standard value returned by stored procedure
                    retval = (int)cmd.Parameters["@returnvalue"].Value;
                }
                else
                {
                    retval = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                await conn.CloseAsync();

                if (_isLogged)
                    _logger.Info($"Execute NonQuery ASYNC|Source: {_qry}|Parameters: {GetParameterList()}|Result: {retval}");

                return retval;

            }
            catch (Exception ex)
            {
                retval = -9;
                _logger.Error($"Execute NonQuery ASYNC|Unexpected Error: {ex.Message}|Source: { _qry}|Result: {retval}");
            }

            // unexpected error
            return retval;
        }

        public async Task<T?> RetrieveFirstFromDataReaderAsync<T>(Func<IDataRecord, T> data)
        {
            _ = data;

            using SqlConnection conn = new(_connection);

            SqlCommand cmd = new()
            {
                Connection = conn
            };

            cmd.CommandText = _qry;
            cmd.CommandType = (_isstp) ? CommandType.StoredProcedure : CommandType.Text;

            cmd.Parameters.AddRange(GetParametersFromAddParam());

            List<T> list = new();

            try
            {
                await conn.OpenAsync();

                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                if (rdr.HasRows)
                {
                    var properties = typeof(T).GetProperties();

                    while (rdr.Read())
                    {
                        var item = Activator.CreateInstance<T>();

                        foreach (var property in properties)
                        {
                            if (HasColumn(rdr, property.Name) && !rdr.IsDBNull(rdr.GetOrdinal(property.Name)))
                            {
                                Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                property.SetValue(item, Convert.ChangeType(rdr[property.Name], convertTo), null);
                            }
                        }

                        list.Add(item);
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"Retrieve First ASYNC|Unexpected Error: {ex.Message}|Source: { _qry}");
            }
            finally
            {
                try
                {
                    // _hasoutputparameter is set in GetParameters method
                    // NB:
                    // output parameters have negative id's
                    // they have the same order as they were given in the AddParam method, like so:
                    // AddParam("firstOutputParameter", null);
                    // AddParam("secondOutputParameter", null);
                    // ---> this will return a resultset with:
                    //		-1, value of firstOutputParameter
                    //		-2, value of secondOutputParameter
                    if (_hasoutputparameter)
                    {
                        int outputID = 0;

                        for (int t = 0; t < cmd.Parameters.Count; t++)
                        {
                            if (cmd.Parameters[t].Direction == ParameterDirection.Output)
                            {
                                outputID++;
                                var properties = typeof(T).GetProperties();
                                var jtem = Activator.CreateInstance<T>();

                                int counter = 0;

                                foreach (var property in properties)
                                {
                                    if (property.PropertyType.Name.ToLower() == "string")
                                    {
                                        switch (counter)
                                        {
                                            case 0:
                                                property.SetValue(jtem, "-" + outputID.ToString());
                                                break;
                                            case 1:
                                                string? s = cmd.Parameters[t].Value.ToString();
                                                property.SetValue(jtem, s);
                                                break;
                                        }
                                        counter++;
                                        if (counter > 1) break;
                                    }

                                }

                                list.Add(jtem);
                            }
                        }
                    }
                }
                catch (Exception exfinally)
                {
                    _logger.Error($"Retrieve First ASYNC|OUTPUTPARAM Error: {exfinally.Message}");
                }

                await conn.CloseAsync();
            }

            if (list.Count >= 1)
            {
                if (_isLogged)
                {
                    string jsn = JsonSerializer.Serialize(list[0]);
                    string jsn_short = (jsn.Length > _maxlengthlogoutput) ? jsn.Substring(0, _maxlengthlogoutput) + " (" + jsn.Length + ")" : jsn;
                    _logger.Info($"Retrieve First ASYNC|Source: {_qry}|Parameters: {GetParameterList()}|Count: {list.Count}|ResultFirstRow: {jsn_short}");
                }
                return list[0];
            }

            return default;
        }

        public async Task<List<T>> RetrieveListFromDataReaderAsync<T>(Func<IDataRecord, T> data)
        {
            _ = data;

            using SqlConnection conn = new(_connection);

            SqlCommand cmd = new()
            {
                Connection = conn
            };

            cmd.CommandText = _qry;
            cmd.CommandType = (_isstp) ? CommandType.StoredProcedure : CommandType.Text;

            cmd.Parameters.AddRange(GetParametersFromAddParam());

            List<T> list = new();

            try
            {
                await conn.OpenAsync();

                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                if (rdr.HasRows)
                {
                    var properties = typeof(T).GetProperties();

                    while (rdr.Read())
                    {
                        var item = Activator.CreateInstance<T>();

                        foreach (var property in properties)
                        {
                            if (HasColumn(rdr, property.Name) && !rdr.IsDBNull(rdr.GetOrdinal(property.Name)))
                            {
                                Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                property.SetValue(item, Convert.ChangeType(rdr[property.Name], convertTo), null);
                            }
                        }

                        list.Add(item);
                    }

                }

                await rdr.CloseAsync();

            }
            catch (Exception ex)
            {
                _logger.Error($"Retrieve List ASYNC|Unexpected Error: {ex.Message}|Source: {_qry}");
            }
            finally
            {
                // _hasoutputparameter is set in GetParameters method
                // NB:
                // output parameters have negative id's
                // they have the same order as they were given in the AddParam method, like so:
                // AddParam("firstOutputParameter", null);
                // AddParam("secondOutputParameter", null);
                // ---> this will return a resultset with:
                //		-1, value of firstOutputParameter
                //		-2, value of secondOutputParameter
                try
                {
                    if (_hasoutputparameter)
                    {
                        int outputID = 0;

                        for (int t = 0; t < cmd.Parameters.Count; t++)
                        {
                            if (cmd.Parameters[t].Direction == ParameterDirection.Output)
                            {
                                outputID++;
                                var properties = typeof(T).GetProperties();
                                var jtem = Activator.CreateInstance<T>();

                                int counter = 0;

                                foreach (var property in properties)
                                {
                                    if (property.PropertyType.Name.ToLower() == "string")
                                    {
                                        switch (counter)
                                        {
                                            case 0:
                                                property.SetValue(jtem, "-" + outputID.ToString());
                                                break;
                                            case 1:
                                                string? s = cmd.Parameters[t].Value.ToString();
                                                property.SetValue(jtem, s);
                                                break;
                                        }
                                        counter++;
                                        if (counter > 1) break;
                                    }

                                }

                                list.Add(jtem);
                            }
                        }
                    }
                }
                catch (Exception exfinally)
                {
                    _logger.Error($"Retrieve List ASYNC|OUTPUTPARAM Error: {exfinally.Message}");
                }

                await conn.CloseAsync();

                if (list.Count > 0 && _isLogged)
                {
                    string jsn = JsonSerializer.Serialize(list[0]);
                    string jsn_short = (jsn.Length > _maxlengthlogoutput) ? jsn.Substring(0, _maxlengthlogoutput) + " (" + jsn.Length + ")" : jsn;
                    _logger.Info($"Retrieve List ASYNC|Source: { _qry}|Parameters: {GetParameterList()}|Count: {list.Count}|ResultExample: {jsn_short}");
                }
            }

            return list;

        }

#endregion



#region private methods

        private SqlParameter[] GetParametersFromAddParam()
        {
            int numberOfParams = _parameters.Count;
            int paramCounter = 0;
            SqlParameter[] sqlparams = new SqlParameter[numberOfParams];

            foreach (Parameter p in _parameters)
            {
                if (p.ParValue == null) // indicator of output variable
                {
                    if (p.ParName == "@returnvalue") // special case
                    {
                        sqlparams[paramCounter] = new SqlParameter(p.ParName, SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                    }
                    else
                    {
                        // all OUTPUT params return a string of 100 NVarChars
                        sqlparams[paramCounter] = new SqlParameter(p.ParName, SqlDbType.NVarChar, 100)
                        {
                            Direction = ParameterDirection.Output
                        };

                        // set flag for detection later
                        if (!_hasoutputparameter) _hasoutputparameter = true;
                    }

                }
                else
                {
                    sqlparams[paramCounter] = new SqlParameter(p.ParName, p.ParValue)
                    {
                        Direction = ParameterDirection.Input
                    };

                }

                if (paramCounter < numberOfParams) paramCounter++;
            }

            return sqlparams;
        }

        private string GetParameterList()
        {
            string list = "";

            foreach (Parameter p in _parameters)
            {

                list += p.ParName + ": " + ((p.ParValue == null) ? "OUTPUT" : p.ParValue.ToString()) + "; ";

            }

            return list;
        }

        private static bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

#endregion

    }
}

internal class Parameter
{
    public string? ParName { get; set; }
    public object? ParValue { get; set; }
}
