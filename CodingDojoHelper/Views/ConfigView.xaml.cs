using System;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CodingDojoHelper.ViewModels;

namespace CodingDojoHelper.Views
{
    internal partial class ConfigView
    {
        private ConfigViewModel _vm;
        private bool _storyboardFinished = true;
        private readonly Storyboard _bounceScrollbarToControl;
        private readonly EasingDoubleKeyFrame _targetingKeyFrame;

        public ConfigView(ConfigViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            Loaded += (s, e) => DataContext = _vm;

            _bounceScrollbarToControl = ((Storyboard)Resources["MoveScrollbarStoryboard"]);
            var animation = (DoubleAnimationUsingKeyFrames)_bounceScrollbarToControl.Children[0];
            _targetingKeyFrame = (EasingDoubleKeyFrame)animation.KeyFrames[0];

            _bounceScrollbarToControl.Completed += (s, e) => _storyboardFinished = true;
        }

        private void Combatants_MouseMove(object sender, MouseEventArgs e)
        {
            TryChangingCurrentConfigValue(ConfigValue.CombatantsCount);
        }

        private void CycleTime_MouseMove(object sender, MouseEventArgs e)
        {
            TryChangingCurrentConfigValue(ConfigValue.CycleTime);
        }

        private void DojoTime_MouseMove(object sender, MouseEventArgs e)
        {
            TryChangingCurrentConfigValue(ConfigValue.DojoTime);
        }

        private void TryChangingCurrentConfigValue(ConfigValue configValue)
        {
            var y = 0;

            switch (configValue)
            {
                case ConfigValue.CombatantsCount:
                    y = 10;
                    break;

                case ConfigValue.CycleTime:
                    y = 115;
                    break;

                case ConfigValue.DojoTime:
                    y = 185;
                    break;
            }

            if (TryStartStoryboard(y))
            {
                _vm.ActiveValue = configValue;
                AdjustScrollbarTooltip(configValue);
            }
        }

        private void AdjustScrollbarTooltip(ConfigValue configValue)
        {
            switch (configValue)
            {
                case ConfigValue.CombatantsCount:
                    scrollBar.ToolTip = CodingDojoHelper.Resources.DefineNumberOfCombatants;
                    break;

                case ConfigValue.CycleTime:
                    scrollBar.ToolTip = CodingDojoHelper.Resources.DefineCycleTime;
                    break;

                case ConfigValue.DojoTime:
                    scrollBar.ToolTip = CodingDojoHelper.Resources.DefineDojoTime;
                    break;
            }
        }

        private bool TryStartStoryboard(int y)
        {
            if (_storyboardFinished)
            {
                _storyboardFinished = false;
                _targetingKeyFrame.Value = y;
                _bounceScrollbarToControl.Begin();
                return true;
            }

            return false;
        }
    }
}
