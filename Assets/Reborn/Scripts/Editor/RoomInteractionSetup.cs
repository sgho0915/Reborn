using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reborn.Editor
{
    public static class RoomInteractionSetup
    {
        [MenuItem("Reborn/Setup Room Interactions")]
        public static void Setup()
        {
            var scene = SceneManager.GetActiveScene();
            if (EditorApplication.isPlayingOrWillChangePlaymode || scene.path != RoomBlockoutBuilder.ScenePath || scene.isDirty)
                throw new InvalidOperationException("Open saved RoomBlockout in Edit mode first.");
            var player = UnityEngine.Object.FindFirstObjectByType<FirstPersonController>();
            if (!player.GetComponent<InteractionController>()) Undo.AddComponent<InteractionController>(player.gameObject);
            Add("Diary", "room.diary", "일기장", "책상 위에 놓인 일기장.\n\n[조사 동작 검증용 설명. 실제 일기 원문과 페이지 넘기기는 후속 단계에서 연결.] ");
            Add("Printer", "room.printer", "프린터", "책상 끝에 놓인 프린터.\n\n[조사 동작 검증용 설명. 출력물과 출력 연출은 후속 단계에서 연결.] ");
            Add("Calendar February placeholder", "room.calendar", "2월 달력", "벽에 걸린 달력은 2월에 머물러 있다.\n\n[날짜 도안과 근접 표현은 후속 단계에서 연결.] ");
            Add("Umbrella placeholder", "room.umbrella", "접힌 우산", "현관 옆에 접힌 우산이 놓여 있다.\n\n[조사 동작 검증용 설명. 들고 가기와 소품 사용은 후속 단계에서 연결.] ");
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void Add(string objectName, string id, string title, string body)
        {
            var go = GameObject.Find(objectName);
            if (!go) throw new InvalidOperationException("Missing room prop: " + objectName);
            var target = go.GetComponent<InspectableObject>();
            if (!target) target = Undo.AddComponent<InspectableObject>(go);
            Undo.RecordObject(target,"Configure inspection"); target.Configure(id,title,body);
            EditorUtility.SetDirty(target);
        }
    }
}
