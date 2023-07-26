using System.Data;
using System.Data.SqlClient;

namespace DBComparer
{
    public class SqlServerChecker
    {

        public bool CheckServerExists(string serverAddress)
        {
            using (SqlConnection connection = new SqlConnection($"Server={serverAddress};Integrated Security=true;"))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        public bool CheckServerAndDatabaseExists(string serverAddress, string databaseName)
        {
            using (SqlConnection connection = new SqlConnection($"Server={serverAddress};Database={databaseName};Integrated Security=true;"))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }


        public List<string> GetDatabaseTables(string serverAddress, string databaseName)
        {
            List<string> tables = new List<string>();

            using (SqlConnection connection = new SqlConnection($"Server={serverAddress};Integrated Security=true;Database={databaseName};"))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");

                //foreach (DataRow row in schema.Rows)
                //{
                //    string tableName = row["TABLE_NAME"].ToString();
                //    tables.Add(tableName);
                //}



                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row["TABLE_NAME"].ToString();
                    string tableType = row["TABLE_TYPE"].ToString();

                    if (tableType == "BASE TABLE")
                    {
                        tables.Add(tableName);
                    }
                }



            }

            return tables;
        }
        public Dictionary<string, List<ColumnDefinition>> GetTableSchema(string serverAddress, string databaseName)
        {
            Dictionary<string, List<ColumnDefinition>> tableSchema = new Dictionary<string, List<ColumnDefinition>>();

            using (SqlConnection connection = new SqlConnection($"Server={serverAddress};Integrated Security=true;Database={databaseName};"))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");

                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row["TABLE_NAME"].ToString();
                    DataTable tableInfo = connection.GetSchema("Columns", new[] { null, null, tableName });

                    List<ColumnDefinition> columns = new List<ColumnDefinition>();

                    // Obtener las columnas que son primary key utilizando la vista INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                    string primaryKeyQuery = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{tableName}' AND CONSTRAINT_NAME LIKE '%PK%'";
                    List<string> primaryKeys = new List<string>();

                    using (SqlCommand command = new SqlCommand(primaryKeyQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["COLUMN_NAME"].ToString();
                                primaryKeys.Add(columnName);
                            }
                        }
                    }

                    foreach (DataRow columnRow in tableInfo.Rows)
                    {
                        string columnName = columnRow["COLUMN_NAME"].ToString();
                        string dataType = columnRow["DATA_TYPE"].ToString();
                        int maxLength = columnRow["CHARACTER_MAXIMUM_LENGTH"] == System.DBNull.Value ? 0 : (Convert.ToInt32(columnRow["CHARACTER_MAXIMUM_LENGTH"]));
                        List<Constraint> constraints = new List<Constraint>();
                        ColumnDefinition columnDefinition = new ColumnDefinition(columnName, dataType, maxLength, constraints);

                        // Verificar si la columna es primary key
                        if (primaryKeys.Contains(columnName))
                        {
                            columnDefinition.IsPrimaryKey = true;
                        }

                        // Obtener constraints de la columna utilizando la vista INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                        string constraintsQuery = $"SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'";
                        using (SqlCommand command = new SqlCommand(constraintsQuery, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string constraintName = reader["CONSTRAINT_NAME"].ToString();

                                    // Agregar información de constraint a la definición de la columna
                                    columnDefinition.Constraints.Add(new Constraint(constraintName));
                                }
                            }
                        }

                        columns.Add(columnDefinition);
                    }

                    tableSchema.Add(tableName, columns);
                }
            }

            return tableSchema;
        }



        public Dictionary<string, List<ColumnDefinition>> GetTableSchemaByTableName(string serverAddress, string databaseName, string tableNameSelected)
        {
            Dictionary<string, List<ColumnDefinition>> tableSchema = new Dictionary<string, List<ColumnDefinition>>();

            using (SqlConnection connection = new SqlConnection($"Server={serverAddress};Integrated Security=true;Database={databaseName};"))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");
               // DataRow[] rslt = schema.Select("Table Name = '"{tableNameSelected}"');
                //DataRow[] rslt = tbl_1.Select("Size_of_team >= 123 AND Team_work = 'd'");
                //foreach (DataRow row in rslt)
                //      foreach (DataRow row in schema.Select("Table Name = '{tableNameSelected}'"))
                //{


                    DataRow[] resultRow = schema.Select($"Table_Name = '{tableNameSelected}'");
                foreach (DataRow newRow in resultRow)
                {
                    Console.WriteLine("Name: {0} Age: {1} Salary: {2}", newRow[0],
                                            newRow[1], newRow[2]);
                }


                string tableName = tableNameSelected;// row["TABLE_NAME"].ToString();
                    DataTable tableInfo = connection.GetSchema("Columns", new[] { null, null, tableName });

                    List<ColumnDefinition> columns = new List<ColumnDefinition>();

                    // Obtener las columnas que son primary key utilizando la vista INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                    string primaryKeyQuery = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{tableName}' AND CONSTRAINT_NAME LIKE '%PK%'";
                    List<string> primaryKeys = new List<string>();

                    using (SqlCommand command = new SqlCommand(primaryKeyQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["COLUMN_NAME"].ToString();
                                primaryKeys.Add(columnName);
                            }
                        }
                    }

                    foreach (DataRow columnRow in tableInfo.Rows)
                    {
                        string columnName = columnRow["COLUMN_NAME"].ToString();
                        string dataType = columnRow["DATA_TYPE"].ToString();
                        int maxLength = columnRow["CHARACTER_MAXIMUM_LENGTH"] == System.DBNull.Value ? 0 : (Convert.ToInt32(columnRow["CHARACTER_MAXIMUM_LENGTH"]));
                        List<Constraint> constraints = new List<Constraint>();
                        ColumnDefinition columnDefinition = new ColumnDefinition(columnName, dataType, maxLength, constraints);

                        // Verificar si la columna es primary key
                        if (primaryKeys.Contains(columnName))
                        {
                            columnDefinition.IsPrimaryKey = true;
                        }

                        // Obtener constraints de la columna utilizando la vista INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                        string constraintsQuery = $"SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'";
                        using (SqlCommand command = new SqlCommand(constraintsQuery, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string constraintName = reader["CONSTRAINT_NAME"].ToString();

                                    // Agregar información de constraint a la definición de la columna
                                    columnDefinition.Constraints.Add(new Constraint(constraintName));
                                }
                            }
                        }

                        columns.Add(columnDefinition);
                    }

                    tableSchema.Add(tableName, columns);
                }
            

            return tableSchema;
        }




        public List<string> GetViews(string serverAddress, string databaseName)
        {
            List<string> views = new List<string>();

            using (SqlConnection connection = new SqlConnection($"Server={serverAddress};Integrated Security=true;Database={databaseName};"))
            {
                connection.Open();

                // Obtener las vistas utilizando la vista INFORMATION_SCHEMA.VIEWS
                string query = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'dbo'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string viewName = reader["TABLE_NAME"].ToString();
                            views.Add(viewName);
                        }
                    }
                }
            }

            return views;
        }




        public Dictionary<string, string> GetViewDefinitions(string serverAddress, string databaseName)
        {
            Dictionary<string, string> viewDefinitions = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection($"Server={serverAddress};Integrated Security=true;Database={databaseName};"))
            {
                connection.Open();

                // Obtener los nombres de las vistas utilizando la vista INFORMATION_SCHEMA.VIEWS
                string query = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'dbo'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string viewName = reader["TABLE_NAME"].ToString();

                            // Obtener la definición de la vista utilizando una consulta SQL adicional
                            string definitionQuery = $"SELECT OBJECT_DEFINITION(OBJECT_ID('{viewName}'))";
                            using (SqlCommand definitionCommand = new SqlCommand(definitionQuery, connection))
                            {
                                string viewDefinition = definitionCommand.ExecuteScalar()?.ToString();
                                viewDefinitions.Add(viewName, viewDefinition);
                            }
                        }
                    }
                }
            }

            return viewDefinitions;
        }




        public List<string> GetProcedures(string serverAddress, string databaseName)
        {
            List<string> storedProcedures = new List<string>();

            using (SqlConnection connection = new SqlConnection($"Server={serverAddress};Integrated Security=true;Database={databaseName};"))
            {
                connection.Open();

                // Obtener los nombres de los stored procedures utilizando la vista sys.procedures
                string query = "SELECT name FROM sys.procedures";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string storedProcedureName = reader["name"].ToString();
                            storedProcedures.Add(storedProcedureName);
                        }
                    }
                }
            }

            return storedProcedures;
        }



        public Dictionary<string, string> GetAllStoredProcedureDefinitions(string serverAddress, string databaseName)
        {
            Dictionary<string, string> storedProcedureDefinitions = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection($"Server={serverAddress};Integrated Security=true;Database={databaseName};"))
            {
                connection.Open();

                // Obtener los nombres y definiciones de los stored procedures utilizando la vista sys.sql_modules
                string query = "SELECT name, definition FROM sys.sql_modules INNER JOIN sys.procedures ON sys.sql_modules.object_id = sys.procedures.object_id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string storedProcedureName = reader["name"].ToString();
                            string storedProcedureDefinition = reader["definition"].ToString();
                            storedProcedureDefinitions.Add(storedProcedureName, storedProcedureDefinition);
                        }
                    }
                }
            }

            return storedProcedureDefinitions;
        }


    }
}









