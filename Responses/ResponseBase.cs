namespace DevJobsBackend.Responses;

public class ResponseBase<T>
{
    public T? Data { get; set; }
    public string Message { get; set; }
    public bool Status { get; set; }
}
