namespace DataContentManager
{
    using System;

    public class ScoreManager
    {
        public void SaveScore(DateTime dateTime, int punctuation)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values["punctuation"] = punctuation;
            settings.Values["date"] = dateTime;
        }

        public int GetScore()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return Convert.ToInt32(settings.Values["punctuation"]);
        }

        public DateTime GetDate()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return Convert.ToDateTime(settings.Values["date"]);
        }
    }
}
