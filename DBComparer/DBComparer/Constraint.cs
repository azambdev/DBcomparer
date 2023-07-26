namespace DBComparer
{
    public class Constraint
    {

        public string ConstraintName { get; set; }
        public string ConstraintType { get; set; }

        public Constraint(string constraintName, string constraintType )
        {
            this.ConstraintName = constraintName;
            this.ConstraintType = constraintType;   

        }
        public Constraint(string constraintName)
        {
            this.ConstraintName = constraintName;
            

        }

    }
}
