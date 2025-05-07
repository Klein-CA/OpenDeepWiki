namespace KoalaWiki.Dto;

public class ResultDto<T>
{
    public int Code { get; set; }
    
    public string Message { get; set; } = string.Empty;
    
    public T Data { get; set; } = default!;
    
    public ResultDto()
    {
        Code = 200;
        Message = "success";
    }
    
    public static ResultDto<T> Success(T data)
    {
        return new ResultDto<T>
        {
            Code = 200,
            Message = "success",
            Data = data
        };
    }
    
    public static ResultDto<T> Fail(string message)
    {
        return new ResultDto<T>
        {
            Code = 500,
            Message = message,
            Data = default!
        };
    }
}
