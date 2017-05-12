using System.Collections.Generic;

namespace Stickler.Engine
{
    public class ResultSet
    {
        public ResultStatus Status { get; set; }
        public IList<Result> Results { get; set; }
    }
}
