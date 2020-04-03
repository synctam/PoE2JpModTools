namespace MieTranslationLib.Exceptions
{
    using System;
    using System.IO;

    public class MieExceptions
    {
        public class InvalidFileTypeException : Exception { }

        public class InvalidFilePathException : Exception
        {
            public InvalidFilePathException(string fileName)
                : base(fileName) { }
        }

        public class DuplicateConversationLinkException : Exception { }

        public class ProgramErrorException : Exception
        {
            public ProgramErrorException(string msg)
                : base(msg) { }
        }

        public class DatabaseNotFoundException : FileNotFoundException
        {
            public DatabaseNotFoundException(string msg, string path)
                : base(msg, path) { }
        }
    }
}
