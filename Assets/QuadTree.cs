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
}
