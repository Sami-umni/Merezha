using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merezha.Moduls;
namespace Merezha
{
    class Method_Minti
    {
        GraphExMinti Graph;
        GraphExample baseGraph;
        List<DataVertexMinti> list_Vert_I = new List<DataVertexMinti>();
        List<DataVertexMinti> list_Vert_J = new List<DataVertexMinti>();
        List<DataEdgeMinti> list_Edges = new List<DataEdgeMinti>();
        List<int[]> Matrix = null;
        public Method_Minti(GraphExample graph, List<int[]> matrix)
        {
            baseGraph = graph;
            var listV = graph.Vertices.ToList();
            foreach (DataVertex t in listV)
            {
                var p = new DataVertexMinti(t);
                list_Vert_I.Add(p);
            }
            
            Matrix = matrix;
            var listE = graph.Edges.ToList();
            foreach (DataEdge t in listE)
            {
                var p = new DataEdgeMinti(t);
                list_Edges.Add(p);
            }
           
        }

        public GraphExMinti find_the_way()
        {
            //int[,] matr_dist = Distanse_Matr();
            DataVertexMinti temp =  Search(list_Vert_I, 1);
            list_Vert_I.Remove(Search(list_Vert_I, 1));
            list_Vert_J.Add(temp);
            while (list_Vert_I.Count > 0)
            {
                int min_dist = int.MaxValue;
                DataEdgeMinti EdgeForLabel = null;
                foreach (DataVertexMinti i in list_Vert_J)
                {
                    //var tmp =  list_Edges.Select(p => p.Source == i.vert && !p.Label);     TRASH               
                    foreach (DataEdgeMinti j in list_Edges)
                    {
                        if (!j.Label && j.Edges.Source == i.vert && list_Vert_I.Contains(Search(list_Vert_I , Convert.ToInt32(j.Edges.Target.ID))))
                        {
                            var dist = Convert.ToInt32(j.Edges.Weight) + Search(list_Vert_J, Convert.ToInt32(j.Edges.Source.ID)).dist;
                            if (min_dist > dist)
                            {
                                min_dist = dist;
                                EdgeForLabel = j;
                            }
                        }
                    }
                    
                }
                DataVertexMinti vetrexToDel = Search(list_Vert_I ,Convert.ToInt32(EdgeForLabel.Edges.Target.ID));
                vetrexToDel.dist = min_dist;
                list_Vert_I.Remove(Search(list_Vert_I, Convert.ToInt32(EdgeForLabel.Edges.Target.ID)));
                list_Vert_J.Add(vetrexToDel);
                EdgeForLabel.Label = true; 
            }
            Graph = new GraphExMinti(list_Edges, list_Vert_J);
            return Graph;
        }

        public int[,] Distanse_Matr()
        {
            int[,] rez = new int[MaxValue(Matrix), MaxValue(Matrix)];
            for(int i = 0; i < Matrix.Count; i++)
            {
                rez[Matrix[i][0], Matrix[i][1]] = Matrix[i][2];                
            }
            return rez;
        }
        public int MaxValue(List<int[]> arr)
        {
            int max = 1;
            for (int i = 0; i < arr.Count; i++)
                for (int j = 0; j < 2; j++)
                {
                    if (arr[i][j] > max)
                        max = arr[i][j];
                }
            return max;
        }
        public DataVertexMinti Search(List<DataVertexMinti> Vertex, int vert)
        {
            DataVertexMinti rezult = null;
            foreach (DataVertexMinti p in Vertex)
            {
                if (p.vert.ID == vert)
                    rezult = p;
            }
            return rezult;
        }

    }
}
