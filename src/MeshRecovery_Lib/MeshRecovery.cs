﻿using System;
using System.Collections.Generic;
using System.Linq;


namespace MeshRecovery_Lib
{
    public static class MeshRecovery
    {
        /// <summary>
        /// Method checks that input graph corresponds to a regular grid
        /// </summary>
        /// <param name="xadj">Index array for graph</param>
        /// <param name="size">Size of index array</param>
        /// <param name="adjncy">Adjacency list</param>
        /// <param name="meshDimension">Dimension of a regular grid that corresponds to the input graph</param>
        /// <returns>true if graph corresponds to a regular grid, otherwise false</returns>
        public static bool Validate(long[] xadj, int size, int[] adjncy, out int meshDimension)
        {
            // TODO: Implement for 2,3 dimensions
            meshDimension = 0;

            Graph graph = new Graph(xadj, adjncy);

            // fast check of valid 
            if (graph.GetVerticesCount() != size - 1)
            {
                return false;
            }

            meshDimension = GetDimension(graph);

            if (meshDimension == 0)
                return false;

            return true;
        }
        /// <summary>
        /// Method retuns the dimension of the graph and checks if graph is conected
        /// </summary>
        /// <param name="graph">It`s a graph</param>
        /// <returns>
        /// Return code:
        /// 0 - error
        /// 1 - ono dimension
        /// 2 - two dimension
        /// 3 - three dimension 
        /// </returns>
        private static int GetDimension(Graph graph)
        {
            int V = graph.GetVerticesCount();
            int E = graph.GetEdgeCount();
            long max_adj = 0;
            long min_adj = 6;

            Traversal t = new Traversal(graph);

            List<long> vertices = new List<long>();
            int min_adj_count = 0;
            t.NewVertex += (sender, e) =>
            {
                if (max_adj < graph.GetAdjVerticesCount(e))
                {
                    max_adj = graph.GetAdjVerticesCount(e);
                }
                if (min_adj > graph.GetAdjVerticesCount(e))
                {
                    min_adj_count = 0;
                    min_adj = graph.GetAdjVerticesCount(e);
                }
                if (min_adj == graph.GetAdjVerticesCount(e))
                    min_adj_count++;
                vertices.Add(e);
            };
            t.Run();

            if (vertices.Count() != graph.GetVerticesCount())
                return 0;

            if (max_adj <= 2 && min_adj_count==2) // should be one dimension graph ( chain )
            {

                return 1;
            }

            if (max_adj <= 4)
            {
                return 2;
            }

            if (max_adj <= 6)
            {
                return 3;
            }

            return 0;
        }
        /// <summary>
        /// Method restores geometry information for each graph node
        /// </summary>
        /// <param name="xadj">Index array for graph</param>
        /// <param name="size">Size of index array</param>
        /// <param name="adjncy">Adjacency list</param>
        /// <param name="graphNumeration">Restored numeration for the input graph</param>
        /// <returns>
        /// Return code:
        /// 0 - success
        /// -1 - error
        /// </returns>
        public static int Numerate(long[] xadj, int size, int[] adjncy, out int[][] graphNumeration)
        {
            Graph graph = new Graph(xadj, adjncy);
            graphNumeration = null;
            int V = graph.GetVerticesCount();
            int error = 0;
            switch (GetDimension(graph))
            {
                case 1:
                    error = new Numerator1D().Run(graph, out graphNumeration);
                    if (error != 0)
                    {
                        NumerationHelper.Clear(graphNumeration);
                        error = new Numerator2D().Run(graph, out graphNumeration);
                    }
                    if (error != 0)
                    {
                        NumerationHelper.Clear(graphNumeration);
                        error = new Numerator3D().Run(graph, out graphNumeration);
                    }
                    break;
                case 2:
                    error = new Numerator2D().Run(graph, out graphNumeration);
                    if (error != 0)
                    {
                        NumerationHelper.Clear(graphNumeration);
                        error = new Numerator3D().Run(graph, out graphNumeration);
                    }
                    break;
                case 3:
                    error = new Numerator3D().Run(graph, out graphNumeration);
                    break;
                default: return -1;
            }
            return error;
        }

        /// <summary>
        /// Method validate structure of one dimension graph
        /// </summary>
        /// <param name="graph">It`s a graph</param>
        /// <returns>
        /// Return code:
        /// true - correct structure
        /// false - not correct structure
        /// </returns>
        private static bool OneDimensionValidate(Graph graph)
        {
            //I was sure that one dimension graphs are good boys, but then..
            // 0--0
            //  \/   0--0
            //  0
            // 

            Traversal traversal = new Traversal(graph);

            int V = graph.GetVerticesCount();
            bool[] union = new bool[V];

            for (int i = 0; i < V; ++i)
            {
                union[i] = false;
            }

            traversal.NewVertex += (sender, e) =>
            {
                union[e] = true;
            };
            traversal.Run();

            for (int i = 0; i < V; ++i)
            {
                if (union[i] == false) return false;
            }
            return true;
        }

        /// <summary>
        /// Method restores geometry information for graph in 2 dimension
        /// </summary>
        /// <param name="graph">It`s a graph, Mr. Graph</param>
        /// <param name="graphNumeration">Restored numeration for the input graph</param>
        /// <returns>
        /// Return code:
        /// 0 - success
        /// -1 - error
        private static int TwoDimensionNumerate(Graph graph, out int[][] graphNumeration)
        {
            int result = 0;
            Traversal traversal = new Traversal(graph);
            int V = graph.GetVerticesCount();
            int[][] res_graphNumeration = new int[V][];
            int[][] history_guess = new int[V][]; // which vertex and which index was increased and where
            int index_guess = 0;
            graphNumeration = res_graphNumeration;
            int start_vertex = -1;
            int another_start_vertex = -1;

            //find start vertex 
            traversal.NewVertex = (sender, e) =>
            {
                if ((graph.GetAdjVerticesCount(e) == 3))
                    another_start_vertex = e;
                if ((graph.GetAdjVerticesCount(e) == 4))
                    start_vertex = e;
            };
            traversal.Run();
            if (another_start_vertex == -1 && start_vertex == -1)
                return -1; // not two dimension graph
            else if (start_vertex == -1)
                start_vertex = another_start_vertex;
            bool fixed_first = false; //
            //set start information
            res_graphNumeration[start_vertex] = new int[] { 0, 0 };
            int[] buf;
            long count_around_start = graph.GetAdjVertices(start_vertex, out buf);
            int x_start = 1, y_start = 0;
            for (int i = 0; i < count_around_start; ++i)
            {
                res_graphNumeration[buf[i]] = new int[] { x_start, y_start }; // NB: make it easy
                y_start = x_start;
                if (--x_start == -2) x_start = 0;
            }
            traversal.NewVertex = (sender, e) =>
            {
                if (res_graphNumeration[e] == null)
                {
                    int[] buff;
                    long adj_count = graph.GetAdjVertices(e, out buff);
                    //try to determine with al least 2 numerated vertex
                    int v1 = -1, v2 = -1;
                    for (int i = 0; i < adj_count; ++i)
                    {
                        if (res_graphNumeration[buff[i]] != null)
                        {
                            v2 = v1;
                            v1 = buff[i];
                        }
                    }
                    if (v1 == -1 || v2 == -1)
                    {//if not
                     //check that there is vertex with all pohers numerated neighbours 
                        if (v1 != -1)
                        {
                            adj_count = graph.GetAdjVertices(v1, out buff);
                            int numerated = 0;
                            int x = 0, y = 0;
                            bool direction = true; // let change x by default
                            for (int i = 0; i < adj_count; ++i)
                            {
                                if (res_graphNumeration[buff[i]] != null)
                                {
                                    x += res_graphNumeration[buff[i]][0];
                                    if (y / (numerated + 1) == res_graphNumeration[buff[i]][1]) direction = false;
                                    y += res_graphNumeration[buff[i]][1];
                                    ++numerated;
                                }
                            }
                            if (adj_count - numerated == 1)
                            {
                                if (adj_count == 4)
                                {
                                    res_graphNumeration[e] = new int[] { res_graphNumeration[v1][0] * 4 - x, res_graphNumeration[v1][1] * 4 - y };
                                }
                                else
                                {
                                    int x_direction = direction ? ((res_graphNumeration[v1][0] * 4 - 1 > 0) ? 1 : -1) : 0;
                                    int y_direction = !direction ? ((res_graphNumeration[v1][1] * 4 - 1 > 0) ? 1 : -1) : 0;
                                    res_graphNumeration[e] = new int[] { res_graphNumeration[v1][0] + x_direction, res_graphNumeration[v1][1] + y_direction };
                                    history_guess[index_guess++] = new int[] { e, x_direction, y_direction };
                                }

                            }
                            else if (adj_count - numerated == 3)
                            {
                                int x_direction = (res_graphNumeration[v1][0] * 4 - 1 > 0) ? 1 : -1;
                                int y_direction = (res_graphNumeration[v1][1] * 4 - 1 > 0) ? 1 : -1;
                                res_graphNumeration[e] = new int[] { res_graphNumeration[v1][0] + x_direction, res_graphNumeration[v1][1] };
                                history_guess[index_guess++] = new int[] { e, x_direction, y_direction };
                            }
                            else if (adj_count - numerated == 2)
                            {
                                int x_direction = direction ? 1 * ((res_graphNumeration[v1][0] * 4 - 1 > 0) ? 1 : -1) : 0;
                                int y_direction = !direction ? 1 * ((res_graphNumeration[v1][1] * 4 - 1 > 0) ? 1 : -1) : 0;
                                res_graphNumeration[e] = new int[] { res_graphNumeration[v1][0] + x_direction, res_graphNumeration[v1][1] + y_direction };
                                history_guess[index_guess++] = new int[] { e, x_direction, y_direction };
                            }
                            //here we give up 
                        }
                        else
                            result = -1;

                    }
                    else
                    {
                        //special case of start vertex
                        //swap v1 and reversed from start vertex neighbours
                        if ((res_graphNumeration[v1][0] == res_graphNumeration[v2][0]) || (res_graphNumeration[v1][1] == res_graphNumeration[v2][1]))
                        {
                            if (!fixed_first)
                            {
                                for (int i = 0; i < count_around_start; ++i)
                                {
                                    if ((res_graphNumeration[buf[i]][0] == res_graphNumeration[v1][1]) && (res_graphNumeration[buf[i]][1] == res_graphNumeration[v1][0]))
                                    {
                                        int[] temp = res_graphNumeration[v1];
                                        res_graphNumeration[v1] = res_graphNumeration[buf[i]];
                                        res_graphNumeration[buf[i]] = temp;
                                        fixed_first = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (index_guess > 0)
                                    res_graphNumeration[history_guess[index_guess - 1][0]] = new int[] { res_graphNumeration[history_guess[index_guess - 1][0]][0] - history_guess[index_guess - 1][1], res_graphNumeration[history_guess[index_guess - 1][0]][1] + history_guess[index_guess - 1][2] };
                                --index_guess;
                            }
                        }

                        int[] bufv1;
                        int[] bufv2;
                        long couunt_bufv1 = graph.GetAdjVertices(v1, out bufv1);
                        long couunt_bufv2 = graph.GetAdjVertices(v2, out bufv2);
                        int opposite_vertex = -1;
                        for (int i = 0; i < couunt_bufv1; ++i)
                            for (int j = 0; j < couunt_bufv2; ++j)
                                if (bufv1[i] == bufv2[j] && bufv1[i] != e)
                                    opposite_vertex = bufv1[i];
                        if ((opposite_vertex != -1) && (res_graphNumeration[opposite_vertex] != null))
                        {
                            int x = res_graphNumeration[v1][0] + res_graphNumeration[v2][0] - res_graphNumeration[opposite_vertex][0];
                            int y = res_graphNumeration[v1][1] + res_graphNumeration[v2][1] - res_graphNumeration[opposite_vertex][1];
                            if ((x == res_graphNumeration[opposite_vertex][0]) && y == res_graphNumeration[opposite_vertex][1])
                                result = -1;
                            res_graphNumeration[e] = new int[] { x, y };
                        }
                        else
                        {
                            result = -1;
                        }
                    }
                    //here we give up
                    //but we will do it again!!
                }
            };

            traversal.Run(start_vertex);

            for (int i = 0; i < index_guess; ++i)
            {
                count_around_start = graph.GetAdjVertices(history_guess[i][0], out buf);
                for (int j = 0; j < count_around_start; ++j)
                {
                    if (NumerationHelper.CompareVertex(res_graphNumeration[history_guess[i][0]], res_graphNumeration[buf[j]], 2) < 0)
                    {
                        //try to repair vertex
                        res_graphNumeration[history_guess[index_guess - 1][0]] = new int[] { res_graphNumeration[history_guess[index_guess - 1][0]][0] - history_guess[index_guess - 1][1], res_graphNumeration[history_guess[index_guess - 1][0]][1] + history_guess[index_guess - 1][2] };
                    }
                }
            }
            graphNumeration = res_graphNumeration;
            return result;
        }
    }
}
