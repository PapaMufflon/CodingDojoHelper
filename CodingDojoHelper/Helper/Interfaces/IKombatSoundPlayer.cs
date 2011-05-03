using System;

namespace CodingDojoHelper.Helper.Interfaces
{
    interface IKombatSoundPlayer
    {
        void BeginPlayStartSound();
        void BeginPlayCycleSound(TimeSpan elapsed);
        void BeginPlayStopSound();
        void BeginPlayFinishHimSound();
    }
}
