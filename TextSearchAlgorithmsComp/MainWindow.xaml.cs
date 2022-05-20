using MaterialDesignExtensions.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TextSearchAlgorithmsComp
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public readonly static int d = 2048;
        private string textFromFile = "";

        //A utility function to get maximum of two integers
        private static int max(int a, int b) { return (a > b) ? a : b; }

        bool IsDigitsOnly(string s) => s.All(char.IsDigit);

        long CalculateAvg(List<long> li)
        {
            long avg = 0;
            foreach (long l in li)
                avg += l;
            return avg/li.Count();
        }

        private static void badCharHeuristic(string str, int size, int[] badchar)
        {
            int i;

            // Initialize all occurrences as -1
            for (i = 0; i < d; i++)
                badchar[i] = -1;

            // Fill the actual value of last occurrence of a character
            for (i = 0; i < size; i++)
                badchar[(int)str[i]] = i;
        }

        private int boyersMoore(string txt, string pat)
        {
            int m = pat.Length;
            int n = txt.Length;

            int[] badchar = new int[d];

            // Fill the bad character array 
            badCharHeuristic(pat, m, badchar);

            int s = 0; // shift of the pattern with respect to text
            while (s <= (n - m))
            {
                int j = m - 1;

                while (j >= 0 && pat[j] == txt[s + j])
                    j--;

                if (j < 0)
                {
                    //FOUND
                    return s;

                }

                else
                    s += max(1, j - badchar[txt[s + j]]);
            }
            return -1;
        }

        // Bruteforce text pattern search
        private int bruteForce(string text, string pattern)
        {
            // Text fragment to compare
            string compText;
            for (int i = 0; i <= text.Length-pattern.Length; i++)
            {
                // Creating fragment
                compText = "";
                for (int j = 0; j < pattern.Length; j++)
                    compText+=text[j+i];
                // Match check
                if (compText == pattern)
                    return i;
            }
            return -1;
        }

        private int KMPSearch(string txt, string pat)
        {
            int M = pat.Length;
            int N = txt.Length;

            // create lps[] that will hold the longest prefix suffix values for pattern
            int[] lps = new int[M];
            int j = 0; // index for pat[]

            // Preprocess the pattern (calculate lps[] array)
            computeLPSArray(pat, M, lps);

            int i = 0; // index for txt[]
            while (i < N)
            {
                if (pat[j] == txt[i])
                {
                    j++;
                    i++;
                }
                if (j == M)
                {
                    j = lps[j - 1];
                    return (i - j);
                }

                // mismatch after j matches
                else if (i < N && pat[j] != txt[i])
                {
                    if (j != 0)
                        j = lps[j - 1];
                    else
                        i = i + 1;
                }
            }
            return -1;
        }



        void computeLPSArray(string pat, int M, int[] lps)
        {
            // length of the previous longest prefix suffix
            int len = 0;
            int i = 1;
            lps[0] = 0; // lps[0] is always 0

            // the loop calculates lps[i] for i = 1 to M-1
            while (i < M)
            {
                if (pat[i] == pat[len])
                {
                    len++;
                    lps[i] = len;
                    i++;
                }
                else
                {
                    if (len != 0)
                    {
                        len = lps[len - 1];
                    }
                    else
                    {
                        lps[i] = len;
                        i++;
                    }
                }
            }
        }

        private int rabinKarp(String txt, String pat)
        {
            // Random huge prime number
            int q = 1000000007;

            int M = pat.Length;
            int N = txt.Length;
            int i, j;
            int p = 0; // hash value for pattern
            int t = 0; // hash value for txt
            int h = 1;

            // The value of h would be "pow(d, M-1)%q"
            for (i = 0; i < M - 1; i++)
                h = (h * d) % q;

            // Calculate the hash value of pattern and first window of text
            for (i = 0; i < M; i++)
            {
                p = (d * p + pat[i]) % q;
                t = (d * t + txt[i]) % q;
            }

            // Slide the pattern over text one by one
            for (i = 0; i <= N - M; i++)
            {

                //If the hash values match then only check for characters one by one
                if (p == t)
                {
                    /* Check for characters one by one */
                    for (j = 0; j < M; j++)
                    {
                        if (txt[i + j] != pat[j])
                            break;
                    }

                    // if p == t and pat[0...M-1] = txt[i, i+1, ...i+M-1]
                    if (j == M)
                        //FOUND SEARCH
                        return i;
                }

                // Calculate hash value
                if (i < N - M)
                {
                    t = (d * (t - txt[i] * h) + txt[i + M]) % q;

                    if (t < 0)
                        t = (t + q);
                }
            }
            return -1;
        }

        private async void onFileSelectionBtnClick(object sender, RoutedEventArgs e)
        {

            string path = "";
            // open file
            OpenFileDialog dlg = new OpenFileDialog();


            dlg.Filter = "Text|*.txt";


            dlg.DefaultExt = ".png"; // Default file extension 

            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                path = dlg.FileName;
                textFromFile = System.IO.File.ReadAllText($@"{path}");

                filedatastore.Text = textFromFile;

            }
        }

        private void onCompareCheckboxChange(object sender, RoutedEventArgs e)
        {
            if (compareAllCheckbox.IsChecked==true)
            {
                showHighest.IsEnabled = true;
                showLowest.IsEnabled = true;
            }
            else
            {
                showHighest.IsChecked = false;
                showLowest.IsChecked = false;
                showHighest.IsEnabled = false;
                showLowest.IsEnabled = false;
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            int repeat;
            if (textFromFile == "")
            {
                ErrorSnackBar.MessageQueue?.Enqueue(
                                "Nie wprowadzono danych do przeszukania.",
                                null, null, null, false, true, TimeSpan.FromSeconds(2));
                return;
            }
            if (toFind.Text == "")
            {
                ErrorSnackBar.MessageQueue?.Enqueue(
                                "Nie wprowadzono danych do znalezienia.",
                                null, null, null, false, true, TimeSpan.FromSeconds(2));
                return;
            }
            if(numOfRepeats.Text == "")
            {
                ErrorSnackBar.MessageQueue?.Enqueue(
                                "Nie wprowadzono ilości powtórzeń.",
                                null, null, null, false, true, TimeSpan.FromSeconds(2));
                return;
            }
            if (!int.TryParse(numOfRepeats.Text,out repeat))
            {
                ErrorSnackBar.MessageQueue?.Enqueue(
                                "Ilość powtórzeń nie jest poprawną liczbą.",
                                null, null, null, false, true, TimeSpan.FromSeconds(2));
                return;
            }
            
            if (repeat == 0)
            {
                ErrorSnackBar.MessageQueue?.Enqueue(
                                "Ilość powtórzeń jest zbyt niska.",
                                null, null, null, false, true, TimeSpan.FromSeconds(2));
                return;
            }
            timersDisplayer.Content = "";
            if (compareAllCheckbox.IsChecked == true) {
                timersDisplayer.HorizontalAlignment = HorizontalAlignment.Left;
                timersDisplayer.VerticalAlignment = VerticalAlignment.Top;
                timersDisplayer.FontSize = 12;
                List<long> values = new List<long>(4);
                values.Add(0);
                values.Add(0);
                values.Add(0);
                values.Add(0);

                List<long> toCalc = new List<long>();
                Stopwatch stopwatch = new Stopwatch();

                for(int i = 0; i < repeat; i++)
                {
                    stopwatch.Start();
                    KMPSearch(textFromFile, textFromFile);
                    stopwatch.Stop();
                    toCalc.Add(stopwatch.ElapsedTicks);
                    stopwatch.Reset();

                }
                values[0] = CalculateAvg(toCalc);
                toCalc.Clear();

                for (int i = 0; i < repeat; i++)
                {
                    stopwatch.Start();
                    rabinKarp(textFromFile, textFromFile);
                    stopwatch.Stop();
                    toCalc.Add(stopwatch.ElapsedTicks);
                    stopwatch.Reset();

                }
                values[1] = CalculateAvg(toCalc);
                toCalc.Clear();

                for (int i = 0; i < repeat; i++)
                {
                    stopwatch.Start();
                    boyersMoore(textFromFile, textFromFile);
                    stopwatch.Stop();
                    toCalc.Add(stopwatch.ElapsedTicks);
                    stopwatch.Reset();

                }
                values[2] = CalculateAvg(toCalc);
                toCalc.Clear();

                for (int i = 0; i < repeat; i++)
                {
                    stopwatch.Start();
                    bruteForce(textFromFile, textFromFile);
                    stopwatch.Stop();
                    toCalc.Add(stopwatch.ElapsedTicks);
                    stopwatch.Reset();

                }
                values[3] = CalculateAvg(toCalc);
                toCalc.Clear();



                List<string> displayVals = new List<string>(4);
                displayVals.Add("KMP: ");
                displayVals.Add("rabinKarp: ");
                displayVals.Add("boyersMoore: ");
                displayVals.Add("bruteForce: ");

                for (int i = 0; i<4; i++)
                {
                    displayVals[i] += values[i].ToString() + " ticks";
                }
                if (showHighest.IsChecked == true)
                {
                    displayVals[values.FindIndex(x => x == values.Max())] += " - NAJWIĘKSZY CZAS";
                }
                if (showLowest.IsChecked == true)
                {
                    displayVals[values.FindIndex(x => x == values.Min())] += " - NAJMNIEJSZY CZAS";
                }
                for (int i = 0; i < 3; i++)
                {
                    displayVals[i] += "\n";
                }
                foreach (string val in displayVals)
                {
                    timersDisplayer.Content += val;
                }
            }
            else
            {
                timersDisplayer.HorizontalAlignment = HorizontalAlignment.Center;
                timersDisplayer.VerticalAlignment = VerticalAlignment.Center;
                timersDisplayer.FontSize = 20;
                timersDisplayer.Content = $"{(algSelector.SelectedItem as ListBoxItem).Content.ToString()}: ";
                Stopwatch stopwatch = new Stopwatch();

                List<long> toCalc = new List<long>();
                switch ((algSelector.SelectedItem as ListBoxItem).Content.ToString())
                {
                    case "bruteforce":
                        for (int i = 0; i < repeat; i++)
                        {
                            stopwatch.Start();
                            bruteForce(textFromFile, textFromFile);
                            stopwatch.Stop();
                            toCalc.Add(stopwatch.ElapsedTicks);
                            stopwatch.Reset();

                        }
                        timersDisplayer.Content += $"{CalculateAvg(toCalc)} ticks";
                        toCalc.Clear();
                        break;
                    case "KMP":
                        for (int i = 0; i < repeat; i++)
                        {
                            stopwatch.Start();
                            KMPSearch(textFromFile, textFromFile);
                            stopwatch.Stop();
                            toCalc.Add(stopwatch.ElapsedTicks);
                            stopwatch.Reset();

                        }
                        timersDisplayer.Content += $"{CalculateAvg(toCalc)} ticks";
                        toCalc.Clear();
                        break;
                    case "Boyers Moore":
                        for (int i = 0; i < repeat; i++)
                        {
                            stopwatch.Start();
                            boyersMoore(textFromFile, textFromFile);
                            stopwatch.Stop();
                            toCalc.Add(stopwatch.ElapsedTicks);
                            stopwatch.Reset();

                        }
                        timersDisplayer.Content += $"{CalculateAvg(toCalc)} ticks";
                        toCalc.Clear();
                        break;
                    case "Rabin - Karp":
                        for (int i = 0; i < repeat; i++)
                        {
                            stopwatch.Start();
                            rabinKarp(textFromFile, textFromFile);
                            stopwatch.Stop();
                            toCalc.Add(stopwatch.ElapsedTicks);
                            stopwatch.Reset();

                        }
                        timersDisplayer.Content += $"{CalculateAvg(toCalc)} ticks";
                        toCalc.Clear();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
