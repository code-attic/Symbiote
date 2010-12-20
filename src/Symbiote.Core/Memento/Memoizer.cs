namespace Symbiote.Core.Memento
{
    public class Memoizer
        : IMemoizer
    {
        public IMemento<T> GetMemento<T>( T instance )
        {
            var memento = Assimilate.GetInstanceOf<IMemento<T>>();
            memento.Capture( instance );
            return memento;
        }

        public T GetFromMemento<T>( IMemento<T> memento )
        {
            return memento.Retrieve();
        }

        public void ResetToMemento<T>( T instance, IMemento<T> memento )
        {
            memento.Reset( instance );
        }
    }
}