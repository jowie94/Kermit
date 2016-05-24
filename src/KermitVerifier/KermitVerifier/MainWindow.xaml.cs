using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Antlr.Runtime;
using Kermit.Parser;
using Kermit.Parser.Exceptions;
using Microsoft.Win32;

namespace KermitVerifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string m_fileName = "";
        public string FileContent = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() {Filter = "Kermit Scripts (*.k)|*.k"};
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                m_fileName = ofd.FileName;
                using (StreamReader reader = new StreamReader(ofd.OpenFile()))
                {
                    FileContent = reader.ReadToEnd();
                    TextRange tr = new TextRange(codeBox.Document.ContentStart, codeBox.Document.ContentEnd);
                    tr.Text = FileContent;
                }
            }
        }

        private void codeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange tr = new TextRange(codeBox.Document.ContentStart, codeBox.Document.ContentEnd);
            FileContent = tr.Text;
        }

        private void checkBtn_Click(object sender, RoutedEventArgs e)
        {
            GlobalScope global = new GlobalScope();
            ANTLRStringStream stream = new ANTLRStringStream(FileContent, m_fileName);
            KermitLexer lexer = new KermitLexer(stream);
            TokenRewriteStream tokens = new TokenRewriteStream(lexer);
            KermitParser parser = new KermitParser(tokens, global);
            parser.TreeAdaptor = new KermitAdaptor();
            parser.StopOnError = false;

            List<string> errors = new List<string>();

            AstParserRuleReturnScope<KermitAST, CommonToken> ret;
            try
            {
                ret = parser.program();
            }
            catch (PartialStatement)
            {
                errors.Add("Partial statement");
            }

            if (parser.NumberOfSyntaxErrors != 0)
                errors.Add($"{parser.NumberOfSyntaxErrors} syntax error(s)");

            MessageBox.Show(this, string.Join("\n", errors.Concat(parser.ErrorList.Select(x=> x.Message))));
        }
    }
}
