using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class viewConfigurer : MonoBehaviour
{
    int selected;

    public float step = 5;

    [SerializeField] Transform root;

    public TMP_InputField[] pos = new TMP_InputField[3];
    public TMP_InputField[] qua = new TMP_InputField[3];
    public TMP_InputField scale;

    bool leftKeyPressing;
    bool rightKeyPressing;
    

    Vector3 rotation = Vector3.zero;

    void Start()
    {
        Vector3 r = root.rotation.ToEuler();
        pos[0].text = root.position.x.ToString();

        pos[1].text = root.position.y.ToString();

        pos[2].text = root.position.z.ToString();

        qua[0].text = r.x.ToString();

        qua[1].text = r.y.ToString();

        qua[2].text = r.z.ToString();

        scale.text = root.localScale.x.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        leftKeyPressing = Input.GetKey(KeyCode.LeftArrow);
        rightKeyPressing = Input.GetKey(KeyCode.RightArrow);

        if(leftKeyPressing != rightKeyPressing)
        {

            switch (selected)
            {
                case 101:
                    if (leftKeyPressing)
                    {
                        float p = root.position.x - Time.deltaTime * step;
                        pos[0].text = p.ToString("0.##");
                        root.position = new Vector3(p,root.position.y,root.position.z);
                    }
                    else if (rightKeyPressing)
                    {
                        float p = root.position.x + Time.deltaTime * step;
                        pos[0].text = p.ToString("0.##");
                        root.position = new Vector3(p, root.position.y, root.position.z);
                    }
                    break;
                case 102:
                    if (leftKeyPressing)
                    {
                        float p = root.position.y - Time.deltaTime * step;
                        pos[1].text = p.ToString("0.##");
                        root.position = new Vector3(root.position.x, p, root.position.z);
                    }
                    else if (rightKeyPressing)
                    {
                        float p = root.position.y + Time.deltaTime * step;
                        pos[1].text = p.ToString("0.##");
                        root.position = new Vector3(root.position.x, p, root.position.z);
                    }
                    break ;
                case 103:
                    if (leftKeyPressing)
                    {
                        float p = root.position.z - Time.deltaTime * step;
                        pos[2].text = p.ToString("0.##");
                        root.position = new Vector3(root.position.x, root.position.y, p);
                    }
                    else if (rightKeyPressing)
                    {
                        float p = root.position.z + Time.deltaTime * step;
                        pos[2].text = p.ToString("0.##");
                        root.position = new Vector3(root.position.x, root.position.y, p);
                    }
                    break;
                case 201:
                    if (leftKeyPressing)
                    {
                        float p = rotation.x - Time.deltaTime * step;

                        qua[0].text = p.ToString("0.##");
                        rotation = new Vector3(p, rotation.y, rotation.z);
                        root.rotation = Quaternion.Euler(rotation);
                    }
                    else if (rightKeyPressing)
                    {
                        float p = rotation.x + Time.deltaTime * step;

                        qua[0].text = p.ToString("0.##");
                        rotation = new Vector3(p, rotation.y, rotation.z);
                        root.rotation = Quaternion.Euler(rotation);
                    }
                    break;
                case 202:
                    if (leftKeyPressing)
                    {
                        float p = rotation.y - Time.deltaTime * step;
                        rotation = new Vector3(rotation.x, p, rotation.z);
                        qua[1].text = p.ToString("0.##");
                        root.rotation = Quaternion.Euler(rotation);
                    }
                    else if (rightKeyPressing)
                    {
                        float p = rotation.y + Time.deltaTime * step;
                        rotation = new Vector3(rotation.x, p, rotation.z);
                        qua[1].text = p.ToString("0.##");
                        root.rotation = Quaternion.Euler(rotation);
                    }
                    break;
                case 203:
                    if (leftKeyPressing)
                    {
                        float p = rotation.z - Time.deltaTime * step;
                        rotation = new Vector3(rotation.x, rotation.y, p);
                        qua[2].text = p.ToString("0.##");
                        root.rotation = Quaternion.Euler(rotation);
                    }
                    else if (rightKeyPressing)
                    {
                        float p = rotation.z + Time.deltaTime * step;
                        rotation = new Vector3(rotation.x, rotation.y, p);
                        qua[2].text = p.ToString("0.##");
                        root.rotation = Quaternion.Euler(rotation);
                    }
                    break;
                case 301:
                    if (leftKeyPressing)
                    {
                        float p = root.localScale.z - Time.deltaTime * step;
                        scale.text = p.ToString("0.##");
                        root.localScale = new Vector3(p,p,p);
                    }
                    else if (rightKeyPressing)
                    {
                        float p = root.localScale.z + Time.deltaTime * step;
                        scale.text = p.ToString("0.##");
                        root.position = new Vector3(p,p,p);
                    }
                    break;
            }
        }

        chooseInput();
    }

    void chooseInput()
    {
        selected = (int)Mathf.Clamp(selected, 101, 301);

        if (Input.GetKeyDown(KeyCode.W))
        {


            selected -= 100;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {

            selected += 100;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            int t = (selected - 1) % 10;

            if(t >= 1)
            {
                selected -= 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            int t = (selected + 1) % 10;

            if(t <= 3)
            {
                selected += 1;
            }
            
        }

        int h = selected / 100;
        int l = selected % 10;

        if(h == 1)
        {
            pos[l - 1].Select();
        }
        if(h == 2)
        {

            qua[l - 1].Select();
        }
        if(h == 3)
        {
            scale.Select();
        }
    }

    public void select(int i)
    {
        selected = i;
    }

    public void changeValue(string i)
    {

        if(i[0] == '1')
        {
            if(i[2] == '1')
            {
                float p = 0;
                bool parse = float.TryParse(pos[0].text, out p);
                if (!parse)
                {
                    return;
                }
                root.position = new Vector3(p, root.position.y, root.position.z);
            }
            else if (i[2] == '2')
            {
                float p = 0;
                bool parse = float.TryParse(pos[1].text, out p);
                if (!parse)
                {
                    return;
                }
                root.position = new Vector3(root.position.x, p, root.position.z);
            }
            else if (i[2] == '3')
            {
                float p = 0;
                bool parse = float.TryParse(pos[2].text, out p);
                if (!parse)
                {
                    return;
                }
                root.position = new Vector3(root.position.x, root.position.y, p);
            }
        }
        else if(i[0] == '2')
        {
            if(i[2] == '1')
            {
                float p = 0;
                bool parse = float.TryParse(qua[0].text, out p);
                if (!parse)
                {
                    return;
                }
                rotation = new Vector3(p, rotation.y, rotation.z);
                root.rotation = Quaternion.Euler(rotation);
            }
            if (i[2] == '2')
            {
                float p = 0;
                bool parse = float.TryParse(qua[1].text, out p);
                if (!parse)
                {
                    return;
                }
                rotation = new Vector3(rotation.x, p, rotation.z);
                root.rotation = Quaternion.Euler(rotation);
            }
            if (i[2] == '3')
            {
                float p = 0;
                bool parse = float.TryParse(qua[2].text, out p);
                if (!parse)
                {
                    return;
                }
                rotation = new Vector3(rotation.x, rotation.y, p);
                root.rotation = Quaternion.Euler(rotation);
            }
        }
        else if(i[0] == '3')
        {
            float p = 0;
            bool parse = float.TryParse(scale.text, out p);
            if (!parse)
            {
                return;
            }
            root.localScale = new Vector3(p, p, p);
        }
    }
}
