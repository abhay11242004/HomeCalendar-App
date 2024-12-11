using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPresenter
{
    public static class TestConstants
    {
        public const string DATABASE_DIRECTORY_PATH = @".\Database";
        public const string DATABASE_MESSY_NAME = "messy";
        public const string DATABASE_CLEAN_NAME = "testdb";
        public static readonly string DATABASE_MESSY_PATH =
            Path.Combine(DATABASE_DIRECTORY_PATH, DATABASE_MESSY_NAME + DATABASE_FILE_EXTENSION);
        public static readonly string DATABASE_MESSY_DIRECTORY =
            DATABASE_DIRECTORY_PATH;
        public static readonly string DATABASE_CLEAN_PATH =
            Path.Combine(DATABASE_DIRECTORY_PATH, DATABASE_CLEAN_NAME + DATABASE_FILE_EXTENSION);

        public static readonly string DATABASE_DEFAULT_DIRECTORY_LOCATION = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        public const string DATABASE_DEFAULT_NAME_NO_EXTENSION = "FileName";
        public const string DATABASE_FILE_EXTENSION = ".db";
    }
}
