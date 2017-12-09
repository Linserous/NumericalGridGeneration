using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MeshRecovery_Console
{
    class NumerationWrapper
    {
        public List<Vertex> VertexCoordinates { get; set; }

        public class Vertex
        {
            public int X { get; set; }

            public Vertex(int value)
            {
                X = value;
            }
        }

        class Vertex2D : Vertex
        {
            public int Y { get; set; }

            public Vertex2D(int[] numeration) : base(numeration[0])
            {
                Y = numeration[1];
            }
        }

        class Vertex3D : Vertex2D
        {
            public int Z { get; set; }

            public Vertex3D(int[] numeration) : base(numeration)
            {
                Z = numeration[2];
            }
        }

        private Vertex CreateVertex(int[] coordinates)
        {
            switch (coordinates.Length)
            {
                case 1:
                    return new Vertex(coordinates[0]);
                case 2:
                    return new Vertex2D(coordinates);
                case 3:
                    return new Vertex3D(coordinates);
                default:
                    throw new Exception($"Coordinates dimension is not supported: {coordinates.Length}");
            }
        }

        public NumerationWrapper(int[][] numeration)
        {
            VertexCoordinates = new List<Vertex>();

            for (int i = 0; i < numeration.Length; i++)
            {
                VertexCoordinates.Add(CreateVertex(numeration[i]));
            }
        }
    }

    static class JsonSerializer
    {
        public static string SerializeNumeration(int[][] numeration)
        {
            string result = null;
            try
            {
                var wrapper = new NumerationWrapper(numeration);
                var data = wrapper.VertexCoordinates
                    .Select((value, index) => new { value, index })
                    .ToDictionary(w => w.index, w => w.value);

                result = JsonConvert.SerializeObject(data);
            }
            catch (Exception e)
            {
                throw new Exception($"Error during serialization: {e.Message}");
            }
            return result;
        }
    }
}
