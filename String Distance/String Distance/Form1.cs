using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SCICT.NLP.Utility.StringDistance;
namespace String_Distance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       // String_Distance
       

        private void Run_Click(object sender, EventArgs e)
        {
            SCICT.NLP.Utility.StringDistance.StringDistanceLayout layout = new SCICT.NLP.Utility.StringDistance.StringDistanceLayout();
            double cosine = layout.GetWordSimilarity("reward", "drawer", SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Cosine);
            double damerauLevenestain = layout.GetWordSimilarity("reward", "drawer", SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.DamerauLevenestain);
            double gnuLevenesain = layout.GetWordSimilarity("reward", "drawer", SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.GNULevenesain);
            double hamming = layout.GetWordSimilarity("reward", "drawer", SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Hamming);
            double jaroWinkler = layout.GetWordSimilarity("reward", "drawer", SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.JaroWinkler);
            double kashefi = layout.GetWordSimilarity("reward", "drawer", SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Kashefi);
            double levenestain = layout.GetWordSimilarity("reward", "drawer", SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Levenestain);
            double needleman = layout.GetWordSimilarity("reward", "drawer", SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Needleman);
            double wagnerFischer = layout.GetWordSimilarity("reward", "drawer", SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.WagnerFischer); 
        }

        private void File_Click(object sender, EventArgs e)
        {
            StreamReader reader = new StreamReader("E:\\Test.txt");
            while (reader.Read() != -1)
            {
                string tmp = reader.ReadLine();
                string[] fiels = tmp.Split(',');
                StringDistanceLayout layout = new StringDistanceLayout();
                double cosine = layout.GetWordSimilarity(fiels[0], fiels[1], SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Cosine);
                double damerauLevenestain = layout.GetWordSimilarity(fiels[0], fiels[1], SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.DamerauLevenestain);
                double gnuLevenesain = layout.GetWordSimilarity(fiels[0], fiels[1], SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.GNULevenesain);
                double hamming = layout.GetWordSimilarity(fiels[0], fiels[1], SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Hamming);
                double jaroWinkler = layout.GetWordSimilarity(fiels[0], fiels[1], SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.JaroWinkler);
                double kashefi = layout.GetWordSimilarity(fiels[0], fiels[1], SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Kashefi);
                double levenestain = layout.GetWordSimilarity(fiels[0], fiels[1], SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Levenestain);
                double needleman = layout.GetWordSimilarity(fiels[0], fiels[1], SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.Needleman);
                double wagnerFischer = layout.GetWordSimilarity(fiels[0], fiels[1], SCICT.NLP.Utility.StringDistance.StringDistanceAlgorithm.WagnerFischer); 
            }
            reader.Close();
        }

    }
}
