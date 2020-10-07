using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree 
{
    private int maxObjects = 1;
    private int maxLevels = 3;
    private int level;
    private List<GameObject> objects;
    private Rect bounds;
    private QuadTree[] nodes;


    public QuadTree(int inputLevel,Rect inputBounds)
    {
        level = inputLevel;
        objects = new List<GameObject>();
        bounds = inputBounds;
        nodes = new QuadTree[4];
    }

    public void Clear()
    {
        objects.Clear();

        for(int i = 0; i < nodes.Length; ++i)
        {
            if (nodes[i] != null)
            {
                nodes[i].Clear();
                nodes[i] = null;
            }
        }
    }

    private void Split()
    {
        int w = (int)(bounds.width / 2);
        int h = (int)(bounds.height / 2);
        int x = (int)bounds.x;
        int y = (int)bounds.y;

        nodes[0] = new QuadTree(level + 1, new Rect(x + w, y, w, h));
        nodes[1] = new QuadTree(level + 1, new Rect(x , y, w, h));
        nodes[2] = new QuadTree(level + 1, new Rect(x, y + h, w, h));
        nodes[3] = new QuadTree(level + 1, new Rect(x + w, y + h, w, h));
    }

    private List<int> GetIndexes(Rect inputRect)
    {
        List<int> indexes = new List<int>();
        double verticalMid = bounds.x + bounds.width / 2.0d;
        double horizontalMid = bounds.y + bounds.height / 2.0d;

        bool topQuad = inputRect.y >= horizontalMid;
        bool bottomQuad = (inputRect.y - inputRect.height) <= horizontalMid;
        bool topAndBottomQuad = inputRect.y + inputRect.height + 1 >= horizontalMid && inputRect.y + 1 <= horizontalMid;

        if (topAndBottomQuad)
        {
            topQuad = false;
            bottomQuad = false;
        }

        if (inputRect.x + inputRect.width + 1 >= verticalMid && inputRect.x - 1 <= verticalMid)
        {
            if (topQuad)
            {
                indexes.Add(2);
                indexes.Add(3);
            }
            else if (bottomQuad)
            {
                indexes.Add(0);
                indexes.Add(1);
            }
            else if (topAndBottomQuad)
            {
                indexes.Add(0);
                indexes.Add(1);
                indexes.Add(2);
                indexes.Add(3);
            }
        }

        else if (inputRect.x + 1 >= verticalMid)
        {
            if (topQuad)
            {
                indexes.Add(3);
            }
            else if (bottomQuad)
            {
                indexes.Add(0);
            }
            else if (topAndBottomQuad)
            {
                indexes.Add(3);
                indexes.Add(0);
            }
        }
        else if (inputRect.x - inputRect.width <= verticalMid)
        {
            if (topQuad)
            {
                indexes.Add(2);
            }
            else if (bottomQuad)
            {
                indexes.Add(1);
            }
            else if (topAndBottomQuad)
            {
                indexes.Add(2);
                indexes.Add(1);
            }
        }
        else
        {
            indexes.Add(-1);
        }

        return indexes;
    }

    public void Insert(GameObject obj)
    {
        Rect inputRect = obj.GetComponent<RectTransform>().rect;
        if (nodes[0] != null)
        {
            List<int> indexes = GetIndexes(inputRect);
            for (int ii = 0; ii < indexes.Count; ii++)
            {
                int index = indexes[ii];
                if (index != -1)
                {
                    nodes[index].Insert(obj);
                    return;
                }
            }
        }
        objects.Add(obj);

        if (objects.Count > maxObjects && level < maxLevels)
        {
            if (nodes[0] == null)
            {
                Split();
            }

            int i = 0;
            while (i < objects.Count)
            {
                GameObject sqaureOne = objects[i];
                Rect oRect = sqaureOne.GetComponent<RectTransform>().rect;
                List<int> indexes = GetIndexes(oRect);
                for (int ii = 0; ii < indexes.Count; ii++)
                {
                    int index = indexes[ii];
                    if (index != -1)
                    {
                        nodes[index].Insert(sqaureOne);
                        objects.Remove(sqaureOne);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }
    }

    private List<GameObject> Retrieve(List<GameObject> objs,Rect inputRect)
    {
        List<int> idx = GetIndexes(inputRect);
        for (int ii = 0; ii < idx.Count; ii++)
        {
            int index = idx[ii];
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].Retrieve(objs, inputRect);
            }

            objs.AddRange(objects);
        }

        return objs;
    }
}
