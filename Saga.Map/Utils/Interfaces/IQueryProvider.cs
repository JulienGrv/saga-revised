namespace Saga.Data
{
    public interface IQueryProvider
    {
        QueryParameterCollection Parameters { get; }

        string CmdText { get; set; }
    }
}