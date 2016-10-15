using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace FlashCards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<FlashCard> _flashCards = new List<FlashCard>();
        // Contains the cards we've already shown - used for back and next functionality
        private List<FlashCard> _shownFlashCards = new List<FlashCard>();
        private int _currentIndex = -1;
        private Random _random = new Random();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            lblQuestion.Content = "";
            lblAnswer.Content = "";
            expAnswer.IsExpanded = false;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML Files |*.xml";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    XDocument doc = XDocument.Load(dlg.FileName);
                    // Get all the flash cards in the doc
                    var cards = from c in doc.Descendants("FlashCard")
                                select new
                                {
                                    Question = c.Element("Question").Value,
                                    Answer = c.Element("Answer").Value
                                };
                    // Create a flash FlashCard object for every flash card
                    // found in the XML file.
                    foreach (var card in cards)
                    {
                        FlashCard fco = new FlashCard(card.Question, card.Answer);
                        _flashCards.Add(fco);
                    }

                    btnNext.IsEnabled = true;
                    btnPrev.IsEnabled = false;
                }
                catch (Exception)
                {
                    _flashCards.Clear();
                }
            };
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            // Get the index for the next card we need
            ++_currentIndex;
            // Enable the back button if we're at the 2nd or later card
            if (_currentIndex > 0)
                btnPrev.IsEnabled = true;

            FlashCard card = null;



            // If there's currently no card to show or we've reached the end
            // of all the cards we've already shown, pluck one from _flashCards
            if (_shownFlashCards.Count == 0 || _currentIndex >= _shownFlashCards.Count)
            {
                int randomIndex = _random.Next(0, _flashCards.Count);
                card = _flashCards[randomIndex];

                // Add it to the list of cards made visible
                _shownFlashCards.Add(card);
                // Remove it from the list of possible new cards to show
                _flashCards.Remove(card);
            }
            else
                // Otherwise we show the next card in the queue of cards we've shown 
                card = _shownFlashCards[_currentIndex];

            if (card != null)
            {
                lblQuestion.Content = card.Question;
                expAnswer.IsExpanded = false;
                lblAnswer.Content = card.Answer;
            }

            // If there's not another card we can show and no new cards to choose
            // then we disable the next button
            if (_currentIndex >= _shownFlashCards.Count - 1 && _flashCards.Count == 0)
                btnNext.IsEnabled = false;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            // If there's nothing to show, just return. 
            if (_currentIndex < 1 || _shownFlashCards.Count == 0)
                return;

            --_currentIndex;

            lblQuestion.Content = _shownFlashCards[_currentIndex].Question;
            expAnswer.IsExpanded = false;
            lblAnswer.Content = _shownFlashCards[_currentIndex].Answer;


            if (_currentIndex <= 0)
                btnPrev.IsEnabled = false;

            if (_currentIndex <= _shownFlashCards.Count || _flashCards.Count != 0)
                btnNext.IsEnabled = true;
        }
    }
}
