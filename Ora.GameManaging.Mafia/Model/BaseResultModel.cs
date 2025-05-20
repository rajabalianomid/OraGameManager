namespace Ora.GameManaging.Mafia.Model
{
    public class BaseResultModel<T>
    {
        public bool Successful { get; set; }
        public T? Error { get; set; }
    }
    public class BaseResultDataModel<T, TData> : BaseResultModel<T>
    {
        public TData? Data { get; set; }
        public int Count { get; set; }
    }
}
