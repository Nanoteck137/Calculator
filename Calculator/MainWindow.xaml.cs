﻿using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[,] buttonNames =
        {
            { "C", "", "<", "" },
            { "1",    "2", "3", "+" },
            { "4",    "5", "6", "-" },
            { "7",    "8", "9", "*" },
            { "^",     "0", "=", "/" },
            { "SQRT", "(", ")", "%" },
        };

        private Lexer lexer;
        private Parser parser;

        private Label resultLabel;

        public MainWindow()
        {
            InitializeComponent();

            lexer = new Lexer("");
            parser = new Parser(lexer);



            for (int i = 0; i < buttonNames.GetLength(1); i++)
            {
                this.grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            this.grid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < buttonNames.GetLength(0); i++)
            {
                this.grid.RowDefinitions.Add(new RowDefinition());
            }

            resultLabel = new Label
            {
                Content = "(2+4)*2",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                FontSize = 40.0
            };
            Grid.SetColumn(resultLabel, 0);
            Grid.SetRow(resultLabel, 0);
            Grid.SetColumnSpan(resultLabel, 4);
            this.grid.Children.Add(resultLabel);

            CreateButtons();
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

        private void DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string labelStr = (string)resultLabel.Content;
            string buttonStr = (string)button.Content;

            string resultStr = labelStr + buttonStr;
            resultLabel.Content = resultStr;
        }



        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            resultLabel.Content = "";
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            string str = (string)resultLabel.Content;
            if (str.Length > 0)
            {
                resultLabel.Content = str.Substring(0, str.Length - 1);
            }
        }

        private void EqualsButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO(patrik): Do the calculation here
            string labelStr = (string)resultLabel.Content;
            Console.WriteLine("DEBUG: Calculation String - '{0}'", labelStr);

            lexer.Reset(labelStr);
            Node node = parser.Parse();
            double result = node.GenerateNumber();

            Console.WriteLine("DEBUG: Calculation Result - '{0}'", result);

            resultLabel.Content = result.ToString(CultureInfo.InvariantCulture);
        }
    }
}
