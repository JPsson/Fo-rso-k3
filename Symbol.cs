namespace ShutTheBoxAdvanced2
{
    public class Symbol : IDisplayable
    {
        char _symb;
        public Symbol(char symb)
        {
            this._symb = symb;
        }
        public string Display()
        {
            return _symb.ToString();
        }
    }
}