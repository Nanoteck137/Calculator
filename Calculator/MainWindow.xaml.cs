using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MAX_CHARACTERS_IN_RESULT = 22;

        private readonly string[,] buttonNames =
        {
            { "C",    "",   "<",  ""  },
            { "1",    "2",  "3",  "+" },
            { "4",    "5",  "6",  "-" },
            { "7",    "8",  "9",  "*" },
            { "^",    "0",  "=",  "/" },
            { "SQRT", "(",  ")",  "%" },
        };

        private Lexer lexer;
        private Parser parser;

        private TextBlock resultLabel;

        private bool error = false;

        public MainWindow()
        {
            InitializeComponent();

            lexer = new Lexer("");
            parser = new Parser(lexer);

            int numCols = buttonNames.GetLength(1);
            int numRows = buttonNames.GetLength(0) + 1;
            SetupGrid(numCols, numRows);
            SetupResultLabel();
            CreateButtons();
        }

        private void SetupResultLabel()
        {
            resultLabel = new TextBlock
            {
                Text = "",
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Right,
                TextWrapping = TextWrapping.WrapWithOverflow,

                FontSize = 40.0,
            };

            Grid.SetColumn(resultLabel, 0);
            Grid.SetRow(resultLabel, 0);
            Grid.SetColumnSpan(resultLabel, 4);
            this.grid.Children.Add(resultLabel);
        }

        private void SetupGrid(int numCols, int numRows)
        {
            for (int i = 0; i < numCols; i++)
            {
                ColumnDefinition def = new ColumnDefinition
                {
                    MaxWidth = 200.0
                };

                this.grid.ColumnDefinitions.Add(def);
            }

            for (int i = 0; i < numRows; i++)
            {
                this.grid.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void CreateButtons()
        {
            Dictionary<string, RoutedEventHandler> customClick = new Dictionary<string, RoutedEventHandler>()
            {
                { "C", ClearButtonClick },
                { "<", BackButtonClick },
                { "=", EqualsButtonClick }
            };

            for (int y = 0; y < buttonNames.GetLength(0); y++)
            {
                for (int x = 0; x < buttonNames.GetLength(1); x++)
                {
                    string name = buttonNames[y, x];
                    if (name == "")
                        continue;

                    Button button = new Button() { Content = name };
                    button.FontSize = 20.0;
                    button.MinWidth = 100.0;
                    button.MinHeight = 40.0;

                    if (customClick.ContainsKey(name))
                    {
                        button.Click += customClick[name];
                    }
                    else
                    {
                        button.Click += DefaultButtonClick;
                    }

                    if (name == "C" || name == "<")
                    {
                        Grid.SetColumnSpan(button, 2);
                    }

                    Grid.SetRow(button, y + 1);
                    Grid.SetColumn(button, x);

                    this.grid.Children.Add(button);
                }
            }
        }

        private bool IsOperator(char c)
        {
            return (c == '+') || (c == '-') || (c == '*') || (c == '/') || (c == '%');
        }

        private void DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            if (error)
            {
                resultLabel.Text = "";
                error = false;
            }

            if (resultLabel.Text.Length >= MAX_CHARACTERS_IN_RESULT)
                return;

            Button button = (Button)sender;
            string labelStr = resultLabel.Text;
            string buttonStr = (string)button.Content;

            // NOTE(patrik): Check if theres already a operator at the end of the label
            if (IsOperator(buttonStr[0]))
            {
                // NOTE(patrik): If theres a operator in the end of the string dont add another
                if (labelStr.Length > 0 && IsOperator(labelStr[labelStr.Length - 1]))
                {
                    return;
                }
                // NOTE(patrik): Check if the label is empty so the user cant add an operator in the beginning of the label
                else if (labelStr.Length == 0 && IsOperator(buttonStr[0]))
                {
                    if (buttonStr[0] != '-')
                        return;
                }
            }

            string resultStr = labelStr + buttonStr;
            resultLabel.Text = resultStr;
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            resultLabel.Text = "";
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            string str = resultLabel.Text;
            if (str.Length > 0)
            {
                resultLabel.Text = str.Substring(0, str.Length - 1);
            }
        }

        private void EqualsButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO(patrik): Do the calculation here
            string labelStr = resultLabel.Text;
            Console.WriteLine("DEBUG: Calculation String - '{0}'", labelStr);

            lexer.Reset(labelStr);
            try
            {
                // NOTE(patrik): Try to do the calculations
                Node node = parser.Parse();
                double result = node.GenerateNumber();

                // NOTE(patrik): Check if theres an error in generating the number
                if (double.IsPositiveInfinity(result) || double.IsNegativeInfinity(result))
                {
                    error = true;
                    return;
                }

                Console.WriteLine("DEBUG: Calculation Result - '{0}'", result);

                resultLabel.Text = result.ToString(CultureInfo.InvariantCulture);
            }
            catch (SyntaxErrorException)
            {
                // NOTE(patrik): Generate an error to the user
                error = true;
                resultLabel.Text = "SYNTAX ERROR";
                Console.WriteLine("DEBUG: Calculation Error");
            }
        }
    }
}
