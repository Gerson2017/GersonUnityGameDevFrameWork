using GersonFrame.SelfILRuntime;
using UnityEditor;
using UnityEngine;



namespace GersonFrame
{

    public class AddMonoScriptsEditor : EditorWindow
    {
        private static GameObject m_viewGoRoot;

        private SerializedObject serializedObject;

        public bool m_addAwake;
        public bool m_addOnEnable;
        public bool m_addStart;
        public bool m_addUpdate;
        public bool m_addFixedUpdate;
        public bool m_addLateUpdate;
        public bool m_addTriggerEnter;
        public bool m_addTriggerStay;
        public bool m_addTriggerExit;
        public bool m_addConsillionEnter;
        public bool m_addConsillionStay;
        public bool m_addConsillionExit;
        public bool m_addParticleStop;
        public bool m_addParticleTrigger;
        public bool m_addAnimationEvt;
        public bool m_addOnDisable;
        public bool m_addOnDestroy;
        public bool m_addPointerDown;
        public bool m_addPointerUp;
        public bool m_addGizmos;

        [MenuItem("GameObject/添加Mono组件", priority = 1)]
        static void AddMonoCompnet()
        {
            m_viewGoRoot = Selection.activeObject as GameObject;
            MyDebuger.InitLogger(LogLevel.All);
            ShowEditor();
        }




        [MenuItem("ILRuntime/添加Mono组件")]
        static void ShowEditor()
        {
            AddMonoScriptsEditor window = EditorWindow.GetWindow<AddMonoScriptsEditor>(false, "添加热更mono组件", true);
            window.Show();
        }


        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);
        }


        private void OnGUI()
        {
            serializedObject.Update();
            GUILayout.BeginHorizontal();


            GUILayout.EndHorizontal();
            //============================
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("组件添加根节点:");
            m_viewGoRoot = (GameObject)EditorGUILayout.ObjectField(m_viewGoRoot, typeof(GameObject), true);
            m_addAwake = EditorGUILayout.Toggle("添加MonoAwake组件", m_addAwake, GUILayout.Width(350), GUILayout.Height(20));
            m_addOnEnable = EditorGUILayout.Toggle("添加MonoEnable组件", m_addOnEnable, GUILayout.Width(350), GUILayout.Height(20));
            m_addStart = EditorGUILayout.Toggle("添加MonoStart组件", m_addStart, GUILayout.Width(350), GUILayout.Height(20));
            m_addUpdate = EditorGUILayout.Toggle("添加MonoUpdate组件", m_addUpdate, GUILayout.Width(350), GUILayout.Height(20));
            m_addFixedUpdate = EditorGUILayout.Toggle("添加MonoFixedUpdate组件", m_addFixedUpdate, GUILayout.Width(350), GUILayout.Height(20));
            m_addLateUpdate = EditorGUILayout.Toggle("添加MonoLateUpdate组件", m_addLateUpdate, GUILayout.Width(350), GUILayout.Height(20));
            m_addTriggerEnter = EditorGUILayout.Toggle("添加MonoTriggerEnter组件", m_addTriggerEnter, GUILayout.Width(350), GUILayout.Height(20));
            m_addTriggerStay = EditorGUILayout.Toggle("添加MonoTriggerStay组件", m_addTriggerStay, GUILayout.Width(350), GUILayout.Height(20));
            m_addTriggerExit = EditorGUILayout.Toggle("添加MonoTriggerExit组件", m_addTriggerExit, GUILayout.Width(350), GUILayout.Height(20));
            m_addConsillionEnter = EditorGUILayout.Toggle("添加MonoConsillionEnter组件", m_addConsillionEnter, GUILayout.Width(350), GUILayout.Height(20));
            m_addConsillionStay = EditorGUILayout.Toggle("添加MonoConsillionStay组件", m_addConsillionStay, GUILayout.Width(350), GUILayout.Height(20));
            m_addConsillionExit = EditorGUILayout.Toggle("添加MonoConsillionExit 组件", m_addConsillionExit, GUILayout.Width(350), GUILayout.Height(20));
            m_addParticleStop = EditorGUILayout.Toggle("添加MonoParticleStop组件", m_addParticleStop, GUILayout.Width(350), GUILayout.Height(20));
            m_addParticleTrigger = EditorGUILayout.Toggle("添加MonoParticleTrigger组件", m_addParticleTrigger, GUILayout.Width(350), GUILayout.Height(20));
            m_addAnimationEvt = EditorGUILayout.Toggle("添加MonoAnimationEvt组件", m_addAnimationEvt, GUILayout.Width(350), GUILayout.Height(20));
            m_addOnDisable = EditorGUILayout.Toggle("添加MonoOnDisable组件", m_addOnDisable, GUILayout.Width(350), GUILayout.Height(20));
            m_addOnDestroy = EditorGUILayout.Toggle("添加MonoOnDestroy组件", m_addOnDestroy, GUILayout.Width(350), GUILayout.Height(20));
            m_addPointerDown = EditorGUILayout.Toggle("添加MonoPointerDown组件", m_addPointerDown, GUILayout.Width(350), GUILayout.Height(20));
            m_addPointerUp = EditorGUILayout.Toggle("添加MonoPointerUp组件", m_addPointerUp, GUILayout.Width(350), GUILayout.Height(20));
             m_addGizmos= EditorGUILayout.Toggle("添加MonoGizmos组件", m_addGizmos, GUILayout.Width(350), GUILayout.Height(20));

            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
            if (GUILayout.Button("添加", GUILayout.Width(100), GUILayout.Height(50)))
            {
                if (m_viewGoRoot==null)
                {
                    Debug.LogError("未选中任何要添加组件的物体");
                    return;
                }

                AddCompentToRoot<MonoAwake>(m_addAwake);
                AddCompentToRoot<MonoEnable>(m_addOnEnable);
                AddCompentToRoot<MonoStart>(m_addStart);
                AddCompentToRoot<MonoUpdate>(m_addUpdate);
                AddCompentToRoot<MonoFixedUpdate>(m_addFixedUpdate);
                AddCompentToRoot<MonoLateUpdate>(m_addLateUpdate);
                AddCompentToRoot<MonoTriggerEnter>(m_addTriggerEnter);
                AddCompentToRoot<MonoTriggerStay>(m_addTriggerStay);
                AddCompentToRoot<MonoTriggerExit>(m_addTriggerExit);
                AddCompentToRoot<MonoCollisionEnter>(m_addConsillionEnter);
                AddCompentToRoot<MonoCollisionStay>(m_addConsillionStay);
                AddCompentToRoot<MonoCollisionExit>(m_addConsillionExit);
                AddCompentToRoot<MonoParticleSystemStop>(m_addParticleStop);
                AddCompentToRoot<MonoParticleTrigger>(m_addParticleTrigger);
                AddCompentToRoot<MonoPointerDown>(m_addPointerDown);
                AddCompentToRoot<MonoPointerUp>(m_addPointerUp);
                AddCompentToRoot<MonoOnDisable>(m_addOnDisable);
                AddCompentToRoot<MonoOnDestroy>(m_addOnDestroy);
                AddCompentToRoot<MonoOnDrawGizmos>(m_addGizmos);
            }
        }


        void AddCompentToRoot<T>(bool add) where T: MonoBehaviour
        {
            if (add)
                m_viewGoRoot.GetCompententOrNew<T>();
        }


    }

}