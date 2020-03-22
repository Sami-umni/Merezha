using System;
using System.Linq;
using System.Windows;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.Controls;
using Merezha.Moduls;
using Microsoft.Win32;
using System.Windows.Media;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Merezha
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        List<int[]> matrix = new List<int[]>();
        int numOfVertices = 0;        

        public MainWindow()
        {
            InitializeComponent();
            
            
            //Customize Zoombox a bit
            //Set minimap (overview) window to be visible by default
            ZoomControl.SetViewFinderVisibility(zoomctrl, Visibility.Visible);
            //Set Fill zooming strategy so whole graph will be always visible
            zoomctrl.ZoomToFill();

            //Lets setup GraphArea settings
            
            GraphAreaExample_Setup();
            gg_but_randomgraph.Click += gg_but_randomgraph_Click;
            //gg_but_relayout.Click += gg_but_relayout_Click;

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //lets create graph
            //Note that you can't create it in class constructor as there will be problems with visuals
            gg_but_randomgraph_Click(null, null);
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter =  "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                matrix = new Reading().ReadFile(openFileDialog.FileName);
            }
            numOfVertices = new Reading().MaxValue(matrix);
            GraphAreaExample_Setup();
            
        }

        void gg_but_relayout_Click(object sender, RoutedEventArgs e)
        {
            //This method initiates graph relayout process which involves consequnet call to all selected algorithms.
            //It behaves like GenerateGraph() method except that it doesn't create any visual object. Only update existing ones
            //using current Area.Graph data graph.
            Area.RelayoutGraph();
            zoomctrl.ZoomToFill();
        }



        void gg_but_randomgraph_Click(object sender, RoutedEventArgs e)
        {
            //Lets generate configured graph using pre-created data graph assigned to LogicCore object.
            //Optionaly we set first method param to True (True by default) so this method will automatically generate edges
            //  If you want to increase performance in cases where edges don't need to be drawn at first you can set it to False.
            //  You can also handle edge generation by calling manually Area.GenerateAllEdges() method.
            //Optionaly we set second param to True (True by default) so this method will automaticaly checks and assigns missing unique data ids
            //for edges and vertices in _dataGraph.
            //Note! Area.Graph property will be replaced by supplied _dataGraph object (if any).
            Area.GenerateGraph(true, true);
            
            /* 
             * After graph generation is finished you can apply some additional settings for newly created visual vertex and edge controls
             * (VertexControl and EdgeControl classes).
             * 
             */

            //This method sets the dash style for edges. It is applied to all edges in Area.EdgesList. You can also set dash property for
            //each edge individually using EdgeControl.DashStyle property.
            //For ex.: Area.EdgesList[0].DashStyle = GraphX.EdgeDashStyle.Dash;
            Area.SetEdgesDashStyle(EdgeDashStyle.Dash);
            
            
            //This method sets edges arrows visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
            //each edge individually using property, for ex: Area.EdgesList[0].ShowArrows = true;
            Area.ShowAllEdgesArrows(true);
            
            //This method sets edges labels visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
            //each edge individually using property, for ex: Area.EdgesList[0].ShowLabel = true;
            Area.ShowAllEdgesLabels(true);
            zoomctrl.ZoomToFill();
        }

        private GraphExample GraphExample_Setup()
        {
            //Lets make new data graph instance
            var dataGraph = new GraphExample();
            //Now we need to create edges and vertices to fill data graph
            //This edges and vertices will represent graph structure and connections
            //Lets make some vertices
            for (int i = 1; i <= numOfVertices; i++)
            {
                //Create new vertex with specified Text. Also we will assign custom unique ID.
                //This ID is needed for several features such as serialization and edge routing algorithms.
                //If you don't need any custom IDs and you are using automatic Area.GenerateGraph() method then you can skip ID assignment
                //because specified method automaticaly assigns missing data ids (this behavior is controlled by method param).

                var dataVertex = new DataVertex {ID = i, Text = string.Format("{0}", i) };
                //Add vertex to data graph
                
                dataGraph.AddVertex(dataVertex);
            }

            if (matrix == null)
            {

                //Now lets make some edges that will connect our vertices
                //get the indexed list of graph vertices we have already added

                var vlist = dataGraph.Vertices.ToList();

                //Then create two edges optionaly defining Text property to show who are connected

                var dataEdge = new DataEdge(vlist[0], vlist[1], 3) { Text = string.Format("{0} -> {1}", vlist[0], vlist[1]) };
                dataGraph.AddEdge(dataEdge);
                dataEdge = new DataEdge(vlist[2], vlist[3], 3) { Text = string.Format("{0} -> {1}", vlist[2], vlist[3]) };
                dataGraph.AddEdge(dataEdge);
            }
            else
            {

                var vlist = dataGraph.Vertices.ToList();
                for (int i = 0; i < matrix.Count; i++)
                {
                                    
                    var dataEdge = new DataEdge(Search(dataGraph, matrix[i][0]), Search(dataGraph, matrix[i][1]), matrix[i][2])
                    { Text = string.Format("{0}", matrix[i][2]) };
                    dataGraph.AddEdge(dataEdge);
                }
            }
            return dataGraph;
        }

        public DataVertex Search(GraphExample GE, int vert)
        {
            DataVertex rezult = null;
            var list = GE.Vertices.ToList();
            foreach(DataVertex p in list)
            {
                if (p.ID == vert)
                    rezult = p;
            }
            return rezult;
        }

        private void GraphAreaExample_Setup()
        {
            //Lets create logic core and filled data graph with edges and vertices
            var logicCore = new GXLogicCoreExample() { Graph = GraphExample_Setup() };
            
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Sugiyama;
            //Now we can set parameters for selected algorithm using AlgorithmFactory property. This property provides methods for
            //creating all available algorithms and algo parameters.
            logicCore.DefaultLayoutAlgorithmParams = logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
            //Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
            ((KKLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            //Default parameters are created automaticaly when new default algorithm is set and previous params were NULL
            logicCore.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 100;
            logicCore.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 100;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            //Bundling algorithm will try to tie different edges that follows same direction to a single channel making complex graphs more appealing.
            logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = true;

            //Finally assign logic core to GraphArea object
            Area.LogicCore = logicCore;
        }

        public void Dispose()
        {
            //If you plan dynamicaly create and destroy GraphArea it is wise to use Dispose() method
            //that ensures that all potential memory-holding objects will be released.
            Area.Dispose();
        }

        private void Search_The_Way_Click(object sender, RoutedEventArgs e)
        {
            Method_Minti Method = new Method_Minti(GraphExample_Setup(), matrix);
            GraphExMinti AftMinti = Method.find_the_way();
            tbSettingText.Clear();
            tbSettingText.AppendText("Список шляхів та відстаней:");
            tbSettingText.AppendText(Environment.NewLine);
            for (int i = 1; i < numOfVertices; i++)
            {
                string LineRezult = "";
                DataVertex tmp = AftMinti.Vertex[i].vert;
                tbSettingText.AppendText(string.Format("H = {0} Дуга: ", AftMinti.Vertex[i].dist));
                
                while (!(tmp == AftMinti.Vertex[0].vert)) {
                    foreach (DataEdgeMinti j in AftMinti.Edges)
                    {
                        if (j.Edges.Target == tmp && j.Label)
                        {
                            LineRezult += tmp.Text + "-";                            
                            tmp = j.Edges.Source;
                            break;
                        }
                    }
                    
                }
                LineRezult += "1";
                char[] s = LineRezult.ToCharArray();
                Array.Reverse(s);
                
                tbSettingText.AppendText(new string(s));
                tbSettingText.AppendText(Environment.NewLine);
            }
        }

        private void Add_new_vertex_and_edge(object sender, RoutedEventArgs e)
        {
            try
            {
                int[] a = textBox.Text.Split(new char[] { ' '}).Select(Int32.Parse).ToArray();
                matrix.Add(a);
                numOfVertices = new Reading().MaxValue(matrix);
                GraphAreaExample_Setup();
                gg_but_randomgraph_Click(null, null);
            }
            catch
            {
                MessageBox.Show("Невірний формат даних");
            }
        }

        
        private void save_btn(object sender, RoutedEventArgs e)
        {
            try {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                saveFileDialog1.ShowDialog();
                // получаем выбранный файл
                string filename = saveFileDialog1.FileName;
                // сохраняем текст в файл
                string pole = "";
                foreach(int[] i in matrix)
                {
                    pole += i[0] + " " + i[1] + " " + i[2] + "\n";
                }
                File.WriteAllText(filename, pole);
                MessageBox.Show("Файл сохранен");
            }
            catch
            {
                MessageBox.Show("Файл не був збережений. Введіть назву файлу коректно!", "Помилка");
            }
        }

        private void AboutProgram_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Автори:\n-> Івасюта Павло\n-> Костюк Віталій\n-> Дубець Василь\n-> Георгіян Євген\n-> Козуб Микола\n\n" + "Викладач:\n-> Руснак Микола Андрійович\n\nЧНУ 2020", "Програма розроблена:" );
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox.Text == "Vert1 Vert2 Distance")
            {
                textBox.Text = "";
            }
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                textBox.Text = "Vert1 Vert2 Distance";
        }
    }
}
