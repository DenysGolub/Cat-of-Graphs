using Main.Classes;
using Main.Enumerators;
using Main.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Main.TestingPart
{
    public class Question//: ITestingPart
    {

        readonly HashSet<QuestionsType> valuesAsList = Enum.GetValues(typeof(QuestionsType)).Cast<QuestionsType>().ToHashSet();
        protected string description = "";
        protected QuestionsType question_type;
        private GraphType graphType;

        protected Canvas graph = null;

        public string Description { get => description; set => description = value; }
        public Canvas GraphImage { get => graph; set => graph=value; }


        public QuestionsType QuestionsType { get { return question_type; } set => question_type=value; }
        public double Points { get { return points; } set => points = value; }
        protected double points = 0.0;

        public HashSet<QuestionsType> Types => valuesAsList;
        public GraphType GraphType { get => graphType; set => graphType = value; }

        /*public override string ToString()
        {
            return question_type.ToString();
        }*/

        private byte[] img_bytes = null;

        /// <summary>
        /// Image stored in byte array
        /// </summary>
        public byte[] ImageSource { get => img_bytes; set => img_bytes = value; }

        public AdjacenceList IncMatrix { get => _inc; set => _inc = value; }
        private AdjacenceList _inc = new AdjacenceList();
        public AdjacenceList AdjMatrix { get => _adj; set => _adj = value; }
        private AdjacenceList _adj = new AdjacenceList();

        public dynamic State { get; set; }
        private AdjacenceList corr_adj_matrix = new AdjacenceList();
        public dynamic CorrectAnswer
        {
            get
            {
                return corr_adj_matrix;
            }
            set
            {
                corr_adj_matrix = value;
            }
        }
    }

    public class QuestionWithTwoCanvas:Question
    {
        public QuestionWithTwoCanvas()
        {
           
        }
    }
}
