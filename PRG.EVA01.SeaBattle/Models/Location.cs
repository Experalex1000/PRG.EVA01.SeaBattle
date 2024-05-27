namespace PRG.EVA01.SeaBattle.Models
{
    public class Location
    {
        public string Letter { get; set; }
        public string Number { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Location location)
            {
                return Letter == location.Letter && Number == location.Number;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Letter, Number);
        }
    }


}
