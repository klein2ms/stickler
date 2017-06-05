using System;

namespace Stickler.Conduit
{
    public sealed class Conduit
    {
        private static readonly Lazy<Conduit> _lazy
            = new Lazy<Conduit>(() => new Conduit());

        public static Conduit Instance => _lazy.Value; 

        private Conduit()
        {
        }
    }
}
