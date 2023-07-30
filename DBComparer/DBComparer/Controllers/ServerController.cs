using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DBComparer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServerController : ControllerBase
    {
        // GET: ServerController

        private readonly SqlServerChecker _sqlServerChecker;

        public ServerController(SqlServerChecker sqlServerChecker)
        {
            _sqlServerChecker = sqlServerChecker;
        }

        [HttpGet("serverAddress")]
        public IActionResult CheckServer([FromQuery]  string serverAddress)
        {
            bool serverExists = _sqlServerChecker.CheckServerExists(serverAddress);
            return Ok(serverExists);
        }

        [HttpGet("serverAddressAndDb")]
        public IActionResult CheckServerAndDatabase([FromQuery] string serverAddress, string databaseName)
        {
            bool serverExists = _sqlServerChecker.CheckServerAndDatabaseExists(serverAddress, databaseName);
            return Ok(serverExists);
        }



        [HttpGet("{serverAddress}/{databaseName}/tables")]
        public IActionResult GetDatabaseTables(string serverAddress, string databaseName)
        {
            List<string> tables = _sqlServerChecker.GetDatabaseTables(serverAddress, databaseName);
            return Ok(tables);
        }

        [HttpGet("{serverAddress}/{databaseName}/table-schema")]
        public IActionResult GetTableSchema(string serverAddress, string databaseName)
        {
            Dictionary<string, List<ColumnDefinition>> tableSchema = _sqlServerChecker.GetTableSchema(serverAddress, databaseName);
            return Ok(tableSchema);
        }



        [HttpGet("{serverAddress}/{databaseName}/views")]
        public IActionResult GetViews(string serverAddress, string databaseName)
        {
            var views = _sqlServerChecker.GetViews(serverAddress, databaseName);
            return Ok(views);
        }


        [HttpGet("{serverAddress}/{databaseName}/views-schema")]
        public IActionResult GetViewDefinitions(string serverAddress, string databaseName)
        {
            var viewDefinitions = _sqlServerChecker.GetViewDefinitions(serverAddress, databaseName);
            return Ok(viewDefinitions);
        }

        [HttpGet("{serverAddress}/{databaseName}/proceduress")]
        public IActionResult GetProcedures(string serverAddress, string databaseName)
        {
            var procedures = _sqlServerChecker.GetProcedures(serverAddress, databaseName);
            return Ok(procedures);
        }

        [HttpGet("{serverAddress}/{databaseName}/procedures-schema")]
        public IActionResult GetProceduresDefinitions(string serverAddress, string databaseName)
        {
            var proceduresDefinitions = _sqlServerChecker.GetAllStoredProcedureDefinitions(serverAddress, databaseName);
            return Ok(proceduresDefinitions);
        }


        [HttpGet("compare-tables")]
        public IActionResult GetTablesDifferences([FromQuery] string serverAddress1, string databaseName1, string serverAddress2, string databaseName2)
        {

            #region Tables

           

            // Obtener las listas de tablas de ambos servidores y bases de datos
            List<string> tables1 = _sqlServerChecker.GetDatabaseTables(serverAddress1, databaseName1);
            List<string> tables2 = _sqlServerChecker.GetDatabaseTables(serverAddress2, databaseName2);

            // Comparar las listas de tablas y obtener las tablas que faltan en cada servidor
            List<string> missingTablesInServer1 = tables2.Except(tables1).ToList();
            List<string> missingTablesInServer2 = tables1.Except(tables2).ToList();


            // Comparar las definiciones de estructuras de tablas

            List<string> differentTables = new List<string>();
            foreach (string tableName in tables1.Intersect(tables2))
            {
                Dictionary<string, List<ColumnDefinition>> tableSchema1 = _sqlServerChecker.GetTableSchemaByTableName(serverAddress1, databaseName1, tableName);
                Dictionary<string, List<ColumnDefinition>> tableSchema2 = _sqlServerChecker.GetTableSchemaByTableName(serverAddress2, databaseName2, tableName);


                bool listasIguales = tableSchema1[tableName].SequenceEqual(tableSchema2[tableName], new ColumnDefinitionComparer());
                // bool listasIguales = tableSchema1[tableName].SequenceEqual(tableSchema2[tableName]);

                if (!listasIguales)


                //if (tableSchema1 != tableSchema2)
                {
                    differentTables.Add(tableName);
                }
            }

            #endregion

            #region Views                     

            // Obtener las listas de vistas de ambos servidores y bases de datos
            List<string> views1 = _sqlServerChecker.GetViews(serverAddress1, databaseName1);
            List<string> views2 = _sqlServerChecker.GetViews(serverAddress2, databaseName2);

            // Comparar las listas de tablas y obtener las tablas que faltan en cada servidor
            List<string> missingViewsInServer1 = views2.Except(views1).ToList();
            List<string> missingViewsInServer2 = views1.Except(views2).ToList();

            // Comparar las definiciones de vistas

            List<string> differentViews = new List<string>();
            foreach (string viewName in views1.Intersect(views2))
            {
                string viewSchema1 = _sqlServerChecker.GetViewDefinition(serverAddress1, databaseName1, viewName);
                string viewSchema2 = _sqlServerChecker.GetViewDefinition(serverAddress2, databaseName2, viewName);

                bool listasIguales = viewSchema1.Trim().SequenceEqual(viewSchema2.Trim());
                // bool listasIguales = tableSchema1[tableName].SequenceEqual(tableSchema2[tableName]);

                if (!listasIguales)

                //if (tableSchema1 != tableSchema2)
                {
                    differentViews.Add(viewName);
                }
            }

            #endregion



            #region Stored Procedures

            // Obtener las listas de stored procedures de ambos servidores y bases de datos
            List<string> storedprocs1 = _sqlServerChecker.GetProcedures(serverAddress1, databaseName1);
            List<string> storedprocs2 = _sqlServerChecker.GetProcedures(serverAddress2, databaseName2);

            // Comparar las listas de stored procs y obtener los que faltan en cada servidor
            List<string> missingStoredProcsInServer1 = storedprocs2.Except(storedprocs1).ToList();
            List<string> missingStoredProcsInServer2 = storedprocs1.Except(storedprocs2).ToList();

            // Comparar las definiciones de stored procs

            List<string> differentStoredProcs = new List<string>();
            foreach (string storedprocName in storedprocs1.Intersect(storedprocs2))
            {
                string storedProcSchema1 = _sqlServerChecker.GetStoredProcedureDefinition(serverAddress1, databaseName1, storedprocName);
                string storedProcSchema2 = _sqlServerChecker.GetStoredProcedureDefinition(serverAddress2, databaseName2, storedprocName);

                bool listasIguales = string.Equals(storedProcSchema1, storedProcSchema2, StringComparison.OrdinalIgnoreCase);//storedProcSchema1.Trim().SequenceEqual(storedProcSchema2.Trim());
                // bool listasIguales = tableSchema1[tableName].SequenceEqual(tableSchema2[tableName]);

                if (!listasIguales)

                //if (tableSchema1 != tableSchema2)
                {
                    differentStoredProcs.Add(storedprocName);
                }
            }
                        

            #endregion


            #region Final Response


            // Construir el objeto de respuesta con los resultados
            var response = new
            {
                MissingTablesInServer1 = missingTablesInServer1,
                MissingTablesInServer2 = missingTablesInServer2,
                DifferentTables = differentTables,


                MissingViewsInServer1 = missingViewsInServer1,
                MissingViewsInServer2 = missingViewsInServer2,
                DifferentViews = differentViews,

                MissingStoredProcsInServer1 = missingStoredProcsInServer1,
                MissingStoredProcsInServer2 = missingStoredProcsInServer2,
                DifferentStoredProcs = differentStoredProcs

            };

            return Ok(response);

            #endregion

        }



        [HttpGet("compare-table-definitions")]
        public IActionResult CompareTableDefinitions(string serverAddress1, string databaseName1, string serverAddress2, string databaseName2)
        {
            // Obtener las listas de tablas de ambos servidores y bases de datos
            List<string> tables1 = _sqlServerChecker.GetDatabaseTables(serverAddress1, databaseName1);
            List<string> tables2 = _sqlServerChecker.GetDatabaseTables(serverAddress2, databaseName2);

            // Comparar las definiciones de las tablas en ambos servidores
            List<string> differentTables = new List<string>();
            foreach (string tableName in tables1.Intersect(tables2))
            {
                Dictionary<string, List<ColumnDefinition>> tableSchema1 = _sqlServerChecker.GetTableSchemaByTableName(serverAddress1, databaseName1, tableName);
                Dictionary<string, List<ColumnDefinition>> tableSchema2 = _sqlServerChecker.GetTableSchemaByTableName(serverAddress2, databaseName2, tableName);


                bool listasIguales = tableSchema1[tableName].SequenceEqual(tableSchema2[tableName], new ColumnDefinitionComparer());
                // bool listasIguales = tableSchema1[tableName].SequenceEqual(tableSchema2[tableName]);

                if (!listasIguales)


                //if (tableSchema1 != tableSchema2)
                {
                    differentTables.Add(tableName);
                }
            }

            return Ok(differentTables);
        }

        public class ColumnDefinitionComparer : IEqualityComparer<ColumnDefinition>
        {
            public bool Equals(ColumnDefinition x, ColumnDefinition y)
            {
                // Implementa la lógica de comparación personalizada aquí.
                // Por ejemplo, puedes comparar las propiedades relevantes de los objetos ColumnDefinition.
                return x.ColumnName == y.ColumnName && x.DataType == y.DataType;
            }

            public int GetHashCode(ColumnDefinition obj)
            {
                // Implementa la generación del código hash aquí si es necesario.
                return obj.GetHashCode();
            }
        }

    }
}
