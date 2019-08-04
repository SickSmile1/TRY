using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using TRY.Kampfmodus.Collision;
using TRY.Kampfmodus.Structure;

namespace TRY.Kampfmodus.Pathfinding
{
    class Pathfinding
    {
        private readonly List<GridNode> mGridNodes;
        private readonly TiledMapObject[] mCollisionObjects;
        private readonly List<Tuple<Vector2, int>> mRelevantPoints;
        private readonly QField<IStaticCollider> mVisibilityObjects;


        private object mLocker = false;
        public Pathfinding(TiledMapObject[] collisionObjects, Point characterSize, Point mapSize, QField<IStaticCollider> visibilityObjects)
        {
            mVisibilityObjects = visibilityObjects;

            //Save the collisionObjects for later Visibility checking
            mCollisionObjects = collisionObjects;

            //Calculate all points on collision objects which are not overlapped by another collision object
            //For every point an orietation is given by Item2 in a Tuple
            var relevantPoints = FindRelevantPoints(collisionObjects, mapSize);

            //Calculate all possible GridNodes without optimization by adding half the charactersize corresponding to the orientation
            var allPossibleGridNodes = ConstructGridNodes(relevantPoints, characterSize);

            //Combine GridNodes which are closer than minDistance
            var farGridNodes = CombineNearGridNodes(allPossibleGridNodes, 31);

            //Find the GridNodes Neighbors. Neighbors are GridNodes which are visible from the GridNode
            //The Visibility Graph is now constructed.
            mGridNodes = FindNeighbors(farGridNodes);

            //For Drawing
            mRelevantPoints = relevantPoints;
        }

        public bool IsVisible(Vector2 start, Vector2 destination)
        {
            var relevantObjects = mVisibilityObjects.GetAllElementsNear(start, destination);

            var distance = destination - start;

            // Find maximum and minimum elements
            float xmax, xmin, ymax, ymin;

            if (destination.X > start.X){ xmax = destination.X; xmin = start.X;}
            else { xmax = start.X; xmin = destination.X; }

            if (destination.Y > start.Y){ymax = destination.Y;ymin = start.Y;}
            else { ymax = start.Y; ymin = destination.Y; }

            // Construct the line
            float k = (destination.Y - start.Y) / (destination.X - start.X);
            float b = (destination.Y - k * destination.X);

            // In case there is no X difference
            if (distance.X.Equals(0f))
            {
                foreach (var collider in relevantObjects)
                {
                    var collisionRectangle = collider.ObjectArea;
                    var upperBound = collisionRectangle.Y;
                    var lowerBound = collisionRectangle.Y + collisionRectangle.Height;
                    var leftBound = collisionRectangle.X;
                    var rightBound = collisionRectangle.X + collisionRectangle.Width;

                    if (destination.X >= leftBound 
                        && destination.X <= rightBound 
                        && ymax > upperBound 
                        && ymin < lowerBound)
                    {
                        return false;
                    }
                }
                return true;
            }

            // In case there is no Y difference
            if (distance.Y.Equals(0f))
            {
                foreach (var collider in relevantObjects)
                {
                    var collisionRectangle = collider.ObjectArea;
                    var upperBound = collisionRectangle.Y;
                    var lowerBound = collisionRectangle.Y + collisionRectangle.Height;
                    var leftBound = collisionRectangle.X;
                    var rightBound = collisionRectangle.X + collisionRectangle.Width;

                    if (destination.Y <= lowerBound 
                        && destination.Y >= upperBound
                        && xmax > leftBound
                        && xmin < rightBound)
                    {
                        return false;
                    }
                }
                return true;
            }

            foreach (var collider in relevantObjects)
            {
                var collisionRectangle = collider.ObjectArea;
                //Construct the four bounds of the Rectangle
                var upperBound = collisionRectangle.Y;
                var lowerBound = collisionRectangle.Y + collisionRectangle.Height;
                var leftBound = collisionRectangle.X;
                var rightBound = collisionRectangle.X + collisionRectangle.Width;

                //Schnittpunkte der Geraden mit der oberen und unteren Geraden des Rechtecks
                var xo = (upperBound - b) / k;
                var xu = (lowerBound - b) / k;

                //Schnittpunkt der Geraden mit der linken und rechten Geraden des Rechtecks
                var yl = k * leftBound + b;
                var yr = k * rightBound + b;

                if (   (xo <= xmax && xo >= xmin) && (xo <= rightBound && xo >= leftBound) 
                    || (xu <= xmax && xu >= xmin) && (xu <= rightBound && xu >= leftBound)
                    || (yl <= ymax && yl >= ymin) && (yl <= lowerBound && yl >= upperBound)
                    || (yr <= ymax && yr >= ymin) && (yr <= lowerBound && yr >= upperBound))
                {
                    return false;
                }
            }
            return true;
        }

        public List<Vector2> FindWay(Vector2 a, Vector2 b)
        {
            //For Multithreading.
            lock (mLocker)
            {
                //The result is a List of Points the Character has to go to
                var result = new List<Vector2>();

                //If there are no Walls between the two points just go there
                if (IsVisible(a, b))
                {
                    result.Add(b);
                    return result;
                }

                //Include start and destination into graph
                GridNode start = new GridNode(a);
                GridNode stop = new GridNode(b);

                IncludeNodeIntoGraph(start, mGridNodes);
                IncludeNodeIntoGraph(stop, mGridNodes);

                //Initialise the Priority Queue with start's neighbors
                PriorityQueue2 pq = new PriorityQueue2();
                foreach (var node in start.Neighbours.Keys)
                {
                    pq.Insert(node, node.Neighbours[start], start);
                }

                //Initialise the visited Nodes with start
                Dictionary<GridNode, PriorityQueue2.Element> visitedNodes =
                    new Dictionary<GridNode, PriorityQueue2.Element>();
                visitedNodes.Add(start, new PriorityQueue2.Element(start, 0, start));

                PriorityQueue2.Element e;
                for (e = pq.Next(); e != null && e.mNode != stop; e = pq.Next())
                {
                    visitedNodes.Add(e.mNode, e);
                    foreach (var neighbour in e.mNode.Neighbours.Keys)
                    {
                        if (visitedNodes.ContainsKey(neighbour))
                        {
                            continue;
                        }

                        pq.Insert(neighbour, neighbour.Neighbours[e.mNode] + e.mDistance, e.mNode);
                    }

                }

                if (e != null)
                {
                    //Weg rekonstruieren
                    while (e.mNode != start)
                    {
                        result.Add(e.mNode.Position);

                        e = visitedNodes[e.mPredecessor];
                    }

                    result.Reverse();
                }

                //Graphen aufräumen
                RemoveNodeFromGraph(start);
                RemoveNodeFromGraph(stop);

                return result;
            }
        }

        private List<GridNode> FindNeighbors(List<GridNode> gridNodes)
        {
            for (var i = 0; i< gridNodes.Count; i++)
            {
                IncludeNodeIntoGraph(gridNodes[i], gridNodes.GetRange(i+1,gridNodes.Count-i-1));
            }
            
            //Eliminate all GridNodes without Neighbors
            for (int i = 0; i < gridNodes.Count; i++)
            {
                if (gridNodes[i].Neighbours.Keys.Count == 0)
                {
                    gridNodes.RemoveAt(i);
                    i--;
                }
            }
            return gridNodes;
        }

        private void IncludeNodeIntoGraph(GridNode gridNode, List<GridNode> graph)
        {
            foreach (var node in graph)
            {
                if (IsVisible(gridNode.Position, node.Position))
                {
                    node.Neighbours.Add(gridNode, (gridNode.Position - node.Position).Length());
                    gridNode.Neighbours.Add(node, (gridNode.Position - node.Position).Length());
                }
            }
        }

        private void RemoveNodeFromGraph(GridNode gridNode)
        {
            foreach (var node in gridNode.Neighbours.Keys)
            {
                node.Neighbours.Remove(gridNode);
            }
        }

        private List<GridNode> CombineNearGridNodes(List<GridNode> gridNodes, int minDistance)
        {
            var gridNodesNew = new List<GridNode>(gridNodes);
            int numNodes = gridNodesNew.Count;
            for (var i = 0; i < numNodes; i++)
            {
                for (var j = i + 1; j < numNodes; j++)
                {
                    var distance = gridNodesNew[j].Position - gridNodesNew[i].Position;
                    if (distance.Length() < minDistance)
                    {
                        gridNodesNew[i].Position += distance / 2;
                        gridNodesNew.RemoveAt(j);
                        numNodes--;
                        j--;
                    }
                }
            }
            return gridNodesNew;
        }

        private List<GridNode> ConstructGridNodes(List<Tuple<Vector2, int>> relevantPoints, Point characterSize)
        {
            var gridNodes = new List<GridNode>();
            for (var i = 0; i < relevantPoints.Count; i++)
            {
                //Add the Character size to the point corresponding to the right orientation
                gridNodes.Add(
                    new GridNode(
                        new Vector2(
                            relevantPoints[i].Item1.X
                            + (-1 + 2 * (relevantPoints[i].Item2 % 2)) * characterSize.X,
                            relevantPoints[i].Item1.Y
                            + (-1 + 2 * (relevantPoints[i].Item2 / 2)) * characterSize.Y)));
            }
            return gridNodes;
        }

        private List<Tuple<Vector2, int>> FindRelevantPoints(TiledMapObject[] collisionObjects, Point mapSize)
        {
            List<Tuple<Vector2,int>> relevantPoints = new List<Tuple<Vector2, int>>();
            foreach (var collisionObject in mCollisionObjects)
            {
                var pointPosition = 0;
                foreach (var point in CollisionPoints(collisionObject))
                {
                    var relevant = 
                        collisionObjects
                            .Where(collisionObject2 => collisionObject2 != collisionObject)
                            .All(collisionObject2 => !IsVectorInCollisionObject(collisionObject2, point));

                    if (relevant && point.X > 0 && point.Y > 0 
                        && point.Y < mapSize.Y 
                        && point.X < mapSize.X
                        )
                    {
                        relevantPoints.Add(new Tuple<Vector2, int>(point, pointPosition));
                    }
                    pointPosition++;
                }
            }

            return relevantPoints;
        }

        private bool IsVectorInCollisionObject(TiledMapObject collisionObject, Vector2 vector)
        {
            if (collisionObject.Position.X <= vector.X
                && collisionObject.Position.X + collisionObject.Size.Width >= vector.X
                && collisionObject.Position.Y <= vector.Y
                && collisionObject.Position.Y + collisionObject.Size.Height >= vector.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D nodes, Texture2D relevantPoints)
        {
            foreach (var relevantPoint in mRelevantPoints)
            {
                spriteBatch.Draw(relevantPoints, new Rectangle((int)relevantPoint.Item1.X-8, (int)relevantPoint.Item1.Y - 8, 16, 16), Color.Green);
            }

            lock (mLocker)
            {
                foreach (var node in mGridNodes)
                {
                    spriteBatch.Draw(nodes, new Rectangle((int)node.Position.X-8, (int)node.Position.Y-8, 16, 16), Color.Red);


                    foreach (var nodeNeighbour in node.Neighbours.Keys)
                    {
                        Vector2 distance = node.Position - nodeNeighbour.Position;
                        float angle = (float) Math.Atan2(distance.Y, distance.X);
                        spriteBatch.Draw(nodes,
                            new Rectangle(
                                (int)nodeNeighbour.Position.X,
                                (int)nodeNeighbour.Position.Y,
                                (int)distance.Length(),
                                2),
                            null,
                            Color.Green,
                            angle,
                            new Vector2(0,0),
                            SpriteEffects.None,
                            0);
                    }
                }
            }
        }

        private List<Vector2> CollisionPoints(TiledMapObject collisionObject)
        {
            List<Vector2> result = new List<Vector2>();
            result.Add(new Vector2(
                collisionObject.Position.X, 
                collisionObject.Position.Y));
            result.Add(new Vector2(
                collisionObject.Position.X + collisionObject.Size.Width,
                collisionObject.Position.Y));
            result.Add(
                new Vector2(
                collisionObject.Position.X, 
                collisionObject.Position.Y + collisionObject.Size.Height));
            result.Add(new Vector2(
                collisionObject.Position.X + collisionObject.Size.Width,
                collisionObject.Position.Y + collisionObject.Size.Height));
            return result;
        }
    }
}
