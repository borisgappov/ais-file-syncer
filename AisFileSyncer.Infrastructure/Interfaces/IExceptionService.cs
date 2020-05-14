using System;

namespace AisFileSyncer.Infrastructure.Interfaces
{
    public interface IExceptionService
    {
        Exception Exception { get; set; }
        void Clear();
    }
}
