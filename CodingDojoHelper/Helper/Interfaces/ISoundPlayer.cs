using System.IO;

namespace CodingDojoHelper.Helper.Interfaces
{
    internal interface ISoundPlayer
    {
        void BeginPlay(Stream stream);
    }
}