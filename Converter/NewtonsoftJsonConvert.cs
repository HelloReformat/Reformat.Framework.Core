using Newtonsoft.Json;
using Reformat.Framework.Core.Common.Extensions.lang;

namespace Reformat.Framework.Core.Converter
{
    #region DateTime

    /// <summary>
    /// 处理Json日期格式,需注入到Startup中
    /// </summary>
    public class NewtonsoftJsonDateTimeConvert : JsonConverter<DateTime>
    {
        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if ( reader.TokenType == JsonToken.String)
            {
              
            }
            DateTime.TryParse(reader.Value.ToStringMissNull(), out DateTime date);
            return date;
        }

        public override void WriteJson(JsonWriter writer, DateTime value,
            JsonSerializer serializer)
        {
            //if (value.Hour == 0 && value.Minute == 0 && value.Second == 0)
            //{
            //    writer.WriteValue(value.ToString("yyyy-MM-dd"));
            //}
            //else
            //{
            //    writer.WriteValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
            //}
            writer.WriteValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    public class NewtonsoftJsonDateTimeNullableConverter : JsonConverter<DateTime?>
    {
        public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
               
            }

            if (DateTime.TryParse(reader.Value.ToStringMissNull(), out DateTime date))
                return (DateTime?)date;
            else
                return null;
        }

        public override void WriteJson(JsonWriter writer, DateTime? value,
            JsonSerializer serializer)
        {
            if (value?.Hour == 0 && value?.Minute == 0 && value?.Second == 0)
            {
                writer.WriteValue(value?.ToString("yyyy-MM-dd"));
            }
            else
            {
                writer.WriteValue(value?.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }

    #endregion

    #region DateOnly
    
    public class NewtonsoftJsonDateOnlyConvert : JsonConverter<DateOnly>
    {
        public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {

            //if (reader.TokenType == Newtonsoft.Json.JsonToken.String)
            //{

              
            //}
            DateTime.TryParse(reader.Value.ToStringMissNull(), out DateTime date);
            return DateOnly.FromDateTime(date);
        }

        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
        {
            string _s = value.ToString("yyyy-MM-dd");
            writer.WriteValue(_s);
        }
    }
    
    
    public class NewtonsoftJsonDateOnlyNullableConverter : JsonConverter<DateOnly?>
    {
        public override DateOnly? ReadJson(JsonReader reader, Type objectType, DateOnly? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return DateOnly.FromDateTime(Convert.ToDateTime(reader.Value.ToStringMissNull()));
            }
            if (DateTime.TryParse(reader.Value.ToStringMissNull(),out DateTime date)) {
               return DateOnly.FromDateTime(date);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, DateOnly? value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString("yyyy-MM-dd"));
        }
    }
    
    
    #endregion

    #region TimeOnly
    
    public class NewtonsoftJsonTimeOnlyConvert : JsonConverter<TimeOnly>
    {
        public override TimeOnly ReadJson(JsonReader reader, Type objectType, TimeOnly existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
               
            }
            DateTime.TryParse(reader.Value.ToStringMissNull(), out DateTime date);
            return TimeOnly.FromDateTime(date);
          
        }

        public override void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("HH:mm:ss"));
        }
    }
    
    public class NewtonsoftJsonTimeOnlyNullableConverter : JsonConverter<TimeOnly?>
    {
        public override TimeOnly? ReadJson(JsonReader reader, Type objectType, TimeOnly? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
           
            if (DateTime.TryParse(reader.Value.ToStringMissNull(), out DateTime date)) {
                return TimeOnly.FromDateTime(date);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, TimeOnly? value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString("HH:mm:ss"));
        }
    }
    
    #endregion
}