using System;

namespace chatApi.Models
{
public struct TimeZoneConfig
    {
        public TimeZoneInfo TimeZone;
        public DateTime Time;

        public TimeZoneConfig(TimeZoneInfo tz, DateTime time)
        {
            if (tz == null)
                throw new ArgumentNullException("The time zone cannot be a null reference.");

            this.TimeZone = tz;
            this.Time = time;
        }

        // Método para adicionar intervalo de tempo, se necessário
        public TimeZoneConfig AddTime(TimeSpan interval)
        {
            // Converte o tempo atual para UTC
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(this.Time, this.TimeZone);
            // Adiciona o intervalo de tempo
            utcTime = utcTime.Add(interval);
            // Converte de volta para o fuso horário original
            return new TimeZoneConfig(this.TimeZone, TimeZoneInfo.ConvertTime(utcTime, TimeZoneInfo.Utc, this.TimeZone));
        }

        // Converte uma data de UTC para o fuso horário de São Paulo
        public static DateTime ConvertToSaoPauloTime(DateTime utcTime)
        {
            TimeZoneInfo saoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, saoPauloTimeZone);
        }

        // Converte uma data do fuso horário de São Paulo para UTC
        public static DateTime ConvertToUtc(DateTime localTime)
        {
            TimeZoneInfo saoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            return TimeZoneInfo.ConvertTimeToUtc(localTime, saoPauloTimeZone);
        }
    }
}
