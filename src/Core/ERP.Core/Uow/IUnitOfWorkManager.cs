namespace ERP.Core.Uow;

public interface IUnitOfWorkManager
{
    /// <summary>
    /// Begins a new unit of work.
    /// </summary>
    /// <returns>A handle to be able to complete the unit of work</returns>
    IUnitOfWork Begin();
}