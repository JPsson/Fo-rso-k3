namespace ShutTheBoxAdvanced2
{
    public class Emoji : IDisplayable
    {
        string _symb;
        public Emoji(string symb)
        {
            this._symb = symb;
        }
        public string Display()
        {
            return _symb;
        }
    }
}