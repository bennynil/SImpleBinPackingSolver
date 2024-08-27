using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class setting : MonoBehaviour
{
    [SerializeField] TMP_InputField filea;
    [SerializeField] TMP_InputField fileb;

    [SerializeField] TMP_InputField size;
    [SerializeField] TMP_InputField rseed;
    [SerializeField] DemandConfigurer configurer;
    [SerializeField] SolvingConfigurer solving;
    [SerializeField] BinPackingSolver binPackingSolver;

    string _load = "D:\\cutBoxesTest\\testData.csv";
    string _save = "D:\\cutBoxesTest\\anss.txt";
    int _size = 0;
    int _rseed = 0;

    private void Start()
    {
        LoadFile();
    }

    public void save()
    {
        binPackingSolver.setFilePath(filea.text);

        binPackingSolver.setSolutionPath(fileb.text);

        configurer.pathaddress.text = filea.text;
        solving.solutionPath.text = fileb.text;

        _load = filea.text;
        _save = fileb.text;

        int x = 0;
        if(int.TryParse(size.text, out x))
        {
            binPackingSolver.Size = x;
            _size = x;
        }

        int y = 0;
        if(int.TryParse(rseed.text, out y))
        {
            binPackingSolver.RandomSeed = y;
            _rseed = y;
        }

        SaveFile();
    }

    public void exit()
    {
        SaveFile();
        Application.Quit();
    }

    public void SaveFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(_size, _rseed, _load, _save);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            configurer.pathaddress.text = "D:\\cutBoxesTest\\testData.csv";
            solving.solutionPath.text = "D:\\cutBoxesTest\\anss.txt";

            binPackingSolver.Size = 0;
            binPackingSolver.RandomSeed = 0;
            SaveFile();
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData)bf.Deserialize(file);
        file.Close();

        configurer.pathaddress.text = data.path_load;
        solving.solutionPath.text = data.path_save;

        binPackingSolver.Size = data.size;
        binPackingSolver.RandomSeed = data.rseed;

        _size = data.size;
        _rseed = data.rseed;
        _load = data.path_load;
        _save = data.path_save;
    }

    [System.Serializable]
    public class GameData
    {
        public int size;
        public int rseed;

        public string path_load;
        public string path_save;

        public GameData(int _size, int _rseed, string _path_load, string _path_save)
        {
            size = _size;
            rseed = _rseed;
            path_load = _path_load;
            path_save = _path_save;
        }
    }
}
