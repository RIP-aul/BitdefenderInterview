using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvMock.Exceptions.ExceptionMessages
{
    public static class ExceptionMessageDictionary
    {
        public static readonly Dictionary<ErrorCodes, string> ErrorCodeDictionary = new()
        {
            { ErrorCodes.OnDemandScanAlreadyRunning, ExceptionMessages.OnDemandScanAlreadyRunning },
            { ErrorCodes.OnDemandScanNotRunning, ExceptionMessages.OnDemandScanNotRunning },
        };
    }
}
