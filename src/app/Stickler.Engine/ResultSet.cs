using System.Collections.Generic;
using System.Linq;

namespace Stickler.Engine
{
    public class ResultSet
    {
        public ResultStatus Status { get; set; }
        public IList<Result> Results { get; set; }

        public ResultSet()
        {
            Results = new List<Result>();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ResultSet);
        }

        protected bool Equals(ResultSet other)
        {
            return other != null 
                && Status == other.Status 
                && Results.SequenceEqual(other.Results);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Status * 397) ^ (Results != null ? Results.GetHashCode() : 0);
            }
        }
    }
}
