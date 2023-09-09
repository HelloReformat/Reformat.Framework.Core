namespace Reformat.Framework.Core.MVC;

public static class Api
{
    #region 同步方法

    public static ApiResult<string> Rest(bool success) 
    {
        return new ApiResult<string>(){code = success ? 0 : -1,message = success ? "操作成功" : "操作失败"};
    }
    
    public static ApiResult<T> Rest<T>(bool success,T data) where T : class
    {
        if (success)
        {
            return new ApiResult<T>(){code = 0,message = "操作成功",data = data};
        }
        else
        {
            return new ApiResult<T>(){code = -1,message = "操作失败"};
        }
    }
    
    public static ApiResult<string> RestSuccess(string msg = "操作成功")
    {
        return new ApiResult<string>(){code = 0,message = msg};
    }

    public static ApiResult<T> RestSuccess<T>(T data)
    {
        return new ApiResult<T>(){code = 0,message = "操作成功",data = data};
    }
    
    public static ApiResult<T> RestSuccess<T>(string msg,T data)
    {
        return new ApiResult<T>(){code = 0,message = msg,data = data};
    }
    
    public static ApiResult<string> RestError(string msg = "操作失败")
    {
        return new ApiResult<string>(){code = -1,message = msg};
    }
    
    public static ApiResult<string> RestError(int codeCode,string msg = "操作失败")
    {
        return new ApiResult<string>(){code = codeCode,message = msg};
    }
    
    public static ApiResult<T> RestError<T>(int codeCode,string codeMsg)
    {
        return new ApiResult<T>(){code = codeCode,message = "操作失败: " + codeMsg};
    }
    
    public static ApiResult<T> RestError<T>(int codeCode,string codeMsg,string details)
    {
        return new ApiResult<T>(){code = codeCode,message = codeMsg + " : " + details};
    }

    #endregion

    #region 异步方法

    public static async Task<ApiResult<string>> AsyncRest(bool success)
    {
        return new ApiResult<string>(){code = success ? 0 : -1,message = success ? "操作成功" : "操作失败"};
    }
    
    public static async Task<ApiResult<T>> AsyncRest<T>(bool success,T data)  where T : class
    {
        if (success)
        {
            return new ApiResult<T>(){code = 0,message = "操作成功",data = data};
        }
        else
        {
            return new ApiResult<T>(){code = -1,message = "操作失败"};
        }
    }
    
    public static async Task<ApiResult<string>> AsyncRestSuccess(string msg = "操作成功")
    {
        return new ApiResult<string>(){code = 0,message = msg};
    }

    public static async Task<ApiResult<T>> AsyncRestSuccess<T>(T data) where T : class
    {
        return new ApiResult<T>(){code = 0,message = "操作成功",data = data};
    }
    
    public static async Task<ApiResult<T>> AsyncRestSuccess<T>(string msg,T data)
    {
        return new ApiResult<T>(){code = 0,message = msg,data = data};
    }
    
    public static async Task<ApiResult<string>> AsyncRestError(string msg = "操作失败")
    {
        return new ApiResult<string>(){code = -1,message = msg};
    }
    
    public static async Task<ApiResult<T>> AsyncRestError<T>(int codeCode,string codeMsg)
    {
        return new ApiResult<T>(){code = codeCode,message = "操作失败: " + codeMsg};
    }

    public static async Task<ApiResult<T>> AsyncRestError<T>(int codeCode,string codeMsg,string details)
    {
        return new ApiResult<T>(){code = codeCode,message = codeMsg + " : " + details};
    }
    #endregion
}