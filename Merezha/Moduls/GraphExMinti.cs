using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merezha.Moduls
{
    class GraphExMinti
    {
        public List<DataEdgeMinti> Edges;
        public List<DataVertexMinti> Vertex;

        public GraphExMinti(List<DataEdgeMinti> e, List<DataVertexMinti> v)
        {
            Edges = e;
            Vertex = v;
        }
    }
}
