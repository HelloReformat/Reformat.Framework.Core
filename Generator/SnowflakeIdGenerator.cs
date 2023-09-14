using Microsoft.Extensions.Configuration;
using Reformat.Framework.Core.IOC;
using Reformat.Framework.Core.IOC.Services;

namespace Reformat.Framework.Core.Generator;

/// <summary>
/// 雪花ID生成器
/// </summary>
public class SnowflakeIdGenerator
{
    private static long epoch = 1631606400000L; // 自定义起始时间戳，单位为毫秒
    private static long workerId;

    private static long sequence = 0L;
    private static long lastTimestamp = -1L;

    private static readonly object lockObject = new object();

    static SnowflakeIdGenerator()
    {
        IConfiguration configuration = ServiceLocator.GetConfiguration();
        workerId = configuration.GetValue<int>("SnowWorkerId",1);
    }

    public static long GenerateId()
    {
        lock (lockObject)
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp < lastTimestamp)
            {
                throw new InvalidOperationException("Invalid system clock.");
            }

            if (timestamp == lastTimestamp)
            {
                sequence = (sequence + 1) & 4095; // sequence 最大取值为 4095
                if (sequence == 0)
                {
                    timestamp = WaitNextMillis();
                }
            }
            else
            {
                sequence = 0L;
            }

            lastTimestamp = timestamp;

            long id = ((timestamp - epoch) << 22) | (workerId << 12) | sequence;
            return id;
        }
    }

    private static long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static long WaitNextMillis()
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
}