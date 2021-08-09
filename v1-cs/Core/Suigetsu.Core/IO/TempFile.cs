using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.IO
{
    public sealed class TempFile : IDisposable
    {
        private string _filePath;
        private FileStream _handle;

        public TempFile() : this(Path.GetTempFileName()) {}

        public TempFile(string filePath)
        {
            if(filePath.IsEmpty())
            {
                throw new ArgumentNullException();
            }

            _filePath = filePath;
        }

        public FileStream Handle
        {
            get
            {
                if(!_filePath.IsEmpty())
                {
                    return _handle
                           ?? (_handle = File.Open(_filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
                }

                return null;
            }
        }

        public string FilePath
        {
            get
            {
                if(_filePath.IsEmpty())
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _filePath;
            }
        }

        public void Dispose() => Dispose(true);

        public void CloseHandle()
        {
            if(Handle == null)
            {
                return;
            }

            Handle.Close();
            _handle = null;
        }

        [ExcludeFromCodeCoverage]
        ~TempFile()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if(disposing)
            {
                GC.SuppressFinalize(this);
            }
            if(_filePath.IsEmpty() || !File.Exists(_filePath))
            {
                return;
            }

            CloseHandle();
            File.Delete(_filePath);
            _filePath = null;
        }
    }
}
