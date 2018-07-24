using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using LAIR.ResourceAPIs.WordNet;
using LAIR.Collections.Generic;

namespace TestApplication
{
    public partial class TestForm : Form
    {
        private WordNetEngine _wordNetEngine;
        private SynSet _semSimSs1;
        private SynSet _semSimSs2;
        private string _origSsLbl;
        private WordNetSimilarityModel _semanticSimilarityModel;

        public TestForm()
        {
            InitializeComponent();

            // create wordnet engine (use disk-based retrieval by default)
            string root = Directory.GetDirectoryRoot(".");
            _wordNetEngine = new WordNetEngine(root + @"NLP\Resources\WordNet_3", false);

            if (!_wordNetEngine.InMemory)
                test.Text += " (will take a while)";

            // populate POS list
            foreach (WordNetEngine.POS p in Enum.GetValues(typeof(WordNetEngine.POS)))
                if (p != WordNetEngine.POS.None)
                    pos.Items.Add(p);

            pos.SelectedIndex = 0;

            // allow scrolling of synset list
            synSets.HorizontalScrollbar = true;

            _semSimSs1 = _semSimSs2 = null;
            _origSsLbl = ss1.Text;
            _semanticSimilarityModel = new WordNetSimilarityModel(_wordNetEngine);
        }

        private void getSynSets_Click(object sender, EventArgs e)
        {
            System.IO.StreamReader sread1, sread2;
            sread1 = System.IO.File.OpenText(@"F:\IR Project\PreProcessing\Pre.txt");
            //  sread2 = System.IO.File.OpenText(@"F:\Machine Translation\Dianati Seminar\S2.txt");
            System.IO.StreamWriter swrite2;
            swrite2 = System.IO.File.CreateText(@"F:\IR Project\Project\S2.txt");
            //string[] stem;
            //   int under = 0, over = 0, i = 0, Eror = 0;
            while (!sread1.EndOfStream)
            {
                //     i++;
                string wordman = sread1.ReadLine();
                //     string wordstem = sread2.ReadLine();


                string[] stem;
                stem = wordman.Split(new char[] { ',' });
                for (int y = 0; y < stem.Length; y++)
                {
                    if (stem[y].ToString().Trim() != "")
                    {
                        synSets.Items.Clear();
                        semanticRelations.Items.Clear();
                        lexicalRelations.Items.Clear();
                        getRelatedSynSets.Enabled = false;

                        // retrive synsets
                        Set<SynSet> synSetsToShow = null;
                        if (synsetID.Text != "")
                        {
                            try { synSetsToShow = new Set<SynSet>(new SynSet[] { _wordNetEngine.GetSynSet(synsetID.Text) }); }
                            catch (Exception)
                            {
                                MessageBox.Show("Invalid SynSet ID");
                                return;
                            }
                        }
                        else
                        {
                            // get single most common synset
                            if (true)
                            {
                                try
                                {
                                    SynSet synset = _wordNetEngine.GetMostCommonSynSet(stem[y].ToString().Trim(), (WordNetEngine.POS)pos.SelectedItem);
                                    if (synset != null)
                                        synSetsToShow = new Set<SynSet>(new SynSet[] { synset });

                                }
                                catch (Exception ex) { MessageBox.Show("Error:  " + ex); return; }
                            }
                            // get all synsets
                            else
                            {
                                try { synSetsToShow = _wordNetEngine.GetSynSets(word.Text, (WordNetEngine.POS)pos.SelectedItem); }
                                catch (Exception ex) { MessageBox.Show("Error:  " + ex); return; }
                            }
                        }

                       try{
                        if (synSetsToShow.Count > 0)
                        {
                            string w1 = "";

                            foreach (SynSet synSet in synSetsToShow)
                            {
                                synSets.Items.Add(synSet);
                                string word = synSets.Items[0].ToString();
                                for (int k = 0; k < word.Length; k++)
                                {
                                    if (word[k] == '{')
                                        continue;
                                    if (word[k] == '}')
                                        break;

                                    w1 = w1 + word[k];
                                }

                                swrite2.Write( w1.ToString().Trim() + ",");
                                swrite2.Flush();
                                //      swrite2.Close();
                                //   MessageBox.Show(w1);

                            }
                        }
                    }
                        catch 
                      //  else
                        {
                            swrite2.Write(stem[y].ToString().Trim() + ",");
                        }
                    }


                }
                swrite2.WriteLine();
                swrite2.Flush();
                
            }
            MessageBox.Show("Sucsessful");
        }

        private void synSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            semanticRelations.Items.Clear();
            lexicalRelations.Items.Clear();
            computeSemSim.Enabled = false;
            getRelatedSynSets.Enabled = synSets.SelectedItem != null;

            // select a synset if none is selected and there is one
            if (synSets.SelectedItems.Count == 0)
            {
                if (synSets.Items.Count > 0)
                    synSets.SelectedIndex = 0;
            }
            else if (synSets.SelectedItems.Count == 1)
            {
                SynSet selectedSynSet = synSets.SelectedItem as SynSet;

                // populate semantic relation list for the current synset
                semanticRelations.Items.Clear();
                foreach (WordNetEngine.SynSetRelation synSetRelation in selectedSynSet.SemanticRelations)
                    semanticRelations.Items.Add(synSetRelation + ":  " + selectedSynSet.GetRelatedSynSetCount(synSetRelation));

                // populate list of lexically related words for the current synset
                lexicalRelations.Items.Clear();
                Dictionary<WordNetEngine.SynSetRelation, Dictionary<string, Set<string>>> lexicallyRelatedWords = selectedSynSet.GetLexicallyRelatedWords();
                foreach (WordNetEngine.SynSetRelation lexicalRelation in lexicallyRelatedWords.Keys)
                    foreach (string word1 in lexicallyRelatedWords[lexicalRelation].Keys)
                        foreach (string word2 in lexicallyRelatedWords[lexicalRelation][word1])
                            lexicalRelations.Items.Add(word1 + " -- " + lexicalRelation + " --> " + word2);

                // show id
                synsetID.Text = selectedSynSet.ID;
            }
        }

        private void getRelatedSynSets_Click(object sender, EventArgs e)
        {
            SynSet selectedSynSet = synSets.SelectedItem as SynSet;

            if (selectedSynSet == null || semanticRelations.SelectedIndex == -1)
                return;

            synSets.Items.Clear();

            // get relations
            string relationStr = semanticRelations.SelectedItem.ToString();
            relationStr = relationStr.Split(':')[0].Trim();
            WordNetEngine.SynSetRelation relation = (WordNetEngine.SynSetRelation)Enum.Parse(typeof(WordNetEngine.SynSetRelation), relationStr);

            // add related synset
            foreach (SynSet relatedSynset in selectedSynSet.GetRelatedSynSets(relation, false))
                synSets.Items.Add(relatedSynset);

            // selected synset
            if (synSets.Items.Count > 0)
                synSets.SelectedIndex = 0;
        }

        private void synSetRelations_DoubleClick(object sender, EventArgs e)
        {
            getRelatedSynSets_Click(sender, e);
        }

        private void word_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                getSynSets_Click(sender, e);
        }

        private void test_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(delegate()
                {
                    Invoke(new MethodInvoker(delegate() { Enabled = false; }));

                    // test all words
                    Dictionary<WordNetEngine.POS, Set<string>> words = _wordNetEngine.AllWords;
                    foreach (WordNetEngine.POS pos in words.Keys)
                        foreach (string word in words[pos])
                        {
                            // get synsets
                            Set<SynSet> synsets = _wordNetEngine.GetSynSets(word, pos);
                            if (synsets.Count == 0)
                                if (MessageBox.Show("Failed to get synset for " + pos + ":  " + word + ". Quit?", "Quit?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    return;

                            // make sure there's a most common synset
                            if (_wordNetEngine.GetMostCommonSynSet(word, pos) == null)
                                throw new NullReferenceException("Failed to find most common synset");

                            // check each synset
                            foreach (SynSet synset in synsets)
                            {
                                // check lexically related words
                                synset.GetLexicallyRelatedWords();

                                // check relations
                                foreach (WordNetEngine.SynSetRelation relation in synset.SemanticRelations)
                                    synset.GetRelatedSynSets(relation, false);

                                // check lex file name
                                if (synset.LexicographerFileName == WordNetEngine.LexicographerFileName.None)
                                    throw new Exception("Invalid lex file name");
                            }
                        }

                    MessageBox.Show("Test completed. Everything looks okay.");

                    Invoke(new MethodInvoker(delegate() { Enabled = true; }));
                }));

            t.Start();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void word_TextChanged(object sender, EventArgs e)
        {
            synsetID.TextChanged -= new EventHandler(synsetID_TextChanged);
            synsetID.Text = "";
            synsetID.TextChanged += new EventHandler(synsetID_TextChanged);
        }

        private void synsetID_TextChanged(object sender, EventArgs e)
        {
            word.TextChanged -= new EventHandler(word_TextChanged);
            word.Text = "";
            word.TextChanged += new EventHandler(word_TextChanged);
        }

        private void synsetID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                getSynSets_Click(sender, e);
        }

        private void computeSemSim_Click(object sender, EventArgs e)
        {
            if (_semSimSs1.POS != _semSimSs2.POS)
            {
                MessageBox.Show("Selected synsets must have the same part-of-speech.");
                return;
            }

            string result = "";
            foreach (WordNetSimilarityModel.Strategy strategy in Enum.GetValues(typeof(WordNetSimilarityModel.Strategy)))
            {
                float sim = _semanticSimilarityModel.GetSimilarity(_semSimSs1, _semSimSs2, strategy, WordNetEngine.SynSetRelation.Hypernym);
                result += strategy + ":  " + sim + Environment.NewLine;
            }

            MessageBox.Show(result);
        }

        private void synSets_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (synSets.SelectedItem == null)
                return;

            SynSet s = (SynSet)synSets.SelectedItem;
            if (_semSimSs1 == null)
            {
                _semSimSs1 = s;
                ss1.Text = _semSimSs1.ToString() + " (double-click to remove)";
            }
            else if (_semSimSs2 == null)
            {
                _semSimSs2 = s;
                ss2.Text = _semSimSs2.ToString()  + " (double-click to remove)";
            }
            else
                MessageBox.Show("Please remove one of the synsets selected below (double-click it)");

            computeSemSim.Enabled = _semSimSs1 != null && _semSimSs2 != null;
        }

        private void ss1_DoubleClick(object sender, EventArgs e)
        {
            ss1.Text = _origSsLbl;
            _semSimSs1 = null;
            computeSemSim.Enabled = false;
        }

        private void ss2_DoubleClick(object sender, EventArgs e)
        {
            ss2.Text = _origSsLbl;
            _semSimSs2 = null;
            computeSemSim.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // private Random random = new Random();
            Random random = new Random();
            for (int i = 0; i < 10; i++)
               MessageBox.Show (random.Next(1, 200).ToString()); 
            //Console.ReadKey();
        }
    }
}
