using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using Unity;
using UnityEngine;


public enum solvingState
{
    Null,
    Solving,
    BuildingAnswer,
    FindAnswer
}
public class BinPackingSolver: MonoBehaviour
{

    public List<string> solutionsStrings = new List<string>();
    string filePath = "D:\\cutBoxesTest\\testData.csv";
    string solutionPath = "D:\\cutBoxesTest\\anss.txt";
    bool useBranchAndBound = true;

    public bool onlyFindingSol;
    public solvingState solvingProcess;
    bool startmp;

    public GameObject BoxPrefab;
    public GameObject BinPrefab;
    public bool solutionValid = true;

    public List<BoxContainer> boxContainers;
    public List<Box> bestRe;

    public List<string> Names;
    public List<int> length;
    public List<int> width;
    public List<int> height;
    public List<int> quantity;

    public Box initBin;
    public bool isCompelete;
    List<Box> boxes;

    int bestCut;
    int bestWaste;

    public Transform boxesHolder;
    public Transform boxesRotater;
    [SerializeField] float showBackwardOffset;

    [SerializeField]BoxInScene[] boxesInScene;

    int index = 0;
    bool allStatic;

    public Thread thread;
    public bool isRandom;
    public int Size;
    public int RandomSeed;

    public void setFilePath(string path)
    {
        filePath = path;
    }

    public void setSolutionPath(string path)
    {
        solutionPath = path;
    }
    public void solveTestingFunction()
    {
        loadP();

        Solve();

        saveSol();
    }
    public void loadP()
    {

        if(filePath == null)
        {
            return;
        }

        Names = new List<string>();
        length = new List<int>();
        width = new List<int>();
        height = new List<int>();
        quantity = new List<int>();
        boxes = new List<Box>();

        bool startbox = false;

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {

                    if (!containNum(line))
                    {
                        continue;
                    }
                    string[] parts = line.Split(',');
                    if (!startbox)
                    {
                        int[] threeDimensionArray = new int[] { int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]) };
                        Array.Sort(threeDimensionArray);
                        Array.Reverse(threeDimensionArray);

                        initBin = new Box(threeDimensionArray, new int[] { 0, 0, 0 }, new int[] { int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]) }, parts[0]);
                        startbox = true;
                    }
                    else
                    {
                        Names.Add(parts[0]);
                        length.Add(int.Parse(parts[1]));
                        width.Add(int.Parse(parts[2]));
                        height.Add(int.Parse(parts[3]));
                        quantity.Add(int.Parse(parts[4]));
                    }


                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("file not found");
        }
    }

    public void saveSol()
    {
        try
        {

            if (!File.Exists(solutionPath))
            {
                File.Create(solutionPath);
            }

            StreamWriter solutionWriter = new StreamWriter(solutionPath);


            solutionWriter.WriteLine("cut: " + bestCut);

            solutionWriter.WriteLine("step");

            foreach (string solution in solutionsStrings)
            {
                solutionWriter.WriteLine(solution);
            }
            solutionWriter.Close();

        }
        catch (Exception ex)
        {
            //Console.WriteLine("write error");
        }
    }

    public void packLog()
    {
        StartCoroutine(packLogBg());
    }
    private IEnumerator packLogBg()
    {

        solvingProcess = solvingState.BuildingAnswer;

        solvingProcess = solvingState.FindAnswer;

        boxesInScene = boxesInScene.OrderBy(p => p.box.lbdCorner[1]).ThenBy(p => p.box.lbdCorner[2]).ThenBy(p => p.box.lbdCorner[0]).ToArray();

        solutionsStrings = new List<string>();
        int i = 0;
        while(i < boxesInScene.Length)
        {
            int[] td = boxesInScene[i].box.threeDimension;
            int[] pos = boxesInScene[i].box.lbdCorner;
            int[] length = boxesInScene[i].box.edgeLength;
            string step = "step" + i;
            string put = "put box " + boxesInScene[i].box.name + " -> " + " ( " + td[0] + " , " + td[1] + " , " + td[2] + ") " + "to";
            string rotation = "direction in: " + " ( " + length[0] + " , " + length[1] + " , " + length[2] + " ) ";
            string with = "position :" + "( " + pos[0] + " , " + pos[1] + " , " + pos[2] + " )";
            Debug.Log("??");
            solutionsStrings.Add(step + "\n" + put + "\n" + rotation + "\n" + with + "\n");
            i++;

            yield return null;
        }
        

    }

    public void Solve()
    {
        thread = new Thread(new ThreadStart(packingSolving));
        thread.Start();
    }

    public void packingSolving()
    {




        solvingProcess = solvingState.Solving;
        isCompelete = false;
        Debug.Log("solving problem");
        Names = new List<string>();
        length = new List<int>();
        width = new List<int>();
        height = new List<int>();
        quantity = new List<int>();
        boxes = new List<Box>();

        boxes.Add(initBin.Copy());

        bestRe = new List<Box>();


        if(solutionsStrings != null)
        {
            solutionsStrings.Clear();
        }
        else
        {
            solutionsStrings = new List<string>();
        }

        List<Box> needs = new List<Box>();

        if(boxContainers != null)
        {
            foreach (BoxContainer co in boxContainers)
            {
                Names.Add(co.Name);
                length.Add(co.X);
                width.Add(co.Y);
                height.Add(co.Z);
                quantity.Add(co.Quantity);
            }
        }
        

        //add require boxes to list
        for (int i = 0; i < length.Count; i++)
        {
            for (int j = 0; j < quantity[i]; j++)
            {
                int[] dimen = new int[] { length[i], width[i], height[i] };
                int[] edgeLength = new int[] { length[i], width[i], height[i] };
                Array.Sort(dimen);
                Array.Reverse(dimen);
                Box newBoxes = new Box(dimen, new int[] {0,0,0}, edgeLength, Names[i]);
                needs.Add(newBoxes);

            }
        }
        //sort dimesnsions to non_decreasing order
        needs = needs.OrderByDescending(z => z.threeDimension[0])
                     .ThenByDescending(z => z.threeDimension[1])
                     .ThenByDescending(z => z.threeDimension[2])
                     .ToList();

        if (isRandom)
        {
            needs = needs.OrderBy(a => UnityEngine.Random.Range(0, needs.Count)).ToList();  
        }

        //try solving the problem using First fit non_decreasing order
        int preCutBox = 0;
        List<Box> result = new List<Box>();
        List<Box> needsTemp = needs.ConvertAll(box => box.Copy());
        List<Box> boxesTemp = boxes.ConvertAll(box => box.Copy());
        List<solutionLog> bestSolution = new List<solutionLog>();


        int wastev = 0;
        List<Box> waste = new List<Box>();

        for (int index = 0; index < needsTemp.Count; index++)
        {
            preCutBox = Cut(needsTemp[index], preCutBox, result, boxesTemp, bestSolution, ref index, needsTemp, waste, ref wastev);
        }
        Debug.Log(preCutBox);

        //preCutBox = int.MaxValue;
        bestRe = result;
        //bestRe = new List<Box>();
        int bestCut = preCutBox;



        if (useBranchAndBound)
        {
            List<solutionLog> initSol = new List<solutionLog>();
            //use branch and bound to decide which box is choose to generate require box
            int cutBox = RecursiveCut(boxes, 0, new List<Box>(), ref bestCut, needs, 0, initSol, ref bestSolution, ref bestRe, new List<Box>(), ref waste, ref wastev, 0);
        }






        //Console.WriteLine("Best number of cuts: " + bestCut);
        Debug.Log("Best number of cuts: " + bestCut);
        int totalVolume = boxes.Sum(box => box.threeDimension[0] * box.threeDimension[1] * box.threeDimension[2]);

        //check if the solution is valid;
        for (int i = 0; i < bestSolution.Count; i++)
        {
            if (i > 0)
            {
                if (BoxListContainAlmostEqual(bestSolution[i - 1].boxes, bestSolution[i].from) < 0)
                {
                    solutionValid = false;
                }
            }
        }

        foreach (solutionLog sl in bestSolution)
        {
            Debug.Log(sl.ToString());
            solutionsStrings.Add(sl.ToString());
        }

        Debug.Log("try get answer");
        isCompelete = true;
    }

    public void showBoxesCoroutine()
    {
        StartCoroutine(showBoxes());
    }

    IEnumerator showBoxes()
    {
        transform.position = Vector3.zero;
        boxesHolder.localPosition = Vector3.zero;
        boxesRotater.rotation = Quaternion.identity;

        boxesInScene = new BoxInScene[bestRe.Count];
        while(boxesHolder.childCount != 0)
        {
            
            foreach(Transform t in boxesHolder)
            {
                DestroyImmediate(t.gameObject);
            }
        }
        BoxInScene tmp;
        PlaceBox(initBin.lbdCorner, new float[] {initBin.edgeLength[0] + 0.01f, initBin.edgeLength[1] + 0.01f, initBin.edgeLength[2] + 0.01f}, BinPrefab, initBin,out tmp);

        int i = 0;
        foreach(Box box in bestRe)
        {
            BoxInScene boxbox;
            PlaceBox(box.lbdCorner, new float[] {box.edgeLength[0], box.edgeLength[1] , box.edgeLength[2] }, BoxPrefab, box, out boxbox);
            boxesInScene[i] = boxbox;
            i += 1;

            yield return new WaitForSeconds(0.2f);
        }

        Vector3 offset = new Vector3((float)-initBin.threeDimension[0] / 2, (float)-initBin.threeDimension[1] / 2, (float)-initBin.threeDimension[0] / 2);
        boxesHolder.localPosition = offset;
        Vector3 offset_t = new Vector3(0, 0, showBackwardOffset);
        transform.position = offset_t;
        boxesRotater.rotation = Quaternion.Euler(0f, 180f, 0f);
        packLog();
        startmp = true;
    }

    void PlaceBox(int[] lbdCorner, float[] dimen, GameObject BoxPrefab, Box bo, out BoxInScene b)
    {
        Vector3 pos = new Vector3((float)lbdCorner[0],(float)lbdCorner[1],(float)lbdCorner[2]);
        pos += new Vector3((float)dimen[0] / 2, (float)dimen[1] / 2, (float)dimen[2] / 2);
        GameObject box = Instantiate(BoxPrefab, pos, Quaternion.identity, boxesHolder);
        BoxInScene boxInScene = box.GetComponent<BoxInScene>();
        if(boxInScene != null)
        {
            boxInScene.box = bo;
            b = boxInScene;
        }
        else
        {
            b = null;
        }

        box.name = "test_Box_name";

        box.transform.localScale = new Vector3(dimen[0], dimen[1], dimen[2]);

        Debug.Log("boxesPlacing");
    }

    int Cut(Box need, int cutBox, List<Box> result, List<Box> boxes, List<solutionLog> solution, ref int index, List<Box> needs, List<Box> waste, ref int wastev)
    {
        int bestFitIndex = -1;
        int bestFitRank = -1;
        int bestFitRankDis = -1;

        Box TempNeed = need.Copy();
        Box source;

        List<int> dimension = new List<int>();
        for (int i = 0; i < boxes.Count; i++)
        {
            if (boxes[i].BiggerThan(need))
            {
                int a = boxes[i].Rank(need);
                int b = boxes[i].RankDistance(need);

                if (a > bestFitRank)
                {
                    bestFitIndex = i;
                    bestFitRank = a;
                    bestFitRankDis = b;
                }
                else if (a == bestFitRank && b >= bestFitRankDis)
                {
                    bestFitIndex = i;
                    bestFitRank = a;
                    bestFitRankDis = b;
                }
            }
        }

        if (bestFitIndex != -1)
        {
            source = boxes[bestFitIndex].Copy();

            boxes.RemoveAt(bestFitIndex);

        }
        else
        {
            waste.Add(need);
            wastev += need.getVolumn();
            return cutBox;
        }

        Box from = source.Copy();
        int cutInIndex = 0;
        string how = "";

        for (int j = 0; j < 3; j++)
        {
            int tempNum = 0;
            int bestMatchDimension = FindSmallestDistanceDimension(need, source, dimension);
            dimension.Add(bestMatchDimension);
            //Console.WriteLine("source before : " + source.ToString());
            if (source.threeDimension[bestMatchDimension] != need.threeDimension[j])
            {
                int d = source.numInD(source.threeDimension[bestMatchDimension]);
                int[] lbdCorner = source.lbdCorner.Select(x => x).ToArray();
                tempNum = source.threeDimension[bestMatchDimension] - need.threeDimension[j];
                lbdCorner[d] = source.lbdCorner[d] + need.threeDimension[j];
                Box newBox = new Box(source.threeDimension.Select(x => x).ToArray(), lbdCorner, source.edgeLength.Select(x => x).ToArray(), "..");
                newBox.ChangeEdgeLength(newBox.threeDimension[bestMatchDimension], tempNum);
                cutBox += 1;

                cutInIndex += 1;
                newBox.threeDimension[bestMatchDimension] = tempNum;
                Array.Sort(newBox.threeDimension);
                Array.Reverse(newBox.threeDimension);
                //Console.WriteLine(newBox);
                boxes.Add(newBox);
            }
            source.ChangeEdgeLength(source.threeDimension[bestMatchDimension], need.threeDimension[j]);
            source.threeDimension[bestMatchDimension] = need.threeDimension[j];
            //Console.WriteLine("source after : " + source.ToString());


            
            how += "\n" + source.ToString() + "\n";
            need.threeDimension[j] = 0;
        }

        Box to = source;
        solution.Add(new solutionLog(from, to, boxes.ConvertAll(x => x.Copy()), index, false, cutInIndex, how));

        if (need.threeDimension.Sum() == 0)
        {

            result.Add(source);
            int q = 0;
            List<Box> needsTemp = new List<Box>();

            for (int x = index + 1; x < needs.Count; x++)
            {
                int tryindex = BoxListContainAlmostEqual(boxes, needs[x]);
                if (tryindex != -1)
                {
                    Box needTemp = boxes[tryindex].Copy();
                    BoxListRemove(boxes, needs[x]);
                    needsTemp.Add(needTemp);
                    needs.RemoveAt(x);
                    Box fromto = needTemp.Copy();
                    solution.Add(new solutionLog(fromto, fromto, boxes.ConvertAll(x => x.Copy()), x, true, 0, ""));
                    //Console.WriteLine("creat log at in : " + x);
                    result.Add(needTemp);
                    q++;
                }
            }

            needs.InsertRange(index + 1, needsTemp);

            index += q;

            return cutBox;
        }
        else
        {
            return int.MaxValue;
        }
    }

    int RecursiveCut(List<Box> boxes, int cutBox, List<Box> result, ref int bestCut, List<Box> needs, int index, List<solutionLog> solution, ref List<solutionLog> bsts, ref List<Box> bestRe, List<Box> waste, ref List<Box> wasteSol, ref int wastevSol, int wastev)
    {

        if(onlyFindingSol && needs.Count == bestRe.Count)
        {
            return int.MaxValue;
        }

        if (!isBetterSol(cutBox, bestCut, wastev, wastevSol) || index >= needs.Count)
        {
            if (isBetterSol(cutBox, bestCut, wastev, wastevSol) && index == needs.Count)
            {
                bestCut = cutBox;

                wasteSol = waste;

                bsts = solution;

                bestRe = result;


                Debug.Log("find new best solution with best cut in: " + bestCut + " min waste: " + wasteSol + "testpgg");
            }

            return cutBox;
        }

        int mincut = int.MaxValue;
        int cutBoxt = cutBox;

        Debug.Log(index);
        Debug.Log(needs[index].Copy());

        for (int i = 0; i < boxes.Count; i++)
        {
            int indexTemp = index;
            Box need = needs[indexTemp].Copy();

            bool hasBigger = false;

            if (boxes[i].BiggerThan(need))
            {
                hasBigger = true;

                List<solutionLog> copySolution = solution.ConvertAll(s => s.Copy());
                cutBox = cutBoxt;
                List<Box> boxesTemp = boxes.ConvertAll(x => x.Copy());
                List<Box> resultTemp = result.ConvertAll(x => x.Copy());

                List<int> dimension = new List<int>();
                Box source = boxesTemp[i].Copy();

                boxesTemp.RemoveAt(i);

                Box from = source.Copy();

                int cutInIndex = 0;
                string how = "";
                //6 rotation cut plan;
                for (int j = 0; j < 3; j++)
                {
                    int tempNum = 0;
                    int bestMatchDimension = FindSmallestDistanceDimension(need, source, dimension);
                    dimension.Add(bestMatchDimension);
                    //Console.WriteLine("source before : " + source.ToString());
                    if (source.threeDimension[bestMatchDimension] != need.threeDimension[j])
                    {
                        int d = source.numInD(source.threeDimension[bestMatchDimension]);
                        int[] lbdCorner = source.lbdCorner.Select(x => x).ToArray();
                        tempNum = source.threeDimension[bestMatchDimension] - need.threeDimension[j];
                        lbdCorner[d] = source.lbdCorner[d] + need.threeDimension[j];
                        Box newBox = new Box(source.threeDimension.Select(x => x).ToArray(), lbdCorner, source.edgeLength.Select(x => x).ToArray(), "..");
                        newBox.ChangeEdgeLength(newBox.threeDimension[bestMatchDimension], tempNum);
                        cutBox += 1;

                        cutInIndex += 1;
                        newBox.threeDimension[bestMatchDimension] = tempNum;
                        Array.Sort(newBox.threeDimension);
                        Array.Reverse(newBox.threeDimension);
                        //Console.WriteLine(newBox);
                        boxesTemp.Add(newBox);
                    }
                    source.ChangeEdgeLength(source.threeDimension[bestMatchDimension], need.threeDimension[j]);
                    source.threeDimension[bestMatchDimension] = need.threeDimension[j];
                    //Console.WriteLine("source after : " + source.ToString());

                    how += "\n" + source.ToString() + "\n";
                    need.threeDimension[j] = 0;
                }

                Box to = source;
                copySolution.Add(new solutionLog(from, to, boxesTemp.ConvertAll(x => x.Copy()), indexTemp, false, cutInIndex, how));

                //Console.WriteLine("creat log at in : " + index);

                if (need.threeDimension.Sum() == 0)
                {

                    resultTemp.Add(source);

                    List<Box> needsTemp = new List<Box>();
                    int q = 0;
                    for (int x = indexTemp + 1; x < needs.Count; x++)
                    {
                        int tryIndex = BoxListContainAlmostEqual(boxesTemp, needs[x]);
                        if (tryIndex != -1)
                        {
                            Box needTemp = boxesTemp[tryIndex].Copy();
                            boxesTemp.RemoveAt(tryIndex);
                            needsTemp.Add(needTemp);
                            needs.RemoveAt(x);
                            Box fromto = needTemp.Copy();
                            copySolution.Add(new solutionLog(fromto, fromto, boxesTemp.ConvertAll(x => x.Copy()), x, true, 0, ""));
                            //Console.WriteLine("creat log at in : " + x);
                            resultTemp.Add(needTemp);
                            q++;
                        }
                    }

                    needs.InsertRange(indexTemp + 1, needsTemp);

                    indexTemp += q;
                }
                else
                {
                    return int.MaxValue;
                }

                if (cutBox < bestCut)
                {
                    boxesTemp = boxesTemp.OrderByDescending(x => x.threeDimension[0])
                                         .ThenByDescending(x => x.threeDimension[1])
                                         .ThenByDescending(x => x.threeDimension[2])
                                         .ToList();


                    int newCutBox = RecursiveCut(boxesTemp, cutBox, resultTemp, ref bestCut, needs, indexTemp + 1, copySolution, ref bsts, ref bestRe, waste.ConvertAll(x => x.Copy()), ref wasteSol, ref wastevSol, wastev);
                    if (newCutBox <= mincut)
                    {
                        mincut = newCutBox;
                    }

                }
                else
                {
                    return int.MaxValue;
                }

                //Console.WriteLine("logCount: " + copySolution.Count);
            }

            if (!hasBigger)
            {
                List<Box> _boxesTemp = boxes.ConvertAll(x => x.Copy());
                List<Box> _resultTemp = result.ConvertAll(x => x.Copy());
                List<solutionLog> _copySolution = solution.ConvertAll(s => s.Copy());

                List<Box> wasteCopy = waste.ConvertAll(x => x.Copy());
                int wasteNew = wastev += needs[index].getVolumn();

                int _newCutBox = RecursiveCut(_boxesTemp, cutBox, _resultTemp, ref bestCut, needs, index + 1, _copySolution, ref bsts, ref bestRe, wasteCopy, ref wasteSol, ref wastevSol, wasteNew);
                if (_newCutBox <= mincut)
                {
                    mincut = _newCutBox;
                }
            }
        }
        return mincut;
    }
    static bool isBetterSol(int nc, int bc, int nw, int bw)
    {
        if (nw < bw)
        {
            return true;
        }
        else if (nc < bc && nw == bw)
        {
            return true;
        }

        return false;
    }

    int FindSmallestDistanceDimension(Box need, Box source, List<int> source2)
    {
        bool m = false;
        int largestNeed = need.threeDimension.Max();
        int Val = int.MinValue;
        int bestDimension = -1;
        for (int i = 0; i < 3; i++)
        {
            if (!source2.Contains(i))
            {
                int distance = Math.Abs(largestNeed - source.threeDimension[i]);
                if (distance == 0)
                {
                    bestDimension = i;
                }
                else if (source.threeDimension[i] - largestNeed >= 0 && distance > Val)
                {
                    Val = distance;
                    bestDimension = i;
                }
            }
        }

        return bestDimension;
    }

    int BoxListContainAlmostEqual(List<Box> boxes, Box box)
    {
        int index = 0;
        foreach (Box obj in boxes)
        {
            if (box.EqualTo(obj))
            {
                return index;
            }

            index++;
        }
        return -1;
    }

    void BoxListRemove(List<Box> boxes, Box box)
    {
        foreach (Box obj in boxes)
        {
            if (box.EqualTo(obj))
            {
                boxes.Remove(obj);
                return;
            }
        }
    }

    bool containNum(string line)
    {
        //testing
        string[] numbe = new string[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

        foreach (string obj in numbe)
        {
            if (line.Contains(obj))
            {
                return true;
            }
        }

        return false;
    }

}

[System.Serializable]
public class Box
{
    public int[] threeDimension;
    public int[] lbdCorner;
    public int[] edgeLength;
    public string name;
    public Box(int[] _threedimension, int[] _lbdCornor, int[] _edgeLength, string _name)
    {
        name = _name;
        threeDimension = _threedimension;
        lbdCorner = _lbdCornor;
        edgeLength = _edgeLength;
    }

    public int numInD(int nu)
    {
        for(int i = 0; i < edgeLength.Length; i++)
        {
            if(edgeLength[i] == nu)
            {
                return i;
            }
        }

        return -1;
    }

    public void ChangeEdgeLength(int f, int t)
    {
        for(int i = 0; i < edgeLength.Length; i++)
        {
            if(f == edgeLength[i])
            {
                edgeLength[i] = t;
                break;
            }
        }
    }

    public bool BiggerThan(Box box2)
    {
        return threeDimension[0] >= box2.threeDimension[0] &&
               threeDimension[1] >= box2.threeDimension[1] &&
               threeDimension[2] >= box2.threeDimension[2];
    }

    public bool EqualTo(Box box2)
    {
        return threeDimension[0] == box2.threeDimension[0] &&
               threeDimension[1] == box2.threeDimension[1] &&
               threeDimension[2] == box2.threeDimension[2];
    }

    public int Rank(Box box2)
    {
        int rank = 0;

        if (threeDimension[0] == box2.threeDimension[0]) { rank++; }
        if (threeDimension[1] == box2.threeDimension[1]) { rank++; }
        if (threeDimension[2] == box2.threeDimension[2]) { rank++; }

        return rank;
    }

    public int RankDistance(Box box2)
    {
        int rank = 0;
        Box need = box2.Copy();
        Box source = Copy();
        List<int> dimension = new List<int>();

        for (int j = 0; j < 3; j++)
        {
            int tempNum = 0;
            int bestMatchDimension = FindSmallestDistanceDimension(need, source, dimension);
            dimension.Add(bestMatchDimension);


            if (source.threeDimension[bestMatchDimension] != need.threeDimension[j])
            {
                Box newBox = new Box(source.threeDimension.Select(x => x).ToArray(), new int[] {0,0,0}, new int[] {0,0,0}, "..");
                tempNum = source.threeDimension[bestMatchDimension] - need.threeDimension[j];
                rank += tempNum;
                newBox.threeDimension[bestMatchDimension] = tempNum;
                Array.Sort(newBox.threeDimension);
                Array.Reverse(newBox.threeDimension);
            }


            source.threeDimension[bestMatchDimension] = need.threeDimension[j];
            need.threeDimension[j] = 0;


        }

        return rank;
    }

    static int FindSmallestDistanceDimension(Box need, Box source, List<int> source2)
    {
        bool m = false;
        int largestNeed = need.threeDimension.Max();
        int Val = int.MinValue;
        int bestDimension = -1;
        for (int i = 0; i < 3; i++)
        {
            if (!source2.Contains(i))
            {
                int distance = Math.Abs(largestNeed - source.threeDimension[i]);
                if (distance == 0)
                {
                    bestDimension = i;
                }
                else if (source.threeDimension[i] - largestNeed >= 0 && distance > Val)
                {
                    Val = distance;
                    bestDimension = i;
                }
            }
        }

        return bestDimension;
    }

    public Box Copy()
    {
        int[] _threeDimension = threeDimension.Select(x => x).ToArray();
        int[] _lbdCorner = lbdCorner.Select(x => x).ToArray();
        int[] _edgeLength = edgeLength.Select(x => x).ToArray();
        return new Box(_threeDimension, _lbdCorner, _edgeLength, name);
    }

    public override string ToString()
    {
        return $"{threeDimension[0]}, {threeDimension[1]}, {threeDimension[2]}";
    }

    public int getVolumn()
    {
        return threeDimension[0] * threeDimension[1] * threeDimension[2];
    }
}

class solutionLog
{

    public Box from;
    public Box to;
    public string how;
    public List<Box> boxes;

    public int index;

    public bool fast;

    public int cutInIndex;
    public solutionLog(Box _from, Box _to, List<Box> _boxes, int _index, bool _fast, int _cutInIndex, string _howto)
    {
        from = _from;
        to = _to;
        boxes = _boxes;
        index = _index;
        fast = _fast;
        cutInIndex = _cutInIndex;
        how = _howto;
    }

    public string ToString()
    {
        string line = "from: " + from.ToString() + "\n" + "to: " + to.ToString() + "\n";
        if (how != "")
            line += "cut step: " + how;
        line += "boxes: " + "\n";


        foreach (Box box in boxes)
        {
            line += " " + box.ToString() + " " + "\n";
        }

        line += "index: " + index + "\n";

        line += "fast: " + fast + "\n";

        line += "cutInIndex: " + cutInIndex + "\n";
        return line;
    }

    public solutionLog Copy()
    {
        return new solutionLog(from, to, boxes.ConvertAll(x => x.Copy()), index, fast, cutInIndex, how);
    }
}
