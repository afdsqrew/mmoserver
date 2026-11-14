using System.Collections;

namespace ServerCrossPlatform.Test
{
    public class DbPlayer
    {
        public int Id { get; set; }
        public virtual ICollection<DbCharacter> TeamMembers { get; set; }
    }
}