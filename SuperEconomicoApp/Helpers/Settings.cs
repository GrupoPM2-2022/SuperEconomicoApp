﻿using System;
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
        public static bool ExistTokenFirebase => AppSettings.Contains(nameof(TokenFirebase));

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
        public static string TypeUser
        {
            get => AppSettings.GetValueOrDefault(nameof(TypeUser), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TypeUser), value);
        }
        public static string StatusDelivery
        {
            get => AppSettings.GetValueOrDefault(nameof(StatusDelivery), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(StatusDelivery), value);
        }
        
        public static string TokenFirebase
        {
            get => AppSettings.GetValueOrDefault(nameof(TokenFirebase), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TokenFirebase), value);
        }
        
        public static string CoordinatesUser
        {
            get => AppSettings.GetValueOrDefault(nameof(CoordinatesUser), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(CoordinatesUser), value);
        }
        public static string CurrentPage
        {
            get => AppSettings.GetValueOrDefault(nameof(CurrentPage), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(CurrentPage), value);
        }

        public static void ClearAllData()
        {
            AppSettings.Clear();
        }
        
        // DELETE KEYS
        public static void RemoveUserName() => AppSettings.Remove(nameof(UserName));
        public static void RemoveIdUser() => AppSettings.Remove(nameof(IdUser));
        public static void RemoveDepartment() => AppSettings.Remove(nameof(Department));
        public static void RemoveCoordinates() => AppSettings.Remove(nameof(Coordinates));
        public static void RemoveTypeUser() => AppSettings.Remove(nameof(TypeUser));
        public static void RemoveStatusDelivery() => AppSettings.Remove(nameof(StatusDelivery));
        public static void RemoveTokenFirebase() => AppSettings.Remove(nameof(TokenFirebase));

    }
}
