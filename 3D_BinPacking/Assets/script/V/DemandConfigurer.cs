using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class DemandConfigurer : MonoBehaviour
{
    List<BoxContainer> boxes = new List<BoxContainer>();
    public BinPackingSolver solver;

    public DemandViewContainer[] dvcs = new DemandViewContainer[5];
    public DemandViewContainer initBinShow;
    BoxContainer initBin;
    public Scrollbar scrollbar;

    public TMP_InputField name;
    public TMP_InputField x;
    public TMP_InputField y;
    public TMP_InputField z;
    public TMP_InputField quantity;

    public TMP_InputField pathaddress;

    public bool isInitBin;
    public int indexmain;

    private void Start()
    {
        initBin = new BoxContainer("..", -1, -1, -1, 0);
    }
    private void Update()
    {
        if(boxes.Count - 4 > 0)
        {
            scrollbar.numberOfSteps = boxes.Count - 4 + 1;
        }
        else
        {
            scrollbar.numberOfSteps = 0;
        }
    }

    public void Loadneeds()
    {
        solver.setFilePath(pathaddress.text);
        solver.loadP();
        boxes = new List<BoxContainer>();

        int[] arr = solver.initBin.threeDimension;
        int x = arr[0]; int y = arr[1]; int z = arr[2];
        initBin = new BoxContainer("container", x, y, z, 1);

        string xyz = initBin.X.ToString() + "," + initBin.Y.ToString() + "," + initBin.Z.ToString();
        string _quan = "1";
        initBinShow.setDescription("container", xyz, _quan);

        for (int i = 0; i < solver.length.Count; i++)
        {

            BoxContainer box = new BoxContainer(solver.Names[i], solver.length[i], solver.width[i], solver.height[i], solver.quantity[i]);
            boxes.Add(box);
        }

        updateScrollBar();
    }

    public void LoadContainer(int i)
    {
        if(i == -10)
        {
            isInitBin = true;
        }
        else
        {
            isInitBin = false;
        }


        if (isInitBin)
        {
            BoxContainer container = initBin;
            name.text = container.Name;
            x.text = container.X.ToString();
            y.text = container.Y.ToString();
            z.text = container.Z.ToString();
            quantity.text = container.Quantity.ToString();
        }
        else
        {
            int scrollbarIndex = (int)scrollbar.value;
            indexmain = scrollbarIndex + i;
            if (indexmain < boxes.Count)
            {
                BoxContainer container = boxes[scrollbarIndex + i];
                name.text = container.Name;
                x.text = container.X.ToString();
                y.text = container.Y.ToString();
                z.text = container.Z.ToString();
                quantity.text = container.Quantity.ToString();
            }
            else
            {
                BoxContainer container = new BoxContainer("..", -1, -1, -1, 0);
                name.text = container.Name;
                x.text = container.X.ToString();
                y.text = container.Y.ToString();
                z.text = container.Z.ToString();
                quantity.text = container.Quantity.ToString();
            }
        }
        
    }

    public void change(bool isNew)
    { 
        if((indexmain < 0 && !isInitBin) || (indexmain >= boxes.Count && !isNew)) 
        {

            return; 
        }

        BoxContainer container = new BoxContainer("..", -1, -1, -1, 0);

        if(!isNew && !isInitBin)
        {
            container = boxes[indexmain];
        }

        if (isInitBin)
        {
            container = initBin;
        }


        container.Name = name.text;
        int _x = 0;
        int _y = 0;
        int _z = 0;
        int _q = 0;
        if(int.TryParse(x.text, out _x) && int.TryParse(y.text, out _y) && int.TryParse(z.text, out _z))
        {
            container.X = _x;
            container.Y = _y;
            container.Z = _z;
            if(int.TryParse(quantity.text, out _q))
            {
                container.Quantity = _q;
            }
            if (isNew && !isInitBin)
            {
                boxes.Add(container);
            }

            if (isInitBin)
            {
                initBinShow.name.text = "container";
                container.Quantity = 1;
                solver.initBin.threeDimension = new int[3] { _x, _y, _z };
                solver.initBin.edgeLength = new int[3] { _x, _y, _z };
                string xyz = container.X.ToString() + "," + container.Y.ToString() + "," + container.Z.ToString();
                string _quan = "1";
                initBinShow.setDescription(name.text, xyz, _quan);
            }

            updateScrollBar();
            return;
        }
        else
        {
            Debug.Log("error");
        }

        
    }

    public void updateScrollBar()
    {

        int scrollbarIndex = (int)scrollbar.value;


        for (int i = 0; i < 4; i++)
        {
            if(boxes.Count <= scrollbarIndex + i)
            {
                continue;
            }
            BoxContainer boxContainer = boxes[scrollbarIndex + i];
            string xyz = boxContainer.X.ToString() + "," + boxContainer.Y.ToString() + "," + boxContainer.Z.ToString();
            string q = boxContainer.Quantity.ToString();

            dvcs[i].setDescription(boxes[scrollbarIndex + i].Name, xyz, q);
        }


        solver.boxContainers = boxes;
    }
    
}

public class BoxContainer
{
    public string Name;
    public int X;
    public int Y;
    public int Z;
    public int Quantity;

    public BoxContainer(string _name, int x, int y, int z, int quantity)
    {
        Name = _name;
        X = x;
        Y = y;
        Z = z;
        Quantity = quantity;
    }
}
