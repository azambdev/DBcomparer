namespace DBComparer
{
    public class ColumnDefinition
    {
        public string ColumnName { get; set; }
        public string DataType  { get; set; }
        public int maxLength { get; set; }

        public bool IsPrimaryKey { get; set; }

        public List<Constraint> Constraints { get; set; }
       
        public ColumnDefinition(string columnName, string dataType, int maxLength)
        {
            this.ColumnName = columnName;
            this.DataType = dataType;
            this.maxLength = maxLength;
        }
        public ColumnDefinition(string columnName, string dataType, int maxLength, List<Constraint> constraints)
        {
            this.ColumnName = columnName;
            this.DataType = dataType;
            this.maxLength = maxLength;
            this.Constraints = constraints;
        }

        public ColumnDefinition(string columnName, string dataType, int maxLength, List<Constraint> constraints, bool isprimaryKey)
        {
            this.ColumnName = columnName;
            this.DataType = dataType;
            this.maxLength = maxLength;
            this.Constraints = constraints;
            this.IsPrimaryKey=isprimaryKey;
        }

    }
}
