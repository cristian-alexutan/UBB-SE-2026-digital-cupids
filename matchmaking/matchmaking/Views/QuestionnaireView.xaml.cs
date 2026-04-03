using matchmaking.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace matchmaking.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuestionnaireView : Page
    {
        internal EditProfileViewModel? ViewModel { get; private set; }

        public QuestionnaireView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as EditProfileViewModel;
            if (ViewModel == null) return;

            ViewModel.PrepareQuestionnaire();
            RenderQuestions();
        }

        private void RenderQuestions()
        {
            QuestionsPanel.Children.Clear();

            for (int i = 0; i < ViewModel!.ShuffledQuestions.Count; i++)
            {
                int questionIndex = i;

                StackPanel questionBlock = new StackPanel();
                questionBlock.Spacing = 12;

                TextBlock questionText = new TextBlock();
                questionText.Text = $"{i + 1}. {ViewModel.ShuffledQuestions[i]}";
                questionText.FontSize = 15;
                questionText.TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap;
                questionBlock.Children.Add(questionText);

                StackPanel scaleRow = new StackPanel();
                scaleRow.Orientation = Orientation.Horizontal;
                scaleRow.Spacing = 0;

                string[] labels = { "1\nNot at all", "2", "3", "4", "5\nCompletely" };

                for (int j = 0; j < 5; j++)
                {
                    int answerValue = j + 1;

                    StackPanel optionStack = new StackPanel();
                    optionStack.Width = 120;
                    optionStack.Spacing = 4;
                    optionStack.HorizontalAlignment = HorizontalAlignment.Left;

                    RadioButton rb = new RadioButton();
                    rb.GroupName = $"Question_{questionIndex}";
                    rb.Content = string.Empty;
                    rb.HorizontalAlignment = HorizontalAlignment.Center;
                    rb.Margin = new Thickness(0);
                    rb.Padding = new Thickness(0);
                    rb.MinWidth = 0;
                    rb.IsChecked = ViewModel.GetAnswer(questionIndex) == answerValue;

                    rb.Checked += (s, e) =>
                    {
                        ViewModel.SetAnswer(questionIndex, answerValue);
                        UpdateSubmitButton();
                    };

                    TextBlock label = new TextBlock();
                    label.Text = labels[j];
                    label.FontSize = 11;
                    label.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.Colors.Gray);
                    label.HorizontalAlignment = HorizontalAlignment.Center;
                    label.TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center;

                    optionStack.Children.Add(rb);
                    optionStack.Children.Add(label);
                    scaleRow.Children.Add(optionStack);
                }

                questionBlock.Children.Add(scaleRow);

                Border divider = new Border();
                divider.Height = 1;
                divider.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.LightGray);
                divider.Margin = new Thickness(0, 8, 0, 0);
                questionBlock.Children.Add(divider);

                QuestionsPanel.Children.Add(questionBlock);
            }
        }

        private void UpdateSubmitButton()
        {
            SubmitButton.IsEnabled = ViewModel!.CanSubmitQuestionnaire();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel!.CancelQuestionnaire();
            Frame.GoBack();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            ViewModel!.SubmitQuestionnaire();
            Frame.GoBack();
        }
    }
}
