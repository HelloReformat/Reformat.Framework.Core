namespace Reformat.Framework.Core.MVC;

public static class Api
{
    #region 同步方法

    public static ApiResult<string> Rest(bool success) 
    {
        return new ApiResult<string>(){Success = success,ErrorCode = success ? 0 : -1,Message = success ? "操作成功" : "操作失败"};
    }
    
    public static ApiResult<T> Rest<T>(bool success,T data) where T : class
    {
        if (success)
        {
            return new ApiResult<T>(){Success = success,ErrorCode = 0,Message = "操作成功",Data = data};
        }
        else
        {
            return new ApiResult<T>(){Success = success,ErrorCode = -1,Message = "操作失败"};
        }
    }
    
    public static ApiResult<string> RestSuccess(string msg = "操作成功")
    {
        return new ApiResult<string>(){Success = true,ErrorCode = 0,Message = msg};
    }

    public static ApiResult<T> RestSuccess<T>(T data)
    {
        return new ApiResult<T>(){Success = true,ErrorCode = 0,Message = "操作成功",Data = data};
    }
    
    public static ApiResult<T> RestSuccess<T>(string msg,T data)
    {
        return new ApiResult<T>(){Success = true,ErrorCode = 0,Message = msg,Data = data};
    }
    
    public static ApiResult<string> RestError(string msg = "操作失败")
    {
        return new ApiResult<string>(){Success = false,ErrorCode = -1,Message = msg};
    }
    
    public static ApiResult<string> RestError(int errorCode,string msg = "操作失败")
    {
        return new ApiResult<string>(){Success = false,ErrorCode = errorCode,Message = msg};
    }
    
    public static ApiResult<T> RestError<T>(int errorCode,string codeMsg)
    {
        return new ApiResult<T>(){Success = false,ErrorCode = errorCode,Message = "操作失败: " + codeMsg};
    }
    
    public static ApiResult<T> RestError<T>(int errorCode,string codeMsg,string details)
    {
        return new ApiResult<T>(){Success = false,ErrorCode = errorCode,Message = codeMsg + " : " + details};
    }

    #endregion

    #region 异步方法

    public static async Task<ApiResult<string>> AsyncRest(bool success)
    {
        return new ApiResult<string>(){Success = success,ErrorCode = success ? 0 : -1,Message = success ? "操作成功" : "操作失败"};
    }
    
    public static async Task<ApiResult<T>> AsyncRest<T>(bool success,T data)  where T : class
    {
        if (success)
        {
            return new ApiResult<T>(){Success = success,ErrorCode = 0,Message = "操作成功",Data = data};
        }
        else
        {
            return new ApiResult<T>(){Success = success,ErrorCode = -1,Message = "操作失败"};
        }
    }
    
    public static async Task<ApiResult<string>> AsyncRestSuccess(string msg = "操作成功")
    {
        return new ApiResult<string>(){Success = true,ErrorCode = 0,Message = msg};
    }

    public static async Task<ApiResult<T>> AsyncRestSuccess<T>(T data) where T : class
    {
        return new ApiResult<T>(){Success = true,ErrorCode = 0,Message = "操作成功",Data = data};
    }
    
    public static async Task<ApiResult<T>> AsyncRestSuccess<T>(string msg,T data)
    {
        return new ApiResult<T>(){Success = true,ErrorCode = 0,Message = msg,Data = data};
    }
    
    public static async Task<ApiResult<string>> AsyncRestError(string msg = "操作失败")
    {
        return new ApiResult<string>(){Success = false,ErrorCode = -1,Message = msg};
    }
    
    public static async Task<ApiResult<T>> AsyncRestError<T>(int errorCode,string codeMsg)
    {
        return new ApiResult<T>(){Success = false,ErrorCode = errorCode,Message = "操作失败: " + codeMsg};
    }

    public static async Task<ApiResult<T>> AsyncRestError<T>(int errorCode,string codeMsg,string details)
    {
        return new ApiResult<T>(){Success = false,ErrorCode = errorCode,Message = codeMsg + " : " + details};
    }
    #endregion
}