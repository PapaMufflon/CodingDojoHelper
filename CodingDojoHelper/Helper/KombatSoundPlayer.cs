using System;
using CodingDojoHelper.Helper.Interfaces;

namespace CodingDojoHelper.Helper
{
    class KombatSoundPlayer : IKombatSoundPlayer
    {
        private readonly PlaylistSoundPlayer _soundPlayer;
        private readonly ISession _session;
        private readonly Random _random = new Random();

        public KombatSoundPlayer(PlaylistSoundPlayer soundPlayer, ISession session)
        {
            _soundPlayer = soundPlayer;
            _session = session;
        }

        #region IKombatSoundPlayer Members

        public void BeginPlayStartSound()
        {
            _soundPlayer.BeginPlay(Resources.RoundOne);
            _soundPlayer.BeginPlay(Resources.Fight);
        }

        public void BeginPlayCycleSound(TimeSpan elapsed)
        {
            if (_session.Get<TimeSpan>(Session.CycleTime).CompareTo(elapsed) >= 0)
                BeginPlaySuccessCirleSound();
            else
                BeginPlayFailCycleSound();
        }

        private void BeginPlaySuccessCirleSound()
        {
            switch (_random.Next(8))
            {
                case 0:
                    _soundPlayer.BeginPlay(Resources.Change_Success_Excellent);
                    break;
                case 1:
                    _soundPlayer.BeginPlay(Resources.Change_Success_Excellent2);
                    break;
                case 2:
                    _soundPlayer.BeginPlay(Resources.Change_Success_Excellent3);
                    break;
                case 3:
                    _soundPlayer.BeginPlay(Resources.Change_Success_Excellent4);
                    break;
                case 4:
                    _soundPlayer.BeginPlay(Resources.Change_Success_Outstanding);
                    break;
                case 5:
                    _soundPlayer.BeginPlay(Resources.Change_Success_WellDone);
                    break;
                case 6:
                    _soundPlayer.BeginPlay(Resources.Change_Success_WellDone2);
                    break;
                case 7:
                    _soundPlayer.BeginPlay(Resources.Change_Success_WellDone3);
                    break;
            }
        }

        private void BeginPlayFailCycleSound()
        {
            switch (_random.Next(6))
            {
                case 0:
                    _soundPlayer.BeginPlay(Resources.Change_TooLong_DontMakeMeLaugh);
                    break;
                case 1:
                    _soundPlayer.BeginPlay(Resources.Change_TooLong_HahahaDontMakeMeLaugh);
                    break;
                case 2:
                    _soundPlayer.BeginPlay(Resources.Change_TooLong_HahahaYouSuck);
                    break;
                case 3:
                    _soundPlayer.BeginPlay(Resources.Change_TooLong_KahnLaugh);
                    break;
                case 4:
                    _soundPlayer.BeginPlay(Resources.Change_TooLong_KahnLaugh2);
                    break;
                case 5:
                    _soundPlayer.BeginPlay(Resources.Change_TooLong_YouAreNothing);
                    break;
            }
        }

        public void BeginPlayStopSound()
        {
            switch (_random.Next(11))
            {
                case 0:
                    _soundPlayer.BeginPlay(Resources.End_Success_FlawlessVictory);
                    break;
                case 1:
                    _soundPlayer.BeginPlay(Resources.End_Success_FlawlessVictory2);
                    break;
                case 2:
                    _soundPlayer.BeginPlay(Resources.End_Success_FlawlessVictory3);
                    break;
                case 3:
                    _soundPlayer.BeginPlay(Resources.End_Success_FlawlessVictory4);
                    break;
                case 4:
                    _soundPlayer.BeginPlay(Resources.End_Success_JohnnyCageWins);
                    break;
                case 5:
                    _soundPlayer.BeginPlay(Resources.End_Success_KanoWins);
                    break;
                case 6:
                    _soundPlayer.BeginPlay(Resources.End_Success_LiuKangWins);
                    break;
                case 7:
                    _soundPlayer.BeginPlay(Resources.End_Success_RaidenWins);
                    break;
                case 8:
                    _soundPlayer.BeginPlay(Resources.End_Success_ScorpionWins);
                    break;
                case 9:
                    _soundPlayer.BeginPlay(Resources.End_Success_SonyaWins);
                    break;
                case 10:
                    _soundPlayer.BeginPlay(Resources.End_Success_SubZeroWins);
                    break;
            }
        }

        public void BeginPlayFinishHimSound()
        {
            switch (_random.Next(8))
            {
                case 0:
                    _soundPlayer.BeginPlay(Resources.StopIt_FinishHim);
                    break;
                case 1:
                    _soundPlayer.BeginPlay(Resources.StopIt_FinishHim2);
                    break;
                case 2:
                    _soundPlayer.BeginPlay(Resources.StopIt_FinishHim3);
                    break;
                case 3:
                    _soundPlayer.BeginPlay(Resources.StopIt_PrepareToDie);
                    break;
                case 4:
                    _soundPlayer.BeginPlay(Resources.StopIt_Scream);
                    break;
                case 5:
                    _soundPlayer.BeginPlay(Resources.StopIt_Scream2);
                    break;
                case 6:
                    _soundPlayer.BeginPlay(Resources.StopIt_YouSuck);
                    break;
                case 7:
                    _soundPlayer.BeginPlay(Resources.StopIt_YouWeakPatheticFool);
                    break;
            }
        }

        #endregion
    }
}
