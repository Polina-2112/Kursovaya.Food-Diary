using SQLite;

namespace References.Models
{
    public class Reference
    {
        [PrimaryKey, AutoIncrement]
        public int ID {  get; set; }
        public string Name {  get; set; }
        public int Calories { get; set; }
    }
}
