using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Threading;
using TMPro;

public class SolvingConfigurer : MonoBehaviour
{
    public BinPackingSolver solver;
    public GameObject solverGo;
    BinPackingSolver[] solvers;
    GameObject[] solverGOar;
    public TextMeshProUGUI answerLog;

    public TMP_InputField solutionPath;

    private void Start()
    {
        solver.solvingProcess = solvingState.Null;
    }

    private void Update()
    {
        if(solver.isCompelete && solver.solvingProcess == solvingState.Solving)
        {
            if (solver.onlyFindingSol)
            {
                if (solvers != null)
                {
                    bool finish = true;
                    foreach (BinPackingSolver solver in solvers)
                    {
                        if(solver.isCompelete == false)
                        {
                            finish = false;
                        }
                    }

                    if (finish)
                    {
                        BinPackingSolver _solvers = null;


                        foreach(BinPackingSolver _solver in solvers)
                        {
                            _solvers = resultCompare(_solver, _solvers);
                        }

                        _solvers = resultCompare(_solvers, solver);
                        Debug.Log("s");
                        _solvers.showBoxesCoroutine();
                    }
                }
                else
                {
                    solver.showBoxesCoroutine();
                }
                
            }

            if(solver.thread != null)
            {
                solver.thread.Join();

                if(solvers != null)
                {
                    bool finish = true;
                    foreach (BinPackingSolver solver in solvers)
                    {
                        if (solver.isCompelete == false)
                        {
                            finish = false;
                        }
                    }

                    if (finish)
                    {
                        foreach (BinPackingSolver _solver in solvers)
                        {
                            if (_solver.thread != null)
                            {
                                _solver.thread.Join();
                            }
                        }
                    }
                }
                
            }

            solver.solvingProcess = solvingState.FindAnswer;
        }
        if (solver.solvingProcess == solvingState.Null)
        {
            answerLog.text = "solution will be print here";
        }
        else if (solver.solvingProcess == solvingState.Solving)
        {
            answerLog.text = "solving......";
        }
        else
        {








            string stext = "";
            foreach(string line in solver.solutionsStrings)
            {

                stext += line + "\n";


            }
            answerLog.text = stext;
        }
    }

    private void resetSolver()
    {
       if(solverGOar == null)
       {
           return;
       }

       foreach(GameObject go in solverGOar)
       {
            if(go != null)
            {
                Destroy(go);
            }
       }

    }

    public void solvepacking()
    {

        solver.onlyFindingSol = true;
        resetSolver();
        solver.Solve();
    }

    public void solveRandomFit()
    {
        solver.onlyFindingSol = true;

        resetSolver();
        solver.Solve();
        solvers = new BinPackingSolver[solver.Size];
        solverGOar = new GameObject[solver.Size];
        for(int i = 0; i < solver.Size; i++)
        {
            GameObject _solver = Instantiate(solverGo, solverGo.transform.position, solverGo.transform.rotation);
            BinPackingSolver BPsolver = _solver.GetComponent<BinPackingSolver>();
            solvers[i] = BPsolver;
            solverGOar[i] = _solver;
            BPsolver.isRandom = true;
            BPsolver.RandomSeed = solver.RandomSeed + i;
            BPsolver.isCompelete = false;



            BPsolver.Solve();
        }
    }

    public void solvecutting()
    {

        solver.onlyFindingSol = false;

        resetSolver();
        solver.Solve();
    }

    public BinPackingSolver resultCompare(BinPackingSolver a, BinPackingSolver b)
    {
        int _a = 0;
        int _b = 0;

        if(a == null)
        {
            return b;
        }

        if(b == null)
        {
            return a;
        }

        a.boxesHolder.gameObject.SetActive(false);
        b.boxesHolder.gameObject.SetActive(false);

        foreach(Box box in a.bestRe)
        {
            _a += box.getVolumn();
        }

        foreach(Box box in b.bestRe)
        {
            _b += box.getVolumn();
        }

        if(_a > _b)
        {
            a.boxesHolder.gameObject.SetActive(true);
            return a;
        }

        b.boxesHolder.gameObject.SetActive(true);
        return b;
    }

    public void saveSolution()
    {
        solver.setSolutionPath(solutionPath.text);




        solver.saveSol();
    }

    public void stopWithAnswer()
    {
        solver.onlyFindingSol = true;
    }
}
