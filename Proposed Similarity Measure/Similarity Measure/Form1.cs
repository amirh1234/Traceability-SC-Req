using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Similarity_Measure
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string TxtString, TxtString1;
        string name1;
        private void button1_Click(object sender, EventArgs e)
        {
            string s1, k1, s, k;
            double Total;
            Total = 0.0;
            string[] split;
            string[] split1;
            List<string> Doc = new List<string>();
          //  System.IO.StreamReader sread1, sread2;
            string name;
            name = "Test";
            for (int N = 1; N <= 10; N++)
            {
                name1 = name + N.ToString();
            }
          //  sread1 = System.IO.File.OpenText(@"D:\Innsbruck\A & A\Similarity Measure\Test01.txt"); //Code
          /*  sread2 = System.IO.File.OpenText(@"D:\Innsbruck\A & A\Similarity Measure\Test02.txt"); //Documents
            while (!sread2.EndOfStream)
            {
                string SR = sread2.ReadLine();
                Doc.Add(SR);
            } */
            //   while (!sread1.EndOfStream)
            //  {
           // string SR = "Add"; //sread1.ReadLine();
             //   split = SR.Split();
               // string Code = split[0];

                string words = "tea";  //Code Query
              //  for (int t = 0; t < Doc.Count(); t++)
             //   {
                    string words1 = "teacher"; //Doc Query
                    words = words.ToLower();
                    words1 = words1.ToLower();

                    s1 = words;
                    k1 = words1;
                    if (s1.Length > k1.Length)
                    {
                        k = s1;
                        s = k1;
                    }
                    else
                    {
                        s = s1;
                        k = k1;
                    }
                    char[] m; m = new char[2 * s.Length + k.Length];
                    int i;
                    for (i = 0; i < s.Length; i++)
                        m[i] = ' ';
                    int j = 0, l;
                    for (i = s.Length; i < k.Length + s.Length; i++, j++)
                        m[i] = k[j];
                    for (i = s.Length + k.Length; i < 2 * s.Length + k.Length; i++, j++)
                        m[i] = ' ';
                    // m[i] = '\0';
                    int[] mn = new int[s.Length + k.Length];  //*********
                    for (i = 0; i < s.Length + k.Length - 1; i++)
                    {
                        l = 0;
                        for (j = 0; j < s.Length; j++)
                        {
                            if (s[j] == m[i + j])
                                l++;
                        }
                        mn[i] = l;
                    }
                    int max = 0;
                    for (i = 0; i < s.Length + k.Length - 1; i++)
                    {
                        if (max < mn[i])
                            max = mn[i];
                        //    textBox1.Text = textBox1.Text + ',' + mn[i].ToString();
                    }
                    double percent = (float)max / s.Length;
                    //  return percent;
                    if (percent > 0.1)
                    {
                        Total = percent + Total;
                    }
            MessageBox.Show(Total.ToString());
            //     }

            //    MessageBox.Show(name1 + " " + ": " + percent);
            Total = 0;
            MessageBox.Show("Successful !!");
        }
    }
}
    

