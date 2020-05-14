using AisFileSyncer.Infrastructure.Interfaces;
using System;

namespace AisFileSyncer.Infrastructure.Services
{
    public class ExceptionService : IExceptionService
    {
        public Exception Exception { get; set; }

        public void Clear()
        {
            Exception = null;
        }
    }
}
