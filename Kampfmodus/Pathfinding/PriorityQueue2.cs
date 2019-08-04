using System.Collections.Generic;

namespace TRY.Kampfmodus.Pathfinding
{
    class PriorityQueue2
    {
        public class Element
        {
            public GridNode mNode;
            public float mDistance;
            public GridNode mPredecessor;

            public Element(GridNode node, float distance, GridNode predecessor)
            {
                mNode = node;
                mDistance = distance;
                mPredecessor = predecessor;
            }
        }

        private Dictionary<GridNode, float> mDistances;
        private Dictionary<GridNode, GridNode> mPredecessor;
        private List<GridNode> mSortedList;

        public PriorityQueue2()
        {
            mDistances = new Dictionary<GridNode, float>();
            mPredecessor = new Dictionary<GridNode, GridNode>();
            mSortedList = new List<GridNode>();
        }

        public void Insert(GridNode newNode, float distance, GridNode predecessor)
        {
            if (mSortedList.Contains(newNode))
            {
                if (mDistances[newNode] > distance)
                {
                    mSortedList.Remove(newNode);
                    mDistances[newNode] = distance;
                    mPredecessor[newNode] = predecessor;
                    var position = 0;
                    for (; position < mSortedList.Count && mDistances[mSortedList[position]] < distance; position++)
                    {
                    }

                    mSortedList.Insert(position, newNode);
                }
            }
            else
            {
                mDistances.Add(newNode,distance);
                mPredecessor.Add(newNode,predecessor);

                var position = 0;
                for (; position < mSortedList.Count && mDistances[mSortedList[position]] < distance; position++)
                {
                }

                mSortedList.Insert(position, newNode);
            }
        }

        public Element Next()
        {
            if (mSortedList.Count == 0)
            {
                return null;
            }
            GridNode next = mSortedList[0];
            Element e = new Element(next,mDistances[next],mPredecessor[next]);
            mSortedList.Remove(next);
            mDistances.Remove(next);
            mPredecessor.Remove(next);
            return e;
        }
    }
}
