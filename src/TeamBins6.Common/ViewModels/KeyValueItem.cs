using System;

namespace TeamBins.Common.ViewModels
{
    public class KeyValueItem : IEquatable<KeyValueItem>
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Code { set; get; }

        public string Color { set; get; }

        public int DisplayOrder { set; get; }

        public  bool Equals(KeyValueItem obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            KeyValueItem p = (KeyValueItem)obj;
            return (Id == p.Id) ;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode() ;
        }
    }
}