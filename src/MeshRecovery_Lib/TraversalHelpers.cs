using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshRecovery_Lib
{
    namespace TraversalHelpers
    {
        public interface IContainer
        {
            void Add(int element);
            int Get();
            void Remove();
            int Count();
            void Clear();
        }

        public class DFSContainer : IContainer
        {
            private Stack<int> stack;

            public DFSContainer(int count)
            {
                stack = new Stack<int>(count);
            }

            public void Add(int element)
            {
                stack.Push(element);
            }

            public int Get()
            {
                return stack.Peek();
            }

            public void Remove()
            {
                stack.Pop();
            }
            public int Count()
            {
                return stack.Count();
            }

            public void Clear()
            {
                stack.Clear();
            }
        }

        public class BFSContainer : IContainer
        {
            private Queue<int> queue;

            public BFSContainer(int count)
            {
                queue = new Queue<int>(count);
            }

            public void Add(int element)
            {
                queue.Enqueue(element);
            }

            public int Get()
            {
                return queue.Peek();
            }

            public void Remove()
            {
                queue.Dequeue();
            }

            public int Count()
            {
                return queue.Count();
            }

            public void Clear()
            {
                queue.Clear();
            }
        }
    }
}
