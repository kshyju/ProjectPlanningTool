namespace TeamBins.DataAccess
{
    public class BaseRepo
    {
        protected string ConnectionString
        {
            get { return "Data Source=DET-4082;Initial Catalog=Team;Integrated Security=true"; }
        }
    }
}