namespace Reformat.Framework.Core.MVC;

public static class Api
{
    #region 同步方法

    public static APIResponse<string> Rest(bool success) 
    {
        return new APIResponse<string>(){code = success ? 0 : -1,message = success ? "操作成功" : "操作失败"};
    }
    
    public static APIResponse<T> Rest<T>(bool success,T data) where T : class
    {
        if (success)
        {
            return new APIResponse<T>(){code = 0,message = "操作成功",data = data};
        }
        else
        {
            return new APIResponse<T>(){code = -1,message = "操作失败"};
        }
    }
    
    public static APIResponse<string> RestSuccess(string msg = "操作成功")
    {
        return new APIResponse<string>(){code = 0,message = msg};
    }

    public static APIResponse<T> RestSuccess<T>(T data)
    {
        return new APIResponse<T>(){code = 0,message = "操作成功",data = data};
    }
    
    public static APIResponse<T> RestSuccess<T>(string msg,T data)
    {
        return new APIResponse<T>(){code = 0,message = msg,data = data};
    }
    
    public static APIResponse<string> RestError(string msg = "操作失败")
    {
        return new APIResponse<string>(){code = 0,message = msg};
    }
    
    public static APIResponse<string> RestError(int codeCode,string msg = "操作失败")
    {
        return new APIResponse<string>(){code = codeCode,message = msg};
    }
    
    public static APIResponse<T> RestError<T>(int codeCode,string codeMsg)
    {
        return new APIResponse<T>(){code = codeCode,message = "操作失败: " + codeMsg};
    }
    
    public static APIResponse<T> RestError<T>(int codeCode,string codeMsg,string details)
    {
        return new APIResponse<T>(){code = codeCode,message = codeMsg + " : " + details};
    }

    #endregion

    #region 异步方法

    public static async Task<APIResponse<string>> AsyncRest(bool success)
    {
        return new APIResponse<string>(){code = success ? 0 : -1,message = success ? "操作成功" : "操作失败"};
    }
    
    public static async Task<APIResponse<T>> AsyncRest<T>(bool success,T data)  where T : class
    {
        if (success)
        {
            return new APIResponse<T>(){code = 0,message = "操作成功",data = data};
        }
        else
        {
            return new APIResponse<T>(){code = -1,message = "操作失败"};
        }
    }
    
    public static async Task<APIResponse<string>> AsyncRestSuccess(string msg = "操作成功")
    {
        return new APIResponse<string>(){code = 0,message = msg};
    }

    public static async Task<APIResponse<T>> AsyncRestSuccess<T>(T data) where T : class
    {
        return new APIResponse<T>(){code = 0,message = "操作成功",data = data};
    }
    
    public static async Task<APIResponse<T>> AsyncRestSuccess<T>(string msg,T data)
    {
        return new APIResponse<T>(){code = 0,message = msg,data = data};
    }
    
    public static async Task<APIResponse<string>> AsyncRestError(string msg = "操作失败")
    {
        return new APIResponse<string>(){code = 0,message = msg};
    }
    
    public static async Task<APIResponse<T>> AsyncRestError<T>(int codeCode,string codeMsg)
    {
        return new APIResponse<T>(){code = codeCode,message = "操作失败: " + codeMsg};
    }

    public static async Task<APIResponse<T>> AsyncRestError<T>(int codeCode,string codeMsg,string details)
    {
        return new APIResponse<T>(){code = codeCode,message = codeMsg + " : " + details};
    }
    #endregion
}