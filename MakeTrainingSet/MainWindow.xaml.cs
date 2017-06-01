using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
//using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;

namespace MakeTrainingSet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtPrefix.Text = Settings1.Default.Prefix;
            txtFolder.Text = Settings1.Default.Folder;
            FlagBadInputs();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (Clipboard.ContainsText())
            {
                txtValue.Text = TidyText(Clipboard.GetText());
                SayUser("Click Yes or No");
                if (chkAutoYes.IsChecked ?? false)
                    SaveToFile(YES_SUFFIX);
                if (chkAutoNo.IsChecked ?? false)
                    SaveToFile(NO_SUFFIX);
            }
            else
            {
                txtValue.Text = String.Empty;
                SayUser("No text in clipboard");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveSettings();
            base.OnClosed(e);
        }

        private string TidyText( string str )
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ( Char.IsWhiteSpace(c) || c == ',')
                    sb.Append(' ');
                else if (c == '"')
                    sb.Append("'");
                else
                    sb.Append(c);
            }
            string result = sb.ToString();
            string twoSpaces = "  ", oneSpace = " ";
            while (result.Contains(twoSpaces))
                result = result.Replace(twoSpaces, oneSpace);
            return result;
        }

        private void SayUser( string msg )
        {
            lblSayuser.Content = msg;
        }

        private void SettingsChanged(object sender, TextChangedEventArgs e)
        {
            FlagBadInputs();
        }

        private void SaveSettings()
        {
            Settings1.Default.Folder = MyFolder;
            Settings1.Default.Prefix = MyPrefix;
            Settings1.Default.Save();
        }


        private void FlagBadInputs()
        {
            txtFolder.Background = ValidFolder ? Brushes.White : Brushes.LightPink;
            txtPrefix.Background = ValidPrefix ? Brushes.White : Brushes.LightPink;

        }

        private bool ValidFolder
        {
            get { return MyFolder != String.Empty && Directory.Exists(MyFolder); }
        }
        private bool ValidPrefix
        {
            get { return MyPrefix != String.Empty; }
        }

        private string MyFolder
        {
            get { return txtFolder.Text.Trim(); }
        }
        private string MyPrefix
        {
            get { return txtPrefix.Text.Trim(); }
        }
        private string MyValue
        {
            get { return txtValue.Text.Trim(); }
        }


        const string YES_SUFFIX = "Yes";
        const string NO_SUFFIX = "No";

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile(YES_SUFFIX);
            
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile(NO_SUFFIX);
        }

        private string lastValue = String.Empty;

        private bool SaveToFile( string suffix )
        {
            if (MyValue == String.Empty)
                return NoBecause("No text to save");
            if (MyValue == lastValue)
                return NoBecause("Already saved " + lastValue);
            if (!ValidFolder )
                return NoBecause("Please enter a valid folder");
            if ( !ValidPrefix)
                return NoBecause("Please enter a file prefix");
            try
            {
                string fullfilename = Path.Combine(MyFolder, MyPrefix + "_" + suffix + ".txt");
                using (StreamWriter sw = new StreamWriter(fullfilename, true))
                    sw.WriteLine(MyValue + "," + suffix);
                SayUser(DateTime.Now.ToString("HH:mm:ss") + " updated " + MyPrefix + "_" + suffix + ".txt");
                lastValue = MyValue;
                return true;
            }
            catch( Exception ex )
            {
                return NoBecause("ERROR " + ex.Message);
            }

        }

        private bool NoBecause(string msg)
        {
            SayUser(msg);
            return false;
        }
    }


}
