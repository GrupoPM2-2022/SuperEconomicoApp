using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace SuperEconomicoApp.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static bool ExistUser => AppSettings.Contains(nameof(UserName));
        public static bool ExistDepartment => AppSettings.Contains(nameof(Department));

        public static string UserName
        {
            get => AppSettings.GetValueOrDefault(nameof(UserName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserName), value);
        }

        public static string IdUser
        {
            get => AppSettings.GetValueOrDefault(nameof(IdUser), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(IdUser), value);
        }
        public static string Department
        {
            get => AppSettings.GetValueOrDefault(nameof(Department), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Department), value);
        }
        public static string Coordinates
        {
            get => AppSettings.GetValueOrDefault(nameof(Coordinates), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Coordinates), value);
        }

        public static void ClearAllData()
        {
            AppSettings.Clear();
        }
    }
}
