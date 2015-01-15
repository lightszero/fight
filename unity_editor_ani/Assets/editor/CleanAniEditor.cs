using UnityEngine;

using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class CleanAniEdiotr : EditorWindow
{
    [MenuItem("CleanAniEdior/ShowWindow")]
    public static void ShowCleanAni()
    {
        var win = EditorWindow.GetWindow<CleanAniEdiotr>();
        win.Show();
    }
    GameObject selectobj = null;
    GameObject sellast = null;
    int toogle = 0;
    void OnGUI()
    {
        if(Application.isPlaying)
        {
            GUILayout.Label("Can't use in playmode.");
            return;
        }
        GUILayout.Label("Clean Ani Editor.");
        {
            var bc = GUI.backgroundColor;
            //GUI.backgroundColor = Color.green;
            ///选择物体
            selectobj = EditorGUILayout.ObjectField("pickAniObj", selectobj, typeof(GameObject), GUILayout.MinHeight(1)) as GameObject;
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(5));
                string set = "<null>";
                if(Selection.activeGameObject!=null)
                    set+=Selection.activeGameObject.name + "<" + Selection.activeGameObject.GetType().Name + ">";
                GUILayout.Label("Active:" +set );
                if (GUILayout.Button("use this.", GUILayout.MinHeight(1)))
                {
                    selectobj = Selection.activeGameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            ChangeSelect();
            EditorGUILayout.HelpBox("选择一个GameObject，要拥有Animator组件和SkindMeshRenderer组件", MessageType.Info);
            {

                toogle = GUILayout.Toolbar(toogle, new string[] { "动画", "骨骼", "上传管理" }, GUILayout.MinHeight(1));
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(1));
                {
                    GUILayout.Space(10);
                    EditorGUILayout.BeginVertical(GUILayout.MinWidth(this.position.width-20));
                    if (toogle == 0)
                    {
                        if (sellast != null)
                        {
                            TestAnis();
                        }
                    }
                    else
                    {

                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
        


        }
    }
    //从一个Animator中获取所有的Animation
    void FindAllAniInControl(UnityEditorInternal.AnimatorController control, List<AnimationClip> list)
    {
        for (int i = 0; i < control.layerCount; i++)
        {
            var layer = control.GetLayer(i);
            FindAllAniInControl(layer.stateMachine, list);
        }
    }
    void FindAllAniInControl(UnityEditorInternal.StateMachine machine, List<AnimationClip> list)
    {
        for (int i = 0; i < machine.stateCount; i++)
        {
            var m = machine.GetState(i).GetMotion();
            if (list.Contains(m as AnimationClip) == false)
            {
                list.Add(m as AnimationClip);
            }
        }
        for (int i = 0; i < machine.stateMachineCount; i++)
        {
            var m = machine.GetStateMachine(i);
            FindAllAniInControl(m, list);
        }
    }
    Animator ani = null;
    void TestAnis()
    {
        ani = sellast.GetComponentInChildren<Animator>();
        Animation aniold = sellast.GetComponentInChildren<Animation>();
        SkinnedMeshRenderer[] skins = sellast.GetComponentsInChildren<SkinnedMeshRenderer>();

        GUILayout.Label("Find Ani:" + (ani != null) + ", skinmesh count=" + skins.Length);
        if (ani == null)
        {
            GUILayout.Label("Need Animator Component");
            if(aniold!=null)
            {
                GUILayout.Label("You have a old Animation Component.But we only support Animator.");

            }
            return;
        }
        List<AnimationClip> anis = new List<AnimationClip>();


        var ac = ani.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
        FindAllAniInControl(ac, anis);


        foreach (var a in anis)
        {
            if (GUILayout.Button("haveani:" + a.name, GUILayout.MinWidth(1)))
            {
                _selectClip = a;
                anistart = 0;
                aniend = a.length;
                bPlay = false;
                //ani.Play(a.name, 0, 0);
            }
        }
        if(_selectClip)
        {
            var an = ani.GetCurrentAnimatorStateInfo(0);
            anistart = GUILayout.HorizontalSlider(anistart, 0, _selectClip.length, GUILayout.MinHeight(1));
            if (anistart >= aniend) anistart = aniend;
            aniend = GUILayout.HorizontalSlider(aniend, 0, _selectClip.length, GUILayout.MinHeight(1));
            if (aniend <= anistart) aniend = anistart;
            aninow = GUILayout.HorizontalSlider(aninow, anistart, aniend, GUILayout.MinHeight(1));
            if (!bPlay)
            {
                ani.Play(_selectClip.name, 0, 0);
                ani.speed = 1.0f;
                ani.Update(aninow);
            }
            bPlay = GUILayout.Toggle(bPlay, "Play", GUILayout.MaxWidth(100));
        }
    }
    float anistart;
    float aniend;
    float aninow;
    AnimationClip _selectClip = null;
    bool bPlay = false;
    void ChangeSelect()
    {
        if (sellast == selectobj)
        {
            return;
        }
        _selectClip = null;
        bPlay = false;
        sellast = selectobj;
    }
    void Update()
    {
        if (Application.isPlaying) return;
        if(bPlay&&ani!=null&&_selectClip!=null)
        {
            ani.Play(_selectClip.name);
            ani.speed = 1.0f;
            ani.Update(0.03f);
            Debug.Log("p"+Time.deltaTime);
        }
        this.Repaint();
    }

}
