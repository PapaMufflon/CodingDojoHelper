using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using CodingDojoHelper.Helper.Interfaces;

namespace CodingDojoHelper.Helper
{
    class PlaylistSoundPlayer : ISoundPlayer
    {
        private readonly SoundPlayer _soundPlayer = new SoundPlayer();
        private readonly Queue<Stream> _playlist = new Queue<Stream>();

        #region ISoundPlayer Members

        public void BeginPlay(Stream stream)
        {
            lock (_playlist)
            {
                _playlist.Enqueue(stream);

                if (_playlist.Count == 1)
                    Task.Factory.StartNew(PlayAll);
            }
        }

        private void PlayAll()
        {
            while (_playlist.Count > 0)
            {
                lock (_playlist)
                {
                    var currentSound = _playlist.Dequeue();

                    _soundPlayer.Stream = currentSound;
                    _soundPlayer.PlaySync(); 
                }
            }
        }

        #endregion
    }
}
