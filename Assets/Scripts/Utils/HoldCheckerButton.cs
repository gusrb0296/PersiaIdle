using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace Utils
{
    [Serializable]
    public class HoldCheckerButton : Button
    {
        public UnityEvent onExit;
    
        private Coroutine timeCheckRoutine;
    
        private bool isPressed;
        private bool isHold;

        [Tooltip("0:무한, 1~:반복회수\n반복회수가 끝나면 다음 반복설정으로 이동하며, 없으면 중지한다.")]
        [SerializeField] private List<HoldCheckerButtonSetting> settings;

        private IEnumerator CheckTime()
        {
            byte index = 0;
            float timeElapse = .0f;
            uint repeatCount = 0;
        
            while (isPressed)
            {
                timeElapse += Time.unscaledDeltaTime;
                if ((uint)(timeElapse / settings[index].repeatDuration) > repeatCount)
                {
                    repeatCount++;
                
                    if (!interactable)
                        yield break;
                
                    onClick?.Invoke();
                    if (!isHold)
                        isHold = true;
                    if (repeatCount == settings[index].repeatLimitCount)
                    {
                        timeElapse = 0.0f;
                        repeatCount = 0;
                        // 마지막 반복 설정을 다시 반복하는 코드. 현재는 마지막 반복설정이 끝나면 중지하도록 되어있다.
                        // index = Mathf.Clamp(index + 1, 0, repeatDuration.Count - 1);
                        ++index;
                        if (index >= settings.Count)
                            yield break;
                    }
                }
                yield return null;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (timeCheckRoutine != null)
            {
                StopCoroutine(CheckTime());
            }
        
            isPressed = true;
            timeCheckRoutine = StartCoroutine(CheckTime());
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!isHold)
            {
                base.OnPointerClick(eventData);
            }
            else
            {
                if (eventData.button != PointerEventData.InputButton.Left)
                    return;
            
                if (!IsActive() || !IsInteractable())
                    return;

                UISystemProfilerApi.AddMarker("Button.onClick", this);
            }
            onExit?.Invoke();
            Debug.Log("click");
            isPressed = false;
            isHold = false;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (isHold)
            {          
                if (eventData.button != PointerEventData.InputButton.Left)
                    return;
            
                if (!IsActive() || !IsInteractable())
                    return;
            }
            onExit?.Invoke();
            Debug.Log("exit");
            isPressed = false;
            isHold = false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(HoldCheckerButton), true)]
    public class HoldCheckerButtonEditor : ButtonEditor
    {
        private SerializedProperty settingProperty;
        protected override void OnEnable()
        {
            base.OnEnable();
        
            settingProperty = serializedObject.FindProperty("settings");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
        
            EditorGUILayout.LabelField("홀드 설정");
            EditorGUILayout.PropertyField(settingProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    [Serializable]
    public struct HoldCheckerButtonSetting
    {
        public float repeatDuration;
        public int repeatLimitCount;
    }
}