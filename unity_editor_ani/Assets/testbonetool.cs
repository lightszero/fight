using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class testbonetool : MonoBehaviour
{
    public GameObject char01;
    // Use this for initialization
    Animation ani;
    SkinnedMeshRenderer skin;
    void Start()
    {
        ani = char01.GetComponent<Animation>();
        skin = char01.GetComponentInChildren<SkinnedMeshRenderer>();
    }
    class MSplit
    {
        public Quaternion r;
        public Vector3 t;
        public Vector3 s;
    }
    MSplit[] constt = null;
    // Update is called once per frame
    void Update()
    {
  
        if (constt == null) constt = new MSplit[skin.bones.Length];
        for (int i = 0; i < constt.Length; i++)
        {
            if (constt[i] == null) constt[i] = new MSplit();
            constt[i].r = skin.bones[i].localRotation;
            constt[i].t = skin.bones[i].localPosition;
            constt[i].s = skin.bones[i].localScale;
        }
        //return ;
        foreach (var iobj in listObj)
        {
            for (int i = 0; i < constt.Length; i++)
            {
                //iobj.bones[i].localPosition = constt[i].t;
                iobj.bones[i].localRotation = constt[i].r;
                //iobj.bones[i].localScale = constt[i].s;
            }
        }
    }
    bool bplay = true;
    int clone100 = 0;
    void OnGUI()
    {
        if (clone100 == 0)
            if (GUI.Button(new Rect(0, 100, 100, 50), "Clone 100"))
            {
                clone100 = 1;
                //ani.enabled = false;
                for (float x = 0; x < 5; x++)
                {
                    for (float y = 0; y < 5; y++)
                    {
                        var obj = GameObject.Instantiate(char01, new Vector3(x - 2.5f, 0, y - 2.5f), Quaternion.identity);

                    }
                }
                char01.SetActive(false);
            }
        if (GUI.Button(new Rect(0, 0, 100, 50), "Stop"))
        {
            bplay = !bplay;
            if (bplay)
                ani.Play();
            else
                ani.Stop();
        }
        if (clone100 == 0)
            if (GUI.Button(new Rect(0, 50, 100, 50), "CloneChar"))
            {
                clone100 = 2;
                CloneChar();
                //skin.bones = null;
            }
    }
    List<SkinnedMeshRenderer> listObj = new List<SkinnedMeshRenderer>();
    void CloneChar()
    {

        GameObject root = new GameObject("clonechar");
        root.transform.position = new Vector3(2, 0, 0);
        GameObject mesh = new GameObject("mesh");
        mesh.transform.parent = root.transform;
        mesh.transform.localPosition = Vector3.zero;
        SkinnedMeshRenderer mr = mesh.AddComponent<SkinnedMeshRenderer>();
        mr.sharedMesh = skin.sharedMesh;
        mr.sharedMaterial = skin.sharedMaterial;
        Transform[] bonew = new Transform[skin.bones.Length];
        GameObject rbone = new GameObject("boneRoot");

        rbone.transform.parent = root.transform;
        rbone.transform.localPosition = Vector3.zero;
        var matr = skin.transform.worldToLocalMatrix;
        int[] parent = new int[skin.bones.Length];
        for (int i = 0; i < skin.bones.Length; i++)
        {
            parent[i] = -1;
            for(int j=0;j<skin.bones.Length;j++)
            {
                if(skin.bones[j]==skin.bones[i].parent)
                {
                    parent[i] = j;
                }
            }
            
            var src = skin.bones[i];
            GameObject nbone = new GameObject("bone=" + i+" parenti="+parent[i]);
            nbone.hideFlags = HideFlags.HideInHierarchy;

            bonew[i] = nbone.transform;

        }
        for (int i = 0; i < bonew.Length;i++ )
        {
            if (parent[i] >= 0)
                bonew[i].parent = bonew[parent[i]];
            else
                bonew[i].parent = rbone.transform;
            bonew[i].localPosition = skin.bones[i].localPosition;
            bonew[i].localRotation = skin.bones[i].localRotation;
            bonew[i].localScale = skin.bones[i].localScale;
        }
            mr.bones = bonew;

        for (float x = 0; x < 5; x++)
        {
            for (float y = 0; y < 5; y++)
            {
                var obj = GameObject.Instantiate(root, new Vector3(x - 1.0f, 0, y - 1.0f), Quaternion.identity);
                listObj.Add((obj as GameObject).GetComponentInChildren<SkinnedMeshRenderer>());
            }
        }
    }
}
